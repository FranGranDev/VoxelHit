using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;
using Puzzles;

namespace Managament
{
    public class PuzzleShelfSceneContext : SceneContext
    {
        [Header("Links")]
        [SerializeField] private PuzzleShelfController shelfController;

        public event System.Action<PuzzleId> OnSelectPuzzle;
        public event System.Action OnExit;

        protected override void SceneInitilize()
        {
            GameInfo gameInfo = new GameInfo(this, Components, Color.white, baseMusic);

            shelfController.Initialize(gameInfo);

            shelfController.OnPlayPuzzle += SelectPuzzle;
            shelfController.OnExit += Exit;


            CallOnLocalInitialize(gameInfo);
        }

        private void SelectPuzzle(PuzzleId obj)
        {
            OnSelectPuzzle?.Invoke(obj);
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
