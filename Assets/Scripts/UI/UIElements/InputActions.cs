using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Items
{
    public class InputActions : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        public UnityEvent OnClick;
        public UnityEvent OnTap;
        public UnityEvent OnTapEnd;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTap?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnTapEnd?.Invoke();
        }
    }
}
