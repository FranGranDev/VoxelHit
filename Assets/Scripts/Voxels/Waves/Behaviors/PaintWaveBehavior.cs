using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;

namespace Voxel.Waves
{
    public class PaintWaveBehavior : WaveBehavior
    {
        public PaintWaveBehavior(MonoBehaviour monoBehaviour, WaveSetting settings, List<IGameVoxel> voxels, float height, float width, float size, ISoundPlayer soundPlayer, IHaptic haptic)
        : base(monoBehaviour, settings, voxels, height, width, size, soundPlayer, haptic)
        {

        }

        private bool stopped = false;

        public override void Start(WaveParams parameter, int index, System.Action<int> onCompleate)
        {
            try
            {
                stopped = false;

                monoBehaviour.StartCoroutine(WaveCour(parameter as PaintWaveParams, index, onCompleate));

                soundPlayer.PlaySound("paint_wave");
                haptic.VibratePop();
            }
            catch
            {
                Debug.LogError("Wave Params error");
            }

        }
        public override void Stop()
        {
            stopped = true;
            splashTime = 0f;
        }


        private IEnumerator WaveCour(PaintWaveParams parameter, int index, System.Action<int> onCompleate)
        {
            float maxCircleRadius = size;

            float time = 0f;

            var wait = new WaitForFixedUpdate();

            IEnumerable<IGameVoxel> filtered = voxels;//.Where(x => x.ColorIndex == parameter.Group);

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

                foreach (IGameVoxel voxel in filtered)
                {
                    float min = Mathf.Max(radius - settings.SplashThreshold, 0);
                    float max = radius;

                    float distance = (voxel.Position - parameter.Center).magnitude;


                    float power = 0;
                    if (distance >= min && distance <= max)
                    {
                        power = settings.ThresholdCurve.Evaluate(Mathf.InverseLerp(min, max, distance));
                        if (voxel.ColorIndex == parameter.Group)
                        {
                            parameter.OnApply?.Invoke(voxel);
                        }
                    }

                    float amplitude = settings.Amplitude * Mathf.Lerp(1, 0, radius / settings.MaxDistance);
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

    public class PaintWaveParams : WaveParams
    {
        public PaintWaveParams(Vector3 center, System.Action<IGameVoxel> onApply, int group, Types waveType) : base(waveType)
        {
            Group = group;
            Center = center;
            OnApply = onApply;
        }

        public Vector3 Center { get; }
        public int Group { get; }
        public System.Action<IGameVoxel> OnApply { get; }
    }
}
