using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Data;

namespace Voxel.Waves
{
    public class ModifyWaveBehavior : WaveBehavior
    {
        public ModifyWaveBehavior(MonoBehaviour monoBehaviour, WaveSetting settings, List<IGameVoxel> voxels, float height, float width, float size, ISoundPlayer soundPlayer, IHaptic haptic)
        : base(monoBehaviour, settings, voxels, height, width, size, soundPlayer, haptic)
        {

        }

        public override void Start(WaveParams parameter, int index, System.Action<int> onCompleate)
        {
            try
            {
                monoBehaviour.StartCoroutine(WaveCour(parameter as ModifyWaveParams, index, onCompleate));

                soundPlayer.PlaySound("wave");

                prevStartTime = Time.time;
            }
            catch { }
        }
        public override void Stop()
        {

        }

        private IEnumerator WaveCour(ModifyWaveParams parameter, int index, System.Action<int> onCompleate)
        {
            float maxCircleRadius = size;

            float time = 0f;
            var wait = new WaitForFixedUpdate();

            while (time < settings.SplashTime)
            {
                float radius = Mathf.Lerp(0, maxCircleRadius, time / settings.SplashTime);
                splashTime = time;

                foreach (IGameVoxel voxel in voxels)
                {
                    float min = Mathf.Max(radius - settings.SplashThreshold, 0);
                    float max = radius;

                    float distance = (voxel.Position - parameter.Center).magnitude;

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
    public class ModifyWaveParams : WaveParams
    {
        public ModifyWaveParams(Vector3 center) : base(Types.Modify)
        {
            Center = center;
        }

        public Vector3 Center { get; }
    }
}
