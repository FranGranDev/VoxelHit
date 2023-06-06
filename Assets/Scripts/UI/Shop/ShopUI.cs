using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Services;
using UI.Items;

namespace UI
{
    public class ShopUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Group baseGroup;
        [SerializeField] private Group premiumGroup;
        [Header("Money")]
        [SerializeField] private ChangableMoney coins;
        [SerializeField] private ChangableMoney gems;
        [Header("Buttons")]
        [SerializeField] private SwipeController swipeController;
        [SerializeField] private ButtonUI exitButton;
        [SerializeField] private ButtonUI nextTab;
        [SerializeField] private ButtonUI prevTab;


        private bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;
                baseGroup.tabMenu.Disabled = value;
                premiumGroup.tabMenu.Disabled = value;
            }
        }
        private bool disabled;
        private IAdsController adsController;
        private IMoneyController coinController;
        private IMoneyController gemController;
        private ISoundPlayer soundPlayer;
        private IHaptic haptic;

        private Dictionary<string, ShopItemsTypes> tabsValues = new Dictionary<string, ShopItemsTypes>()
        {
            {"cannons", ShopItemsTypes.Cannons },
            {"bullets", ShopItemsTypes.Bullets },
        };

        private Dictionary<ShopTypes, Group> shopValues;

        private string currantTabName;
        private ShopTypes currantShop;
        private IMoneyController MoneyController
        {
            get
            {
                switch(currantShop)
                {
                    case ShopTypes.Base:
                        return coinController;
                    case ShopTypes.Premium:
                        return gemController;
                    default:
                        return coinController;
                }
            }
        }

        private Group CurrantShop
        {
            get => shopValues[currantShop];
        }


        private event System.Action<UIMoneyValue> OnCoinsUpdated;
        private event System.Action<UIMoneyValue> OnGemsUpdated;

        public event System.Action<ShopItemsTypes> OnTabSelected;
        public event System.Action<ShopTypes> OnShopSelected;
        public event System.Action<int> OnItemSelected;
        public event System.Action<ShopTypes> OnAdsClick;
        public event System.Action<ShopTypes> OnBuyClick;
        public event System.Action OnExit;


        public void Initialize(GameInfo info)
        {
            adsController = info.Components.Ads;
            coinController = info.Components.Money;
            gemController = info.Components.Gems;
            soundPlayer = info.Components.SoundPlayer;
            haptic = info.Components.Haptic;

            shopValues = new Dictionary<ShopTypes, Group>()
            {
                {ShopTypes.Base, baseGroup },
                {ShopTypes.Premium, premiumGroup },
            };

            GetComponentsInChildren<Initializable<ISoundPlayer>>(true)
                .ToList()
                .ForEach(x => x.Initialize(soundPlayer));

            GetComponentsInChildren<Initializable<IAdsController>>(true)
                .ToList()
                .ForEach(x => x.Initialize(adsController));


            exitButton.OnClick += ExitClick;
            swipeController.OnSwipe += (x) => SwitchShop();
            nextTab.OnClick += SwitchShop;
            prevTab.OnClick += SwitchShop;

            baseGroup.tabMenu.OnSelect += SelectTab;
            baseGroup.buyButton.OnClick += () => BuyItem(ShopTypes.Base);
            baseGroup.adsButton.OnClick += () => BuyItemAds(ShopTypes.Base);

            premiumGroup.tabMenu.OnSelect += SelectTab;
            premiumGroup.buyButton.OnClick += () => BuyItem(ShopTypes.Premium);
            premiumGroup.adsButton.OnClick += () => BuyItemAds(ShopTypes.Premium);



            gemController.OnVisualUpdated += (x) => OnGemsUpdated?.Invoke(x);
            coinController.OnVisualUpdated += (x) => OnCoinsUpdated?.Invoke(x);

            coins.Bind(ref OnCoinsUpdated);
            gems.Bind(ref OnGemsUpdated);

            coinController.CallVisualUpdate();
            gemController.CallVisualUpdate();
        }



        public void SetupItems(ShopTypes shopType, ShopItemsTypes itemsType, List<ItemBase> items)
        {
            switch(itemsType)
            {
                case ShopItemsTypes.Bullets:
                    shopValues[shopType].bulletsItems.Initialize(items, CallOnItemSelected);
                    break;
                case ShopItemsTypes.Cannons:
                    shopValues[shopType].cannonItems.Initialize(items, CallOnItemSelected);
                    break;
            }
        }
        public void ForceOpen(ShopTypes shopType, ShopItemsTypes itemsType, int index)
        {
            switch (itemsType)
            {
                case ShopItemsTypes.Bullets:
                    shopValues[shopType].bulletsItems.ForceOpen(index);
                    break;
                case ShopItemsTypes.Cannons:
                    shopValues[shopType].cannonItems.ForceOpen(index);
                    break;
            };
        }
        public void Open(ShopTypes shopType, ShopItemsTypes itemsType, int index)
        {
            switch (itemsType)
            {
                case ShopItemsTypes.Bullets:
                    shopValues[shopType].bulletsItems.Open(index);
                    break;
                case ShopItemsTypes.Cannons:
                    shopValues[shopType].cannonItems.Open(index);
                    break;
            };
        }
        public void SelectItem(ShopItemsTypes itemsType, int index)
        {
            switch (itemsType)
            {
                case ShopItemsTypes.Bullets:
                    baseGroup.bulletsItems.TrySelect(index);
                    premiumGroup.bulletsItems.TrySelect(index);
                    break;
                case ShopItemsTypes.Cannons:
                    baseGroup.cannonItems.TrySelect(index);
                    premiumGroup.cannonItems.TrySelect(index);
                    break;
            };
        }
        public void PlayOpeningAnimation(ShopTypes shopType, ShopItemsTypes itemsType, int index)
        {
            switch (itemsType)
            {
                case ShopItemsTypes.Bullets:
                    shopValues[shopType].bulletsItems.OpeningAnimation(index);
                    break;
                case ShopItemsTypes.Cannons:
                    shopValues[shopType].cannonItems.OpeningAnimation(index);
                    break;
            };
        }
        public void UpdateButtons(ShopTypes shopType, CostInfo info, bool disabled)
        {
            this.Disabled = disabled;

            currantShop = shopType;

            if(info == null || disabled)
            {
                CurrantShop.adsButton.gameObject.SetActive(false);
                CurrantShop.buyButton.gameObject.SetActive(false);
                return;
            }

            CurrantShop.adsButton.gameObject.SetActive(info.OpenByAds);
            CurrantShop.buyButton.gameObject.SetActive(true);

            CurrantShop.adsButton.UpdateValue(info.RemainingAds);
            CurrantShop.buyButton.UpdateCost(info.Cost, MoneyController);
        }
        public void ExitClick()
        {
            if (Disabled)
                return;
            OnExit?.Invoke();
        }


        private void SelectTab(TabsMenu.Tab value)
        {
            currantTabName = value.Id;

            OnShopSelected?.Invoke(currantShop);
            OnTabSelected?.Invoke(tabsValues[value.Id]);
        }
        private void SwitchShop()
        {
            if (Disabled)
                return;
            CurrantShop.Hide();
            if (CurrantShop.shopType == ShopTypes.Base)
            {
                currantShop = ShopTypes.Premium;
            }
            else
            {
                currantShop = ShopTypes.Base;
            }

            CurrantShop.Show();

            OnShopSelected?.Invoke(currantShop);
            CurrantShop.tabMenu.Select(currantTabName);
        }
        public void SelectShop(ShopTypes shopType)
        {
            if (Disabled)
                return;
            if (CurrantShop.shopType == shopType)
                return;
            CurrantShop.Hide();
            currantShop = shopType;

            CurrantShop.Show();

            OnShopSelected?.Invoke(currantShop);
            CurrantShop.tabMenu.Select(currantTabName);
        }
        private void CallOnItemSelected(int index)
        {
            OnItemSelected?.Invoke(index);
        }
        private void BuyItemAds(ShopTypes shopType)
        {
            OnAdsClick?.Invoke(shopType);            
        }
        private void BuyItem(ShopTypes shopType)
        {
            OnBuyClick?.Invoke(shopType);
        }


        [System.Serializable]
        public class Group
        {
            public ShopTypes shopType;
            public GameObject gameObject;
            [Space]
            public TabsMenu tabMenu;
            [Space]
            public ShopItems cannonItems;
            public ShopItems bulletsItems;
            [Space]
            public BuyButtonUI buyButton;
            public AdsRatioButtonUI adsButton;

            public void Hide()
            {
                gameObject.SetActive(false);
            }
            public void Show()
            {
                gameObject.SetActive(true);
            }
        }
    }
}
