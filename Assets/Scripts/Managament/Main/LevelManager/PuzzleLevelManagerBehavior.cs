using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managament.Levels
{
    public class PuzzleLevelManagerBehavior : LevelManagerBehavior
    {
        public PuzzleLevelManagerBehavior(LevelManagement manager) : base(manager)
        {

        }

        public override int CurrantLocationIndex
        {
            get
            {
                return 0;
            }
            set { }
        }
        public override int CurrantLevelIndex
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public override int LevelsDone { get => 0; set { } }
        public override int LocationsDone { get => 0; set { } }
    }

}