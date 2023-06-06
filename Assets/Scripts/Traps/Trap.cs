using Cannons.Bullets;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Services;

namespace Traps
{
    public class Trap : MonoBehaviour, Initializable<GameInfo>, IBindable<IGameEventsHandler>
    {
        private List<HitTrigger> triggers;
        private List<TrapPart> parts;

        public bool Disabled { get; set; }

        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnDone += OnDone;

            eventsHandler.OnClearScene += (x) =>
            {
                eventsHandler.OnDone -= OnDone;
            };
        }

        private void OnDone()
        {
            if (Disabled)
                return;
            parts.ForEach(x => x.Demolish(false));
        }

        public void Initialize(GameInfo gameInfo)
        {
            triggers = GetComponentsInChildren<HitTrigger>().ToList();
            parts = GetComponentsInChildren<TrapPart>().ToList();

            parts.ForEach(x => x.Initialize(gameInfo));
            triggers.ForEach(x => x.OnBulletEnter += OnBulletEnter);
        }


        private void OnBulletEnter(RepairHitInfo obj)
        {
            if(obj.Bullet.Settings.IsSuper)
            {
                parts.ForEach(x => x.Demolish());
                triggers.ForEach(x => x.gameObject.SetActive(false));
            }
            else
            {
                parts.ForEach(x => x.PlayHit());
            }
        }
    }
}
