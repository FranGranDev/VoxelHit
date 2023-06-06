using Cannons.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons
{
    public class AdsCannon : MonoBehaviour
    {
        [SerializeField] private CannonBase.Settings settings;
        [SerializeField] private Transform firePoint;
        [Space]
        [SerializeField] private Transform center;

        private Vector3 position;
        private bool firing;
        private float fireTime;
        private float prevFireTime;
        public float ReloadTime(float fireTime = 0)
        {
            if (fireTime > settings.RushTime)
                return 1f / (settings.MaxFireRate * settings.RushRateRatio);
            float ratio = Mathf.Clamp01(fireTime / settings.RushTime);
            return 1f / Mathf.Lerp(settings.FireRate, settings.MaxFireRate, settings.RushCurve.Evaluate(ratio));
        }



        private void MouseInput()
        {
            if(Input.GetMouseButton(0))
            {
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000, LayerMask.GetMask("Ground")))
                {
                    position = hit.point;
                    firing = true;
                }
                else
                {
                    firing = false;
                }
            }
            else
            {
                firing = false;
            }
        }

        private void DoFire()
        {
            fireTime += Time.fixedDeltaTime;


            if (Time.time < prevFireTime)
            {
                return;
            }


            Vector3 direction = (center.position - position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            float tone = settings.ToneCurve.Evaluate(fireTime / settings.RushTime);

            RepairBulletBase bullet = Instantiate(settings.BulletPrefab, position, rotation, transform.parent).GetComponent<RepairBulletBase>();
            bullet.Run(new RepairBulletInfo(bullet, settings.RepairCount, settings.BulletSpeed, 1, tone));


            prevFireTime = Time.time + ReloadTime(fireTime);
        }
        private void EndFire()
        {
            fireTime = 0;
        }


        void Update()
        {
            MouseInput();
        }
        private void FixedUpdate()
        {
            if(firing)
            {
                DoFire();
            }
            else
            {
                EndFire();
            }
        }
    }
}
