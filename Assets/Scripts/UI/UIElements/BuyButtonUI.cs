using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using TMPro;
using UnityEngine.UI;

namespace UI.Items
{
    public class BuyButtonUI : ButtonUI
    {
        [SerializeField] private TextMeshProUGUI value;
        [SerializeField] private ItemTint tint;

        public void UpdateCost(int cost, IMoneyController money)
        {
            value.text = cost.ToString();

            if(money != null)
            {
                bool enabled = money.Money >= cost;
                tint.Tinted = !enabled;

                if(TryGetComponent(out Button button))
                {
                    button.interactable = enabled;
                }
            }
        }
    }
}
