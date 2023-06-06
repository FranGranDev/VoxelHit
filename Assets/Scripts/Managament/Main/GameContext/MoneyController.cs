using Services;
using UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

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

        public MoneyController(MonoBehaviour monoBehaviour, Types type)
        {
            this.type = type;

            monoBehaviour.StartCoroutine(UpdateCour());
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
        private IEnumerator UpdateCour()
        {
            var wait = new WaitForFixedUpdate();

            while(true)
            {
                while (tempMoney > 0)
                {
                    tempMoney--;
                    tempMoney = Mathf.CeilToInt(tempMoney * 0.9f);
                    CallVisualUpdate();
                    yield return wait;
                }

                while (tempMoney < 0)
                {
                    tempMoney++;
                    tempMoney = Mathf.CeilToInt(tempMoney * 0.9f);
                    CallVisualUpdate();
                    yield return wait;
                }

                tempMoney = 0;

                yield return wait;
            }
        }


        public enum Types { Coin, Gem}
    }
}
