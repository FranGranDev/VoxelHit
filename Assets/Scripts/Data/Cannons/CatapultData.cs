using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game Data/Catapult Data", fileName = "Catapult Data")]
    public class CatapultData : ScriptableObject
    {
        [SerializeField] private Settings settings;

        public Settings Data => settings;
        

        [System.Serializable]
        public class Settings
        {
            [Header("Important")]
            [SerializeField] private int bulletsCount;
            [SerializeField] private int restartBulletsAdd;
            [Header("Main")]
            [SerializeField] private float bulletSpeed;
            [SerializeField] private float bulletPower;
            [SerializeField] private float splashRadius;
            [Space]
            [Header("Animation")]
            [SerializeField] private float maxRopeLenght;
            [SerializeField, Range(0f, 1f)] private float minFireRatio;
            [SerializeField] private AnimationCurve flyCurve;
            [SerializeField] private float topHeight;
            [Space]
            [SerializeField] private float fireKeyTime;
            [SerializeField] private float fireTime;
            [SerializeField] private AnimationCurve fireCurve;
            [Space]
            [SerializeField] private float cancelTime;
            [SerializeField] private AnimationCurve cancelCurve;


            public int BulletsCount => bulletsCount;
            public int RestartBulletsAdd => restartBulletsAdd;

            public float BulletSpeed => bulletSpeed;
            public float BulletPower => bulletPower;
            public float SplashRadius => splashRadius;

            public float MaxRopeLenght => maxRopeLenght;
            public float MinFireRatio => minFireRatio;
            public AnimationCurve FlyCurve => flyCurve;
            public float TopHeight => topHeight;

            public float FireKeyTime => fireKeyTime;
            public float FireTime => fireTime;
            public AnimationCurve FireCurve => fireCurve;

            public float CancelTime => cancelTime;
            public AnimationCurve CancelCurve => cancelCurve;
        }
    }
}
