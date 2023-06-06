using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxel.Data;
using Voxel;
using TMPro;
using DG.Tweening;
using Data;


namespace Managament.Shelfs
{
    public class Shelf : MonoBehaviour, IEnumerable<Shelf.Item>
    {
        [SerializeField] private List<Item> items;
        [SerializeField] private GameObject model;
        [SerializeField] private TextMeshPro groupName;
        [SerializeField] private PaintData paintData;
        [Space]
        [SerializeField] private Transform radial;

        public List<Item> Items => items;
        public bool IsNull { get; private set; }
        public VoxelModelsData.Group Group { get; private set; }
        public IEnumerator<Item> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void Initialize(List<ModelId> itemsData, VoxelModelsData.Group group, bool async = false)
        {
            IsNull = false;
            Group = group;

            groupName.text = group.Name.ToUpper();

            if(async)
            {
                StartCoroutine(CreateItemsCour(itemsData, group));
            }
            else
            {
                CreateItems(itemsData, group);
            }
        }
        private void CreateItems(List<ModelId> itemsData, VoxelModelsData.Group group)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (i < itemsData.Count)
                {
                    items[i].Initilize(itemsData[i], paintData);
                }
                else
                {
                    items[i].Hide();
                }
            }
        }
        private IEnumerator CreateItemsCour(List<ModelId> itemsData, VoxelModelsData.Group group)
        {
            yield return new WaitForSeconds(0.1f);


            for (int i = 0; i < items.Count; i++)
            {
                yield return new WaitForSeconds(0.15f);

                if (i < itemsData.Count)
                {
                    items[i].Initilize(itemsData[i], paintData);
                    items[i].SpawnAnimation();
                }
                else
                {
                    items[i].Hide();
                }
            }

            yield break;
        }
        public void Hide()
        {
            IsNull = true;
            model.SetActive(false);

            foreach(Item item in items)
            {
                item.Hide();
            }
        }

        public void PlayShine()
        {
            radial.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.InOutSine);
        }


        private void OnDestroy()
        {
            Items.ForEach(x => x.OnDestroy());
        }


        [System.Serializable]
        public class Item
        {
            [SerializeField] private string Name;
            [SerializeField] private ShelfID place;

            private Tween tween;

            public int Index
            {
                get;
                private set;
            }
            public string Group
            {
                get;
                private set;
            }
            public bool IsNull
            {
                get;
                private set;
            }
            public SavedData.VoxelModelInfo Info
            {
                get;
                private set;
            }

            public VoxelObject Model
            {
                get;
                private set;
            }
            public void Initilize(ModelId modelId, PaintData paintData)
            {
                IsNull = false;
                Index = modelId.Index;
                Group = modelId.Group;

                Info = new SavedData.VoxelModelInfo(modelId, paintData);

                GameObject model = Instantiate(modelId.GetModel, place.ModelParent);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;

                place.Initialize(this);

                Model = model.GetComponent<VoxelObject>();
                Model.InitializeShelf(new SavedData.VoxelModelInfo(modelId, paintData));
            }
            public void SpawnAnimation()
            {
                Model.transform.localScale = Vector3.zero;
                tween = Model.transform.DOScale(Vector3.one, 0.2f)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(UpdateType.Fixed);
            }
            public void Hide()
            {
                IsNull = true;
            }


            public void PlayOpened(System.Action<Item> onDone)
            {
                place.PlayOpened(onDone);
            }
            public void PlayPainted()
            {
                place.PlayPainted();
            }
            public void PlayAllCollected()
            {
                place.PlayAllCollected();
            }
            public void StartShine()
            {
                place.StartShine();
            }
            public void StopShine()
            {
                place.StopShine();
            }


            public void OnDestroy()
            {
                if(tween.IsActive())
                {
                    tween.Kill();
                }
            }
        }
    }
}
