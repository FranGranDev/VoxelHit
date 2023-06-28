using UnityEngine;
using Zenject;
using Services;
using UserInput;

public class TouchInputInstaller : MonoInstaller
{
    [SerializeField] private GameObject input;

    public override void InstallBindings()
    {
        Container.Bind<IUserInput>()
            .To<TouchInput>()
            .FromComponentOn(input)
            .AsSingle();

        Container.Bind<IDragInput>()
            .To<DragInput>()
            .FromComponentOn(input)
            .AsSingle();
    }
}