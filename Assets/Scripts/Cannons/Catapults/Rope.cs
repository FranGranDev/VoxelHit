using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons.Catapults
{
    [ExecuteAlways]
    public class Rope : MonoBehaviour
    {
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [Space]
        [SerializeField] private LineRenderer lineRenderer;

        private void LateUpdate()
        {
            if (!startPoint || !endPoint || !lineRenderer)
                return;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, endPoint.position);
        }
    }
}
