using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UI.Items;
using Services;
using System;

namespace UI
{
    public class PuzzleUI : MonoBehaviour, IBindable<IUserInput>
    {
        [Foldout("Menus"), SerializeField] private Transform selectMenu;
        [Foldout("Menus"), SerializeField] private Transform installMenu;
        [Foldout("Menus"), SerializeField] private Transform doneMenu;


        [Foldout("Buttons"), SerializeField] private List<ButtonUI> exitButtons;
        [Foldout("Buttons"), SerializeField] private ButtonUI selectRandom;

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


        public event Action OnExit;
        public event Action OnSelectRandom;

        public void Initialize(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;



            TurnUI(false);

            menuPanels = new Dictionary<States, IEnumerable<UIPanel>>()
            {
                { States.None, new List<UIPanel>() },
                { States.Select, selectMenu.GetComponentsInChildren<UIPanel>(true) },
                { States.Done, doneMenu.GetComponentsInChildren<UIPanel>(true) },
                { States.Install, installMenu.GetComponentsInChildren<UIPanel>(true) },
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

            exitButtons.ForEach(x => x.OnClick += ExitClick);

            if (selectRandom)
            {
                selectRandom.OnClick += SelectClick;
            }

            TurnUI(true);
        }

        public void Bind(IUserInput input)
        {
            input.OnTap += () =>
            {
                if (State == States.Done)
                {
                    OnExit?.Invoke();
                }
            };
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
            selectMenu.gameObject.SetActive(value);
            installMenu.gameObject.SetActive(value);
            doneMenu.gameObject.SetActive(value);
        }


        private void SelectClick()
        {
            OnSelectRandom?.Invoke();
        }
        private void ExitClick()
        {
            OnExit?.Invoke();
        }


        public enum States
        {
            None, Select, Install, Done
        }
    }
}
