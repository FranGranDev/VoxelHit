using System.Collections;
using System.Linq;
using UnityEngine;
using Data;
using NaughtyAttributes;

namespace Voxel
{
    public class VoxelSpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Min(0.02f)] private float spawnTime;
        [SerializeField, Range(0, 1f)] private float randomize;
        [SerializeField, Min(1f)] private float destroyTime;
        [SerializeField] private Transform place;
        [SerializeField] private float power;
        [SerializeField] private PaintData paintData;
        [Header("Prefab")]
        [SerializeField] private Voxel prefab;

        private Coroutine coroutine;

        [Button]
        public void StartSpawn()
        {
            if (coroutine != null)
                return;

            coroutine = StartCoroutine(SpawnCour());
        }
        public void StopSpawn()
        {
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private IEnumerator SpawnCour()
        {
            var wait = new WaitForSeconds(spawnTime);

            while(true)
            {
                Voxel voxel = Instantiate(prefab, place.position, place.rotation, place);

                voxel.Initilize(this, Services.GameTypes.Break);
                voxel.Rigidbody.isKinematic = false;
                voxel.Material = paintData.GetRandomBaseColor().Material;

                Vector3 direction = (place.forward + Random.onUnitSphere * randomize).normalized;
                voxel.AddImpulse(direction * power, Random.onUnitSphere * 270);

                Destroy(voxel.gameObject, destroyTime);

                yield return wait;
            }
        }


    }
}
