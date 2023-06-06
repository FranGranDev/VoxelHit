using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel.Paint
{
    public class PaintInfo
    {
        public PaintInfo(Vector3 center, Material material, int group, Types type)
        {
            Center = center;
            Material = material;
            Group = group;
            Type = type;
        }


        public Vector3 Center { get; }
        public Material Material { get; }
        public int Group { get; }
        public Types Type { get; }

        public enum Types { Instantly, WaveGroup}
    }
}
