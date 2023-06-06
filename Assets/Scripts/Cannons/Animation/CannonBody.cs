using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons.Animations
{
    public class CannonBody : MonoBehaviour, IMainPart
    {
        public Transform Part
        {
            get => transform;
        }
    }
}
