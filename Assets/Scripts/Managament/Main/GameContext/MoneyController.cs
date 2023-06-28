using Services;
using UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Managament
{
    public class MoneyController : IMoneyController
    {
        private const string MONEY_KEY = "money";

        private Dictionary<Types, string> typeKeys = new Dictionary<Types, string>()
        {
            {Types.Coin, "Coin" },
            {Types.Gem, "Gem" },
        };

        public MoneyController(Types type)
        {
            this.type = type;

            UpdateState();
        }

        private Types type;
        private int tempMoney;

        public int Money
        {
            get
            {
                return PlayerPrefs.GetInt(typeKeys[type] + MONEY_KEY, 0);
            }
            private set
            {
                PlayerPrefs.SetInt(typeKeys[type] + MONEY_KEY, value);
            }
        }

        public event System.Action<UIMoneyValue> OnVisualUpdated;
        public event System.Action<MoneyValue> OnUpdated;


        public void ClearBinds()
        {
            OnUpdated = null;
            OnVisualUpdated = null;
        }
        public void Add(int value)
        {
            Money += value;
            tempMoney = value;
        }

        public void CallVisualUpdate()
        {
            OnVisualUpdated?.Invoke(new UIMoneyValue(Money - tempMoney));
        }
        public void TryBuy(System.Action onSuccsess, System.Action onFail, int cost)
        {
            if(Money >= cost)
            {
                Money -= cost;
                tempMoney = -cost;

                onSuccsess?.Invoke();
            }
            else
            {
                onFail?.Invoke();
            }
        }
        private async void UpdateState()
        {
            while(true)
            {
                while (tempMoney > 0)
                {
                    tempMoney--;
                    tempMoney = Mathf.CeilToInt(tempMoney * 0.9f);
                    CallVisualUpdate();
                    await UniTask.WaitForFixedUpdate();
                }

                while (tempMoney < 0)
                {
                    tempMoney++;
                    tempMoney = Mathf.CeilToInt(tempMoney * 0.9f);
                    CallVisualUpdate();
                    await UniTask.WaitForFixedUpdate();
                }

                tempMoney = 0;

                await UniTask.WaitForFixedUpdate();
            }
        }


        public enum Types { Coin, Gem}
    }
}
