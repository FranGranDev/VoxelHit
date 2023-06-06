using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventRoad
{
    public class RoadCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private MovePoint movePoint;
        [SerializeField] private float ratioX;
        [SerializeField] private float offsetZ;
        [Header("Points")]
        [SerializeField] private Transform min;
        [SerializeField] private Transform max;

        private void Start()
        {
            if (movePoint == null)
                return;

            transform.position = GetPosition();
        }
        private void FixedUpdate()
        {
            if (movePoint == null)
                return;
            transform.position = Vector3.Lerp(transform.position, GetPosition(), 0.1f);
        }

        private Vector3 GetPosition()
        {
            float targetZ = Mathf.Clamp(movePoint.Point.position.z, min.position.z, max.position.z);
            return new Vector3(movePoint.Point.position.x * ratioX, transform.position.y, targetZ + offsetZ);
        }
    }
}
