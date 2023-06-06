using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class AdsWheelBase : MonoBehaviour
    {
        public abstract Transform SpawnPoint { get; }

        public abstract void Activate(int money, System.Action<float> onClick);
        public abstract void Stop();
    }
}
