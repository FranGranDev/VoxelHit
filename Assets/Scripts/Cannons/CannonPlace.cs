using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;

namespace Cannons
{
    public class CannonPlace : MonoBehaviour, IPlaceable<CannonBase>
    {
        [SerializeField] private Transform cannonPlace;

        public Transform Place => cannonPlace;

        public CannonBase TryGetObject()
        {
            return cannonPlace.GetComponentInChildren<CannonBase>();
        }
    }
}
