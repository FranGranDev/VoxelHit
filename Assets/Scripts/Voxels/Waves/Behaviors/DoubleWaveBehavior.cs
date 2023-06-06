using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;

namespace Voxel.Waves
{
    public class DoubleWaveBehavior : WaveBehavior
    {
        public DoubleWaveBehavior(MonoBehaviour monoBehaviour, WaveSetting settings, List<IGameVoxel> voxels, float height, float width, float size,ISoundPlayer soundPlayer, IHaptic haptic) : base(monoBehaviour, settings, voxels, height, width, size, soundPlayer, haptic)
        {

        }

        private bool stopped;


        public override void Start(WaveParams parameter, int index, System.Action<int> onCompleate)
        {
            stopped = false;

            monoBehaviour.StartCoroutine(DoubleWaveCour(parameter, index, onCompleate));

            if (soundPlayer != null)
            {
                soundPlayer.PlaySound("wave");
                soundPlayer.PlaySound("wave", 1, 1, settings.SplashTime);

                haptic.VibrateHaptic();
                monoBehaviour.Delayed(settings.SplashTime, () => haptic.VibrateHaptic());
            }
        }
        public override void Stop()
        {
            stopped = true;
            splashTime = 0;
        }

        private IEnumerator DoubleWaveCour(WaveParams parameters, int index, System.Action<int> onCompleate)
        {
            splashTime = 0;

            WaveSetting first = settings.Clone() as WaveSetting;
            first.Center = new Vector3(0, 0, 0);

            WaveSetting second = settings.Clone() as WaveSetting;
            second.Center = new Vector3(0, height, 0);

            yield return monoBehaviour.StartCoroutine(WaveCour(first, parameters, index));
            yield return monoBehaviour.StartCoroutine(WaveCour(second, parameters, index));

            onCompleate?.Invoke(index);
            splashTime = 0;

            yield break;
        }
        private IEnumerator WaveCour(WaveSetting settings, WaveParams parameters, int index)
        {
            float maxCircleRadius = size;

            float time = 0f;
            splashTime = 0;

            var wait = new WaitForFixedUpdate();

            while (time < settings.SplashTime)
            {
                if(stopped)
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

                    float distance = (voxel.Position - settings.Center).magnitude;

                    float power = 0;
                    if (distance > min && distance < max)
                    {
                        power = settings.ThresholdCurve.Evaluate(Mathf.InverseLerp(min, max, distance));
                    }

                    float amplitude = settings.Amplitude * Mathf.Lerp(1, 0, radius / settings.MaxDistance);
                    voxel.SetWaveScale(Mathf.Lerp(0, amplitude, power), index);
                }

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            splashTime = 0;
            yield break;
        }

    }
}
