using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Services;
using UI.Items;
using UnityEngine;

namespace UI
{
    public class NotflicationController : MonoBehaviour, Initializable<GameInfo>
    {
        [SerializeField] private List<Item> eventItems;


        public void Initialize(GameInfo info)
        {
            eventItems.ForEach(x => x.Initialize());
        }

        public void Show(string group, Notflication notflication, System.Action<Item> OnPlay)
        {
            Item item = eventItems.FirstOrDefault(x => x.Group == group);
            if (item == null)
                return;
            item.OnPlay = OnPlay;

            item.Show();
        }

        [System.Serializable]
        public class Item
        {
            [SerializeField] private string group;
            [SerializeField] private Transform menu;            


            private IEnumerable<UIPanel> panels;
            private ButtonUI playButton;
            private ButtonUI noThanksButton;


            public string Group => group;
            public Transform Menu => menu;

            public System.Action<Item> OnPlay;

            public void Show()
            {
                menu.gameObject.SetActive(true);

                foreach (UIPanel panel in panels)
                {
                    panel.IsShown = true;
                }
            }
            public void Hide()
            {
                foreach (UIPanel panel in panels)
                {
                    panel.IsShown = false;
                }
            }

            public void Initialize()
            {
                IEnumerable<ButtonUI> buttons = menu.GetComponentsInChildren<ButtonUI>(true);

                playButton = buttons.First(x => x.ID == "play");
                noThanksButton = buttons.First(x => x.ID == "no_thanks");

                playButton.OnClick += () => OnPlay?.Invoke(this);
                noThanksButton.OnClick += Hide;

                panels = menu.GetComponentsInChildren<UIPanel>(true);
                foreach(UIPanel panel in panels)
                {
                    panel.Initilize();
                }
            }
        }
    }
}
