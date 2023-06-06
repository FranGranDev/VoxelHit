using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Services;
using System;

namespace UserInput
{
    public class TouchInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IUserInput
    {
        private bool holding;

        public event Action OnTap;
        public event Action OnHold;
        public event Action OnTapEnded;

        public void OnPointerDown(PointerEventData eventData)
        {
            holding = true;

            OnTap?.Invoke();
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            holding = false;

            OnTapEnded?.Invoke();
        }
        private void Update()
        {
            if(holding)
            {
                OnHold?.Invoke();
            }
        }
    }
}
