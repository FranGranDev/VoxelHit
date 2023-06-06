using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


namespace Voxel.Utils
{
    public class VolumeMaker : MonoBehaviour
    {
        [SerializeField] private float maxAddScale;
        [SerializeField] private AnimationCurve scaleCurve;

        [Button]
        public void MakeVolume()
        {
            MakeVoxels();
            Normalize();

            Dictionary<Vector3, IVoxel> voxelsDict = GetComponentsInChildren<IVoxel>().ToDictionary(x => x.Position);

            List<List<IVoxel>> voxelLayers = new List<List<IVoxel>>();

            int num = 0;
            while (voxelsDict.Count > 0 && num < 10)
            {
                List<IVoxel> layer = new List<IVoxel>();
                foreach (IVoxel voxel in voxelsDict.Values)
                {
                    if (OnEdge(voxel, voxelsDict))
                    {
                        layer.Add(voxel);
                    }
                }
                foreach (IVoxel voxel in layer)
                {
                    voxelsDict.Remove(voxel.Position);
                }
                voxelLayers.Add(layer);

                num++;
            }
            
            for(int i = 0; i < voxelLayers.Count; i++)
            {
                float ratio = (float)(i) / ((float)voxelLayers.Count - 1);

                foreach(IVoxel voxel in voxelLayers[i])
                {
                    voxel.Scale = Vector3.one + Vector3.forward * Mathf.Lerp(0, maxAddScale, scaleCurve.Evaluate(ratio));
                }
            }
        }
        private bool OnEdge(IVoxel voxel, Dictionary<Vector3, IVoxel> dict)
        {
            return !dict.ContainsKey(voxel.Position + Vector3.up) ||
                   !dict.ContainsKey(voxel.Position + Vector3.right) ||
                   !dict.ContainsKey(voxel.Position + Vector3.down) ||
                   !dict.ContainsKey(voxel.Position + Vector3.left);
        }

        [Button]
        public void Normalize()
        {
            List<IVoxel> voxels = new List<IVoxel>(GetComponentsInChildren<IVoxel>());
            foreach(IVoxel voxel in voxels)
            {
                voxel.Scale = Vector3.one;
                voxel.Position = new Vector3(voxel.Position.x, voxel.Position.y, 0);
            }
        }

        [Button]
        public void MakeVoxels()
        {
            foreach(Transform child in transform)
            {
                if(!child.gameObject.TryGetComponent(out Voxel voxel))
                {
                    child.gameObject.AddComponent<Voxel>();
                }
            }
        }

        [Button]
        public void Up()
        {
            List<IVoxel> voxels = new List<IVoxel>(GetComponentsInChildren<IVoxel>());
            foreach (IVoxel voxel in voxels)
            {
                voxel.Position += Vector3.up;
            }
        }
        [Button]
        public void Down()
        {
            List<IVoxel> voxels = new List<IVoxel>(GetComponentsInChildren<IVoxel>());
            foreach (IVoxel voxel in voxels)
            {
                voxel.Position -= Vector3.up;
            }
        }
        [Button]
        public void Right()
        {
            List<IVoxel> voxels = new List<IVoxel>(GetComponentsInChildren<IVoxel>());
            foreach (IVoxel voxel in voxels)
            {
                voxel.Position += Vector3.right;
            }
        }
        [Button]
        public void Left()
        {
            List<IVoxel> voxels = new List<IVoxel>(GetComponentsInChildren<IVoxel>());
            foreach (IVoxel voxel in voxels)
            {
                voxel.Position -= Vector3.right;
            }
        }
    }
}
