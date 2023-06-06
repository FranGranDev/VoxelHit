using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Services;
using NaughtyAttributes;

namespace UI
{
    public class PaintedUI : MonoBehaviour, Initializable<GameInfo>
    {
        [Foldout("Menus"), SerializeField] private Transform mainMenu;

        private IEnumerable<UIPanel> menuPanels;
        private IMoneyController moneyController;
        private ISoundPlayer soundPlayer;
        private IAdsController adsController;


        public event System.Action OnDone;

        public void Initialize(GameInfo info)
        {
            adsController = info.Components.Ads;
            moneyController = info.Components.Money;
            soundPlayer = info.Components.SoundPlayer;

            mainMenu.gameObject.SetActive(true);

            menuPanels = mainMenu.GetComponentsInChildren<UIPanel>(true);
            menuPanels
                .ToList()
                .ForEach(x => x.Initilize());

            GetComponentsInChildren<Initializable<ISoundPlayer>>(true)
                .ToList()
                .ForEach(x => x.Initialize(soundPlayer));

            GetComponentsInChildren<Initializable<IAdsController>>(true)
                .ToList()
                .ForEach(x => x.Initialize(adsController));
        }


        public async void EarnMoney(int value)
        {
            foreach (UIPanel panel in menuPanels)
            {
                panel.IsShown = true;
            }

            IFinal finalUI = GetComponentInChildren<IFinal>();
            if (finalUI == null)
            {
                DoneClick();
                return;
            }

            IFinal.Result result = await finalUI.Execute(value);

            finalUI.StopWheel();

            if (result.WatchAds)
            {
                bool watched = await adsController.ShowRewarded();

                if (watched)
                {
                    int extraMoney = Mathf.RoundToInt(value * result.RewardRatio);
                    moneyController.Add(extraMoney);

                    await finalUI.ExtraMoney(value);

                    DoneClick();
                }
                else
                {
                    DoneClick();
                }
            }
            else
            {
                await adsController.TryShowInter();
                DoneClick();
            }
        }
        private void DoneClick()
        {
            OnDone?.Invoke();
        }
    }
}
