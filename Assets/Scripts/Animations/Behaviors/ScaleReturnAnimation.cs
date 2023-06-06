using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{
    public class ScaleReturnAnimation : AnimationBase
    {
        private Vector3 startScale;

        private void Awake()
        {
            startScale = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        public override void Play()
        {
            gameObject.SetActive(true);
            StartCoroutine(ScaleCour());
        }

        private IEnumerator ScaleCour()
        {
            yield return new WaitForSeconds(animationData.Delay);

            float time = 0;
            var wait = new WaitForFixedUpdate();

            while(time < animationData.Time)
            {
                float ratio = time / animationData.Time;

                transform.localScale = Vector3.Lerp(Vector3.zero, startScale, animationData.AnimationCurves[0].Evaluate(ratio));


                time += Time.fixedDeltaTime;
                yield return wait;
            }

            yield break;
        }
    }
}
