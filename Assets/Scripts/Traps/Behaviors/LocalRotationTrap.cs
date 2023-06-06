using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animations;


namespace Traps
{
    public class LocalRotationTrap : TrapBase
    {
        public LocalRotationTrap(Transform target, Settings settings, MonoBehaviour monoBehaviour, AnimationData animation, BaseSettings baseSettings) : base(target, monoBehaviour, animation, baseSettings)
        {
            this.settings = settings;
        }
        private Settings settings;
        private Coroutine coroutine;

        private float currantSpeed;
        private float targetSpeed;


        public override void Disable()
        {
            base.Disable();

            targetSpeed = 0;
        }

        public override void Enable()
        {
            base.Enable();

            targetSpeed = 1f;
            if (coroutine == null)
            {
                coroutine = StartCoroutine(MoveCour());

                StartCoroutine(SpeedCour());
            }
        }

        private IEnumerator MoveCour()
        {
            if (settings.Angles.Count == 0)
                yield break;

            int index = 0;
            var wait = new WaitForFixedUpdate();


            while (true)
            {
                float time = 0;
                Vector3 prevRotation = transform.localRotation.eulerAngles;
                Vector3 nextRotation = prevRotation + RotationAxis * settings.Angles[index].Angle;


                yield return new WaitForSeconds(settings.Angles[index].Delay);

                while (time < settings.Angles[index].Time)
                {
                    float ratio = settings.MoveCurve.Evaluate(time / settings.Angles[index].Time);
                    transform.localRotation = Quaternion.Euler(Vector3.Lerp(prevRotation, nextRotation, ratio));

                    time += Time.fixedDeltaTime * currantSpeed;
                    yield return wait;
                }

                index++;
                if (index >= settings.Angles.Count)
                {
                    index = 0;
                }
            }
        }
        private IEnumerator SpeedCour()
        {
            var wait = new WaitForFixedUpdate();

            while (true)
            {
                currantSpeed = Mathf.Lerp(currantSpeed, targetSpeed, 0.025f);

                yield return wait;
            }
        }

        [System.Serializable]
        public class Settings
        {
            public List<AngleData> Angles;
            public AnimationCurve MoveCurve;

            [System.Serializable]
            public class AngleData
            {
                public float Delay;
                public float Angle;
                public float Time;
            }
        }
    }
}
