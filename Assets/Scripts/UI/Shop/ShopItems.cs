using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UI.Items;
using Data;


namespace UI
{
    public class ShopItems : MonoBehaviour
    {
        private List<ShopIcon> icons;

        public void Initialize(List<ItemBase> items, System.Action<int> onClick)
        {
            icons = GetComponentsInChildren<ShopIcon>(true).ToList();

            items = items.OrderBy(x => x.Index).ToList();

            for(int i = 0; i < items.Count && i < icons.Count; i++)
            {
                icons[i].Initialize(items[i].Index, items[i].Icon, onClick);
            }
        }
        public void Open(int index)
        {
            icons.Where(x => x.Index == index).First().Open();
        }
        public void TrySelect(int index)
        {
            icons.ForEach(x => x.Selected = false);

            ShopIcon icon = icons.Where(x => x.Index == index).FirstOrDefault();
            if(icon)
            {
                icon.Selected = true;
            }
        }
        public void OpeningAnimation(int index)
        {
            icons.Where(x => x.Index == index).First().OpeningAnimation();
        }
        public void ForceOpen(int index)
        {
            icons.Where(x => x.Index == index).First().Opened = true;
        }
    }
}
