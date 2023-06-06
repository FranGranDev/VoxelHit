using NaughtyAttributes;
using Services;
using System.Collections;
using Data;
using System.Collections.Generic;
using System.Linq;
using UI.Items;
using UnityEngine;

namespace UI
{
    public class EventRoadUI : MonoBehaviour, IBindable<IUserInput>
    {
        [Foldout("Menus"), SerializeField] private Transform mainMenu;
        [Foldout("Menus"), SerializeField] private Transform doneMenu;
        [Foldout("Menus"), SerializeField] private Transform allDoneMenu;

        [Foldout("Buttons"), SerializeField] private List<ButtonUI> exitButtons;

        [Foldout("Links"), SerializeField] private UIPanel nextEventPanel;

        [Foldout("State"), SerializeField] private States state;


        private Dictionary<States, IEnumerable<UIPanel>> menuPanels;
        private ISoundPlayer soundPlayer;
        private IAdsController adsController;

        public States State
        {
            get => state;
            set
            {
                if (!menuPanels.ContainsKey(value))
                {
                    state = value;
                    return;
                }

                OnStateEnd(state);
                state = value;
                OnStateStart(state);
            }
        }


        public event System.Action OnExit;
        public event System.Action OnPlay;

        public void Initialize(GameInfo info, PuzzleModelsData.Collection currant, PuzzleModelsData.Collection next)
        {
            soundPlayer = info.Components.SoundPlayer;


            TurnUI(false);

            menuPanels = new Dictionary<States, IEnumerable<UIPanel>>()
            {
                { States.None, new List<UIPanel>() },
                { States.Main, mainMenu.GetComponentsInChildren<UIPanel>(true) },
                { States.Done, doneMenu.GetComponentsInChildren<UIPanel>(true) },
                { States.AllDone, allDoneMenu.GetComponentsInChildren<UIPanel>(true) },
            };

            GetComponentsInChildren<UIPanel>(true)
                .ToList()
                .ForEach(x => x.Initilize());

            GetComponentsInChildren<Initializable<ISoundPlayer>>(true)
                .ToList()
                .ForEach(x => x.Initialize(soundPlayer));

            GetComponentsInChildren<Initializable<IAdsController>>(true)
                .ToList()
                .ForEach(x => x.Initialize(adsController));

            GetComponentsInChildren<IChangeable<string>>(true)
                .ToList()
                .ForEach(x => x.SetValue(currant.Name));

            if (next != null)
            {
                GetComponentsInChildren<IChangeable<LevelNumber>>(true)
                    .ToList()
                    .ForEach(x => x.SetValue(new LevelNumber(next.MinLevel)));

                nextEventPanel.SetPredicate(() => SavedData.LevelsDone <= next.MinLevel);
            }

            foreach (ButtonUI button in exitButtons)
            {
                button.OnClick += ExitClick;
            }

            TurnUI(true);
        }

        public void Bind(IUserInput input)
        {
            input.OnTap += PlayClick;
        }


        private void OnStateEnd(States state)
        {
            foreach (UIPanel panel in menuPanels[state])
            {
                panel.IsShown = false;
            }
        }
        private void OnStateStart(States state)
        {
            foreach (UIPanel panel in menuPanels[state])
            {
                panel.IsShown = true;
            }
        }
        private void TurnUI(bool value)
        {
            mainMenu.gameObject.SetActive(value);
            doneMenu.gameObject.SetActive(value);
            allDoneMenu.gameObject.SetActive(value);
        }


        private void PlayClick()
        {
            if (State == States.None)
            {
                return;
            }
            OnPlay?.Invoke();
        }
        private void ExitClick()
        {
            if (State == States.None)
            {
                return;
            }
            OnExit?.Invoke();
        }


        public enum States
        {
            None, Main, Done, AllDone
        }
    }
}

