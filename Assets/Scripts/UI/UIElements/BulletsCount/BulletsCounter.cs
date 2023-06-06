using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;

namespace UI.Items
{
    public class BulletsCounter : MonoBehaviour, IBindable<IStock>
    {
        [Header("Components")]
        [SerializeField] private Transform container;
        [Header("Prefabs")]
        [SerializeField] private BulletIndicator prefab;


        private List<BulletIndicator> bullets = new List<BulletIndicator>();


        public void Bind(IStock stock)
        {
            foreach (BulletIndicator bullet in bullets)
            {
                Destroy(bullet.gameObject);
            }
            bullets.Clear();

            for (int i = 0; i < stock.MaxCount; i++)
            {
                bullets.Add(Instantiate(prefab, container).Initialize(true));
            }
            for (int i = 0; i < stock.Count; i++)
            {
                bullets[i].Hidden = false;
            }

            stock.OnChanged += OnChanged;
        }

        private void OnChanged(IStock stock)
        {
            for(int i = 0; i < stock.MaxCount; i++)
            {
                bullets[i].Hidden = i >= stock.Count;
            }
        }
    }
}
