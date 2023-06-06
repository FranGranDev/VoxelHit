using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;
using Factory;

namespace Managament
{
    public class PuzzleSceneContext : SceneContext
    {
        [SerializeField] private PuzzleController puzzleController;
        [SerializeField] private PuzzleModelsData puzzleData;

        public event System.Action<PuzzleId, ModelId> OnSelectPuzzle;
        public event System.Action OnExit;

        protected override void SceneInitilize()
        {
            puzzleController.OnSelectItem += SelectPuzzleItem;
            puzzleController.OnExit += Exit;

            GameInfo info = new GameInfo(this, Components, Color.white, baseMusic);

            try
            {
                PuzzleId puzzle = puzzleData.GetModels.First(x => x.Group == ((PuzzleSceneData)Data).PuzzleId.Group).PuzzleId;
                LevelTransform.GetComponentsInChildren<IFactory<PuzzleId>>()
                    .ToList()
                    .ForEach(x => x.Create(puzzle));
            }
            catch
            {
                LevelTransform.GetComponentsInChildren<IFactory<PuzzleId>>()
                    .ToList()
                    .ForEach(x => x.Create(null));
                Debug.Log("Scene Data Error, factory use test mode.");
            }
            UseFactories();

            AutoBind<IUserInput>();
            AutoBind<IFillEvent>();
            AutoBind<IGameEventsHandler>();


            InitializeComponents(info);

            CallOnLocalInitialize(info);


            BackgroundInitialize(background);

            GameState = GameStates.Game;
        }

        private void Exit()
        {
            OnExit?.Invoke();
        }
        private void SelectPuzzleItem(PuzzleId arg1, ModelId arg2)
        {
            if (GameState != GameStates.Game)
                return;

            GameState = GameStates.Done;
            OnSelectPuzzle?.Invoke(arg1, arg2);
        }

        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
