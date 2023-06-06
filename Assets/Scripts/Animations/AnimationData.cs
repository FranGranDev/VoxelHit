using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Animations
{
    [System.Serializable]
    public class AnimationData
    {
        public AnimationData Clone(AnimationData other)
        {
            return new AnimationData(other.time, other.delay, other.animationCurves);
        }
        public AnimationData(float time, float delay, List<AnimationCurve> curves)
        {
            this.delay = delay;
            this.time = time;

            animationCurves = curves;
        }

        [SerializeField] private float time = 0.5f;
        [SerializeField] private float delay = 0f;
        [SerializeField] private List<AnimationCurve> animationCurves;

        public float Time
        {
            get => time;
            set => time = value;
        }
        public float Delay
        {
            get => delay;
            set => delay = value;
        }
        public List<AnimationCurve> AnimationCurves => animationCurves;
    }
}
