using System.Collections.Generic;
using UnityEngine;


namespace Voxel.Data
{
    [System.Serializable]
    public class VoxelObjectData
    {        
        public VoxelObjectData(int groupCount, List<VoxelData> voxels)
        {
            GroupCount = groupCount;
            Voxels = voxels;
        }

        public int GroupCount;
        public List<VoxelData> Voxels;
    }
}
