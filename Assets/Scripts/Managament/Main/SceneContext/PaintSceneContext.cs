using Services;
using Factory;
using System.Linq;
using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Managament
{
    public class PaintSceneContext : SceneContext
    {
        [SerializeField] private VoxelModelsData modelsData;
        [SerializeField] private PaintData paintData;
        [SerializeField] private PaintController paintController;

        public event System.Action OnExit;

        protected override void SceneInitilize()
        {
            paintController.OnDone += OnPainted;
            paintController.OnExit += Exit;

            GameInfo gameInfo = new GameInfo(this, Components, Color.white, baseMusic);

            try
            {
                UseFactory(modelsData.GetItem(((PaintSceneData)Data).ModelIndex));
            }
            catch
            {
                Debug.LogError("Scene Data Error!");
            }

            UseTargetEvents();
            AutoBind<IGameEventsHandler>();

            InitializeComponents(gameInfo);


            GameState = GameStates.Game;
        }


        private void SaveModel()
        {
            ModelId model = LevelTransform.GetComponentInChildren<ModelId>();
            SavedData.VoxelModelInfo.SetPainted(model, paintController.PaintedGroups);
        }
        private void OnPainted()
        {
            SaveModel();
            OnLevelCompleate();   
        }
        private void Exit()
        {
            OnExit?.Invoke();
        }

        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }

        public class PaintSceneData : SceneData
        {
            public PaintSceneData(int index)
            {
                ModelIndex = index;
            }

            public int ModelIndex { get; }
        }
    }
}
