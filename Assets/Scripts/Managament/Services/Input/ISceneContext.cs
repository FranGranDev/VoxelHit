using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface ISceneContext
    {
        public GameTypes GameType { get; }
        public Transform LevelTransform { get; }
    }
}
