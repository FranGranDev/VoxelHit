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
    public class InfinitySceneContext : RepairSceneContext
    {
        [Space, SerializeField] private GameDoneUI gameDoneUI;

        protected override void SceneInitilize()
        {
            base.SceneInitilize();

            if(!SavedData.EndShown)
            {
                gameDoneUI.Show();
                SavedData.EndShown = true;
            }
        }

        protected override void GoNextLevel()
        {
            ClearScene();
            levelManagement.NextLevel();
        }
        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
