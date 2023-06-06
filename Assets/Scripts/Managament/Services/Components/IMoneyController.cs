using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IMoneyController
    {
        public event System.Action<UIMoneyValue> OnVisualUpdated;
        public event System.Action<MoneyValue> OnUpdated;

        public int Money { get; }
        public void Add(int value);
        public void TryBuy(System.Action onSuccsess, System.Action onFail, int cost);

        public void CallVisualUpdate();
    }
}
