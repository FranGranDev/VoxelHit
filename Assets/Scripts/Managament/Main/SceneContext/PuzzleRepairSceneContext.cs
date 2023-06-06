using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UI;
using Factory;
using Cannons;
using NaughtyAttributes;
using Traps;
using Managament.Levels;
using Services;
using Voxel;

namespace Managament
{
    public class PuzzleRepairSceneContext : SceneContext
    {
        [Foldout("Links"), SerializeField] protected LevelManagement levelManagement;
        [Foldout("Links"), Space, SerializeField] private RepairPuzzleUI mainUI;
        [Foldout("Links"), Space, SerializeField] private PuzzleModelsData puzzleData;
        [Foldout("Test"), Space, SerializeField] private ModelId testModel;

        private int collectedMoney;
        private IUserInput input;

        public override Transform LevelTransform => levelManagement.LevelTransform;


        public event System.Action<PuzzleId> OnFinalEnd;
        public event System.Action OnExitClick;


        protected override void SceneInitilize()
        {
            levelManagement.OnLevelLoaded += OnLevelLoaded;

            GameInfo info = new GameInfo(this, Components, levelManagement.CurrantLocation.mainColor, null);

            input = GetComponentInChildren<IUserInput>();
            mainUI.Initilize(info);
            mainUI.BindInput(input);

            mainUI.OnDoneClick += UIDoneClick;
            mainUI.OnPlayClick += UIPlayClick;
            mainUI.OnContinueClick += UIContinueClick;
            mainUI.OnRestartClick += UIRestartClick;
            mainUI.OnExitClick += UIExitClick;

            try
            {
                levelManagement.Initialize((Data as PuzzleModelSceneData).ModelId);
            }
            catch
            {
                levelManagement.Initialize(testModel);
            }
        }


        private void OnLevelLoaded()
        {
            collectedMoney = 0;

            GameInfo info = new GameInfo(this, Components, levelManagement.CurrantLocation.mainColor, levelManagement.LevelMusic);

            UseFactories();

            ModelId model = null;
            try
            {
                model = (Data as PuzzleModelSceneData).ModelId;
            }
            catch
            {
                model = null;
            }
            finally
            {
                UseFactory(model);
            }

            AutoBind<IUserInput>();
            AutoBind<IFillEvent>();
            AutoBind<IGameEventsHandler>();

            UseTargetEvents();


            LevelTransform.GetComponentsInChildren<ISuperShotEvents>()
                .ToList()
                .ForEach(x => mainUI.Bind(x));

            InitializeComponents(info);


            CallOnLocalInitialize(info);


            FreeCannon cannon = GetComponentInChildren<FreeCannon>();
            VoxelObject voxelObject = GetComponentInChildren<VoxelObject>();

            GetComponentInChildren<IFactory<FreeCannon, VoxelObject>>().Create(cannon, voxelObject);

            BackgroundInitialize(levelManagement.CurrantLocation.background);

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
            if (finalEvent != null)
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

            //Components.EventsTracker.OnLevelFailed(SavedData.LevelsDone + 1);

            Physics.gravity = Vector3.zero;
        }
        protected virtual void GoNextLevel()
        {
            //Components.EventsTracker.OnLevelCompleted(SavedData.LevelsDone + 1);

            SaveModel();
            ClearScene();
            levelManagement.LevelDone();

            ModelId instance = GetComponentInChildren<ModelId>();
            PuzzleId puzzleId = puzzleData
                .GetModels
                .First(x => x.PuzzleId.Group == instance.Group).PuzzleId;
            OnFinalEnd?.Invoke(puzzleId);
        }
        protected override void OnCollectMoney(MoneyValue obj)
        {
            collectedMoney += obj.Value;

            Components.Gems.Add(obj.Value);
        }

        private void SaveModel()
        {
            ModelId model = LevelTransform.GetComponentInChildren<ModelId>();

            SavedData.ModelInfo.SetOpened(model, false);
        }


        private async void UIRestartClick()
        {
            if (GameState != GameStates.Failed)
                return;

            await Components.Ads.TryShowInter();

            ClearScene();
            levelManagement.RestartLevel();

            Physics.gravity = new Vector3(0, -20, 0);
        }
        private async void UIContinueClick()
        {
            if (GameState != GameStates.Failed)
                return;
            if(await Components.Ads.ShowRewarded())
            {
                GameState = GameStates.Game;

                Components.SoundPlayer.PlaySound("restart");

                Physics.gravity = new Vector3(0, -20, 0);
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

            //Components.EventsTracker.OnLevelStarted(SavedData.LevelsDone + 1);
        }
        private void UIDoneClick()
        {
            if (GameState != GameStates.Final)
                return;

            GoNextLevel();
        }


        private void UIExitClick()
        {
            OnExitClick?.Invoke();
        }



        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
