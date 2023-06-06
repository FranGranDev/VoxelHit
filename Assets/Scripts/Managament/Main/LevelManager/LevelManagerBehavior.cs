using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Managament.Levels
{
    public abstract class LevelManagerBehavior
    {
        public LevelManagerBehavior(LevelManagement manager) => this.manager = manager;

        protected LevelManagement manager;

        public virtual void Accept(ModelId modelId)
        {

        }
        public virtual void Accept(PuzzleId puzzleId)
        {

        }


        public abstract int CurrantLocationIndex { get; set; }
        public abstract int CurrantLevelIndex { get; set; }

        public abstract int LevelsDone { get; set; }
        public abstract int LocationsDone { get; set; }
    }
}