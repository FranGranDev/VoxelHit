using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Voxel.Paint;

namespace Voxel.Utils
{
    public class VoxelMaterialSetter : MonoBehaviour
    {
        [SerializeField] private List<Material> materialGroup;

        [Button("Apply Materials")]
        private void SetColor()
        {
            List<IVoxel> allVoxels = new List<IVoxel>(GetComponentsInChildren<IVoxel>());

            foreach(Material material in materialGroup)
            {
                if (material == null)
                    continue;

                allVoxels.Where(x => x.ColorIndex == materialGroup.IndexOf(material)).ToList().ForEach(x => x.Material = material);
            }
        }
        
        [Button("Make Material Groups")]
        private void MakeGroups()
        {
            List<IVoxel> allVoxels = new List<IVoxel>(GetComponentsInChildren<IVoxel>());

            Dictionary<Material, List<IVoxel>> voxelsDict = new Dictionary<Material, List<IVoxel>>();

            foreach(IVoxel voxel in allVoxels)
            {
                if (voxelsDict.ContainsKey(voxel.Material))
                {
                    voxelsDict[voxel.Material].Add(voxel);
                }
                else
                {
                    voxelsDict.Add(voxel.Material, new List<IVoxel>() { voxel });
                }
            }

            voxelsDict = voxelsDict.OrderByDescending(x => x.Value.Count).ToDictionary(x => x.Key, x => x.Value);

            int index = 0;

            foreach (List<IVoxel> voxelsGroup in voxelsDict.Values)
            {
                foreach (IVoxel voxel in voxelsGroup)
                {
                    voxel.ColorIndex = index;
                    allVoxels.Add(voxel);
                }
                index++;
            }

            Debug.Log(index);

            GetComponent<VoxelObject>().InitilizeOnCreate(index);
        }

        [Button("Set Grayscale Colors")]
        private void SetGrayscaleColors()
        {
            if(TryGetComponent(out ColorSetter setter))
            {
                List<IVoxel> allVoxels = new List<IVoxel>(GetComponentsInChildren<IVoxel>());

                setter.Initilize(allVoxels, materialGroup.Count);
                setter.SetGrayscaleColors();
            }
        }
    }
}
