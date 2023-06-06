using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace UI.Items
{
    public class ProgressBar : BarBase
    {
        [Header("Settings")]
        [SerializeField] private float fillTime;
        [Header("Components")]
        [SerializeField] private Transform bar;

        private Tween tween;

        protected override void Fill(float value)
        {
            value = Mathf.Clamp01(value);

            if(tween != null)
            {
                tween.Kill();
            }

            Vector3 scale = new Vector3(value, bar.localScale.y, bar.localScale.z);
            tween = bar.DOScale(scale, fillTime);
        }
    }
}
