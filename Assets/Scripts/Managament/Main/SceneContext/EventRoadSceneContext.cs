using Data;
using Managament.Levels;
using NaughtyAttributes;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managament
{
    public class EventRoadSceneContext: SceneContext
    {
        [Foldout("Links"), SerializeField] protected EventRoadController roadController;
        [Foldout("Links"), SerializeField] protected LevelManagement levelManagement;
        [Foldout("Links"), SerializeField] protected SavedData savedData;
        [Foldout("Links"), SerializeField] protected PuzzleModelsData modelsData;
        [Foldout("Factory"), SerializeField] protected bool testEvent;
        [Foldout("Factory"), SerializeField] protected string testGroup;

        public override Transform LevelTransform => levelManagement.LevelTransform;


        public event System.Action OnExit;
        public event System.Action<PuzzleId, ModelId, bool> OnPlay;
        public event System.Action OnNextRoad;


        protected override void SceneInitilize()
        {
            levelManagement.OnLevelLoaded += OnLevelLoaded;
            roadController.OnExit += Exit;
            roadController.OnPlay += Play;
            roadController.OnRoadDone += NextRoad;

            try
            {
                levelManagement.Initialize(savedData.GetActiveEvent().PuzzleId);
            }
            catch
            {
                levelManagement.Initialize();
            }
        }
        private void OnLevelLoaded()
        {
            UseFactories();

            AutoBind<IUserInput>();
            AutoBind<IGameEventsHandler>();

            GameInfo info = new GameInfo(this, Components, levelManagement.CurrantLocation.mainColor, levelManagement.LevelMusic);
            InitializeComponents(info);

            SavedData.EventInfo eventInfo = savedData.GetActiveEvent();            
            SavedData.EventInfo nextInfo = savedData.GetNextEvent();
            if (testEvent)
            {
                eventInfo = savedData.GetEvents().First(x => x.PuzzleId.Group == testGroup);
                nextInfo = null;
            }



            roadController.Initialize(info, eventInfo, nextInfo);

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


        private void Play(PuzzleId puzzle, ModelId model, bool boss)
        {
            OnPlay?.Invoke(puzzle, model, boss);
        }
        private void NextRoad(PuzzleId puzzle)
        {
            OnNextRoad?.Invoke();
        }
        private void Exit()
        {
            OnExit?.Invoke();
        }

        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
