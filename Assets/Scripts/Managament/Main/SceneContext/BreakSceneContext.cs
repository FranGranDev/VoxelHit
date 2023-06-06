using Data;
using Factory;
using Managament.Levels;
using NaughtyAttributes;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Traps;
using UI;
using UnityEngine;

namespace Managament
{
    public class BreakSceneContext : SceneContext
    {
        [Foldout("Links"), SerializeField] protected LevelManagement levelManagement;
        [Foldout("Links"), Space, SerializeField] private BreakUI mainUI;
        [Foldout("Links"), Space, SerializeField] private PuzzleModelsData modelsData;
        [Foldout("Test"), Space, SerializeField] private ModelId testModel;

        private int collectedMoney = 0;


        public override Transform LevelTransform => levelManagement.LevelTransform;




        public event System.Action OnFinalEnd;
        public event System.Action OnExitClick;


        protected override void SceneInitilize()
        {
            levelManagement.OnLevelLoaded += OnLevelLoaded;

            GameInfo info = new GameInfo(this, Components, levelManagement.CurrantLocation.mainColor, null);

            mainUI.Initilize(info);


            mainUI.OnDoneClick += UIDoneClick;
            mainUI.OnPlayClick += UIPlayClick;
            mainUI.OnContinueClick += UIContinueClick;
            mainUI.OnRestartClick += UIExitClick;
            mainUI.OnExitClick += UIExitClick;

            try
            {
                levelManagement.Initialize((Data as PuzzleModelSceneData).ModelId);
            }
            catch
            {
                if (testModel)
                {
                    levelManagement.Initialize(testModel);
                }
                else
                {
                    levelManagement.Initialize();
                }
            }
        }

        protected override void SetLayerMatrix()
        {
            Physics.IgnoreLayerCollision(6, 6, false);
        }

        private void OnLevelLoaded()
        {
            collectedMoney = 0;

            GameInfo info = new GameInfo(this, Components, levelManagement.CurrantLocation.mainColor, levelManagement.LevelMusic);

            UseFactories();

            ModelId model = null;
            bool megaModel = false;
            try
            {
                model = (Data as PuzzleModelSceneData).ModelId;
                megaModel = (Data as PuzzleModelSceneData).Super;
            }
            catch
            {
                model = testModel;
            }
            finally
            {
                UseFactory(model);
                UseFactory(megaModel);
            }



            AutoBind<IUserInput>();
            AutoBind<IFillEvent>();
            AutoBind<IGameEventsHandler>();
            AutoBind<IStock>();

            UseTargetEvents();

            InitializeComponents(info);

            CallOnLocalInitialize(info);

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
        }
        protected virtual void GoNextLevel()
        {
            //Components.EventsTracker.OnLevelCompleted(SavedData.LevelsDone + 1);

            SaveModel();
            ClearScene();
            levelManagement.LevelDone();

            OnFinalEnd?.Invoke();
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
