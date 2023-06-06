using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UI.Items;
using Services;
using System;

namespace UI
{
    public class ShelfUI : MonoBehaviour, IFillEvent
    {
        [Foldout("Menus"), SerializeField] private Transform openedMenu;
        [Foldout("Menus"), SerializeField] private Transform watchMenu;
        [Foldout("Menus"), SerializeField] private Transform nextMenu;

        [Foldout("Links"), SerializeField] private UIPanel tapToContinue;
        [Foldout("Links"), SerializeField] private SwipeController swipeController;

        [Foldout("Buttons"), SerializeField] private ButtonUI exitButton;
        [Foldout("Buttons"), SerializeField] private ButtonUI rightButton;
        [Foldout("Buttons"), SerializeField] private ButtonUI leftButton;

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

        public float Fill
        {
            get;
            private set;
        }

        public event Action OnExit;
        public event Action OnRight;
        public event Action OnLeft;
        public event Action<float> OnFilled;

        public void Initialize(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;



            TurnUI(false);

            menuPanels = new Dictionary<States, IEnumerable<UIPanel>>()
            {
                { States.None, new List<UIPanel>() },
                { States.Opening, openedMenu.GetComponentsInChildren<UIPanel>(true) },
                { States.Watch, watchMenu.GetComponentsInChildren<UIPanel>(true) },
                { States.Next, nextMenu.GetComponentsInChildren<UIPanel>(true) },
            };

            GetComponentsInChildren<UIPanel>(true)
                .ToList()
                .ForEach(x => x.Initilize());

            GetComponentsInChildren<Initializable<ISoundPlayer>>(true)
                .ToList()
                .ForEach(x => x.Initialize(soundPlayer));

            GetComponentsInChildren<IBindable<IFillEvent>>(true)
                .ToList()
                .ForEach(x => x.Bind(this));

            GetComponentsInChildren<Initializable<IAdsController>>(true)
                .ToList()
                .ForEach(x => x.Initialize(adsController));



            exitButton.OnClick += ExitClick;
            rightButton.OnClick += RightClick;
            leftButton.OnClick += LeftClick;


            swipeController.OnSwipe += (x) =>
            {
                if (State != States.Watch)
                    return;
                if(!x)
                {
                    RightClick();
                }
                else
                {
                    LeftClick();
                }
            };

            TurnUI(true);
        }


        public void PlayOpening(IUserInput input, float compleateProcent)
        {
            input.OnTap += () =>
            {
                if(tapToContinue.IsShown)
                {
                    ExitClick();
                }
            };

            State = States.Opening;
            compleateProcent = Mathf.Round(compleateProcent * 100) / 100f;

            Fill = compleateProcent;
            OnFilled?.Invoke(Fill);
        }
        public void PlayNext()
        {
            State = States.Next;
        }
        public void ShowNextButton()
        {
            tapToContinue.IsShown = true;
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

            switch(state)
            {
                case States.Next:
                    tapToContinue.IsShown = true;
                    break;
            }
        }
        private void TurnUI(bool value)
        {
            openedMenu.gameObject.SetActive(value);
            nextMenu.gameObject.SetActive(value);
            watchMenu.gameObject.SetActive(value);
        }

        private void ExitClick()
        {
            OnExit?.Invoke();
        }
        private void RightClick()
        {
            OnRight?.Invoke();
        }
        private void LeftClick()
        {
            OnLeft?.Invoke();
        }

        public enum States
        {
            None, Opening, Watch, Next, Tutor
        }
    }
}
