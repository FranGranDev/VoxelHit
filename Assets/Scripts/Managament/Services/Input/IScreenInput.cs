using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IScreenInput
    {
        public void SetMask(LayerMask layerMask);

        public event System.Action<Vector3, GameObject> OnTap;
        public event System.Action<Vector3, GameObject> OnTapEnded;
        public event System.Action<Vector3, GameObject> OnClick;
        public event System.Action<Vector3, GameObject> OnDrag;
    }
}
