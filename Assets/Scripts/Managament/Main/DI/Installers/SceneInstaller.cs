using UnityEngine;
using Zenject;
using Services;


public class SceneInstaller : MonoInstaller
{
    [SerializeField] private Managament.SceneContext sceneContext;

    public override void InstallBindings()
    {
        Container.Bind<ISceneContext>()
            .To<Managament.SceneContext>()
            .FromInstance(sceneContext);


        Container.Bind<IGameEventsHandler>()
            .To<Managament.SceneContext>()
            .FromInstance(sceneContext);
    }
}