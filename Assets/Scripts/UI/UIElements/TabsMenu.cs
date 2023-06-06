using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Items
{
    public class TabsMenu : MonoBehaviour
    {
        [SerializeField] private string type;
        [SerializeField] private int defualtTabIndex;
        [SerializeField] private List<Tab> tabs;
        [SerializeField] private Image background;


        public string Name => type;
        public bool Disabled { get; set; }

        public event System.Action<Tab> OnSelect;


        private void Awake()
        {
            foreach(Tab tab in tabs)
            {
                tab.Initilize(Select);
            }

            Select(tabs[defualtTabIndex]);
        }

        private void Select(Tab selected)
        {
            if (Disabled)
                return;

            foreach(Tab tab in tabs)
            {
                tab.Selected = false;
            }

            selected.Selected = true;
            background.color = selected.Color;

            OnSelect?.Invoke(selected);
        }
        public void Select(string name)
        {
            Tab target = tabs.Find(x => x.Id == name);
            if (target == null)
            {
                Select(tabs[0]);
                return;
            }

            Select(target);
        }



        [System.Serializable]
        public class Tab
        {
            [SerializeField] private string Name = "New Tab";
            [Space]
            [SerializeField] private RectTransform tab;
            [SerializeField] private Image colorSource;
            [SerializeField] private Button button;
            [Space]
            [SerializeField] private GameObject content;
            [Space]
            [SerializeField] private float selectedScale = 1.1f;

            public string Id => Name;
            public Color Color => colorSource.color;
            public bool Selected
            {
                set
                {
                    if (value)
                    {
                        tab.localScale = Vector3.one * selectedScale;
                        tab.SetAsLastSibling();
                    }
                    else
                    {
                        tab.localScale = Vector3.one;
                        tab.SetAsFirstSibling();
                    }

                    if (content)
                    {
                        content.SetActive(value);
                    }
                }
            }


            public void Initilize(System.Action<Tab> onClick)
            {
                button.onClick.AddListener(() => onClick?.Invoke(this));
            }
        }
    }
}
