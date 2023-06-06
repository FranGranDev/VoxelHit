using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


namespace UI.Items
{
    public class GemAdsWheel : AdsWheelBase
    {
        [Header("Settings")]
        [SerializeField] private List<Multiplier> multipliers;
        [SerializeField] private float speed = 5f;
        [Header("Components")]
        [SerializeField] private ButtonUI button;
        [SerializeField] private TextMeshProUGUI rewardValue;
        [SerializeField] private RectTransform line;
        [SerializeField] private RectTransform container;
        [SerializeField] private Transform coinSpawnPoint;

        private float currant;
        private float money;


        private System.Action<float> onClick;



        public float Value
        {
            get
            {
                Multiplier currant = multipliers
                    .OrderBy(x => (x.point.position - line.position).magnitude)
                    .First();

                foreach(Multiplier multi in multipliers)
                {
                    multi.Active = false;
                }
                currant.Active = true;

                return currant.value;
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

            Moving();
        }
        public override void Stop()
        {
            Active = false;
        }

        private async void Moving()
        {
            while (Active)
            {
                currant = Mathf.Cos(Time.time * speed) * 0.5f + 0.5f;
                float x = Mathf.Lerp(0, container.rect.width, currant);
                line.anchoredPosition = new Vector2(x, line.anchoredPosition.y);
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

            public Transform point;
            public TextMeshProUGUI text;

            public float value;

            public bool Active
            {
                set
                {
                    text.color = value ? activeColor : Color.white;
                    text.transform.localScale = value ? Vector3.one * 1.25f : Vector3.one;
                }
            }
        }
    }
}