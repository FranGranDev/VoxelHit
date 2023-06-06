using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Data;

namespace Voxel.Waves
{
    public class SingleWaveBehavior : WaveBehavior
    {
        public SingleWaveBehavior(MonoBehaviour monoBehaviour, WaveSetting settings, List<IGameVoxel> voxels, float height, float width, float size, ISoundPlayer soundPlayer, IHaptic haptic)
        : base(monoBehaviour, settings, voxels, height, width, size, soundPlayer, haptic)
        {

        }

        private bool stopped = false;

        public override void Start(WaveParams parameter, int index, System.Action<int> onCompleate)
        {
            stopped = false;

            if (prevStartTime + settings.MinDeltaTime > Time.time)
            {
                prevStartTime = Time.time;
                onCompleate?.Invoke(index);
                return;
            }
                

            monoBehaviour.StartCoroutine(WaveCour(parameter, index, onCompleate));

            soundPlayer.PlaySound("wave");

            prevStartTime = Time.time;
        }
        public override void Stop()
        {
            stopped = true;
            splashTime = 0f;
        }

        private IEnumerator WaveCour(WaveParams parameter, int index, System.Action<int> onCompleate)
        {
            float maxCircleRadius = size;

            float time = 0f;
            var wait = new WaitForFixedUpdate();

            Vector3 center = new Vector3(settings.Center.x * width, settings.Center.y * height, 0);

            while (time < settings.SplashTime)
            {
                if (stopped)
                {
                    foreach (IGameVoxel voxel in voxels)
                    {
                        voxel.SetWaveScale(0, index);
                    }
                    break;
                }

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
