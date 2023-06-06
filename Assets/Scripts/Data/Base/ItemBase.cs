using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public interface ItemBase
    {
        public int Index { get; }
        public Sprite Icon { get; }
    }

    public interface IShopItem
    {
        int Index { get; }
        bool Opened { get; }
        void SetOpened();

        ItemBase ItemUI { get; }
    }
}
