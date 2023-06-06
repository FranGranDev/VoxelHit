using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UI;
using TMPro;
using Cysharp.Threading.Tasks;

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
        


        public async void Fill(int value)
        {
            extraScale = 0;
            textMesh.text = "0";

            float time = 0;
            await UniTask.Delay(delay);

            while (time < fillTime)
            {
                float ratio = time / fillTime;

                currantValue = Mathf.RoundToInt(Mathf.Lerp(0, value, ratio));
                textMesh.text = text.Replace(replaceItem, currantValue.ToString());

                time += Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate();
            }

            currantValue = value;
            textMesh.text = text.Replace(replaceItem, currantValue.ToString());

            Scale();
        }
        private async void Scale()
        {
            await UniTask.Delay(0.25f);

            float time = 0;
            while (gameObject.activeInHierarchy && !stopped)
            {
                scaleTarget.localScale = Vector3.one * (1 + Mathf.Sin(time * period) * punchScale) * (extraScale + 1f);
                time += Time.fixedDeltaTime;

                await UniTask.WaitForFixedUpdate();
            }
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
