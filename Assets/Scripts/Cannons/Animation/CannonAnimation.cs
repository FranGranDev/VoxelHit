using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons.Animations
{
    public class CannonAnimation : MonoBehaviour
    {
        [SerializeField] private IMainPart body;
        [SerializeField] private IEnumerable<IJointPart> joints;


        private void Awake()
        {
            body = GetComponentInChildren<IMainPart>();
            joints = GetComponentsInChildren<IJointPart>();
        }

        public Transform Main
        {
            get => body.Part;
        }
    }
}
