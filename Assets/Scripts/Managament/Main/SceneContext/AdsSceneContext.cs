using Factory;
using Services;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Traps;
using UnityEngine;

namespace Managament.Ads
{
    public class AdsSceneContext : SceneContext
    {
        protected override void SceneInitilize()
        {
            IFillEvent fillEvent = GetComponentInChildren<IFillEvent>();
            transform.GetComponentsInChildren<IBindable<IFillEvent>>(true).ToList()
                .ForEach(x => x.Bind(fillEvent));

            targetEventHandlers = transform.GetComponentsInChildren<ITargetEventHandler>(true).
                Where(x => !x.Equals(this)).ToList();
            targetEventHandlers.ForEach(x =>
            {
                x.OnDone += OnLevelCompleate;
                x.OnFailed += OnLevelFailed;
            });


            List<IBindable<IGameEventsHandler>> eventListeners = transform.GetComponentsInChildren<IBindable<IGameEventsHandler>>(true).ToList();

            eventListeners.ForEach(x => x.Bind(this));


            List<Initializable<GameInfo>> initializables = new List<Initializable<GameInfo>>(
                transform.GetComponentsInChildren<Initializable<GameInfo>>(true));

            GameInfo info = new GameInfo(this, Components, Color.white, baseMusic);
            initializables.ForEach(x =>
            {
                x.Initialize(info);
            });
            CallOnLocalInitialize(info);


            if (autoStart)
            {
                GameState = GameStates.Game;
            }
            else
            {
                GameState = GameStates.Idle;
            }
        }

        public override void Visit(ISceneVisitor visitor)
        {
            
        }
    }
}
