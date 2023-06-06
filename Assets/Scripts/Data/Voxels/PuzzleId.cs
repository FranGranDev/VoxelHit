using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class PuzzleId : MonoBehaviour
    {
        [SerializeField] private string group;

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
