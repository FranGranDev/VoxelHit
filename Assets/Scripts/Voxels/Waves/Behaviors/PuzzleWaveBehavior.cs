using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;

namespace Voxel.Waves
{
    public class PuzzleWaveBehavior : WaveBehavior
    {
        public PuzzleWaveBehavior(MonoBehaviour monoBehaviour, WaveSetting settings, List<IGameVoxel> voxels, float height, float width, float size, ISoundPlayer soundPlayer, IHaptic haptic)
        : base(monoBehaviour, settings, voxels, height, width, size, soundPlayer, haptic)
        {

        }


        public override void Start(WaveParams parameter, int index, System.Action<int> onCompleate)
        {
            monoBehaviour.StartCoroutine(WaveCour(parameter, index, onCompleate));

            soundPlayer.PlaySound("wave");

            prevStartTime = Time.time;
        }
        public override void Stop()
        {

        }

        private IEnumerator WaveCour(WaveParams parameter, int index, System.Action<int> onCompleate)
        {
            float maxCircleRadius = size;

            float time = 0f;
            var wait = new WaitForFixedUpdate();

            Vector3 center = new Vector3(0, height, 0) / 2;

            while (time < settings.SplashTime)
            {
                float radius = Mathf.Lerp(0, maxCircleRadius, time / settings.SplashTime);
                splashTime = time;

                foreach (IGameVoxel voxel in voxels)
                {
                    float min = Mathf.Max(radius - settings.SplashThreshold, 0);
                    float max = radius;

                    float distance = (voxel.Position - center).magnitude;

                    float power = 0;
                    if (distance > min && distance < max)
                    {
                        power = settings.ThresholdCurve.Evaluate(Mathf.InverseLerp(min, max, distance));
                    }

                    float amplitude = settings.Amplitude * Mathf.Lerp(1, 0, settings.MaxDistanceCurve.Evaluate(radius / settings.MaxDistance));
                    voxel.SetWaveScale(Mathf.Lerp(0, amplitude, power), index);
                }

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            splashTime = 0f;
            onCompleate?.Invoke(index);


            yield break;
        }


    }

}
