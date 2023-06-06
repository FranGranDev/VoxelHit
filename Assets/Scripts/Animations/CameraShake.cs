using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Setting")]
        [SerializeField] private float shakeTime;
        [SerializeField] private float maxShakeAngle;
        [SerializeField] private AnimationCurve shakeCurve;
        [Header("Components")]
        [SerializeField] private Transform main;

        public void DoShake()
        {
            if (ShakeCoroutine == null)
            {
                ShakeCoroutine = StartCoroutine(ShakeCour(1));
            }
        }
        public void DoShake(float Power)
        {
            if (ShakeCoroutine == null)
            {
                ShakeCoroutine = StartCoroutine(ShakeCour(Power));
            }
        }

        private Coroutine ShakeCoroutine;
        private IEnumerator ShakeCour(float Power)
        {
            float shakeTime = this.shakeTime * Power;
            float currantTime = 0;

            float Angle = 0;

            Quaternion prevRotation = main.rotation;

            while (currantTime < shakeTime)
            {
                Angle = shakeCurve.Evaluate(currantTime / shakeTime) * maxShakeAngle * Power * Mathf.Deg2Rad;

                main.rotation = new Quaternion(main.rotation.x, main.rotation.y, prevRotation.z + Angle, main.rotation.w);


                currantTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            main.rotation = prevRotation;

            ShakeCoroutine = null;
            yield break;
        }

        private void Start()
        {
            if (main == null)
            {
                main = transform;
            }
        }
    }
}
