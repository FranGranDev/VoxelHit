using TMPro;
using UnityEngine;

namespace UI.Items
{
    public class AdsRatioButtonUI : AdsButtonUI
    {
        [SerializeField] private TextMeshProUGUI ratio;

        public void UpdateValue(int value)
        {
            ratio.text = $"x{value}";
        }
    }
}
