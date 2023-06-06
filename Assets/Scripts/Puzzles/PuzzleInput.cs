using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using DG.Tweening;
using Voxel;

namespace Puzzles
{
    [RequireComponent(typeof(IScreenInput))]
    public class PuzzleInput : MonoBehaviour, IBindable<IGameEventsHandler>
    {
        [SerializeField] private LayerMask voxelMask;
        [SerializeField] private LayerMask groundMask;


        public PuzzleItem CurrantItem { get; private set; }
        private GameStates gameState;
        private IScreenInput input;

        public event System.Action<PuzzleItem> OnTake;
        public event System.Action<PuzzleItem> OnDrop;
        public event System.Action<PuzzleItem> OnDrag;
        public event System.Action<PuzzleItem> OnClick;

        public event System.Action<PuzzlePlace> OnSelect;
        public event System.Action<PuzzleItem> OnPlaced;

        private Vector3 currantItemOffset;
        private Vector3 currantItemPoint;

        public void Initialize()
        {
            input = GetComponent<IScreenInput>();

            input.OnTap += TakeItem;
            input.OnTapEnded += DropItem;
            input.OnClick += InteractItem;
            input.OnDrag += DragItem;

            input.SetMask(voxelMask);
        }


        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnStateChanged += OnGameStateChanged;
        }
        private void OnGameStateChanged(GameStates obj)
        {
            gameState = obj;
        }

        private void InteractItem(Vector3 point, GameObject obj)
        {
            if (gameState != GameStates.Game)
                return;
            PuzzleItem item = obj.GetComponentInParent<PuzzleItem>();
            if (item && obj.TryGetComponent(out IVoxel voxel))
            {
                item.Interact(voxel);
            }

            PuzzlePlace place = obj.GetComponentInParent<PuzzlePlace>();
            if(place)
            {
                OnSelect?.Invoke(place);
            }
        }
        private void TakeItem(Vector3 point, GameObject obj)
        {
            if (gameState != GameStates.Game)
                return;
            PuzzleItem item = obj.GetComponentInParent<PuzzleItem>();
            if (item)
            {
                if (item.Placed)
                    return;
                CurrantItem = item;
                currantItemPoint = point;
                currantItemOffset = CurrantItem.transform.position - currantItemPoint;
                CurrantItem.Take();
                OnTake?.Invoke(CurrantItem);

                input.SetMask(groundMask);
            }
        }
        private void DragItem(Vector3 point, GameObject obj)
        {
            if (!CurrantItem)
                return;

            currantItemPoint = point;
            OnDrag?.Invoke(CurrantItem);
        }
        private void DropItem(Vector3 point, GameObject obj)
        {
            if (gameState != GameStates.Game)
                return;
            if (!CurrantItem)
                return;

            CurrantItem.Throw(currantItemPoint + currantItemOffset);
            OnDrop?.Invoke(CurrantItem);

            CurrantItem = null;
            input.SetMask(voxelMask);
        }

        public void OnItemPlaced()
        {
            if(CurrantItem == null)
            {
                input.SetMask(voxelMask);
                return;
            }
            OnPlaced?.Invoke(CurrantItem);

            CurrantItem = null;
            input.SetMask(voxelMask);
        }


        private void FixedUpdate()
        {
            if(CurrantItem)
            {
                Vector3 position = currantItemPoint + currantItemOffset + Vector3.up * 10;
                Quaternion rotation = Quaternion.Euler(90, 0, 0);

                CurrantItem.transform.position = Vector3.Lerp(CurrantItem.transform.position, position, 0.5f);
                CurrantItem.transform.rotation = Quaternion.Lerp(CurrantItem.transform.rotation, rotation, 0.1f);
            }
        }
    }
}
