using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Linq;


namespace Managament.Levels
{
    public class GameLevelManagerBehavior : LevelManagerBehavior
    {
        public GameLevelManagerBehavior(LevelManagement manager) : base(manager)
        {

        }

        public override int CurrantLocationIndex
        {
            get
            {
                int index = manager.editorMode ? manager.editorLocationIndex : SavedData.LocationIndex;
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
                }
                else
                {
                    manager.LocationsDone++;

                    if (manager.LocationsDone >= manager.Locations.Count)
                    {
                        List<int> except = new List<int>();
                        except.Add(CurrantLocationIndex);
                        manager.Locations.ForEach(x => { if (!x.useInRandom) except.Add(manager.Locations.IndexOf(x)); });

                        SavedData.LocationIndex = Extentions.GetRandom(0, manager.Locations.Count, except);

                        manager.CurrantLevels = manager.GenerateRandomLevels();
                        CurrantLevelIndex = 0;
                    }
                    else
                    {
                        SavedData.LocationIndex = value;
                        manager.CurrantLevels = manager.Locations[CurrantLocationIndex].levels;
                        CurrantLevelIndex = 0;
                    }
                }
            }
        }
        public override int CurrantLevelIndex
        {
            get
            {
                return manager.editorMode ? manager.editorLevelIndex : SavedData.LevelIndex;
            }
            set
            {
                try
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
                    else
                    {
                        if (value >= manager.CurrantLevels.Count)
                        {
                            CurrantLocationIndex++;
                            return;
                        }
                        SavedData.LevelIndex = value;
                    }
                }
                catch { }
            }
        }

        public override int LevelsDone { get => SavedData.LevelsDone; set => SavedData.LevelsDone = value; }
        public override int LocationsDone { get => SavedData.LocationsDone; set => SavedData.LocationsDone = value; }
    }

}