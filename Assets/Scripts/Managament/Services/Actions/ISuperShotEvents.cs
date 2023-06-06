using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface ISuperShotEvents
    {
        public event System.Action<bool> OnReadyChanged;
    }
}
