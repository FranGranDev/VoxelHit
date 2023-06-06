using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Data;
using UI;
using Animations;
using Services;
using Factory;
using Voxel;


namespace Managament
{
    public class PaintedSceneContext : SceneContext
    {
        [SerializeField] private PaintedUI paintedUI;
        [SerializeField] private VoxelModelsData modelsData;
        [SerializeField] private PaintData paintData;
        [Header("Final Components")]
        [SerializeField] private VoxelModelPlace modelPlace;

        protected override void SceneInitilize()
        {
            paintedUI.OnDone += OnLevelCompleate;

            GameInfo gameInfo = new GameInfo(this, Components, Color.white, baseMusic);

            IFactory<ModelId> factory = transform.GetComponentInChildren<IFactory<ModelId>>();
            try
            {
                ModelId model = UseFactory(modelsData.GetItem(((PaintSceneContext.PaintSceneData)Data).ModelIndex));
                VoxelObject voxelObject = model.GetComponent<VoxelObject>();
                voxelObject.InitializePainted(new SavedData.VoxelModelInfo(factory.Created, paintData));
            }
            catch
            {
                Debug.Log("Scene Data Error!");
            }


            AutoBind<IGameEventsHandler>();
            InitializeComponents(gameInfo);

            GameState = GameStates.Game;

            StartCoroutine(FinalCour());
        }

        
        private IEnumerator FinalCour()
        {
            yield return new WaitForFixedUpdate();

            modelPlace.StartFinalEvent(() =>
            {
                paintedUI.EarnMoney(paintData.PaintReward);
            });
            yield break;
        }

        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
