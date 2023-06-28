using UnityEngine;
using Zenject;
using Audio;
using Services;
using Ads;
using Vibration;

namespace Managament.DI
{
    public class ComponentsInstaller : MonoInstaller
    {
        [SerializeField] private SoundPlayer soundPlayer;
        [SerializeField] private AdsController adsController;

        public override void InstallBindings()
        {
            Container.Bind<ISoundPlayer>()
                .To<SoundPlayer>()
                .FromInstance(soundPlayer)
                .NonLazy();

            Container.Bind<IAdsController>()
                .To<AdsController>()
                .FromInstance(adsController)
                .NonLazy();

            Container.Bind<IHaptic>()
                .To<Haptic>()
                .AsSingle()
                .NonLazy();

            Container.Bind<IMoneyController>()
                .WithId("Coin")
                .FromInstance(new MoneyController(MoneyController.Types.Coin))
                .WithArguments(MoneyController.Types.Coin)
                .NonLazy();

            Container.Bind<IMoneyController>()
                .WithId("Gem")
                .FromInstance(new MoneyController(MoneyController.Types.Gem))
                .WithArguments(MoneyController.Types.Gem)
                .NonLazy();

            Container.Bind<ITrackEvents>()
                .To<EventsTracker>()
                .AsSingle()
                .NonLazy();
        }
    }
}