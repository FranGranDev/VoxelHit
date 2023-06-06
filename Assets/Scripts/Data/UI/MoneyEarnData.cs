using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animations;
using DG.Tweening;


namespace Data
{
    [CreateAssetMenu(fileName = "UI Money", menuName = "UI Data/Money")]
    public class MoneyEarnData : ScriptableObject
    {
        [Header("Animations")]
        [SerializeField, Min(0)] private float spawnTime;
        [SerializeField, Min(0)] private float showTime;
        [SerializeField, Min(0)] private float flyTime;
        [SerializeField] private Ease spawnEase;
        [SerializeField] private Ease flyEase;
        [Header("Coins")]
        [SerializeField, Min(1)] private int moneyPerIcon;
        [SerializeField] private GameObject moneyIcon;
        [Header("Gems")]
        [SerializeField, Min(1)] private int gemPerIcon;
        [SerializeField] private GameObject gemIcon;


        public float SpawnTime => spawnTime;
        public float ShowTime => showTime;
        public float FlyTime => flyTime;
        public Ease SpawnEase => spawnEase;
        public Ease FlyEase => flyEase;

        public int GetMoneyPerIcon(CoinTypes coin)
        {
            switch(coin)
            {
                case CoinTypes.Coin:
                    return moneyPerIcon;
                case CoinTypes.Gem:
                    return gemPerIcon;
                default:
                    return 1;
            }
        }
        public GameObject GetCoinIcon(CoinTypes coin)
        {
            switch (coin)
            {
                case CoinTypes.Coin:
                    return moneyIcon;
                case CoinTypes.Gem:
                    return gemIcon;
                default:
                    return moneyIcon;
            }
        }
        public enum CoinTypes { Coin, Gem}
    }
}
