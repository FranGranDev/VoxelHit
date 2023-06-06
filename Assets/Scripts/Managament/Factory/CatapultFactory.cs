using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Services;
using Data;

namespace Factory
{
    public class CatapultFactory : MonoBehaviour, IFactory<bool>
    {
        [Header("Links")]
        [SerializeField] private CatapultData data;
        [SerializeField] private CatapultData superData;
        [SerializeField] private BulletData bullets;
        [Header("Settings")]
        [SerializeField] private Transform place;

        public bool Created => false;

        public void Create(bool super)
        {
            GameObject bullet = bullets.First(x => x.Index == SavedData.CurrantBulletIndex).CatapultPrefab;

            place.GetComponentsInChildren<Initializable<CatapultData.Settings, GameObject>>()
                .ToList()
                .ForEach(x => x.Initialize(super ? superData.Data : data.Data, bullet));
        }
    }
}
