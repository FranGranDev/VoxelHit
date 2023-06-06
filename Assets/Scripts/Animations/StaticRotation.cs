using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{
    public class StaticRotation : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Vector3 axis;

        private void FixedUpdate()
        {
            transform.Rotate(axis, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
