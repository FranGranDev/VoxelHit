using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using Services;

namespace Data
{
    [CreateAssetMenu(fileName = "Cannon Data", menuName = "Game Data/Cannon Data")]
    public class CannonsData : ScriptableObject, IEnumerable<CannonsData.Item>
    {
        [SerializeField] private List<Item> items;
        [Space]
        [SerializeField] private List<CostInfo> baseCosts;
        [SerializeField] private List<CostInfo> premiumCosts;

        public CostInfo GetCost(ShopTypes shopType, int index)
        {
            try
            {
                switch (shopType)
                {
                    case ShopTypes.Base:
                        return baseCosts[index];
                    case ShopTypes.Premium:
                        return premiumCosts[index];
                    default:
                        return baseCosts[index];
                }
            }
            catch
            {
                return null;
            }
        }

        public Item GetCannon(int index)
        {
            try
            {
                return items.Where(x => x.Index == index).First();;
            }
            catch
            {
                return null;
            }
        }
        public IEnumerator<Item> GetEnumerator()
        {
            return ((IEnumerable<Item>)items).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }



        [System.Serializable]
        public class Item : ItemBase
        {
            [SerializeField] private string name;
            [SerializeField] private GameObject prefab;
            [SerializeField] private int index;
            [SerializeField] private ShopTypes type;
            [Space]
            [SerializeField] private Sprite icon;

            public int Index => index;
            public GameObject Prefab => prefab;
            public Sprite Icon => icon;
            public ShopTypes ShopType => type;
        }


        #region Internal

        private void OnValidate()
        {
            int baseCount = items.Count(x => x.ShopType == ShopTypes.Base);

            if (baseCosts.Count > baseCount)
            {
                baseCosts.RemoveAt(baseCount - 1);
            }
            else if (baseCosts.Count < baseCount)
            {
                baseCosts.Add(new CostInfo());
            }

            foreach (CostInfo cost in baseCosts)
            {
                cost.Initialize($"cannons_base_{baseCosts.IndexOf(cost)}");
            }

            int premiumCount = items.Count(x => x.ShopType == ShopTypes.Premium);

            if (premiumCosts.Count > premiumCount)
            {
                premiumCosts.RemoveAt(premiumCount - 1);
            }
            else if (premiumCosts.Count < premiumCount)
            {
                premiumCosts.Add(new CostInfo());
            }

            foreach (CostInfo cost in premiumCosts)
            {
                cost.Initialize($"cannons_premium_{premiumCosts.IndexOf(cost)}");
            }
        }


        #endregion
    }
}
