using System.Linq;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Services;
using UI.Items;
using UnityEngine.UI;


namespace UI.Painting
{
    public class ColorSelect : MonoBehaviour, Initializable<GameInfo>
    {
        [SerializeField] private PaintData paintData;
        [SerializeField] private SavedData savedData;
        [SerializeField] private PaintData.Item.Types type;
        [SerializeField] private float minVelocity;
        [Space]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform main;
        [Space]
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        [Header("Items")]
        [SerializeField] private List<ColorItem> baseItems = new List<ColorItem>();

        private bool holding;
        private ColorItem currant;
        private ISoundPlayer soundPlayer;
        private float Center
        {
            get
            {
                return main.rect.width / 2;
            }
        }

        public ColorItem Currant
        {
            get => currant;
            set
            {
                if (currant == value)
                    return;
                currant = value;
                OnSelect?.Invoke(value);

                soundPlayer.PlaySound("trap_show", 0.5f);
            }
        }
        public event System.Action<ColorItem> OnSelect;



        public void Initialize(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;

            foreach (PaintData.Item item in paintData.GetColors(type))
            {
                ColorItem colorItem = Instantiate(paintData.GetPrefab(item), content).GetComponent<ColorItem>();

                colorItem.Initilize(item, SelectItem);
                colorItem.Opened = savedData.GetColorInfo(item.Index).Opened;
                baseItems.Add(colorItem);
            }

            layoutGroup.padding.right = Mathf.RoundToInt(Center * 2);
            layoutGroup.padding.left = Mathf.RoundToInt(Center * 2);


            ColorItem first = baseItems.FirstOrDefault();
            if (first)
            {
                content.anchoredPosition = new Vector2(-first.Rect.anchoredPosition.x + Center, content.anchoredPosition.y);
            }
        }


        public void OpenItem(int index)
        {
            baseItems.FirstOrDefault(x => x.Data.Index == index).Opened = true;
        }

        public void SetActive(bool value)
        {
            if(value)
            {
                OnSelect?.Invoke(Currant);
            }
        }
        public void OnScroll(Vector2 vector)
        {
            SelectNear();
        }
        public void OnTap()
        {
            holding = true;
        }
        public void OnTapEnded()
        {
            holding = false;
        }


        private void SelectNear()
        {
            baseItems.ForEach(x => x.Selected = false);
            Currant = baseItems.OrderBy(x => Mathf.Abs(content.anchoredPosition.x + x.Rect.anchoredPosition.x - Center)).FirstOrDefault();
            if (Currant)
            {
                Currant.Selected = true;
            }

            if (!holding && scrollRect.velocity.magnitude < minVelocity)
            {
                Vector2 position = new Vector2(-Currant.Rect.anchoredPosition.x + Center, content.anchoredPosition.y);
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, position, 0.1f);
                scrollRect.velocity *= 0.9f;
            }
        }
        private void SelectItem(ColorItem item)
        {
            Vector2 direction = new Vector2(Center - (content.anchoredPosition.x + item.Rect.anchoredPosition.x), 0);

            scrollRect.velocity += direction * 7;
        }


        [System.Serializable]
        private class ColorTab
        {
            [SerializeField] private RectTransform tab;
            [SerializeField] private Image image;
            [SerializeField] private Button button;

            public float selectedScale = 1.1f;

            public Color Color => image.color;


            public void Initilize(Color color, System.Action<ColorTab> onClick)
            {
                image.color = color;

                button.onClick.AddListener(() => onClick?.Invoke(this));
            }

            public bool Selected
            {
                set
                {
                    image.transform.localScale = Vector3.one * (value ? selectedScale : 1f);
                }
            }
        }


        private void Update()
        {

        }

    }
}
