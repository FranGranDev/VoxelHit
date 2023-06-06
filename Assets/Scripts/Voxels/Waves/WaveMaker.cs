using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using System.Linq;
using NaughtyAttributes;
using System;
using Data;

namespace Voxel.Waves
{
    public class WaveMaker : MonoBehaviour
    {
        [SerializeField] private bool isEnabled = true;
        [Space]
        [SerializeField] private WavesData wavesData;


        private ISoundPlayer soundPlayer;
        private IHaptic haptic;
        private List<IGameVoxel> voxels;
        private Dictionary<WaveParams.Types, IWave> wavesDict;

        private float size;
        private int height;
        private int width;
        private Vector3 minBounds;
        private Vector3 maxBounds;

        private int WavesCount
        {
            get => wavesCount;
            set
            {
                if(value < 0)
                {
                    Debug.LogError("Waves count less then zero");
                    value = 0;
                }
                wavesCount = value;
            }
        }
        [SerializeField] private int wavesCount;


        public void Initilize(List<IGameVoxel> voxels, ISoundPlayer soundPlayer, IHaptic haptic)
        {
            this.voxels = voxels;
            this.soundPlayer = soundPlayer;
            this.haptic = haptic;
            
            minBounds = new Vector3(voxels.Min(x => x.Position.x), voxels.Min(x => x.Position.y), 0);
            maxBounds = new Vector3(voxels.Max(x => x.Position.x), voxels.Max(x => x.Position.y), 0);

            height = Mathf.RoundToInt(maxBounds.y - minBounds.y);
            width = Mathf.RoundToInt(maxBounds.x - minBounds.x);

            size = Mathf.Sqrt(height * height + width * width);

            wavesDict = new Dictionary<WaveParams.Types, IWave>()
            {
                {WaveParams.Types.Single, new SingleWaveBehavior(this, wavesData.singleWaveSettings, voxels, height, width, size, soundPlayer, haptic) },
                {WaveParams.Types.Hit, new SingleWaveBehavior(this, wavesData.hitWaveSettings, voxels, height, width, size, soundPlayer, haptic) },
                {WaveParams.Types.MiniHit, new MiniWaveBehavior(this, wavesData.miniHitWaveSettings, voxels, height, width, size, soundPlayer, haptic) },
                {WaveParams.Types.Double, new DoubleWaveBehavior(this, wavesData.doubleWaveSettings, voxels, height, width, size, soundPlayer, haptic) },
                {WaveParams.Types.Paint, new PaintWaveBehavior(this, wavesData.paintWaveSettings, voxels, height, width, size, soundPlayer, haptic) },
                {WaveParams.Types.Center, new PuzzleWaveBehavior(this, wavesData.singleWaveSettings, voxels, height, width, size, soundPlayer, haptic) },
                {WaveParams.Types.Modify, new ModifyWaveBehavior(this, wavesData.singleWaveSettings, voxels, height, width, size, soundPlayer, haptic) },
            };
        }
        public void MakeWave(WaveParams parameters)
        {
            if (!isEnabled)
                return;

            switch(parameters.WaveType)
            {
                case WaveParams.Types.MiniHit:
                    if(wavesDict[WaveParams.Types.Hit].State > 0.4f || wavesDict[WaveParams.Types.Hit].State == 0)
                    {
                        wavesDict[parameters.WaveType].Start(parameters, WavesCount++, (x) => WavesCount--);
                    }
                    break;
                default:
                    wavesDict[parameters.WaveType].Start(parameters, WavesCount++, (x) => WavesCount--);
                    break;
            }
        }
        public void StopAllWaves()
        {
            foreach(IWave wave in wavesDict.Values)
            {
                wave.Stop();
            }
        }

        #region Internal
        [Button("Make Simple Wave")]
        public void MakeSimpleWave()
        {
            MakeWave(new WaveParams(WaveParams.Types.Single));
        }
        [Button("Make Hit Wave")]
        public void MakeHitWave()
        {
            MakeWave(new WaveParams(WaveParams.Types.Hit));
        }
        [Button("Make Double Wave")]
        public void MakeDoubleWave()
        {
            MakeWave(new WaveParams(WaveParams.Types.Double));
        }


#endregion
    }
}
