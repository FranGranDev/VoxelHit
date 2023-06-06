using Data;
using Voxel;
using Voxel.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    public class PaintModelFactory : MonoBehaviour, IFactory<ModelId>
    {
        [SerializeField] private PaintData paintData;
        [SerializeField] private Transform place;

        private ModelId modelId;

        public void Create(ModelId parameter)
        {
            ModelId other = place.GetComponentInChildren<ModelId>();
            if(other != null)
            {
                other.GetModel.transform.parent = null;
                Destroy(other.GetModel);
            }

            modelId = Instantiate(parameter.GetModel, place).GetComponent<ModelId>();
        }
        public ModelId Created
        {
            get => modelId;
        }
    }
}
