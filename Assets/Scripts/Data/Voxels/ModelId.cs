using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class ModelId : MonoBehaviour
    {
        [SerializeField] private int index;
        [SerializeField] private string group;

        public int Index
        {
            get => index;
            set => index = value;
        }
        public string Group
        {
            get => group;
            set => group = value;
        }
        public GameObject GetModel
        {
            get
            {
                return gameObject;
            }
        }
    }
}
