using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Services;
using UnityEngine.UI;

namespace UI.Items
{
    public class AdsButtonUI : ButtonUI, Initializable<IAdsController>
    {
        [SerializeField] private Button targetButton;
        [SerializeField] private ItemTint tint;

        private IAdsController adsController;

        public void Initialize(IAdsController source)
        {
            adsController = source;

            adsController.OnRewardedReadyChanged += OnAdsStateChanged;

            OnAdsStateChanged(adsController.RewardedReady);
        }

        public void OnDestroy()
        {
            try
            {
                adsController.OnRewardedReadyChanged -= OnAdsStateChanged;
            }
            catch { }
        }


        private void OnAdsStateChanged(bool active)
        {
            targetButton.interactable = active;
            if (tint)
            {
                tint.Tinted = !active;
            }
        }
    }
}
