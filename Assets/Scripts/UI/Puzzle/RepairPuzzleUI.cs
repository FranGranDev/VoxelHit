using System.Linq;
using System.Collections.Generic;
using Services;
using UnityEngine;
using NaughtyAttributes;
using Animations;
using Data;
using UI.Items;

namespace UI
{
    public class RepairPuzzleUI : MonoBehaviour, IBindable<IGameEventsHandler>, IBindable<ISuperShotEvents>
    {
        [Foldout("Menus"), SerializeField] private Transform startMenu;
        [Foldout("Menus"), SerializeField] private Transform gameMenu;
        [Foldout("Menus"), SerializeField] private Transform failMenu;
        [Foldout("Menus"), SerializeField] private Transform doneMenu;
        [Foldout("Menus"), SerializeField] private Transform finalMenu;

        [Foldout("Links"), SerializeField] private SegmentBar progressBar;
        [Foldout("Links"), SerializeField] private UIPanel moneyPanel;

        [Foldout("Others"), SerializeField] private UIPanel superReadyIcon;

        [Foldout("Buttons"), SerializeField] private ButtonUI homeButton;
        [Foldout("Buttons"), SerializeField] private ButtonUI continueButton;
        [Foldout("Buttons"), SerializeField] private ButtonUI restartButton;


        [Foldout("State"), SerializeField] private GameStates state;

        [Foldout("Debug"), SerializeField] private bool allButtonsActive;


        private Dictionary<GameStates, IEnumerable<UIPanel>> menuPanels;
        private Dictionary<GameStates, bool> moneyActiveDict = new Dictionary<GameStates, bool>()
        {
            {GameStates.Idle, true },
            {GameStates.Game, true },
            {GameStates.Failed, false },
            {GameStates.Done, false },
            {GameStates.Final, false },
        };
        private IMoneyController moneyController;
        private ISoundPlayer soundPlayer;
        private IAdsController adsController;

        private GameStates State
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

        private event System.Action<LevelNumber> OnLevelUpdated;
        private event System.Action<UIMoneyValue> OnMoneyUpdated;


        public event System.Action OnPlayClick;
        public event System.Action OnContinueClick;
        public event System.Action OnRestartClick;
        public event System.Action OnDoneClick;

        public event System.Action OnExitClick;



        public void Initilize(GameInfo info)
        {
            moneyController = info.Components.Gems;
            soundPlayer = info.Components.SoundPlayer;
            adsController = info.Components.Ads;

            TurnUI(false);
            

            GetComponentsInChildren<IChangeable<LevelNumber>>(true).ToList()
                .ForEach(x => x.Bind(ref OnLevelUpdated));

            menuPanels = new Dictionary<GameStates, IEnumerable<UIPanel>>()
            {
                {GameStates.Idle, startMenu.GetComponentsInChildren<UIPanel>(true) },
                {GameStates.Game, gameMenu.GetComponentsInChildren<UIPanel>(true) },
                {GameStates.Final, finalMenu.GetComponentsInChildren<UIPanel>(true) },
                {GameStates.Failed, failMenu.GetComponentsInChildren<UIPanel>(true) },
                {GameStates.Done, doneMenu.GetComponentsInChildren<UIPanel>(true) },
            };
            GetComponentsInChildren<UIPanel>(true)
                .ToList()
                .ForEach(x => x.Initilize());

            GetComponentsInChildren<IChangeable<UIMoneyValue>>(true)
                .ToList()
                .ForEach(x => x.Bind(ref OnMoneyUpdated));

            GetComponentsInChildren<Initializable<ISoundPlayer>>(true)
                .ToList()
                .ForEach(x => x.Initialize(soundPlayer));

            GetComponentsInChildren<Initializable<IAdsController>>(true)
                .ToList()
                .ForEach(x => x.Initialize(adsController));


            moneyController.OnVisualUpdated += UpdateMoney;
            moneyController.CallVisualUpdate();

            homeButton.OnClick += HomeClick;
            restartButton.OnClick += RestartClick;
            continueButton.OnClick += ContinueClick;


            OnLevelUpdated?.Invoke(new LevelNumber(SavedData.LevelsDone + 1));
            this.Delayed(Time.fixedDeltaTime, () =>
            {
                TurnUI(true);
                State = state;
            });
        }


        public void InitilizeBar(List<float> values)
        {
            progressBar.SetSegments(values);
        }
        public void BindInput(IUserInput input)
        {
            this.Delayed(Time.fixedDeltaTime, () => input.OnTap += PlayClick);
        }

        public void Bind(ISuperShotEvents obj)
        {
            obj.OnReadyChanged += SuperShotChanged;
        }

        private void SuperShotChanged(bool value)
        {
            if (State != GameStates.Game)
                return;
            superReadyIcon.IsShown = value;
        }

        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnStateChanged += OnGameStateChanged;
        }
        private void OnGameStateChanged(GameStates state)
        {
            switch (state)
            {
                case GameStates.Failed:
                    this.Delayed(2, () => State = state);
                    break;
                default:
                    State = state;
                    break;
            }

        }


        private void OnStateEnd(GameStates state)
        {
            foreach (UIPanel panel in menuPanels[state])
            {
                panel.IsShown = false;
            }

            switch (state)
            {
                case GameStates.Game:
                    superReadyIcon.IsShown = false;
                    break;
            }

            moneyPanel.IsShown = moneyActiveDict[state];
        }
        private void OnStateStart(GameStates state)
        {
            foreach (UIPanel panel in menuPanels[state])
            {
                panel.IsShown = true;
            }

            switch (state)
            {
                case GameStates.Idle:
                    moneyPanel.IsShown = true;

                    this.Delayed(0.33f, () => OnLevelUpdated?.Invoke(new LevelNumber(SavedData.LevelsDone + 1)));
                    break;
                case GameStates.Done:
                    superReadyIcon.IsShown = false;
                    break;
                case GameStates.Failed:
                    soundPlayer.PlaySound("fail");
                    break;
            }

            moneyPanel.IsShown = moneyActiveDict[state];
        }
        private void TurnUI(bool value)
        {
            startMenu.gameObject.SetActive(value);
            gameMenu.gameObject.SetActive(value);
            failMenu.gameObject.SetActive(value);
            finalMenu.gameObject.SetActive(value);
            doneMenu.gameObject.SetActive(value);
        }


        public void PlayClick()
        {
            OnPlayClick?.Invoke();
        }
        public void RestartClick()
        {
            OnRestartClick?.Invoke();
        }
        public void ContinueClick()
        {
            OnContinueClick?.Invoke();
        }
        public void HomeClick()
        {
            OnExitClick?.Invoke();
        }
        public void DoneClick()
        {
            OnDoneClick?.Invoke();
        }

        public async void EarnMoney(int value)
        {
            IFinal finalUI = GetComponentInChildren<IFinal>();
            if (finalUI == null)
            {
                DoneClick();
                return;
            }

            IFinal.Result result = await finalUI.Execute(value);

            finalUI.StopWheel();

            if (result.WatchAds)
            {
                if (await adsController.ShowRewarded())
                {
                    int extraMoney = Mathf.RoundToInt(value * result.RewardRatio);
                    moneyController.Add(extraMoney);

                    await finalUI.ExtraMoney(value, result.RewardRatio);

                    DoneClick();
                }
                else
                {
                    DoneClick();
                }
            }
            else
            {
                await adsController.TryShowInter();
                DoneClick();
            }
        }
        private void UpdateMoney(UIMoneyValue obj)
        {
            OnMoneyUpdated?.Invoke(obj);
        }

    }
}
