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
    public class MainUI : MonoBehaviour, IBindable<IGameEventsHandler>, IBindable<ISuperShotEvents>
    {
        [Foldout("Menus"), SerializeField] private Transform startMenu;
        [Foldout("Menus"), SerializeField] private Transform gameMenu;
        [Foldout("Menus"), SerializeField] private Transform failMenu;
        [Foldout("Menus"), SerializeField] private Transform doneMenu;
        [Foldout("Menus"), SerializeField] private Transform finalMenu;
        [Foldout("Menus"), SerializeField] private Transform otherMenu;

        [Foldout("Links"), SerializeField] private SegmentBar progressBar;
        [Foldout("Links"), SerializeField] private UIPanel moneyPanel;
        [Foldout("Links"), SerializeField] private NotflicationController notflications;

        [Foldout("Buttons"), SerializeField] private ButtonUI shopButton;
        [Foldout("Buttons"), SerializeField] private UIPanel shopButtonPanel;
        [Foldout("Buttons"), SerializeField] private ButtonUI shelfButton;
        [Foldout("Buttons"), SerializeField] private UIPanel shelfButtonPanel;
        [Foldout("Buttons"), SerializeField] private ButtonUI puzzleButton;
        [Foldout("Buttons"), SerializeField] private UIPanel puzzleButtonPanel;
        [Foldout("Buttons"), SerializeField] private ButtonUI eventButton;
        [Foldout("Buttons"), SerializeField] private UIPanel eventButtonPanel;
        [Foldout("Buttons"), SerializeField] private ButtonUI continueButton;
        [Foldout("Buttons"), SerializeField] private ButtonUI restartButton;


        [Foldout("Others"), SerializeField] private UIPanel superReadyIcon;


        [Space, Foldout("Buttons"), SerializeField] private UIToggle soundToggle;
        [Foldout("Buttons"), SerializeField] private UIToggle vibroToggle;

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
                if(!menuPanels.ContainsKey(value))
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

        public event System.Action OnShelfClick;
        public event System.Action OnShopClick;
        public event System.Action OnPuzzleClick;
        public event System.Action OnEventClick;

        public event System.Action<bool> OnTurnSound;
        public event System.Action<bool> OnTurnVibro;


        public void Initilize(GameInfo info)
        {
            moneyController = info.Components.Money;
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

            soundToggle.Initialize((x) => OnTurnSound?.Invoke(x), SavedData.SoundEnabled);
            vibroToggle.Initialize((x) => OnTurnVibro?.Invoke(x), SavedData.VibroEnabled);

            if (!allButtonsActive)
            {
                shelfButtonPanel.SetPredicate(() => SavedData.LevelsDone >= 2);
                shopButtonPanel.SetPredicate(() => SavedData.LevelsDone >= 3);
                puzzleButtonPanel.SetPredicate(() => SavedData.LevelsDone >= 5);
                eventButtonPanel.SetPredicate(() => SavedData.LevelsDone >= 7);
            }

            shopButton.OnClick += ShopClick;
            shelfButton.OnClick += ShelfClick;

            puzzleButton.OnClick += PuzzleClick;

            eventButton.OnClick += EventClick;

            restartButton.OnClick += RestartClick;
            continueButton.OnClick += ContinueClick;


            OnLevelUpdated?.Invoke(new LevelNumber(SavedData.LevelsDone + 1));

            this.Delayed(0.1f, () =>
            {
                TurnUI(true);
                State = state;
            });
        }
        public void InitilizeBar(List<float> values)
        {
            progressBar.SetSegments(values);
        }
        public void Bind(IUserInput input)
        {
            this.Delayed(Time.fixedDeltaTime, () => input.OnTap += PlayClick);
        }
        public void Bind(ISuperShotEvents shotEvents)
        {
            shotEvents.OnReadyChanged += OnSuperShotChanged;
        }
        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnStateChanged += OnGameStateChanged;
        }


        public void ShowNotflication(string group, Notflication info, System.Action OnWatched)
        {
            if (State != GameStates.Idle)
                return;
            notflications.Show(group, info, OnPlayEvent);
            OnWatched?.Invoke();
        }
        public async void OnPlayEvent(NotflicationController.Item item)
        {
            bool watched = await adsController.ShowRewarded();

            if(watched)
            {
                OnEventClick?.Invoke();
            }
        }


        private void OnSuperShotChanged(bool state)
        {
            if (State != GameStates.Game)
                return;
            superReadyIcon.IsShown = state;
        }
        private void OnGameStateChanged(GameStates state)
        {
            switch(state)
            {
                case GameStates.Failed:
                    this.Delayed(1.75f, () => State = state);
                    break;
                default:
                    State = state;
                    break;
            }

        }


        private void OnStateEnd(GameStates state)
        {
            foreach(UIPanel panel in menuPanels[state])
            {
                panel.IsShown = false;
            }

            switch(state)
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

            switch(state)
            {
                case GameStates.Idle:
                    moneyPanel.IsShown = true;

                    this.Delayed(0.33f, () => OnLevelUpdated?.Invoke(new LevelNumber(SavedData.LevelsDone + 1)));
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
            otherMenu.gameObject.SetActive(value);
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
        public void DoneClick()
        {
            OnDoneClick?.Invoke();
        }

        private void ShelfClick()
        {
            OnShelfClick?.Invoke();
        }
        private void PuzzleClick()
        {
            OnPuzzleClick?.Invoke();
        }
        private void EventClick()
        {
            OnEventClick?.Invoke();
        }
        private void ShopClick()
        {
            OnShopClick?.Invoke();
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

                    await finalUI.ExtraMoney(value);

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
