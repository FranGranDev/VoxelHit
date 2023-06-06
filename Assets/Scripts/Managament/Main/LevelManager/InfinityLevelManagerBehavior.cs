
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Linq;


namespace Managament.Levels
{
    public class InfinityLevelManagerBehavior : LevelManagerBehavior
    {
        public InfinityLevelManagerBehavior(LevelManagement manager) : base(manager)
        {

        }

        public override int CurrantLocationIndex
        {
            get
            {
                int index = manager.editorMode ? manager.editorLocationIndex : SavedData.InfinityLocationIndex;
                return Mathf.Clamp(index, 0, manager.Locations.Count - 1);
            }
            set
            {
                LocationsDone++;

                List<int> except = new List<int>();
                except.Add(CurrantLocationIndex);
                manager.Locations.ForEach(x =>
                {
                    if (!x.useInRandom)
                        except.Add(manager.Locations.IndexOf(x));
                });
                SavedData.InfinityLocationIndex = Extentions.GetRandom(0, manager.Locations.Count, except);


                manager.CurrantLevels = manager.GenerateRandomLevels();
                CurrantLevelIndex = 0;
            }
        }
        public override int CurrantLevelIndex
        {
            get
            {
                return manager.editorMode ? manager.editorLevelIndex : SavedData.InfinityLevelIndex;
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
                        else if (value < 0)
                        {
                            CurrantLocationIndex--;
                            return;
                        }
                        SavedData.InfinityLevelIndex = value;
                        LevelsDone++;
                    }
                }
                catch { }
            }
        }

        public override int LevelsDone { get => int.MaxValue; set => SavedData.InfinityLevelsDone = value; }
        public override int LocationsDone { get => int.MaxValue; set => SavedData.InfinityLocationsDone = value; }
    }
}