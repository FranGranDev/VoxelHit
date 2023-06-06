using UnityEngine;


namespace Services
{
    public interface IShopAnimation
    {
        public ShopItemsTypes ShopType { get; }
        public void Create(GameObject prefab);
        public void Remove();
    }
}
