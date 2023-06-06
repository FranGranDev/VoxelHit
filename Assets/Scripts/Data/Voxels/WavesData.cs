using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Waves Data", menuName = "Game Data/Waves Data")]
    public class WavesData : ScriptableObject
    {
        public WaveSetting singleWaveSettings;
        public WaveSetting hitWaveSettings;
        public WaveSetting miniHitWaveSettings;
        public WaveSetting doubleWaveSettings;
        public WaveSetting paintWaveSettings;
        public WaveSetting puzzleWaveSettings;
    }

    [System.Serializable]
    public class WaveSetting : ICloneable
    {
        public float SplashTime;
        [Space]
        public float MaxDistance;
        public AnimationCurve MaxDistanceCurve;
        [Space]
        public float Amplitude;
        [Space]
        public float SplashThreshold;
        public AnimationCurve ThresholdCurve;
        [Space]
        public float MinDeltaTime;
        [Space]
        public Vector3 Center;

        private WaveSetting(WaveSetting other)
        {
            SplashTime = other.SplashTime;
            MaxDistance = other.MaxDistance;
            MaxDistanceCurve = other.MaxDistanceCurve;
            Amplitude = other.Amplitude;
            SplashThreshold = other.SplashThreshold;
            ThresholdCurve = new AnimationCurve(other.ThresholdCurve.keys);
            Center = other.Center;
        }

        public object Clone()
        {
            return new WaveSetting(this);
        }
    }
}
