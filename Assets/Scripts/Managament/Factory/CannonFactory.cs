using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Cannons;
using Data;
using System.Linq;

namespace Factory
{
    public class CannonFactory : MonoBehaviour, IFactory
    {
        [Header("Data")]
        [SerializeField] private CannonSettings settings;
        [Space]
        [SerializeField] private SavedData data;
        [Header("Test")]
        [SerializeField] private bool testMode;
        [SerializeField] private int cannonTestIndex;
        [SerializeField] private int bulletTestIndex;

        public void Create()
        {
            List<IPlaceable<CannonBase>> places = transform.GetComponentsInChildren<IPlaceable<CannonBase>>().ToList();

            foreach (IPlaceable<CannonBase> place in places)
            {
                CannonBase other = place.TryGetObject();
                if (other)
                {
                    other.transform.parent = null;
                    Destroy(other.gameObject);
                }
            }
            foreach (IPlaceable<CannonBase> place in places)
            {
                int cannonIndex = testMode ? cannonTestIndex : SavedData.CurrantCannonIndex;
                int bulletIndex = testMode ? bulletTestIndex : SavedData.CurrantBulletIndex;

                CannonBase cannon = Instantiate(data.GetCannonInfo(cannonIndex).CannonItem.Prefab, place.Place).GetComponent<CannonBase>();
                cannon.transform.localPosition = Vector3.zero;
                cannon.transform.localRotation = Quaternion.identity;
                cannon.Apply(new CannonBase.Settings(settings, data.GetBulletInfo(bulletIndex).BulletItem.Prefab));
            }
        }
    }
}