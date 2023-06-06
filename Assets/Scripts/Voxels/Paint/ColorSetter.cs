using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Voxel;
using NaughtyAttributes;
using Voxel.Data;

namespace Voxel.Paint
{
    public class ColorSetter : MonoBehaviour
    {
        [SerializeField] private Material baseMaterial;
        [SerializeField] private AnimationCurve grayscaleCurve;

        private List<IVoxel> voxels;
        private int groupCount;

        public void Initilize(List<IVoxel> voxels, int groupCount)
        {
            this.groupCount = groupCount;
            this.voxels = voxels;
        }

        public float TransformedGrayscale(float grayscale)
        {
            return grayscaleCurve.Evaluate(grayscale);
        }

        public void SetGrayscaleColors()
        {
            for(int i = 0; i < groupCount; i++)
            {
                float grayScale = 0.25f;
                try
                {
                    grayScale = voxels.FirstOrDefault(x => x.ColorIndex == i).Grayscale;
                }
                catch { }
                grayScale = TransformedGrayscale(grayScale);

                Material material = new Material(baseMaterial);
                material.color = new Color(grayScale, grayScale, grayScale, 1);

                PaintGroup(i, material);
            }
        }


        public void PaintGroup(int group, Material material)
        {
            voxels.Where(x => x.ColorIndex == group).ToList().ForEach(x => x.Material = material);
        }
        public void PaintVoxel(IVoxel voxel, Material material)
        {
            voxel.Material = material;
        }
    }
}
