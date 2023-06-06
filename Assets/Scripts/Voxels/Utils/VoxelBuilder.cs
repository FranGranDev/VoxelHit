using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Voxel.Data;
using NaughtyAttributes;


namespace Voxel.Utils
{
    public class VoxelBuilder : MonoBehaviour
    {
        [SerializeField] private Texture2D inputTexture;
        [SerializeField] private GameObject voxelPrefab;
        [SerializeField] private GameObject parentPrefab;

        [Button]
        private void Build()
        {
#if UNITY_EDITOR
            VoxelObjectData data = GenerateData();
            if (data == null)
                return;

            VoxelObject voxelObject = (UnityEditor.PrefabUtility.InstantiatePrefab(parentPrefab) as GameObject).GetComponent<VoxelObject>();

            foreach (VoxelData info in data.Voxels)
            {
                GameObject voxelItem = UnityEditor.PrefabUtility.InstantiatePrefab(voxelPrefab) as GameObject;
                voxelItem.transform.SetParent(voxelObject.transform);
                voxelItem.GetComponent<Voxel>().Initilize(info);
            }

            voxelObject.InitilizeOnCreate(data.GroupCount);

            if(voxelObject.TryGetComponent(out VolumeMaker volumeMaker))
            {
                volumeMaker.MakeVolume();
            }
#endif
        }

        private VoxelObjectData GenerateData()
        {
            if (inputTexture == null)
            {
                Debug.LogWarning("No input texture!");
                return null;
            }

            Dictionary<float, List<VoxelData>> voxels = new Dictionary<float, List<VoxelData>>();

            int halfWidht = inputTexture.width / 2;

            for (int y = 0; y < inputTexture.height; y++)
            {
                for (int x = 0; x < inputTexture.width; x++)
                {
                    Color pixel = inputTexture.GetPixel(x, y);
                    if (pixel.a < 0.5f)
                    {
                        continue;
                    }

                    VoxelData voxelData = new VoxelData(new Vector3(x - halfWidht, y, 0));

                    if (voxels.ContainsKey(ColorId(pixel)))
                    {
                        voxels[ColorId(pixel)].Add(voxelData);
                    }
                    else
                    {
                        voxels.Add(ColorId(pixel), new List<VoxelData>() { voxelData });
                    }
                }
            }

            voxels = voxels.OrderByDescending(x => x.Value.Count).ToDictionary(x => x.Key, x => x.Value);

            int index = 0;
            List<VoxelData> allVoxels = new List<VoxelData>();
            
            foreach (List<VoxelData> voxelsGroup in voxels.Values)
            {
                foreach (VoxelData voxel in voxelsGroup)
                {
                    voxel.ColorIndex = index;
                    allVoxels.Add(voxel);
                }

                index++;
            }

            return new VoxelObjectData(index, allVoxels);
        }
        private int ColorId(Color pixel)
        {
            return Mathf.RoundToInt(pixel.grayscale * 100);
        }
    }
}
