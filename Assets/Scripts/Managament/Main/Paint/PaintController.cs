using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;
using Voxel.Paint;
using Voxel;
using UI.Painting;
using System;


namespace Managament
{
    [RequireComponent(typeof(IScreenInput))]
    public class PaintController : MonoBehaviour, ITargetEventHandler, Initializable<GameInfo>, IBindable<IGameEventsHandler>, IFillEvent
    {
        [Header("Links")]
        [SerializeField] private PaintUI paintUI;
        [Header("Components")]
        [SerializeField] private Material baseMaterial;
        [Header("States")]
        [SerializeField] private GameStates gameState;
        [SerializeField] private bool touched;
        [Space]
        [SerializeField] private bool painted;

        private VoxelObject voxelObject;
        private Dictionary<int, PaintData.Item> paintedGroups;

        public float Fill
        {
            get
            {
                return (float)paintedGroups.Count / (float)voxelObject.GroupCount;
            }
        }
        public Dictionary<int, PaintData.Item> PaintedGroups
        {
            get => paintedGroups;
        }

        public event Action<float> OnFilled;
        public event Action OnExit;
        public event Action OnDone;
        public event Action OnFailed;
        public event Action<MoneyValue> OnMoney;


        public void Initialize(GameInfo info)
        {
            IScreenInput input = GetComponent<IScreenInput>();
            input.OnTap += OnTap;

            paintUI.OnDone += Done;
            paintUI.OnExit += Exit;
            paintUI.SetBar(this);

            paintedGroups = new Dictionary<int, PaintData.Item>();
            voxelObject = info.SceneContext.LevelTransform.GetComponentInChildren<VoxelObject>();
        }


        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnStateChanged += OnStateChanged;
        }
        private void OnStateChanged(GameStates gameState)
        {
            this.gameState = gameState;
        }
        public void Restart(Action onDone)
        {
            onDone?.Invoke();
        }


        private void OnTap(Vector3 point, GameObject obj)
        {
            if (gameState != GameStates.Game)
                return;

            if (obj.TryGetComponent(out IVoxel voxel))
            {
                SavedData.ColorInfo info = paintUI.ColorInfo;
                if (!info.Opened)
                {
                    //Play can't paint;
                    return;
                }
                PaintData.Item paintData = info.ColorItem;

                voxelObject.Paint(new PaintInfo(voxel.Position, paintData.Material, voxel.ColorIndex, PaintInfo.Types.WaveGroup));
                if (!paintedGroups.ContainsKey(voxel.ColorIndex))
                {
                    paintedGroups.Add(voxel.ColorIndex, paintData);
                }
                else
                {
                    paintedGroups[voxel.ColorIndex] = paintData;
                }

                if (paintedGroups.Count >= voxelObject.GroupCount)
                {
                    OnAllPainted();
                }
                OnFilled?.Invoke(Fill);
            }
        }
        private void OnAllPainted()
        {
            if (gameState == GameStates.Game && !painted)
            {               
                paintUI.ShowButton();
                painted = true;
            }
        }
        private void Done()
        {
            OnDone?.Invoke();
        }

        private void Exit()
        {
            OnExit?.Invoke();
        }
    }
}
