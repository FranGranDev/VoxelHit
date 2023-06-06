using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animations;

namespace Traps
{
    public class StaticRotationTrap : TrapBase
    {
        public StaticRotationTrap(Transform target, Settings settings, MonoBehaviour monoBehaviour, AnimationData animation, BaseSettings baseSettings) : base(target, monoBehaviour, animation, baseSettings)
        {
            this.settings = settings;
        }
        private Settings settings;
        private Coroutine coroutine;

        float currantSpeed = 0;

        public override void Disable()
        {
            base.Disable();

            currantSpeed = 0;
        }

        public override void Enable()
        {
            base.Enable();

            currantSpeed = settings.RotationSpeed;
            if (coroutine == null)
            {
                coroutine = StartCoroutine(MoveCour());
            }
        }

        private IEnumerator MoveCour()
        {
            var wait = new WaitForFixedUpdate();

            float speed = 0;

            while (true)
            {
                speed = Mathf.Lerp(speed, currantSpeed, 0.075f);

                transform.Rotate(RotationAxis, speed * Time.fixedDeltaTime);
                yield return wait;
            }
        }



        [System.Serializable]
        public class Settings
        {
            public float RotationSpeed;
        }
    }
}
