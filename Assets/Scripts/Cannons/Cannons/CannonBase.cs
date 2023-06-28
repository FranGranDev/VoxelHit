using Cannons.Bullets;
using Services;
using Data;
using System;
using UnityEngine;
using Zenject;

namespace Cannons
{
    public abstract class CannonBase : MonoBehaviour, Initializable<GameInfo>, IFillable
    {
        [Header("Internal Settings")]
        [SerializeField] protected Settings settings;

        [Header("Components")]
        [SerializeField] protected Transform firePoint;


        [Inject]
        protected ISoundPlayer soundPlayer;


        protected GameTypes gameType = GameTypes.Shop;
        protected float fireTime;
        protected bool superReady;


        public float ReloadTime(float fireTime = 0)
        {
            if (fireTime > settings.RushTime)
                return 1f / (settings.MaxFireRate * settings.RushRateRatio);
            float ratio = Mathf.Clamp01(fireTime / settings.RushTime);
            return 1f / Mathf.Lerp(settings.FireRate, settings.MaxFireRate, settings.RushCurve.Evaluate(ratio));
        }
        public float Fill
        {
            get
            {
                return superReady ? 1 : Mathf.Clamp01(fireTime / settings.RushTime);
            }
        }


        public event Action OnFire;

        public virtual void Initialize(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;

            gameType = info.SceneContext.GameType;
        }
        public void Apply(Settings settings)
        {
            this.settings = settings;
        }



        public void Fire()
        {
            DoFire();

            OnFire?.Invoke();
        }
        public abstract void EndFire();

        protected abstract void DoFire();
        protected virtual void ImpulseRecoil()
        {

        }



        [Serializable]
        public sealed class Settings
        {
            public Settings(CannonSettings data, GameObject bullet)
            {
                bulletSpeed = data.BulletSpeed;

                fireRate = data.FireRate;
                maxFireRate = data.MaxFireRate;
                maxRushTime = data.MaxRushTime;
                rushRateRatio = data.RushRateRatio;
                rushCurve = data.RushCurve;

                toneCurve = data.ToneCurve;
                volume = data.Volume;

                returnSpeed = data.ReturnSpeed;

                repairCount = data.RepairCount;
                bullerPrefab = bullet;

                maxRecoil = data.MaxRecoil;
                recoilCurve = data.RecoilCurve;
            }

            [Space, SerializeField, Min(1f)] private float bulletSpeed;

            [SerializeField, Min(1f)] private float fireRate;
            [SerializeField, Min(1f)] private float maxFireRate;
            [SerializeField, Min(0f)] private float maxRushTime;
            [SerializeField, Min(0f)] private float returnSpeed;
            [SerializeField, Min(1f)] private float rushRateRatio;
            [SerializeField] private AnimationCurve rushCurve;
            [Space]
            [SerializeField] private AnimationCurve toneCurve;
            [SerializeField] private float volume;

            [Space, SerializeField, Range(1, 10)] private int repairCount;
            [Space, SerializeField] private GameObject bullerPrefab;

            [Space, SerializeField, Min(0)] private float maxRecoil;
            [SerializeField] private AnimationCurve recoilCurve;


            public int RepairCount => repairCount;
            public float FireRate => fireRate;
            public float RushTime => maxRushTime;
            public AnimationCurve RushCurve => rushCurve;
            public float MaxFireRate => maxFireRate;
            public float RushRateRatio => rushRateRatio;
            public float ReturnSpeed => returnSpeed;
            public float BulletSpeed => bulletSpeed;
            public GameObject BulletPrefab => bullerPrefab;

            public float MaxRecoil => maxRecoil;
            public AnimationCurve RecoilCurve => recoilCurve;
            public AnimationCurve ToneCurve => toneCurve;
            public float Volume => volume;
        }
    }
}
