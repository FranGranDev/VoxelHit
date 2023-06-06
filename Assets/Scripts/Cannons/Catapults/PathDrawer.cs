using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cannons.Catapults
{
    public class PathDrawer : MonoBehaviour, IPathDrawer
    {
        [Header("Settings")]
        [SerializeField] private float delta = 0.1f;
        [Header("Components")]
        [SerializeField] private LineRenderer lineRenderer;


        public void Draw(Path path)
        {
            if (path == null)
            {
                lineRenderer.positionCount = 0;
                return;
            }

            float distance = (path.StartPoint - path.EndPoint).magnitude;
            int count = Mathf.FloorToInt(distance / delta);

            lineRenderer.positionCount = count;
            for (int i = 0; i < count; i++)
            {
                float ratio = (float)i / (float)count;

                lineRenderer.SetPosition(i, path.GetPoint(ratio));
            }
        }

    }

    public class NullPathDrawer : IPathDrawer
    {
        public void Draw(Path path)
        {
            
        }
    }
}
