using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace UI.Items
{
    public class BulletIndicator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Color activeColor;
        [SerializeField] private Color hiddenColor;
        [Header("Components")]
        [SerializeField] private Image icon;

        private bool hidden;

        public bool Hidden
        {
            get => hidden;
            set
            {
                if (hidden == value)
                    return;
                hidden = value;

                Color color = value ? hiddenColor : activeColor;

                DOTween.To(() => icon.color, (x) => icon.color = x, color, 0.5f)
                    .SetEase(Ease.InOutSine);
            }
        }

        public BulletIndicator Initialize(bool hidden = false)
        {
            Hidden = hidden;

            return this;
        }
    }
}
