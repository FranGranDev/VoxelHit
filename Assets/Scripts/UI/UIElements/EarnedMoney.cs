using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UI;
using TMPro;

namespace UI.Items
{
    public class EarnedMoney : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string text = "EARNED @";
        [SerializeField] private string replaceItem = "@";
        [SerializeField] private float delay;
        [SerializeField] private float fillTime;
        [Header("Loop Scale")]
        [SerializeField] private Transform scaleTarget;
        [SerializeField] private float period = 1f;
        [SerializeField] private float punchScale = 0.1f;
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private Transform coinPosition;

        private Tween tween;
        private int currantValue;
        private float extraScale;
        private bool stopped;

        public Vector3 CoinPosition => coinPosition.position;
        


        public void Fill(MonoBehaviour monoBehaviour, float value, System.Action onDone)
        {
            extraScale = 0;
            textMesh.text = "0";
            monoBehaviour.StartCoroutine(FillCour(value, onDone));
        }

        private IEnumerator FillCour(float value, System.Action onDone)
        {
            float time = 0;
            var wait = new WaitForFixedUpdate();

            yield return new WaitForSeconds(delay);

            while(time < fillTime)
            {
                float ratio = time / fillTime;

                currantValue = Mathf.RoundToInt(Mathf.Lerp(0, value, ratio));
                string earnedValue = currantValue.ToString();
                textMesh.text = text.Replace(replaceItem, earnedValue);

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            onDone?.Invoke();

            yield return new WaitForSeconds(0.25f);

            time = 0;
            while (gameObject.activeInHierarchy && !stopped)
            {
                scaleTarget.localScale = Vector3.one * (1 + Mathf.Sin(time * period) * punchScale) * (extraScale + 1f);
                time += Time.fixedDeltaTime;

                yield return wait;
            }

            yield break;
        }

        public void StopScale()
        {
            stopped = true;
        }

        public void AddReward(int value)
        {
            currantValue += value;
            textMesh.text = text.Replace(replaceItem, currantValue.ToString());

            if (tween == null)
            {
                tween = DOTween.To(() => extraScale, (x) => extraScale = x, 0.15f, 0.15f)
                    .SetEase(Ease.InOutBounce)
                    .OnComplete(() =>
                    {
                        tween = DOTween.To(() => extraScale, (x) => extraScale = x, 0f, 0.15f)
                            .SetEase(Ease.InOutBounce)
                            .OnKill(() =>
                            {
                                tween = null;
                                extraScale = 0;
                            });
                    });
            }
        }
    }
}
