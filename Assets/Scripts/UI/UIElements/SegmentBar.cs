using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace UI.Items
{
    public class SegmentBar : BarBase
    {
        [Header("Settings")]
        [SerializeField] private float fillTime;
        [Header("Components")]
        [SerializeField] private Transform bar;
        [SerializeField] private RectTransform segmentParent;
        [Space]
        [SerializeField] private List<RectTransform> segmentsTransform;
        [SerializeField] private List<RectTransform> circleTransform;

        private List<Segment> segments = new List<Segment>();
        private Tween tween;

        protected override void Fill(float value)
        {
            value = Mathf.Clamp01(value);

            if (tween != null)
            {
                tween.Kill();
            }

            Vector3 scale = new Vector3(value, bar.localScale.y, bar.localScale.z);
            tween = bar.DOScale(scale, fillTime).
                OnKill(() =>
                {
                    foreach (Segment segment in segments)
                    {
                        if (value >= segment.MaxFill)
                        {
                            segment.Filled = true;
                        }
                    }
                });
        }

        public void SetSegments(List<float> values)
        {
            if (values.Count == 0)
                return;

            segments = new List<Segment>();
            List<float> segmentsValue = new List<float>();
            segmentsValue.AddRange(values);
            segmentsValue.AddRange(new List<float>() { 0 });
            segmentsValue = segmentsValue.OrderBy(x => x).ToList();

            for(int i = 0; i < segmentsValue.Count - 1; i++)
            {
                segments.Add(new Segment(segmentsTransform[i], circleTransform[i], segmentsValue[i], segmentsValue[i + 1], segmentParent));
            }
        }

        private class Segment
        {
            private RectTransform segment;
            private RectTransform circle;
            private float minValue;
            private float maxValue;

            private float width;

            public Segment(RectTransform segment, RectTransform circle, float minValue, float maxValue, RectTransform parent)
            {
                this.segment = segment;
                this.circle = circle;

                this.minValue = minValue;
                this.maxValue = maxValue;

                float width = parent.rect.width;

                if (maxValue < 1)
                {
                    segment.gameObject.SetActive(true);
                    segment.anchoredPosition = new Vector2(width * maxValue, 0);
                }

                circle.gameObject.SetActive(false);
                circle.anchoredPosition = new Vector2(width * (maxValue + minValue) / 2, 0);
            }
        
            public bool Filled
            {
                set
                {
                    circle.localScale = Vector3.one * (value ? 0.5f : 1f);
                }
            }
            public float MaxFill
            {
                get => maxValue;
            }
        }
    }
}
