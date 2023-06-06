using System;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;

namespace Voxel.Waves
{
    public abstract class WaveBehavior : IWave
    {
        public WaveBehavior(MonoBehaviour monoBehaviour, WaveSetting settings, List<IGameVoxel> voxels, float height, float width, float size, ISoundPlayer soundPlayer, IHaptic haptic)
        {
            this.monoBehaviour = monoBehaviour;
            this.soundPlayer = soundPlayer;
            this.settings = settings;
            this.haptic = haptic;
            this.height = height;
            this.width = width;
            this.voxels = voxels;
            this.size = size;
        }

        protected readonly List<IGameVoxel> voxels;
        protected readonly ISoundPlayer soundPlayer;
        protected readonly IHaptic haptic;
        protected readonly MonoBehaviour monoBehaviour;
        protected readonly WaveSetting settings;
        protected readonly float height;
        protected readonly float width;
        protected readonly float size;
        protected float splashTime;

        protected float prevStartTime = 0;


        public abstract void Start(WaveParams parameter, int index, Action<int> onCompleate);
        public abstract void Stop();
        public float State
        {
            get => splashTime / settings.SplashTime;
        }
    }

    public interface IWave
    {
        public void Start(WaveParams parameter, int index, System.Action<int> onCompleate);
        public void Stop();
        public float State { get; }
    }



    public class WaveParams
    {
        public WaveParams(Types waveType)
        {
            WaveType = waveType;
        }

        public virtual Types WaveType { get; }


        public enum Types { Single, Hit, MiniHit, Double, Paint, Center, Modify }

    }

}
