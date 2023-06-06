using Services;
using System.Linq;
using System.Collections.Generic;
using Data;
using EventRoad;
using UI;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

namespace Managament
{
    public class EventRoadController : MonoBehaviour
    {
        [Header("Links")]
        [SerializeField] private PuzzleModelsData modelsData;
        [SerializeField] private EventRoadUI roadUI;


        private Road road;
        private SavedData.EventInfo eventInfo;
        private SavedData.EventInfo nextInfo;

        public event System.Action<PuzzleId> OnRoadDone;
        public event System.Action<PuzzleId, ModelId, bool> OnPlay;
        public event System.Action OnExit;


        public void Initialize(GameInfo gameInfo, SavedData.EventInfo eventInfo, SavedData.EventInfo nextInfo)
        {
            this.eventInfo = eventInfo;
            this.nextInfo = nextInfo;

            roadUI.OnPlay += PlayClick;
            roadUI.OnExit += ExitClick;

            

            roadUI.Initialize(gameInfo, eventInfo.Collection, nextInfo?.Collection);

            road = GetComponentInChildren<Road>();

            road.Initialize(gameInfo, eventInfo);
            SavedData.PuzzleItemInfo item = eventInfo.Items
                .Where(x => x.Opened && !x.Placed)
                .FirstOrDefault();


            EventRoadUI.States state = EventRoadUI.States.Main;
            if(eventInfo.Items.Count(x => !x.Opened) == 0)
            {
                if(nextInfo == null)
                {
                    state = EventRoadUI.States.AllDone;
                }
                else
                {
                    state = EventRoadUI.States.Done;
                }
            }


            if (item != null)
            {
                item.SetPlaced();


                road.SetPointDone(item, () => roadUI.State = state);
            }
            else
            {
                road.SetPoint(eventInfo.Currant);

                roadUI.State = state;
            }
        }

        private void ExitClick()
        {
            OnExit?.Invoke();
        }
        private void PlayClick()
        {
            if (eventInfo.Remaining > 0)
            {
                bool boss = eventInfo.Currant == eventInfo.Items.Last();
                OnPlay?.Invoke(eventInfo.PuzzleId, eventInfo.Currant.ModelId, boss);
                return;
            }

            if(nextInfo == null)
            {
                OnExit?.Invoke();
                return;
            }


            if (SavedData.LevelsDone >= nextInfo.MinLevel)
            {
                OnRoadDone?.Invoke(eventInfo.PuzzleId);
            }
            else
            {
                OnExit?.Invoke();
            }
        }

        #region Internal

        [Button]
        private void MoveNext()
        {
            roadUI.State = EventRoadUI.States.None;
            road.MoveNext(() =>
            {
                roadUI.State = EventRoadUI.States.Main;
            });
        }

        #endregion
    }
}
