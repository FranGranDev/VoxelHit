using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Taken from 2017
    /// </summary>
    public class SwipeController : MonoBehaviour
    {
        [SerializeField] private float DeadZone;
        [SerializeField] private float DeltaTime;

        private bool isMobile => Application.isMobilePlatform;

        private bool isSwiping;
        private float startTapTime;
        private Vector2 tapPosition;
        private Vector2 swipeDelta;

        public event System.Action<bool> OnSwipe;

        private void Swiping()
        {
            if (!isMobile)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isSwiping = true;
                    tapPosition = Input.mousePosition;
                    startTapTime = Time.time;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    ResetSwipe();
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        isSwiping = true;
                        tapPosition = Input.GetTouch(0).position;
                        startTapTime = Time.time;
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Ended ||
                        Input.GetTouch(0).phase == TouchPhase.Canceled)
                    {
                        ResetSwipe();
                    }
                }
            }

            CheckSwipe();
        }
        private void CheckSwipe()
        {
            if (isSwiping)
            {
                if (!isMobile && Input.GetMouseButton(0))
                {
                    swipeDelta = (Vector2)Input.mousePosition - tapPosition;
                }
                else if (Input.touchCount > 0)
                {
                    swipeDelta = Input.GetTouch(0).position - tapPosition;
                }
            }
            if (swipeDelta.magnitude > DeadZone)
            {
                if (Time.time - startTapTime < DeltaTime)
                {
                    OnSwipe?.Invoke(swipeDelta.x > 0);
                }
                ResetSwipe();
            }
        }
        private void ResetSwipe()
        {
            isSwiping = false;
            tapPosition = Vector2.zero;
            swipeDelta = Vector2.zero;
        }

        private void Update()
        {
            Swiping();
        }
    }
}