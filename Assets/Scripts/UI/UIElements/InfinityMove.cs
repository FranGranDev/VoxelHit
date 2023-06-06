using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Items
{
    public class InfinityMove : MonoBehaviour
    {
        [SerializeField] private TutorHand hand;
        [SerializeField] private float speedX;
        [SerializeField] private float speedY;
        [Space]
        [SerializeField] private Transform leftPoint;
        [SerializeField] private Transform rightPoint;


        private void Start()
        {
            StartCoroutine(MoveCour());
        }

        private IEnumerator MoveCour()
        {
            float height = (leftPoint.position - rightPoint.position).magnitude;

            float time = 0;

            var wait = new WaitForFixedUpdate();

            while(true)
            {
                float ratioX = Mathf.Sin(Time.fixedTime * speedX) * 0.5f + 0.5f;
                float ratioY = Mathf.Cos(Time.fixedTime * speedY) * 0.5f - 0.5f;

                Vector3 position = Vector3.Lerp(leftPoint.position, rightPoint.position, ratioX)
                    + Vector3.up * height * ratioY;
                hand.GetTransform.position = position;

                yield return wait;
            }
        }
    }
}
