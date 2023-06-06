using System;
using TMPro;
using Services;
using UnityEngine;
using DG.Tweening;

namespace UI.Items
{
    public class ChangableMoney : MonoBehaviour, IChangeable<UIMoneyValue>
    {
        [SerializeField] private string text;
        [SerializeField] private string replaceItem = "@";
        [Header("Animation")]
        [SerializeField] private Transform target;
        [SerializeField] private AnimationTypes animationType;
        [SerializeField] private Ease ease;
        [SerializeField] private float time;
        [SerializeField] private float power;
        [SerializeField] private int vibro;


        private TextMeshProUGUI textMesh;
        private Tween tween;

        public void Bind(ref Action<UIMoneyValue> onChanged)
        {
            onChanged += UpdateText;

            textMesh = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void UpdateText(UIMoneyValue value)
        {
            string procent = value.ToString();
            string finalText = text.Replace(replaceItem, procent);

            textMesh.text = finalText;

            switch(animationType)
            {
                case AnimationTypes.PunchScale:
                    DoPunchScale();
                    break;
            }
        }

        private void DoPunchScale()
        {
            if(tween == null && target != null)
            {
                target.localScale = Vector3.one;
                tween = target.DOPunchScale(Vector3.one * power, time, vibro)
                    .SetEase(ease)
                    .OnKill(() => tween = null);
            }
        }

        public void SetValue(UIMoneyValue value)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            UpdateText(value);
        }

        private enum AnimationTypes { None, PunchScale}
    }
}
