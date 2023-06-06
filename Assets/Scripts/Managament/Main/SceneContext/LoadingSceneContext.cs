using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managament
{
    public class LoadingSceneContext : SceneContext
    {
        protected override void SceneInitilize()
        {

        }

        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }
    }
}
