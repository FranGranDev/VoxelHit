using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Factory
{
    public class VoxelModelFactory : MonoBehaviour, IFactory<ModelId>
    {
        [Header("Test")]
        [SerializeField] private bool testMode;
        [SerializeField] private ModelId testModel;

        public ModelId Created { get; private set; }

        public void Create(ModelId modelId)
        {
            if (modelId == null)
                return;

            ModelId other = GetComponentInChildren<ModelId>();
            if (other == null)
            {
                Debug.LogError($"No model on level");
                return;
            }
                

            if (other.Index == modelId.Index && other.Group == modelId.Group && !testMode)
            {
                Created = other;
                return;
            }

            Transform parent = other.transform.parent;

            other.transform.parent = null;
            Destroy(other.gameObject);

            try
            {
                if (testMode || modelId == null)
                {
                    Created = Instantiate(testModel, parent);
                    return;
                }
                Created = Instantiate(modelId, parent);
            }
            catch
            {
                Debug.LogError($"Test Model is null");
            }
        }
    }
}
