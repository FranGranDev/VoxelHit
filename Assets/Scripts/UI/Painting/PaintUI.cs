using System.Linq;
using System.Collections;
using Data;
using UI.Items;
using Animations;
using Services;
using UnityEngine;
using System.Collections.Generic;

namespace UI.Painting
{
    public class PaintUI : MonoBehaviour, Initializable<GameInfo>
    {
        [Header("Links")]
        [SerializeField] private SavedData savedData;
        [Space]
        [SerializeField] private UIPanel doneButton;
        [SerializeField] private BarBase progressBar;
        [Space]
        [SerializeField] private TabsMenu tabsMenu;
        [SerializeField] private ColorSelect baseColorSelect;
        [SerializeField] private ColorSelect vipColorSelect;
        [Space]
        [SerializeField] private ButtonUI exitButton;
        [SerializeField] private BuyButtonUI buyButton;
        [SerializeField] private AdsRatioButtonUI adsButton;
        [Header("States")]
        [SerializeField] private PaintData.Item.Types type;

        private Dictionary<string, PaintData.Item.Types> tabKeys = new Dictionary<string, PaintData.Item.Types>()
        {
            {"base", PaintData.Item.Types.Base },
            {"vip", PaintData.Item.Types.Vip },
        };
        private PaintData.Item currantColor
        {
            get;
            set;
        }
        private IMoneyController moneyController;
        private IAdsController adsController;
        private ISoundPlayer soundPlayer;
        private IHaptic haptic;


        public SavedData.ColorInfo ColorInfo
        {
            get
            {
                if (currantColor == null)
                    return null;

                return savedData.GetColorInfo(currantColor.Index);
            }
        }
        private event System.Action<UIMoneyValue> OnMoneyUpdated;
        public event System.Action OnDone;
        public event System.Action OnExit;

        public void Initialize(GameInfo info)
        {
            adsController = info.Components.Ads;
            moneyController = info.Components.Money;
            soundPlayer = info.Components.SoundPlayer;
            haptic = info.Components.Haptic;

            GetComponentsInChildren<IUiBehavior>()
                .ToList()
                .ForEach(x => x.Initilize());

            GetComponentsInChildren<IChangeable<UIMoneyValue>>()
                .ToList()
                .ForEach(x => x.Bind(ref OnMoneyUpdated));

            GetComponentsInChildren<Initializable<ISoundPlayer>>(true)
                .ToList()
                .ForEach(x => x.Initialize(soundPlayer));

            GetComponentsInChildren<Initializable<IAdsController>>(true)
                .ToList()
                .ForEach(x => x.Initialize(adsController));


            moneyController.OnVisualUpdated += UpdateMoney;
            tabsMenu.OnSelect += SwitchTab;

            
            baseColorSelect.OnSelect += OnColorSelected;
            vipColorSelect.OnSelect += OnColorSelected;

            exitButton.OnClick += ExitButtonClick;
            adsButton.OnClick += AdsButtonClick;
            buyButton.OnClick += BuyButtonClick;
        }


        private void UpdateMoney(UIMoneyValue obj)
        {
            OnMoneyUpdated?.Invoke(obj);
        }

        private void BuyButtonClick()
        {
            SavedData.ColorInfo info = ColorInfo;

            if (info.Opened)
                return;

            moneyController.TryBuy(
                () =>
                {
                    info.SetOpened();

                    switch(type)
                    {
                        case PaintData.Item.Types.Vip:
                            vipColorSelect.OpenItem(info.Index);
                            break;
                    }

                    soundPlayer.PlaySound("buy");
                },
                () =>
                {

                },
                info.ColorItem.Cost.Cost);



            UpdateButtons(info);
        }
        private async void AdsButtonClick()
        {
            SavedData.ColorInfo info = ColorInfo;

            if (info.Opened)
                return;

            if (await adsController.ShowRewarded())
            {
                info.ColorItem.Cost.AdsWatched++;

                if (info.ColorItem.Cost.RemainingAds <= 0)
                {
                    info.SetOpened();

                    switch (type)
                    {
                        case PaintData.Item.Types.Vip:
                            vipColorSelect.OpenItem(info.Index);
                            break;
                    }
                }

                soundPlayer.PlaySound("win");
                UpdateButtons(info);
            }
        }
        private void ExitButtonClick()
        {
            OnExit?.Invoke();
        }

        private void OnColorSelected(ColorItem color)
        {
            if (color == null)
                return;
            currantColor = color.Data;


            UpdateButtons(ColorInfo);

            haptic.VibrateHaptic();
        }
        private void UpdateButtons(SavedData.ColorInfo colorInfo)
        {

            if(colorInfo == null || colorInfo.Opened)
            {
                adsButton.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(false);
                return;
            }

            adsButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(true);

            adsButton.UpdateValue(colorInfo.ColorItem.Cost.RemainingAds);
            buyButton.UpdateCost(colorInfo.ColorItem.Cost.Cost, moneyController);
        }
        public void SetBar(IFillEvent fillEvent)
        {
            progressBar.Bind(fillEvent);
        }
        private void SwitchTab(TabsMenu.Tab obj)
        {
            type = tabKeys[obj.Id];

            baseColorSelect.SetActive(type == PaintData.Item.Types.Base);
            vipColorSelect.SetActive(type == PaintData.Item.Types.Vip);
        }
        public void ShowButton()
        {
            doneButton.IsShown = true;
        }
        public void Done()
        {
            OnDone?.Invoke();
        }

    }
}
