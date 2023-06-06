using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;
using UI;
using Cysharp.Threading.Tasks;

namespace Shop
{
    public class ShopController : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField] private int maxCount;
        [SerializeField] private float startTime;
        [SerializeField] private float endTime;
        [Header("Data")]
        [SerializeField] private SavedData savedData;
        [Header("Shops")]
        [SerializeField] private CannonShop cannonShop;
        [SerializeField] private BulletShop bulletShop;
        [Header("Links")]
        [SerializeField] private ShopUI shopUI;

        private Dictionary<ShopItemsTypes, IShopAnimation> shopsDictionary;
        private ShopItemsTypes currantShopItems;
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
                        return gemsController;
                    default:
                        return coinController;
                }
            }
        }


        private IMoneyController coinController;
        private IMoneyController gemsController;
        protected IAdsController adsController;
        private ISoundPlayer soundPlayer;
        private IHaptic haptic;


        private bool isOpening;

        private ShopItemsTypes CurrantItem
        {
            get => currantShopItems;
            set
            {
                if (currantShopItems == value)
                    return;

                RemoveModel();

                currantShopItems = value;

                CreateModel();
            }
        }
        private ShopTypes CurrantShop
        {
            get => currantShop;
            set
            {
                if (currantShop == value)
                    return;

                currantShop = value;

                UpdateButtons();
            }
        }
        private int CurrantIndex
        {
            get
            {
                return savedData.GetCurrantItemIndex(currantShopItems);
            }
            set
            {
                if (CurrantIndex == value)
                    return;

                RemoveModel();

                savedData.SetCurrantItemIndex(currantShopItems, value);

                CreateModel();
            }
        }
        private GameObject CurrantPrefab
        {
            get
            {
                switch(currantShopItems)
                {
                    case ShopItemsTypes.Bullets:
                        return savedData.GetBulletInfo(CurrantIndex).BulletItem.Prefab;
                    case ShopItemsTypes.Cannons:
                        return savedData.GetCannonInfo(CurrantIndex).CannonItem.Prefab;
                    default:
                        return null;
                }
            }
        }
        private CostInfo CurrantCost
        {
            get
            {
                return savedData.GetCurrantItemCost(currantShop, currantShopItems);
            }
        }


        public event System.Action OnExit;


        public void Initialize(GameInfo info)
        {
            adsController = info.Components.Ads;
            coinController = info.Components.Money;
            gemsController = info.Components.Gems;
            soundPlayer = info.Components.SoundPlayer;
            haptic = info.Components.Haptic;

            shopsDictionary = new Dictionary<ShopItemsTypes, IShopAnimation>()
            {
                {ShopItemsTypes.Cannons, cannonShop },
                {ShopItemsTypes.Bullets, bulletShop },
            };
            shopUI.Initialize(info);

            SetupItems();

            shopUI.OnExit += Exit;
            shopUI.OnBuyClick += BuyClick;
            shopUI.OnAdsClick += AdsClick;
            shopUI.OnTabSelected += TabSelected;
            shopUI.OnItemSelected += SelectItem;
            shopUI.OnShopSelected += ShopSelected;

            CreateModel();

            if(IsPremiumAvaliable())
            {
                shopUI.SelectShop(ShopTypes.Premium);
            }
            else
            {
                shopUI.SelectShop(ShopTypes.Base);
            }
        }



        private void SetupItems()
        {
            foreach (ShopTypes shopTypes in System.Enum.GetValues(typeof(ShopTypes)))
            {
                foreach (ShopItemsTypes shopItemTypes in shopsDictionary.Keys)
                {
                    List<ItemBase> items = new List<ItemBase>();
                    IEnumerable<IShopItem> savedInfo = savedData.GetShopItems(shopTypes, shopItemTypes);

                    foreach (IShopItem info in savedInfo)
                    {
                        items.Add(info.ItemUI);
                    }
                    shopUI.SetupItems(shopTypes, shopItemTypes, items);
                    foreach (IShopItem info in savedInfo)
                    {
                        if (info.Opened)
                        {
                            shopUI.ForceOpen(shopTypes, shopItemTypes, info.Index);
                        }
                        items.Add(info.ItemUI);
                    }
                }
            }
        }

        private void SelectItem(int value)
        {
            CurrantIndex = value;
        }
        private void ShopSelected(ShopTypes shopType)
        {
            if (isOpening)
                return;
            CurrantShop = shopType;
        }
        private void TabSelected(ShopItemsTypes itemsType)
        {
            if (isOpening)
                return;
            CurrantItem = itemsType;
        }
        private void BuyClick(ShopTypes shopType)
        {
            if (!IsAnyItemAvaliable() || isOpening)
                return;
                MoneyController.TryBuy(
                () =>
                {
                    OpenRandomItem();

                    soundPlayer.PlaySound("buy");
                },
                () =>
                {
                    Debug.Log("Not enought money");
                },
                CurrantCost.Cost);
        }
        private async void AdsClick(ShopTypes shopType)
        {
            if (!IsAnyItemAvaliable() || isOpening || !CurrantCost.OpenByAds)
                return;

            if (await adsController.ShowRewarded())
            {
                CurrantCost.AdsWatched++;

                if (CurrantCost.RemainingAds == 0)
                {
                    OpenRandomItem();
                }
                else
                {
                    UpdateButtons();
                }

                soundPlayer.PlaySound("win");
            }
        }
        private void Exit()
        {
            soundPlayer.PlaySound("click");

            OnExit?.Invoke();
        }


        private void CreateModel()
        {
            shopsDictionary[currantShopItems].Create(CurrantPrefab);

            UpdateButtons();

            shopUI.SelectItem(currantShopItems, CurrantIndex);
        }
        private void RemoveModel()
        {
            shopsDictionary[CurrantItem].Remove();
        }
        private void UpdateButtons()
        {
            shopUI.UpdateButtons(currantShop, CurrantCost, isOpening);
        }
        private bool IsAnyItemAvaliable()
        {
            return savedData.GetShopItems(currantShop, currantShopItems).Count(x => !x.Opened) > 0;
        }
        private bool IsPremiumAvaliable()
        {
            CostInfo info = savedData.CannonCost(ShopTypes.Premium);
            if (info != null)
            {
                if (gemsController.Money >= info.Cost)
                {
                    return true;
                }
            }

            info = savedData.BulletsCost(ShopTypes.Premium);
            if (info != null)
            {
                if (gemsController.Money >= info.Cost)
                {
                    return true;
                }
            }

            return false;
        }
        private void OpenRandomItem()
        {
            if (isOpening)
                return;
            isOpening = true;
            UpdateButtons();

            OpenRandom();
        }

        private async void OpenRandom()
        {
            List<IShopItem> closed = savedData
                .GetShopItems(currantShop, currantShopItems)
                .Where(x => !x.Opened)
                .ToList();

            if (closed.Count == 0)
                return;

            IShopItem target = closed[Random.Range(0, closed.Count)];
            if (closed.Count > 1)
            {
                for (int i = 0; i < maxCount - closed.Count; i++)
                {
                    closed.Add(closed[Random.Range(0, closed.Count)]);
                }

                System.Random random = new System.Random(Time.frameCount);
                random.Shuffle(closed);

                float time = startTime;
                int count = 0;
                foreach (IShopItem item in closed)
                {
                    time = Mathf.Lerp(startTime, endTime, (float)count / (float)closed.Count);

                    shopUI.PlayOpeningAnimation(currantShop, currantShopItems, item.Index);
                    soundPlayer.PlaySound("trap_show", 0.5f);
                    haptic.VibrateHaptic();

                    await UniTask.Delay(time);
                    count++;
                }
            }

            await UniTask.Delay(endTime * 0.25f);

            target.SetOpened();
            shopUI.Open(currantShop, currantShopItems, target.Index);

            soundPlayer.PlaySound("item_opened");
            haptic.VibrateHaptic();

            CurrantIndex = target.Index;

            isOpening = false;
            UpdateButtons();
        }
    }
}
