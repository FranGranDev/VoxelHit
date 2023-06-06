using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using DG.Tweening;


namespace UI.Items
{
    public class TutorHand : MonoBehaviour, ICursor
    {
        [Header("Animation")]
        [SerializeField] private float downTime;
        [SerializeField] private float upTime;
        [SerializeField] private float downScale;
        [SerializeField] private Ease ease = Ease.InOutSine;

        private Tween tween;
        private bool hidden;

        public Transform GetTransform
        {
            get => transform;
        }
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        public bool Disabled
        {
            get => !gameObject.activeSelf;
            set => gameObject.SetActive(!value);
        }
        public bool Hidden
        {
            get => hidden;
            set
            {
                hidden = value;

                Vector3 scale = value ? Vector3.zero : Vector3.one;
                if (Disabled && !value)
                {
                    Disabled = false;
                    transform.localScale = Vector3.zero;
                }


                if (tween.IsActive())
                {
                    tween.Kill();
                }

                tween = transform.DOScale(scale, 0.33f)
                    .SetEase(ease)
                    .OnComplete(() => tween = null);
            }
        }

        public void Down()
        {
            if (Hidden || Disabled)
                return;

            if(!tween.IsActive())
            {
                tween = transform.DOScale(Vector3.one * downScale, downTime)
                    .SetEase(ease)
                    .OnComplete(() => tween = null);
            }
        }

        public void Up()
        {
            if (Hidden || Disabled)
                return;

            if (!tween.IsActive())
            {
                tween.Kill();
            }

            tween = transform.DOScale(Vector3.one, upTime)
                .SetEase(ease)
                .OnComplete(() => tween = null);
        }
    }
}
