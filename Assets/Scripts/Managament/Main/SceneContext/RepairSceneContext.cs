using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UI;
using Factory;
using NaughtyAttributes;
using Traps;
using Managament.Levels;
using Services;


namespace Managament
{
    public class RepairSceneContext : SceneContext
    {
        [Foldout("Links"), SerializeField] protected LevelManagement levelManagement;
        [Foldout("Links"), Space, SerializeField] private MainUI mainUI;
        [Foldout("Links"), Space, SerializeField] private SavedData savedData;

        [Foldout("States"), Space, SerializeField] private int collectedMoney;

        private TrapsController trapsController;
        private IUserInput input;


        public override Transform LevelTransform => levelManagement.LevelTransform;

        public event System.Action OnShopClick;
        public event System.Action OnShelfClick;
        public event System.Action OnPuzzleClick;
        public event System.Action OnEventClick;
        public event System.Action OnFinalEnd;


        protected override void SceneInitilize()
        {
            levelManagement.OnLevelLoaded += OnLevelLoaded;


            input = GetComponentInChildren<IUserInput>();
            mainUI.Initilize(new GameInfo(this, Components, levelManagement.CurrantLocation.mainColor, null));
            mainUI.Bind(input);

            mainUI.OnDoneClick += UIDoneClick;
            mainUI.OnPlayClick += UIPlayClick;
            mainUI.OnContinueClick += UIContinueClick;
            mainUI.OnRestartClick += UIRestartClick;

            mainUI.OnShelfClick += UIShelfClick;
            mainUI.OnShopClick += UIShopClick;
            mainUI.OnPuzzleClick += UIPuzzleClick;
            mainUI.OnEventClick += UIEventClick;

            mainUI.OnTurnSound += TurnSound;
            mainUI.OnTurnVibro += TurnVibro;



            levelManagement.Initialize();
        }


        private void OnLevelLoaded()
        {
            collectedMoney = 0;

            GameInfo info = new GameInfo(this, Components, levelManagement.CurrantLocation.mainColor, levelManagement.LevelMusic);

            UseFactories();
            UseTargetEvents();

            AutoBind<IUserInput>();
            AutoBind<IFillEvent>();
            AutoBind<IGameEventsHandler>();

            InitializeComponents(info);


            trapsController = LevelTransform.GetComponentInChildren<TrapsController>();
            mainUI.InitilizeBar(trapsController.FillValues);

            LevelTransform.GetComponentsInChildren<ISuperShotEvents>()
                .ToList()
                .ForEach(x => mainUI.Bind(x));


            CallOnLocalInitialize(info);


            BackgroundInitialize(levelManagement.CurrantLocation.background);

            this.Delayed(0.1f, CheckForEventNotflication);

            if (autoStart)
            {
                GameState = GameStates.Game;
            }
            else
            {
                GameState = GameStates.Idle;
            }
        }
        protected override async void OnLevelCompleate()
        {
            GameState = GameStates.Done;

            IFinalEvent finalEvent = LevelTransform.GetComponentInChildren<IFinalEvent>();
            if(finalEvent != null)
            {
                await finalEvent.Execute();

                GameState = GameStates.Final;
                mainUI.EarnMoney(collectedMoney);
                return;
            }

            GameState = GameStates.Final;
        }
        protected override void OnLevelFailed()
        {
            base.OnLevelFailed();

            Components.EventsTracker.OnLevelFailed(SavedData.LevelsDone + 1);
        }
        protected override void OnCollectMoney(MoneyValue obj)
        {
            collectedMoney += obj.Value;

            Components.Money.Add(obj.Value);
        }
        protected virtual void GoNextLevel()
        {
            Components.EventsTracker.OnLevelCompleted(SavedData.LevelsDone + 1);

            SaveModel();
            ClearScene();
            levelManagement.LevelDone();

            OnFinalEnd?.Invoke();
        }


        private void SaveModel()
        {
            ModelId model = LevelTransform.GetComponentInChildren<ModelId>();
            SavedData.ModelInfo.SetOpened(model);
        }

        private void CheckForEventNotflication()
        {
            SavedData.EventInfo avaliable = savedData.GetActiveEvent();
            if (avaliable == null || avaliable.NotflicationWatched || avaliable.Items.Count(x => x.Opened) > 0)
                return;

            mainUI.ShowNotflication(avaliable.PuzzleId.Group, avaliable.Notflication, () =>
            {
                avaliable.NotflicationWatched = true;
            });
        }


        private async void UIRestartClick()
        {
            if (GameState != GameStates.Failed)
                return;

            await Components.Ads.TryShowInter();

            ClearScene();
            levelManagement.RestartLevel();
        }
        private async void UIContinueClick()
        {
            if (GameState != GameStates.Failed)
                return;
            if(await Components.Ads.ShowRewarded())
            {
                GameState = GameStates.Game;

                Components.SoundPlayer.PlaySound("restart");
            }
            else
            {
                UIRestartClick();
            }
        }
        private void UIPlayClick()
        {
            if (GameState != GameStates.Idle)
                return;

            GameState = GameStates.Game;

            Components.EventsTracker.OnLevelStarted(SavedData.LevelsDone + 1);
        }
        private void UIDoneClick()
        {
            if (GameState != GameStates.Final)
                return;

            GoNextLevel();
        }

        private void UIShelfClick()
        {
            OnShelfClick?.Invoke();
        }
        private void UIShopClick()
        {
            OnShopClick?.Invoke();
        }
        private void UIPuzzleClick()
        {
            OnPuzzleClick?.Invoke();
        }
        private void UIEventClick()
        {
            OnEventClick?.Invoke();
        }


        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
