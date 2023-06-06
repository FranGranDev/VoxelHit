using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cannons;
using Services;

namespace Cannons.Controllers
{
    [RequireComponent(typeof(CannonBase))]
    public class Controller : MonoBehaviour, IBindable<IUserInput>, IBindable<IGameEventsHandler>
    {
        private CannonBase cannon;
        private bool fire;

        private GameStates gameState;

        private IUserInput input;
        private bool binded = false;
        private bool active;


        private void Awake()
        {
            cannon = GetComponent<CannonBase>();
        }


        public void Bind(IUserInput input)
        {
            this.input = input;

            input.OnTap += OnTap;
            input.OnTapEnded += OnTapEnded;

            binded = true;
        }
        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnStarted += LevelStarted;
            eventsHandler.OnDone += LevelDone;
            eventsHandler.OnFailed += LevelFailed;
            eventsHandler.OnRestart += LevelRestarted;

            eventsHandler.OnClearScene += (x) =>
            {
                eventsHandler.OnStarted -= LevelStarted;
                eventsHandler.OnDone -= LevelDone;
                eventsHandler.OnFailed -= LevelFailed;
            };
        }

        private void LevelRestarted()
        {
            try
            {
                this.Delayed(1f, () => active = true);
            }
            catch
            {
                active = true;
            }
        }

        private void LevelStarted()
        {
            fire = false;
            this.Delayed(Time.fixedDeltaTime, () => active = true);
        }
        private void LevelFailed()
        {
            active = false;
            fire = false;

            cannon.EndFire();
        }
        private void LevelDone()
        {
            active = false;
            fire = false;

            cannon.EndFire();
        }

        private void OnDestroy()
        {
            if (binded)
            {
                input.OnTap -= OnTap;
                input.OnTapEnded -= OnTapEnded;
            }
        }


        private void OnTapEnded()
        {
            if (!active)
            {
                return;
            }
            fire = false;
            cannon.EndFire();
        }

        private void OnTap()
        {
            if (!active)
            {
                return;
            }
            fire = true;
        }

        private void FixedUpdate()
        {
            if(fire)
            {
                cannon.Fire();
            }
        }

    }
}
