using Services;
using System.Collections;
using System.Collections.Generic;
using Data;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace EventRoad
{
    public class Road : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string group;

        private List<RoadPoint> roadPoints;
        private MovePoint movePoint;
        private RoadPoint currant;

        public string Group
        {
            get => group;
        }
        public RoadPoint Currant
        {
            get => currant;
            private set
            {
                currant = value;
                currant.State = RoadPoint.States.Currant;
            }
        }

        public void Initialize(GameInfo gameInfo, SavedData.EventInfo eventInfo)
        {
            roadPoints = GetComponentsInChildren<RoadPoint>()
                .OrderBy(x => x.Index)
                .ToList();
            movePoint = GetComponentInChildren<MovePoint>();

            foreach (RoadPoint point in roadPoints)
            {
                SavedData.PuzzleItemInfo info = eventInfo.Items.FirstOrDefault(a => a.Index == point.Index);
                SavedData.PuzzleItemInfo nextInfo = eventInfo.Items.FirstOrDefault(a => a.Index == point.Index + 1);

                RoadPoint.States state = RoadPoint.States.Closed;
                if (info != null && info.Placed)
                {
                    state = RoadPoint.States.Done;
                }
                else
                {
                    state = RoadPoint.States.Closed;
                }

                RoadPoint next = roadPoints.FirstOrDefault(a => a.Index == point.Index + 1);
                RoadPoint prev = roadPoints.FirstOrDefault(a => a.Index == point.Index - 1);

                point.Initialize(gameInfo, prev, next, state);
            }
        }

        public void SetPoint(SavedData.PuzzleItemInfo item)
        {
            if (item == null)
            {
                Currant = roadPoints.Last();
            }
            else
            {
                Currant = roadPoints.FirstOrDefault(x => x.Index == item.Index).PrevPoint;
            }
            movePoint.SetPoint(Currant);

            if (currant.Type == RoadPoint.Types.Final)
            {
                movePoint.Dance();
                currant.Hide();
            }
        }
        public void SetPointDone(SavedData.PuzzleItemInfo item, System.Action callback = null)
        {
            if (item == null)
            {
                Currant = roadPoints.Last();
            }
            else
            {
                Currant = roadPoints.FirstOrDefault(x => x.Index == item.Index).PrevPoint;
            }
            movePoint.SetPoint(Currant);

            MoveNext(callback);
        }

        public void MoveNext(System.Action callback = null)
        {
            if (currant == null)
            {
                callback?.Invoke();
                return;
            }

            this.Delayed(0.25f, () =>
            {
                if (currant.NextPoint)
                {
                    currant.State = RoadPoint.States.Done;
                    currant = Currant.NextPoint;

                    movePoint.MoveToPoint(currant, 0f, () =>
                    {
                        currant.OnStartEnter();
                    },
                    () =>
                    {
                        currant.State = RoadPoint.States.Currant;
                        currant.OnEnter();
                        callback?.Invoke();

                        if (currant.Type == RoadPoint.Types.Final)
                        {
                            movePoint.Dance();
                        }
                    });
                }
                else
                {
                    callback?.Invoke();
                }
            });
        }
    }
}
