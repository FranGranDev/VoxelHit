using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Paint Data", menuName = "Game Data/Paint Data")]
    public class PaintData : ScriptableObject
    {
        [SerializeField, Min(1)] private int paintReward;
        [Space]
        [SerializeField] private List<Item> items;
        [Space]
        [SerializeField] private GameObject baseColorPrefab;
        [Space]
        [SerializeField] private Material lockedMaterial;


        public int PaintReward => paintReward;

        public IEnumerable<Item> BaseColors
        {
            get
            {
                return items.Where(x => x.Type == Item.Types.Base);
            }
        }
        public IEnumerable<Item> VipColors
        {
            get
            {
                return items.Where(x => x.Type == Item.Types.Vip);
            }
        }
        public Item GetColor(int index)
        {
            return items.Where(x => !x.Null && x.Index == index).First();
        }
        public IEnumerable<Item> GetColors(Item.Types type)
        {
            switch(type)
            {
                case Item.Types.Base:
                    return BaseColors;
                case Item.Types.Vip:
                    return VipColors;
                default:
                    return null;
            }
        }

        public Item GetRandomBaseColor()
        {
            List<Item> rand = items.Where(x => x.Type == Item.Types.Base).ToList();

            return rand[Random.Range(0, rand.Count)];
        }

        public GameObject GetPrefab(Item item)
        {
            if (item.IconPrefab == null)
                return baseColorPrefab;
            return item.IconPrefab;
        }
        public Material LocketMaterial => lockedMaterial;


        private void OnValidate()
        {
            foreach(Item item in items)
            {
                item.OnValidate();
            }
        }


        [System.Serializable]
        public class Item : ItemBase
        {
            [SerializeField] private string name;
            [SerializeField] private Types type;
            [Space]
            [SerializeField] private int index;
            [SerializeField] private Color color;
            [SerializeField] private Sprite sprite;
            [SerializeField] private GameObject iconPrefab;
            [Space]
            [SerializeField] private Material material;
            [Space]
            [SerializeField] private CostInfo cost;
            

            public int Index => index;
            public bool Null => type == Types.Null;
            public Types Type => type;
            public Color Color => color;
            public Sprite Icon => sprite;
            public GameObject IconPrefab => iconPrefab;
            public Material Material => material;
            public CostInfo Cost => cost;

            public void OnValidate()
            {
                if (material == null)
                    return;
                cost.Initialize($"paint_color_{index}");

                name = material.name;

                if(sprite == null && color == new Color(1, 1, 1, 1))
                {
                    color = material.color;
                }
            }

            public enum Types { Null, Base, Vip}
        }
    }
}
