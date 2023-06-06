using Cannons.Bullets;
using DG.Tweening;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons
{
    public class FreeCannon : CannonBase, IBindable<IGameEventsHandler>, ISuperShotEvents
    {
        [Header("Touch Settings")]
        [SerializeField] private LayerMask layerMask;


        private Vector3 center;

        private bool isFiring;
        private float prevFireTime;
        private bool superReadyUI;


        public event Action<bool> OnReadyChanged;

        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnFailed += OnFailed;
        }
        public void Apply(Vector3 center)
        {
            this.center = center;
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
            if (fireTime > settings.RushTime && !superReadyUI)
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

            bullet.transform.localScale *= 2;
        }
        private void SuperShot()
        {
            RepairBulletBase bullet = Instantiate(settings.BulletPrefab, firePoint.position, firePoint.rotation, transform.parent).GetComponent<RepairBulletBase>();
            bullet.Run(new RepairBulletInfo(bullet, settings.RepairCount * 2, settings.BulletSpeed * 0.75f, settings.Volume, 1, true));

            soundPlayer.PlaySound("super_shot");
            bullet.transform.localScale *= 3;
        }
        private void ReduseFireTime()
        {
            if (!isFiring && fireTime > 0)
            {
                fireTime -= Time.fixedDeltaTime * settings.ReturnSpeed;
            }
        }


        private void MouseInput()
        {
            if(Input.GetKey(KeyCode.Mouse0))
            {
                UpdatePosition(Input.mousePosition);
            }
        }
        private void ScreenInput()
        {
            if (Input.touchCount > 0)
            {
                UpdatePosition(Input.GetTouch(0).position);
            }
        }
        private void UpdatePosition(Vector3 screenPoint)
        {

            Ray ray = Camera.main.ScreenPointToRay(screenPoint);
            if(Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask))
            {
                firePoint.position = hit.point;
            }
        }
        private void UpdateDirection()
        {
            firePoint.forward = (center - firePoint.position).normalized;
        }

        private void FixedUpdate()
        {
            if (gameType == GameTypes.Game)
            {
                ReduseFireTime();
            }
        }
        private void Update()
        {
#if UNITY_EDITOR
            MouseInput();
#else
            ScreenInput();       
#endif
            UpdateDirection();
        }
    }
}
