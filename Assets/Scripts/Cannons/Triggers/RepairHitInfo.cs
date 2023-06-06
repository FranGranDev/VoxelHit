using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons.Bullets
{
    public class RepairHitInfo
    {
        public RepairHitInfo(RepairBulletBase bullet, Types hitType)
        {
            Bullet = bullet;
            Info = bullet.Settings;
            HitType = hitType;
        }

        public RepairBulletInfo Info { get; }
        public RepairBulletBase Bullet { get; }
        public Types HitType { get; }

        public enum Types { Repair, Break}
    }
    public class BreakHitInfo
    {

    }
}
