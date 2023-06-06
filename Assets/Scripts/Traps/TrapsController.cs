using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Services;
using Animations;


namespace Traps
{
    public class TrapsController : MonoBehaviour, Initializable<GameInfo>, IBindable<IGameEventsHandler>, IBindable<IFillEvent>
    {
        [Header("Settings")]
        [SerializeField] private List<Data> trapsData;
        [SerializeField] private float trapSwitchDelay = 0.5f;
        [Space, SerializeField] private AnimationData showHideAnimation;
        [Header("States")]
        [SerializeField] private int stateIndex;
        [SerializeField, Range(0, 1)] private float currantFill;

        private List<State> trapStates;
        private Coroutine cycleCoroutine;


        protected GameStates gameState;

        private System.Action onDestroy;
        private int StateIndex
        {
            get => stateIndex;
            set
            {
                stateIndex = value;
                if(stateIndex >= trapStates.Count)
                {
                    stateIndex = 0;
                    currantFill = 0;
                }
            }
        }
        private State CurrantState
        {
            get
            {                
                return trapStates[StateIndex];
            }
        }
        public List<float> FillValues
        {
            get
            {
                return trapsData
                            .Where(x => x.IsEnabled)
                            .ToList()
                            .ConvertAll(x => x.MaxFill);
            }
        }



        public void Initialize(GameInfo info)
        {
            trapStates = new List<State>();
            foreach(Data data in trapsData)
            {
                if (!data.IsEnabled)
                    return;
                trapStates.Add(new State(data, this, showHideAnimation));
            }

            StartCycle();
        }
        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnStateChanged += OnStateChanged;
            eventsHandler.OnDone += LevelDone;
            eventsHandler.OnFailed += LevelFailed;
            eventsHandler.OnRestart += LevelRestarted;
            eventsHandler.OnStarted += LevelStarted;

            eventsHandler.OnClearScene += (x) =>
            {
                eventsHandler.OnStateChanged -= OnStateChanged;
                eventsHandler.OnDone -= LevelDone;
                eventsHandler.OnFailed -= LevelFailed;
                eventsHandler.OnRestart -= LevelRestarted;
                eventsHandler.OnStarted -= LevelStarted;
            };
        }
        public void Bind(IFillEvent fillEvent)
        {
            fillEvent.OnFilled += OnFilled;

            OnFilled(0);
        }

        private void OnStateChanged(GameStates gameState)
        {
            this.gameState = gameState;
        }


        private void OnFilled(float value)
        {
            currantFill = value;   
        }


        private void LevelStarted()
        {
            
        }
        private void LevelRestarted()
        {
            StartCycle();
        }
        private void LevelFailed()
        {
            StopCycle();
        }
        private void LevelDone()
        {
            //StopCycle();
        }


        private void StartCycle()
        {
            StopCycle();

            cycleCoroutine = StartCoroutine(TrapCycleCour());
        }
        private void StopCycle()
        {
            if (cycleCoroutine != null)
            {
                StopCoroutine(cycleCoroutine);
                cycleCoroutine = null;
            }

            foreach (State state in trapStates)
            {
                state.Traps.ForEach(x => x.Disable());
            }
        }
        private IEnumerator TrapCycleCour()
        {
            if (trapStates.Count == 0)
            {
                cycleCoroutine = null;
                yield break;
            }


            while (true)
            {
                CurrantState.Traps.ForEach(x => x.Enable());
                yield return new WaitWhile(() => currantFill <= CurrantState.MaxFill);
                CurrantState.Traps.ForEach(x => x.Disable());
                StateIndex++;
                yield return new WaitForSeconds(trapSwitchDelay);
            }
        }


        public class State
        {
            public State(Data data, MonoBehaviour monoBehaviour, AnimationData animationData)
            {
                MaxFill = data.MaxFill;

                Traps = new List<ITrap>();

                foreach(TrapData trap in data.TrapsData)
                {
                    switch (trap.Type)
                    {
                        case TrapTypes.Static:
                            Traps.Add(new StaticTrap(trap.Target, trap.SettingsStatic, monoBehaviour, animationData, trap.baseSettings));
                            break;
                        case TrapTypes.StaticRotation:
                            Traps.Add(new StaticRotationTrap(trap.Target, trap.SettingsStaticRotation, monoBehaviour, animationData, trap.baseSettings));
                            break;
                        case TrapTypes.ComplexRotation:
                            Traps.Add(new ComplextRotationTrap(trap.Target, trap.SettingsComplextRotation, monoBehaviour, animationData, trap.baseSettings));
                            break;
                        case TrapTypes.LocalRotation:
                            Traps.Add(new LocalRotationTrap(trap.Target, trap.SettingsLocalRotation, monoBehaviour, animationData, trap.baseSettings));
                            break;
                    }
                }
            }

            public List<ITrap> Traps { get; }
            public float MaxFill { get; }
        }
        [System.Serializable]
        public class Data
        {
            public string Name = "New Trap";
            public bool IsEnabled = true;
            [Space, Range(0, 1)]
            public float MaxFill;

            public List<TrapData> TrapsData;
        }
        [System.Serializable]
        public class TrapData
        {
            public TrapTypes Type;
            public Transform Target;
            public TrapBase.BaseSettings baseSettings;

            [Header("For Local Rotation")]
            public LocalRotationTrap.Settings SettingsLocalRotation;

            [Header("For Static Rotation")]
            public StaticRotationTrap.Settings SettingsStaticRotation;

            [Header("For Static")]
            public StaticTrap.Settings SettingsStatic;

            [Header("For Complex Rotation")]
            public ComplextRotationTrap.Settings SettingsComplextRotation;
        }

        public enum TrapTypes { Static, StaticRotation, ComplexRotation, LocalRotation}
    }
}
