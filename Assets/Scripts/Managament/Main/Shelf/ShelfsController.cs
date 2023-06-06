using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Services;
using DG.Tweening;
using UI;
using Data;


namespace Managament.Shelfs
{
    [RequireComponent(typeof(IScreenInput))]
    public class ShelfsController : MonoBehaviour, ITutor<ShelfTutorValue>
    {
        private const float SIZE = 200f;

        [Header("Switch Animation")]
        [SerializeField] private float switchTime;
        [SerializeField] private Ease swichEase;
        [Header("Links")]
        [SerializeField] private ShelfUI shelfUI;
        [SerializeField] private VoxelModelsData data;
        [SerializeField] private SavedData savedData;
        [Header("Components")]
        [SerializeField] private Transform place;
        [SerializeField] private new Transform camera;
        [Header("Prefabs")]
        [SerializeField] private ShelfCollection collectionPrefab;
        [SerializeField] private ShelfCollection singleCollectionPrefab;
        [Header("Info")]
        [SerializeField] private List<VoxelModelsData.Group> groups;


        private ISoundPlayer soundPlayer;
        private IHaptic haptic;

        private bool switching;
        private int currantIndex;
        private int maxIndex;

        private int GroupRange(int index)
        {
            int remaind = Groups.Count - index * ShelfCollection.ShelfCount;

            return remaind < ShelfCollection.ShelfCount ? remaind : ShelfCollection.ShelfCount;
        }
        private int GetCollectionIndex(int modelId)
        {
            VoxelModelsData.Group group = data.GetGroups().First(x => x.Id == data.GetItem(modelId).Group);
            int groupIndex = groups.IndexOf(group);

            int result = Mathf.CeilToInt((float)(groupIndex + 1) / ShelfCollection.ShelfCount) - 1;


            Debug.Log(group.Name + "  " + groupIndex + "  " + result);


            return result;
        }
        private float OpenedProcent(Shelf items)
        {
            int all = 5;
            int needed = items.Count(x => !x.IsNull && x.Info.Opened);

            return (float)needed / (float)all;
        }
        public Dictionary<string, List<ModelId>> ModelDictionary
        {
            get;
            private set;
        }
        public List<VoxelModelsData.Group> Groups
        {
            get => groups;
            private set => groups = value;
        }
        public ShelfCollection Currant
        {
            get;
            private set;
        }
        public int CurrantIndex
        {
            get => currantIndex;
            set
            {
                currantIndex = value;
                if(currantIndex < 0)
                {
                    currantIndex = maxIndex;
                }
                else if(currantIndex > maxIndex)
                {
                    currantIndex = 0;
                }

                SavedData.LastShelfIndex = currantIndex;
            }
        }

        public event System.Action<Shelf.Item> OnSelectItem;
        public event System.Action OnExit;


        public void Initialize(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;
            haptic = info.Components.Haptic;


            IScreenInput clickInput = GetComponent<IScreenInput>();
            clickInput.OnClick += TryPaintModel;

            ModelDictionary = data.GetDictionary();
            Groups = data.GetGroups();

            shelfUI.OnExit += Exit;
            shelfUI.OnRight += SwitchRight;
            shelfUI.OnLeft += SwitchLeft;


            shelfUI.Initialize(info);
        }

        public void CreateAllCollection()
        {
            maxIndex = Mathf.CeilToInt((float)Groups.Count / 3f) - 1;

            CreateCollection(CurrantIndex);

            shelfUI.State = ShelfUI.States.Watch;
        }
        public void CreateLastOpenedModelCollection(int modelId)
        {
            maxIndex = Mathf.CeilToInt((float)Groups.Count / 3f) - 1;

            CurrantIndex = GetCollectionIndex(modelId);

            CreateCollection(CurrantIndex, false);

            shelfUI.State = ShelfUI.States.Watch;
        }
        public void CreateAllCollection(int index)
        {
            maxIndex = Mathf.CeilToInt((float)Groups.Count / 3f) - 1;

            CurrantIndex = index;
            CreateCollection(CurrantIndex);

            shelfUI.State = ShelfUI.States.Watch;
        }
        public void CreateSingleCollection(int targetIndex)
        {
            Currant = Instantiate(singleCollectionPrefab, place);

            ModelId targetItem = data.GetItem(targetIndex);
            List<VoxelModelsData.Group> groups = new List<VoxelModelsData.Group>() { Groups.First(x => x.Id == targetItem.Group) };
            Currant.Initialize(ModelDictionary, groups, 0);


            shelfUI.State = ShelfUI.States.None;
            Currant.Where(x => x.Group.Id == targetItem.Group)
                .First()
                .Where(x => x.Index == targetIndex)
                .First()
                .PlayOpened((x) =>
                {
                    OnItemOpened(x);
                });

            this.Delayed(0.1f, () =>
            {
                soundPlayer.PlaySound("model_opened");
                haptic.VibrateHaptic();
            });
        }
        public void PlayPaintedAnimation(int targetIndex)
        {
            ModelId targetItem = data.GetItem(targetIndex);

            Currant.Where(x => x.Group.Id == targetItem.Group)
                .First()
                .Where(x => x.Index == targetIndex)
                .First()
                .PlayPainted();

            this.Delayed(0.1f, () =>
            {
                soundPlayer.PlaySound("model_opened");
                haptic.VibrateHaptic();
            });
        }


        private void OnItemOpened(Shelf.Item item)
        {
            if(savedData.IsModelGroupOpened(item.Group))
            {
                StartCoroutine(ShelfCollectedDoneCour(item));
            }
            else
            {
                item.StartShine();
                shelfUI.PlayNext();
            }

            shelfUI.PlayOpening(GetComponentInChildren<IUserInput>(), OpenedProcent(Currant.First()));
        }
        private IEnumerator ShelfCollectedDoneCour(Shelf.Item last)
        {
            Shelf shelf = Currant.First();
            List<Shelf.Item> items = shelf.ToList();

            shelf.PlayShine();
            yield return new WaitForSeconds(0.25f);

            Currant.ConfettiBoom();

            soundPlayer.PlaySound("collection_done");

            foreach (Shelf.Item item in items)
            {
                item.PlayAllCollected();
                haptic.VibrateHaptic();
                yield return new WaitForSeconds(0.25f);
            }

            yield return new WaitForSeconds(0.5f);

            int groupIndex = Groups.IndexOf(Groups.First(x => x.Id == last.Group));

            if(groupIndex < Groups.Count - 1)
            {
                SwitchNext(Groups[groupIndex + 1].Id);

                shelfUI.PlayNext();
            }
            else
            {
                shelfUI.ShowNextButton();
            }
            yield break;
        }
        private void SwitchNext(string group)
        {
            ShelfCollection prev = Currant;

            Currant = Instantiate(singleCollectionPrefab, place);
            List<VoxelModelsData.Group> groups = new List<VoxelModelsData.Group>() { Groups.First(x => x.Id == group) };

            Currant.Initialize(ModelDictionary, groups, 0);

            SwitchAnimation(true, Currant, prev);
        }

        private ShelfCollection CreateCollection(int index, bool async = false)
        {
            Currant = Instantiate(collectionPrefab, place);

            List<VoxelModelsData.Group> groups = Groups.GetRange(index * ShelfCollection.ShelfCount, GroupRange(index));

            Currant.Initialize(ModelDictionary, groups, index, async);

            return Currant;
        }

        private void TryPaintModel(Vector3 point, GameObject obj)
        {
            if(obj.TryGetComponent(out ShelfID model))
            {
                if (model.Item == null)
                    return;
                if (model.Item.Info.Opened)
                {
                    OnSelectItem?.Invoke(model.Item);

                    soundPlayer.PlaySound("click");
                }
                else
                {
                    //item.PlayLockAniamtion(...);
                }
            }
        }
        private void Exit()
        {
            soundPlayer.PlaySound("click");

            OnExit?.Invoke();
        }
        private void SwitchLeft()
        {
            if (switching)
                return;
            switching = true;

            CurrantIndex--;

            Switch(false);
        }
        private void SwitchRight()
        {
            if (switching)
                return;
            switching = true;

            CurrantIndex++;

            Switch(true);
        }
        private void Switch(bool right)
        {
            ShelfCollection prev = Currant;

            CreateCollection(CurrantIndex, true);

            SwitchAnimation(right, Currant, prev);
        }
        private void SwitchAnimation(bool right, ShelfCollection next, ShelfCollection prev)
        {
            float positionX = (right ? 1 : -1) * SIZE;

            next.transform.localPosition = new Vector3(positionX, 0, 0);
            prev.transform.localPosition = new Vector3(-positionX, 0, 0);

            Vector3 startPosition = camera.position;
            startPosition.x = -positionX;
            camera.transform.position = startPosition;
            camera.DOLocalMoveX(next.transform.position.x, switchTime)
                .SetEase(swichEase)
                .SetUpdate(UpdateType.Fixed)
                .OnComplete(() =>
                {
                    Destroy(prev.gameObject);
                    switching = false;
                });

            //prev.transform.DOLocalMoveX(-positionX, switchTime)
            //    .SetEase(swichEase)
            //    .SetUpdate(UpdateType.Fixed)
            //    .OnComplete(() => Destroy(prev.gameObject));

            //next.transform.DOLocalMoveX(0, switchTime)
            //    .SetEase(swichEase)
            //    .SetUpdate(UpdateType.Fixed)
            //    .OnComplete(() => switching = false);
        }


        #region Tutor
        ShelfTutorValue ITutor<ShelfTutorValue>.Start(ShelfTutorValue parameter)
        {
            if (shelfUI.State != ShelfUI.States.Watch)
                return new ShelfTutorValue();
            Shelf first = Currant.First();

            return new ShelfTutorValue(first.Items[1].Model.transform.position);
        }
        #endregion
    }
}
