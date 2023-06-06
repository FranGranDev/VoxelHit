using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IFinalEvent
    {
        public void Execute(System.Action onDone);
    }
}
