using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IDragInput
    {
        public event System.Action<Vector2> OnDrag;
    }
}
