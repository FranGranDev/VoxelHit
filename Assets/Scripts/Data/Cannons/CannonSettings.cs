using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


namespace Data
{
    [CreateAssetMenu(fileName = "Cannon Settings", menuName = "Game Data/Cannon Settings")]
    public class CannonSettings : ScriptableObject
    {
        [Foldout("Base Settings"), SerializeField, Min(1f)] private float bulletSpeed;
        [Foldout("Base Settings"), SerializeField, Min(1f)] private float fireRate;
        [Foldout("Base Settings"), SerializeField, Min(1f)] private float maxFireRate;

        [Foldout("Rush Settings"), Space, SerializeField, Min(0f)] private float maxRushTime;
        [Foldout("Rush Settings"), SerializeField, Min(1f)] private float rushRateRatio;
        [Foldout("Rush Settings"), SerializeField, Min(0f)] private float returnSpeed;
        [Foldout("Rush Settings"), SerializeField] private AnimationCurve rushCurve;

        [Foldout("Sound Settings"), SerializeField] private float volume;
        [Foldout("Sound Settings"), SerializeField] private AnimationCurve toneCurve;

        [Foldout("Repair Settings"), Space, SerializeField, Range(1, 10)] private int repairCount;

        [Foldout("Recoil Settings"), Space, SerializeField, Min(0)] private float maxRecoil;
        [Foldout("Recoil Settings"), SerializeField] private AnimationCurve recoilCurve;


        public float BulletSpeed { get => bulletSpeed; }
        public float FireRate { get => fireRate; }
        public float MaxFireRate { get => maxFireRate; }
        public float MaxRushTime { get => maxRushTime; }
        public float RushRateRatio { get => rushRateRatio; }
        public float ReturnSpeed { get => returnSpeed; }
        public AnimationCurve RushCurve { get => rushCurve; }
        public int RepairCount { get => repairCount; }
        public float MaxRecoil { get => maxRecoil; }
        public AnimationCurve RecoilCurve { get => recoilCurve; }
        public float Volume { get => volume; }
        public AnimationCurve ToneCurve { get => toneCurve; }
    }
}
