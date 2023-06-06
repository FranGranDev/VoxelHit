using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface Initializable<T, A>
    {
        public void Initialize(T arg1, A arg2);
    }
    public interface Initializable<T>
    {
        public void Initialize(T info);
    }


    public class GameInfo
    {
        public GameInfo(ISceneContext sceneContext, ComponentsInfo components, Color mainColor, AudioClip music)
        {
            SceneContext = sceneContext;
            MainColor = mainColor;
            Music = music;
            Components = components;
        }

        public Color MainColor { get; }
        public AudioClip Music { get; }
        public ISceneContext SceneContext { get; }
        public ComponentsInfo Components { get; }
    }
    public class ComponentsInfo
    {
        public ComponentsInfo(IMoneyController money, IMoneyController gems, ISoundPlayer soundPlayer, ITrackEvents eventsTracker, IAdsController ads, IHaptic haptic)
        {
            Money = money;
            Gems = gems;
            SoundPlayer = soundPlayer;
            EventsTracker = eventsTracker;
            Ads = ads;
            Haptic = haptic;
        }

        public IMoneyController Money { get; }
        public IMoneyController Gems { get; }
        public ISoundPlayer SoundPlayer { get; }
        public ITrackEvents EventsTracker { get; }
        public IAdsController Ads { get; }
        public IHaptic Haptic { get; }
    }
}
