using Cannons.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;
using System;

namespace Cannons.Catapults
{
    public class Catapult : MonoBehaviour,
                            Initializable<GameInfo>,
                            IBindable<IGameEventsHandler>,
                            ITargetEventHandler,
                            IStock,
                            Initializable<CatapultData.Settings, GameObject>
    {
        [Header("Settings")]
        [SerializeField] private CatapultData.Settings settings;
        [SerializeField] private LayerMask layerMask;
        [Header("Links")]
        [SerializeField] private GameObject bulletPrefab;
        [Header("Components")]
        [SerializeField] private Transform center;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform bulletPlace;
        [Header("Collider")]
        [SerializeField] private Collider zoneCollider;

        private Transform levelTransform;
        private ISoundPlayer soundPlayer;
        private IPathDrawer pathDrawer;
        private Path currantPath;
        private BreakBullet currantBullet;
        private Coroutine coroutine;

        private Bounds fireBounds;
        private Vector3 normal;

        private GameStates gameState;
        private int bulletsCount;
        private bool soundPlayed;


        public event Action OnDone;
        public event Action OnFailed;
        public event Action<MoneyValue> OnMoney;
        public event Action<IStock> OnChanged;


        public int Count
        {
            get => bulletsCount;
            private set
            {
                bulletsCount = value;
                bulletsCount = Mathf.Max(bulletsCount, 0);

                OnChanged?.Invoke(this);
            }
        }
        public int MaxCount
        {
            get => settings.BulletsCount;
        }


        private bool Reloaded
        {
            get => currantBullet != null;
        }
        private bool ActionOn
        {
            get => coroutine != null;
        }
        private float Lenght
        {
            get
            {
                return Mathf.Min((center.position - firePoint.position).magnitude, settings.MaxRopeLenght);
            }
        }
        private float Ratio
        {
            get => Lenght / settings.MaxRopeLenght;
        }
        private Vector3 Direction
        {
            get
            {
                return (center.position - firePoint.position).normalized;
            }
        }




        public void Initialize(GameInfo info)
        {
            levelTransform = info.SceneContext.LevelTransform;
            soundPlayer = info.Components.SoundPlayer;

            pathDrawer = GetComponentInChildren<IPathDrawer>();
            if (pathDrawer == null)
            {
                pathDrawer = new NullPathDrawer();
            }

            fireBounds = zoneCollider.bounds;
        }
        public void Initialize(CatapultData.Settings settings, GameObject bullet)
        {
            this.settings = settings;

            bulletPrefab = bullet;

            bulletsCount = settings.BulletsCount;
        }


        public void Restart(Action onDone)
        {
            Count += settings.RestartBulletsAdd;

            onDone?.Invoke();
        }


        public void Bind(IGameEventsHandler obj)
        {
            obj.OnStarted += OnStarted;
            obj.OnStateChanged += OnStateChanged;
            obj.OnClearScene += (x) =>
            {
                x.OnStarted -= OnStarted;
                x.OnStateChanged -= OnStateChanged;
            };
        }


        private void OnStarted()
        {
            Reload();

            StartCoroutine(CheckForFail());
        }
        private void Fail()
        {
            if (gameState != GameStates.Game)
                return;

            OnFailed?.Invoke();


            if(ActionOn)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(FireCour());

            ClearPath();
        }
        private void OnStateChanged(GameStates obj)
        {
            gameState = obj;
        }

        public void SetNormal(Vector3 normal)
        {
            this.normal = normal;
        }

        public void Scope(Vector3 point)
        {
            if (ActionOn)
            {
                return;
            }
            if(!Reloaded)
            {
                Reload();
                return;
            }

            point.x *= 0.6f;

            Vector3 vector = (point - center.position);
            float lenght = vector.magnitude > settings.MaxRopeLenght ? settings.MaxRopeLenght : vector.magnitude;
            Quaternion rotation = Quaternion.LookRotation(-vector.normalized, normal);
            Vector3 position = center.position + vector.normalized * lenght;

            firePoint.position = Vector3.Lerp(firePoint.position, position, 0.33f);
            firePoint.rotation = Quaternion.Lerp(firePoint.rotation, rotation, 0.33f);

            pathDrawer.Draw(currantPath);

            UpdatePath();

            if(Lenght > 0.4f && !soundPlayed)
            {
                soundPlayer.PlaySound("catapul_scope");
                soundPlayed = true;
            }
        }
        public void Fire(Vector3 point)
        {
            if (ActionOn)
                return;

            if(currantPath == null || Count <= 0)
            {
                coroutine = StartCoroutine(CancelCour());
            }
            else
            {
                coroutine = StartCoroutine(FireCour());
            }

            ClearPath();
        }
        public void Reload()
        {
            if (ActionOn || Count <= 0)
                return;

            coroutine = StartCoroutine(ReloadCour());
        }


        private void Shot(float power)
        {
            if (!Reloaded || currantPath == null)
                return;

            
            BreakBulletInfo info = new BreakBulletInfo(currantBullet, currantPath, settings.BulletSpeed * power, settings.BulletPower, settings.SplashRadius);
            currantBullet.transform.parent = levelTransform;
            currantBullet.Run(info);
            currantBullet = null;
            currantPath = null;

            Count--;
        }
        private void UpdatePath()
        {
            if(Ratio < settings.MinFireRatio || !Reloaded)
            {
                currantPath = null;
                return;
            }

            if (Physics.Raycast(center.position, Direction, out RaycastHit hit, 1000, layerMask))
            {
                float ratio = Mathf.Pow(Ratio, 2f);
                Vector3 endPoint = hit.point;

                endPoint.y = Mathf.Lerp(fireBounds.min.y, hit.point.y, ratio);
                endPoint.x = Mathf.Clamp(endPoint.x, fireBounds.min.x, fireBounds.max.x);

                float topHeight = settings.TopHeight * (1 - ratio * 0.75f);

                currantPath = new Path(center.position, endPoint, Vector3.up, topHeight, settings.FlyCurve);
            }
            else
            {
                currantPath = null;
            }
        }
        private void ClearPath()
        {
            pathDrawer.Draw(null);
            soundPlayed = false;
        }

        private IEnumerator FireCour()
        {
            if (Reloaded)
            {
                soundPlayer.PlaySound("catapul_fire");
            }

            float time = 0f;
            Vector3 startPoint = firePoint.localPosition;

            var wait = new WaitForFixedUpdate();
            bool shotDone = false;

            float power = 1f;

            while(time < settings.FireTime)
            {
                float ratio = time / settings.FireTime;

                if (!shotDone && ratio > settings.FireKeyTime)
                {
                    Shot(power);
                    shotDone = true;
                }

                firePoint.rotation = Quaternion.Lerp(firePoint.rotation, Quaternion.identity, settings.FireCurve.Evaluate(ratio));
                firePoint.localPosition = Vector3.LerpUnclamped(startPoint, Vector3.zero, settings.FireCurve.Evaluate(ratio));

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            coroutine = null;

            Reload();
        }
        private IEnumerator CancelCour()
        {
            float time = 0f;
            Vector3 startPoint = firePoint.localPosition;

            var wait = new WaitForFixedUpdate();

            while (time < settings.CancelTime)
            {
                firePoint.rotation = Quaternion.Lerp(firePoint.rotation, Quaternion.identity, settings.CancelCurve.Evaluate(time / settings.FireTime));
                firePoint.localPosition = Vector3.LerpUnclamped(startPoint, Vector3.zero, settings.CancelCurve.Evaluate(time / settings.FireTime));

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            coroutine = null;
            yield break;
        }
        private IEnumerator ReloadCour()
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletPlace.position, bulletPlace.rotation, bulletPlace);
            currantBullet = bullet.GetComponent<BreakBullet>();
            currantBullet.transform.localScale = Vector3.one * 1.5f;
            currantBullet.PlaySpawn();

            yield return new WaitForSeconds(0.5f);

            coroutine = null;
            yield break;
        }


        private IEnumerator CheckForFail()
        {
            float maxTime = 2f;
            float time = 0f;

            var wait = new WaitForFixedUpdate();

            while(true)
            {
                time = 0f;

                yield return new WaitUntil(() => Count <= 0);

                while(gameState == GameStates.Game)
                {
                    if(Count > 0)
                    {
                        break;
                    }
                    if (time >= maxTime)
                    {
                        Fail();
                    }

                    time += Time.fixedDeltaTime;
                    yield return wait;
                }
            }
        }
    }
}
