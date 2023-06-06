using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IFinalEvent
    {
        public UniTask Execute();
    }
}
