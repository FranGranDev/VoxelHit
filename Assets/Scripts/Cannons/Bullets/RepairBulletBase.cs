using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons.Bullets
{
    public abstract class RepairBulletBase : MonoBehaviour
    {
        public RepairBulletInfo Settings { get; private set; }

        public virtual void Run(RepairBulletInfo settings)
        {
            Settings = settings;
        }
        public abstract void Redirect(RicochetInfo path);
        public abstract void Demolish(DemolishTypes type);
    }

    public enum DemolishTypes { None, Repair, OuterRepair }
    public class RicochetInfo
    {
        public RicochetInfo(Vector3 target, float topHeight, AnimationCurve curve, System.Action onCompleate)
        {
            Target = target;
            TopHeight = topHeight;
            Curve = curve;
            OnCompleate = onCompleate;
        }

        public Vector3 Target { get; }
        public float TopHeight { get; }
        public AnimationCurve Curve { get; }
        public System.Action OnCompleate { get; }
    }
    public class RepairBulletInfo
    {
        public RepairBulletInfo(RepairBulletBase bullet, int repairCount, float speed, float volume, float tone, bool isSuper = false)
        {
            Bullet = bullet;
            Speed = speed;
            RepairCount = repairCount;
            Tone = tone;
            Volume = volume;

            IsSuper = isSuper;
        }

        public RepairBulletBase Bullet { get; }
        public int RepairCount { get; }
        public float Speed { get; }
        public float Tone { get; }
        public float Volume { get; }

        public bool IsSuper { get; }
    }
}
