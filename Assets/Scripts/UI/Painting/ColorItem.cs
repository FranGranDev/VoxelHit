using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Painting
{
    public class ColorItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [Space]
        [SerializeField] private Image background;
        [SerializeField] private Color unselectedColor;
        [SerializeField] private Color selectedColor;
        [Space]
        [SerializeField] private GameObject locked;
        [Space]
        [SerializeField] private float selectedScale = 1.25f;

        private PaintData.Item data;
        private System.Action<ColorItem> onClick;


        public PaintData.Item Data => data;
        public RectTransform Rect => transform as RectTransform;
        

        public void Initilize(PaintData.Item item, System.Action<ColorItem> select)
        {
            data = item;
            onClick = select;

            image.color = data.Color;
            if (data.Icon != null)
            {
                image.sprite = data.Icon;
            }

            GetComponentInChildren<Button>().onClick.AddListener(Click);
        }

        public bool Selected
        {
            set
            {
                transform.localScale = Vector3.one * (value ? selectedScale : 1f);
                background.color = value ? selectedColor : unselectedColor;
            }
        }
        public bool Opened
        {
            set
            {
                locked.gameObject.SetActive(!value);
            }
        }

        public void Click()
        {
            onClick?.Invoke(this);
        }
    }
}
