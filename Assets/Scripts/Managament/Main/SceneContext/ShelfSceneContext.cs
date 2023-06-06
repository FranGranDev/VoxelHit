using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managament.Shelfs;
using Data;

namespace Managament
{
    public class ShelfSceneContext : SceneContext
    {
        [SerializeField] private ShelfsController shelfsController;


        public event System.Action<Shelf.Item> OnSelectItem;
        public event System.Action OnExit;


        protected override void SceneInitilize()
        {
            Services.GameInfo info = new Services.GameInfo(this, Components, Color.white, baseMusic);

            shelfsController.Initialize(info);
            shelfsController.OnExit += Exit;

            try
            {
                ShelfSceneData data = (ShelfSceneData)Data;
                switch(data.ActionType)
                {
                    case ShelfSceneData.ActionTypes.NewOpened:
                        Debug.Log("Opened New Model: " + data.TargetIndex);
                        break;
                }


                shelfsController.OnSelectItem += SelectItem;

                switch (data.ActionType)
                {
                    case ShelfSceneData.ActionTypes.Watch:
                        shelfsController.CreateLastOpenedModelCollection(SavedData.LastOpenedModel);
                        break;
                    case ShelfSceneData.ActionTypes.NewOpened:
                        shelfsController.CreateSingleCollection(data.TargetIndex);
                        break;
                    case ShelfSceneData.ActionTypes.Painted:
                        shelfsController.CreateAllCollection(SavedData.LastShelfIndex);
                        shelfsController.PlayPaintedAnimation(data.TargetIndex);
                        break;
                }
            }
            catch
            {
                shelfsController.CreateAllCollection();
            }
        }

        private void Exit()
        {
            OnExit?.Invoke();
        }

        public void SelectItem(Shelf.Item item)
        {
            OnSelectItem?.Invoke(item);
        }


        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }


        public class ShelfSceneData : SceneData
        {
            public ShelfSceneData(ActionTypes action, int index)
            {
                TargetIndex = index;
                ActionType = action;
            }
            public ShelfSceneData()
            {
                TargetIndex = -1;
                ActionType = ActionTypes.Watch;
            }

            public int TargetIndex { get; }

            public ActionTypes ActionType { get; }


            public enum ActionTypes { Watch, NewOpened, Painted}
        }
    }
}
