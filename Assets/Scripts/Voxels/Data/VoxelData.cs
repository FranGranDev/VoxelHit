using UnityEngine;


namespace Voxel.Data
{
    public class VoxelData
    {
        public VoxelData(Vector3 position)
        {
            Position = position;
        }

        public int ColorIndex { get; set; }
        public Vector3 Position { get; }
    }
}
