using Cannons;
using Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxel;

namespace Factory
{
    public class FreeCannonApplier : MonoBehaviour, IFactory<FreeCannon, VoxelObject>
    {
        [Header("Data")]
        [SerializeField] private CannonSettings settings;
        [Space]
        [SerializeField] private SavedData data;
        [Header("Test")]
        [SerializeField] private bool testMode;
        [SerializeField] private int bulletTestIndex;


        public void Create(FreeCannon cannon, VoxelObject voxelObject)
        {
            int bulletIndex = testMode ? bulletTestIndex : SavedData.CurrantBulletIndex;

            cannon.Apply(new CannonBase.Settings(settings, data.GetBulletInfo(bulletIndex).BulletItem.Prefab));
            cannon.Apply(voxelObject.Center);
        }
    }
}
