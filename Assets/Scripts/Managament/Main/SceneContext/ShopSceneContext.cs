using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shop;

namespace Managament
{
    public class ShopSceneContext : SceneContext
    {
        [SerializeField] private ShopController shopController;

        protected override void SceneInitilize()
        {
            shopController.Initialize(new Services.GameInfo(this, Components, Color.white, baseMusic));

            shopController.OnExit += OnLevelCompleate;
        }


        public override void Visit(ISceneVisitor visitor)
        {
            visitor.Visited(this);
        }

        public class ShopData : SceneData
        {

        }
    }
}
