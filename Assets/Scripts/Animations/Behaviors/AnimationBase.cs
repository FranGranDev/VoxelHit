using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{
    public abstract class AnimationBase : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected AnimationData animationData;

        public abstract void Play();
    }
}
