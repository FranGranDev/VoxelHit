using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IUserInput
    {
        public event System.Action OnTap;
        public event System.Action OnHold;
        public event System.Action OnTapEnded;
    }
}
