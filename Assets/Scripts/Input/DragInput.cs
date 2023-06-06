using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Services;
using System;

namespace UserInput
{
    public class DragInput : MonoBehaviour, IDragInput, IDragHandler
    {
        public event Action<Vector2> OnDrag;

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            OnDrag?.Invoke(eventData.delta);
        }
    }
}
