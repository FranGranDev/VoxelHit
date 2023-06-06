using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class ShelfTutorValue
    {
        public ShelfTutorValue(Vector3 itemPosition)
        {
            ItemPosition = itemPosition;
            Activated = true;
        }
        public ShelfTutorValue()
        {
            Activated = false;
        }

        public bool Activated { get; }
        public Vector3 ItemPosition { get; }
    }

    public interface ITutor<T>
    {
        public T Start(T parameter);
    }
} 
