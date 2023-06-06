using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IFillEvent : IFillable
    {
        public event System.Action<float> OnFilled;
    }
    public interface IFillable
    {
        public float Fill { get; }
    }

}
