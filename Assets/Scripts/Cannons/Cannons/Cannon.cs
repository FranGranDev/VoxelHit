using UnityEngine;
using Services;
using Cannons.Bullets;
using Animations;
using DG.Tweening;
using Zenject;

namespace Cannons
{
    public class Cannon : CannonBase, ISuperShotEvents
    {      
        [Header("Model")]
        [SerializeField] private Transform mainModel;
        [Header("Animation")]
        [SerializeField] private float rotateTime;
        [SerializeField] private Ease rotateEase;


        [Inject]
        private IGameEventsHandler eventsHandler;



        private bool isFiring;
        private bool superReadyUI;

        private float prevFireTime;
        private Vector3 startPosition;
        private Quaternion startRotation;


        public Transform Point => mainModel;
        public event System.Action<bool> OnReadyChanged;


        public override void Initialize(GameInfo info)
        {
            base.Initialize(info);

            startPosition = mainModel.localPosition;
            startRotation = mainModel.rotation;

            if (gameType == GameTypes.Game)
            {
                transform.Rotate(new Vector3(0, 180, 0));
            }

            eventsHandler.OnStarted += OnStarted;
            eventsHandler.OnFailed += OnFailed;
        }



        private void OnStarted()
        {
            if (gameType == GameTypes.Game)
            {
                mainModel.DORotate(new Vector3(0, 180, 0), rotateTime, RotateMode.WorldAxisAdd)
                .SetEase(rotateEase);
            }

        }
        private void OnFailed()
        {
            isFiring = false;
            superReady = false;
            superReadyUI = false;
            fireTime = 0f;
        }

        protected override void DoFire()
        {
            if (fireTime == 0 && superReady)
            {
                SuperShot();
                superReady = false;
                superReadyUI = false;
                prevFireTime = Time.time + 0.5f;

                OnReadyChanged?.Invoke(false);
                return;
            }

            isFiring = true;
            fireTime += Time.fixedDeltaTime;

            if (Time.time < prevFireTime)
            {
                return;
            }
            if(fireTime > settings.RushTime && !superReadyUI)
            {
                superReadyUI = true;
                OnReadyChanged?.Invoke(true);
            }

            BaseShot();


            prevFireTime = Time.time + ReloadTime(fireTime);
        }
        public override void EndFire()
        {
            if (fireTime > settings.RushTime)
            {
                superReady = true;
                fireTime = 0;
            }

            isFiring = false;
        }

        private void BaseShot()
        {
            RepairBulletBase bullet = Instantiate(settings.BulletPrefab, firePoint.position, firePoint.rotation, transform.parent).GetComponent<RepairBulletBase>();
            float tone = settings.ToneCurve.Evaluate(fireTime / settings.RushTime);
            bullet.Run(new RepairBulletInfo(bullet, settings.RepairCount, settings.BulletSpeed, settings.Volume, tone));

            mainModel.localPosition += firePoint.forward * 0.25f;
        }
        private void SuperShot()
        {
            RepairBulletBase bullet = Instantiate(settings.BulletPrefab, firePoint.position, firePoint.rotation, transform.parent).GetComponent<RepairBulletBase>();

            bullet.Run(new RepairBulletInfo(bullet, settings.RepairCount * 2, settings.BulletSpeed * 0.75f, settings.Volume, 1, true));
            bullet.transform.localScale *= 1.5f;

            mainModel.localPosition += firePoint.forward * 5f;

            soundPlayer.PlaySound("super_shot");
        }

        private void ReduseFireTime()
        {
            if(!isFiring && fireTime > 0)
            {
                fireTime -= Time.fixedDeltaTime * settings.ReturnSpeed;
            }
        }
        private void Recoil()
        {
            if (superReady && fireTime == 0f)
            {
                Vector3 random = Random.onUnitSphere * 0.4f;
                random.y = Mathf.Clamp01(random.y);
                mainModel.localPosition = startPosition + random;
            }
            else
            {
                float ratio = fireTime / settings.RushTime;
                float z = Mathf.Lerp(0, settings.MaxRecoil, settings.RecoilCurve.Evaluate(ratio));

                mainModel.localPosition = Vector3.Lerp(mainModel.localPosition, startPosition + firePoint.forward * z, 0.1f);
            }
        }


        private void FixedUpdate()
        {
            if (gameType == GameTypes.Game)
            {
                Recoil();
                ReduseFireTime();
            }
        }
    }
}
