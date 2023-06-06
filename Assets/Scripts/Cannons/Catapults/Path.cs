using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons.Catapults
{
    public class Path
    {
        private const int CALC_STEP_COUNT = 25;

        public Path(Vector3 startPoint, Vector3 endPoint, Vector3 normal, float topHeight, AnimationCurve curve)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Normal = normal;
            TopHeight = topHeight;
            Curve = curve;
        }

        public Vector3 StartPoint { get; set; }
        public Vector3 EndPoint { get; }
        public Vector3 Normal { get; }
        public float TopHeight { get; }
        public AnimationCurve Curve { get; }

        public Vector3 GetPoint(float normalizedTime)
        {
            return Vector3.Lerp(StartPoint, EndPoint, normalizedTime) + Curve.Evaluate(normalizedTime) * Normal * TopHeight;
        }
        public float GetDistance()
        {
            float distance = 0;
            int stepCount = CALC_STEP_COUNT;
            Vector3 prevPos = StartPoint;
            for (int i = 1; i < stepCount; i++)
            {
                float ratio = (float)i / (float)stepCount;
                Vector3 point = Vector3.Lerp(StartPoint, EndPoint, ratio);
                point += Normal * TopHeight * Curve.Evaluate(ratio);

                distance += (prevPos - point).magnitude;
                prevPos = point;
            }

            return distance;
        }
    }
}
