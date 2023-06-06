using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using DG.Tweening;
using UI.Items;
using Cysharp.Threading.Tasks;
using Data;


namespace UI
{
    public class UIFinal : MonoBehaviour, IFinal, Initializable<GameInfo>
    {
        [Header("Links")]
        [SerializeField] private MoneyEarnData earnData;
        [Header("Settings")]
        [SerializeField] private MoneyEarnData.CoinTypes coinType;
        [Header("Components")]
        [SerializeField] private EarnedMoney earnedMoney;
        [SerializeField] private AdsWheelBase adsWheel;
        [Header("Buttons")]
        [SerializeField] private ButtonUI noThanksButton;

        private ISoundPlayer soundPlayer;

        private UniTaskCompletionSource<IFinal.Result> finalResultSource;
        private bool active;


        public void Initialize(GameInfo gameInfo)
        {
            soundPlayer = gameInfo.Components.SoundPlayer;

            noThanksButton.OnClick += ClickNoThanks;
        }

        public UniTask<IFinal.Result> Execute(int money)
        {
            active = true;
            finalResultSource = new UniTaskCompletionSource<IFinal.Result>();

            earnedMoney.Fill(money);
            adsWheel.Activate(money, ClickWheelAds);

            return finalResultSource.Task;
        }
        public async UniTask ExtraMoney(int money, float ratio)
        {
            soundPlayer.PlaySound("coins_get");

            int moneyPerIcon = earnData.GetMoneyPerIcon(coinType);
            int count = Mathf.FloorToInt(money / moneyPerIcon * ratio);
            int remain = Mathf.Max(money - count * moneyPerIcon, 0);


            earnedMoney.StopScale();

            for (int i = 0; i < count; i++)
            {
                Vector3 position = adsWheel.SpawnPoint.position + new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);
                GameObject icon = Instantiate(earnData.GetCoinIcon(coinType), position, Quaternion.identity, transform);
                icon.transform.localScale = Vector3.zero;

                icon.transform.DOScale(Vector3.one, earnData.ShowTime)
                    .SetEase(earnData.SpawnEase);

                icon.transform.DOMove(earnedMoney.CoinPosition, earnData.FlyTime)
                    .SetDelay(earnData.ShowTime * Random.Range(1, 1.25f))
                    .SetEase(earnData.FlyEase)
                    .OnKill(() =>
                    {
                        earnedMoney.AddReward(moneyPerIcon);
                        Destroy(icon);
                    });

                await UniTask.WaitForFixedUpdate();
            }
            await UniTask.Delay(Mathf.RoundToInt((earnData.FlyTime + earnData.SpawnTime / 2) * 1000));

            earnedMoney.AddReward(remain);

            await UniTask.Delay(250);
        }

        public void StopWheel()
        {
            adsWheel.Stop();
        }


        public void ClickNoThanks()
        {
            if (!active)
                return;

            active = false;

            finalResultSource.TrySetResult(new IFinal.Result(false));
        }
        public void ClickWheelAds(float value)
        {
            if (!active)
                return;
            active = false;

            finalResultSource.TrySetResult(new IFinal.Result(true, value));
        }
    }
}
