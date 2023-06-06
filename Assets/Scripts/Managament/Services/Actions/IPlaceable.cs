using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IPlaceable<T>
    {
        public Transform Place { get; }
        public T TryGetObject();
    }
}
