using Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traps
{
    public class StaticTrap : TrapBase
    {
        public StaticTrap(Transform target, Settings settings, MonoBehaviour monoBehaviour, AnimationData animation, BaseSettings baseSettings) : base(target, monoBehaviour, animation, baseSettings)
        {
            this.settings = settings;
        }
        private Settings settings;
        private Coroutine coroutine;

        public override void Disable()
        {
            base.Disable();

            StopCoroutine(coroutine);
        }

        public override void Enable()
        {
            base.Enable();

            coroutine = StartCoroutine(MoveCour());
        }

        private IEnumerator MoveCour()
        {
            while(IsShown)
            {
                yield return new WaitForSeconds(settings.ShowTime);

                Hide();

                yield return new WaitForSeconds(settings.HideTime);

                Show();

            }

            yield break;
        }
        


        [System.Serializable]
        public class Settings
        {
            public float HideTime;
            public float ShowTime;
        }
    }
}
