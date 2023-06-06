using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Managament.Levels
{
    public class StaticLevelManagerBehavior : LevelManagerBehavior
    {
        public StaticLevelManagerBehavior(LevelManagement manager) : base(manager)
        {

        }

        private int location = 0;
        private int level = 0;

        public override void Accept(ModelId modelId)
        {
            try
            {
                location = manager.Locations.IndexOf(manager.Locations.First(x => x.name == modelId.Group));
                level = modelId.Index;

                manager.editorMode = false;
            }
            catch
            {
                Debug.LogError($"Can'f find location named: {modelId.Group} or level with index: {modelId.Index}");
            }
        }
        public override void Accept(PuzzleId puzzleId)
        {
            try
            {
                location = manager.Locations.IndexOf(manager.Locations.First(x => x.name == puzzleId.Group));
                level = 0;

                manager.editorMode = false;
            }
            catch
            {
                Debug.LogError($"Can'f find location named: {puzzleId.Group}");
            }
        }


        public override int CurrantLocationIndex
        {
            get
            {
                int index = manager.editorMode ? manager.editorLocationIndex : location;
                return Mathf.Clamp(index, 0, manager.Locations.Count - 1);
            }
            set
            {
                if (manager.Locations.Count == 0)
                    return;

                if (manager.editorMode)
                {
                    manager.editorLocationIndex = value;
                    if (manager.editorLocationIndex >= manager.Locations.Count)
                    {
                        manager.editorLocationIndex = 0;
                    }
                    else if (manager.editorLocationIndex < 0)
                    {
                        manager.editorLocationIndex = manager.Locations.Count - 1;
                    }

                    CurrantLevelIndex = 0;
                    manager.CurrantLevels = manager.Locations[CurrantLocationIndex].levels;

                    location = manager.editorLocationIndex;
                }
            }
        }
        public override int CurrantLevelIndex
        {
            get
            {
                return manager.editorMode ? manager.editorLevelIndex : level;
            }
            set
            {
                if (manager.editorMode)
                {
                    if (value >= manager.Locations[CurrantLocationIndex].levels.Count)
                    {
                        CurrantLocationIndex++;
                        return;
                    }
                    else if (value < 0)
                    {
                        CurrantLocationIndex--;
                        return;
                    }
                    manager.editorLevelIndex = value;
                }
            }
        }

        public override int LevelsDone { get => 0; set { } }
        public override int LocationsDone { get => 0; set { } }
    }

}