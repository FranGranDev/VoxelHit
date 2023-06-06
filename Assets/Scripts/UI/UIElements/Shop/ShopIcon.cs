using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


namespace UI.Items
{
    public class ShopIcon : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float selectedScale;
        [SerializeField] private Color baseColor;
        [SerializeField] private Color selectedColor;
        [Header("Open Animation")]
        [SerializeField] private float openTime;
        [SerializeField] private Ease openEase;
        [Header("Opening Animation")]
        [SerializeField] private float openingTime;
        [SerializeField] private Ease openingEase;
        [SerializeField] private int openingVibro;
        [Header("Components")]
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private Transform locked;


        private Tween openingTween;

        private int index;
        private bool opened;
        private System.Action<int> onClick;

        public int Index
        {
            get => index;
        }
        public bool Selected
        {
            set
            {
                Color color = value ? selectedColor : baseColor;
                float scale = value ? selectedScale : 1f;

                background.color = color;
                background.transform.localScale = Vector3.one * scale;
            }
        }
        public bool Opened
        {
            get => opened;
            set
            {
                opened = value;
                if(value)
                {
                    locked.gameObject.SetActive(false);
                }
            }
        }

        public void Initialize(int index, Sprite icon, System.Action<int> onClick)
        {
            this.onClick += onClick;
            this.icon.sprite = icon;
            this.index = index;
        }

        public void Open()
        {
            if (opened)
                return;
            opened = true;

            locked.DOScale(Vector3.zero, openTime)
                .SetEase(openEase)
                .OnComplete(() => locked.gameObject.SetActive(false));
        }
        public void OpeningAnimation()
        {
            if (opened || openingTween.IsActive())
                return;

            openingTween = locked.DOPunchScale(Vector3.one * 0.25f, openingTime, openingVibro)
                .SetEase(openingEase)
                .OnComplete(() => openingTween = null);
        }

        public void Click()
        {
            if (!opened)
                return;
            onClick?.Invoke(index);
        }
    }
}
