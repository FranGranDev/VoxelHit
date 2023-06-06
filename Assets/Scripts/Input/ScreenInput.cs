using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;

namespace UserInput
{
    public class ScreenInput : MonoBehaviour, IScreenInput
    {
        private const float MIN_CLICK_DELAY = 0.5f;

        [SerializeField] private LayerMask layerMask;

        private bool touched;

        private float prevTapTime;
        private GameObject prevObject;


        public event Action<Vector3, GameObject> OnTap;
        public event Action<Vector3, GameObject> OnTapEnded;
        public event Action<Vector3, GameObject> OnClick;
        public event Action<Vector3, GameObject> OnDrag;

        private Vector3 prevPoint;

        public void SetMask(LayerMask layerMask)
        {
            this.layerMask = layerMask;
        }


        private void PhoneInput()
        {
            if(Input.touchCount > 0 && !touched)
            {
                OnTouched(Input.GetTouch(0).position);
            }

            if(Input.touchCount == 0 && touched)
            {
                OnTouchEnd();
            }

            if(Input.touchCount > 0 && touched)
            {
                Drag(Input.GetTouch(0).position);
            }
        }
        private void MouseInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !touched)
            {
                OnTouched(Input.mousePosition);
            }
            if(Input.GetKeyUp(KeyCode.Mouse0) && touched)
            {
                OnTouchEnd();
            }
            if(Input.GetKey(KeyCode.Mouse0) && touched)
            {
                Drag(Input.mousePosition);
            }
        }

        private void Drag(Vector2 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask))
            {
                if (hit.transform != null)
                {
                    prevPoint = hit.point;
                    OnDrag?.Invoke(prevPoint, hit.transform.gameObject);
                }
            }
        }
        private void OnTouched(Vector2 screenPosition)
        {
            touched = true;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask))
            {
                if(hit.transform != null)
                {
                    prevObject = hit.transform.gameObject;
                    OnTap?.Invoke(hit.point, prevObject);
                    prevTapTime = Time.time;
                }
            }
        }
        private void OnTouchEnd()
        {
            if (prevObject != null && prevTapTime + MIN_CLICK_DELAY > Time.time)
            {
                OnClick?.Invoke(prevPoint, prevObject);
            }
            OnTapEnded?.Invoke(prevPoint, prevObject);

            touched = false;
            prevObject = null;
        }

        private void Update()
        {
#if UNITY_EDITOR
            MouseInput();
#else
            PhoneInput();
#endif
        }
    }
}
