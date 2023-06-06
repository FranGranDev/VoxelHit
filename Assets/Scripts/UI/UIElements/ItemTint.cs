using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace UI.Items
{
    public class ItemTint : MonoBehaviour
    {
        [SerializeField] private Color tintColor;
        [SerializeField, Range(0, 1f)] private float power = 0.5f;

        [SerializeField] private List<GraphicItem> items;

        private bool initialized;

        public bool Tinted
        {
            set
            {
                if (value)
                {
                    MakeTint();
                }
                else
                {
                    MakeNormal();
                }

            }
        }


        public void Initialize()
        {
            if (initialized)
                return;
            items = new List<GraphicItem>();

            IEnumerable<Graphic> graphics = GetComponentsInChildren<Graphic>(true);

            foreach (Graphic graphic in graphics)
            {
                items.Add(new GraphicItem(graphic));
            }
            initialized = true;
        }
        [Button("Initialize")]
        public void ForceInitialize()
        {
            items = new List<GraphicItem>();

            IEnumerable<Graphic> graphics = GetComponentsInChildren<Graphic>(true);

            foreach (Graphic graphic in graphics)
            {
                items.Add(new GraphicItem(graphic));
            }
        }

        [Button("Make Normal")]
        private void MakeNormal()
        {
            Initialize();
            foreach (GraphicItem item in items)
            {
                item.CurrantColor = item.BaseColor;
            }
        }
        [Button("Make Gray")]
        private void MakeTint()
        {
            Initialize();
            foreach (GraphicItem item in items)
            {
                item.CurrantColor = Color.Lerp(item.BaseColor, tintColor, power);
            }
        }

        [System.Serializable]
        private class GraphicItem
        {
            public GraphicItem(Graphic icon)
            {
                Icon = icon;
                BaseColor = new Color(icon.color.r, icon.color.g, icon.color.b, 1f);
            }

            public Graphic Icon { get; }
            public Color BaseColor { get; }

            public Color CurrantColor
            {
                get => Icon.color;
                set => Icon.color = value;
            }
        }
    }
}
