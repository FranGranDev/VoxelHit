using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Cannons;
using Data;
using System.Linq;

namespace Factory
{
    public class SimpleCannonFactory : MonoBehaviour, IFactory
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
            CannonBase other = transform.GetComponentInChildren<CannonBase>(true);
            if(!other)
            {
                return;
            }
            Transform parent = other.transform.parent;

            other.transform.parent = null;
            Destroy(other.gameObject);

            int cannonIndex = testMode ? cannonTestIndex : SavedData.CurrantCannonIndex;
            int bulletIndex = testMode ? bulletTestIndex : SavedData.CurrantBulletIndex;

            CannonBase cannon = Instantiate(data.GetCannonInfo(cannonIndex).CannonItem.Prefab, parent).GetComponent<CannonBase>();
            cannon.transform.localPosition = Vector3.zero;
            cannon.transform.localRotation = Quaternion.identity;
            cannon.Apply(new CannonBase.Settings(settings, data.GetBulletInfo(bulletIndex).BulletItem.Prefab));
        }
    }
}