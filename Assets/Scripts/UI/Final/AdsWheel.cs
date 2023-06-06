using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

namespace UI.Items
{
    public class AdsWheel : AdsWheelBase
    {
        [Header("Settings")]
        [SerializeField] private List<Multiplier> multipliers;
        [SerializeField] private float speed = 5f;
        [SerializeField] private float maxRotation = 75f;
        [Header("Components")]
        [SerializeField] private ButtonUI button;
        [SerializeField] private TextMeshProUGUI rewardValue;
        [SerializeField] private Transform ratator;
        [SerializeField] private Transform coinSpawnPoint;

        private float currant;
        private float money;


        private System.Action<float> onClick;



        public float Value
        {
            get
            {
                int min = 0;
                for (int i = 0; i < multipliers.Count; i++)
                {
                    multipliers[i].Active = false;
                    if (Mathf.Abs(multipliers[i].cos - currant) < Mathf.Abs(multipliers[min].cos - currant))
                    {
                        min = i;
                    }
                }

                multipliers[min].Active = true;
                return multipliers[min].value;
            }
        }
        public bool Active { get; private set; }
        public override Transform SpawnPoint => coinSpawnPoint;


        public override void Activate(int money, System.Action<float> onClick)
        {
            this.money = money;
            this.onClick = onClick;

            Active = true;
            button.OnClick = Click;

            Rotation();
        }
        public override void Stop()
        {
            Active = false;
        }

        private async void Rotation()
        {
            while (Active)
            {
                currant = Mathf.Cos(Time.time * speed);
                ratator.rotation = Quaternion.Euler(0, 0, currant * maxRotation);
                rewardValue.text = Mathf.RoundToInt(money * Value).ToString();

                await UniTask.WaitForFixedUpdate();
            }
        }

        public void Click()
        {
            onClick?.Invoke(Value);
        }


        [System.Serializable]
        private class Multiplier
        {
            public Color activeColor;

            public float cos;
            public int value;
            public TextMeshProUGUI text;

            public bool Active
            {
                set
                {
                    text.color = value ? activeColor : Color.white;
                    text.transform.localScale = value ? Vector3.one * 1.2f : Vector3.one;
                }
            }
        }
    }
}