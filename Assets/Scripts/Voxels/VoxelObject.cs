using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxel.Waves;
using Voxel.Paint;
using Data;
using Animations;
using Services;
using Cannons.Bullets;
using System.Linq;
using NaughtyAttributes;
using Voxel.Data;


namespace Voxel
{
    public class VoxelObject : MonoBehaviour, ITargetEventHandler, IBindable<IGameEventsHandler>, Initializable<GameInfo>, IFillEvent
    {
        #region Inspector
        [Foldout("Repair Settings"), SerializeField] private float speed;
        [Foldout("Repair Settings"), SerializeField] private Vector3 repairCenter;
        [Foldout("Repair Settings"), SerializeField] private bool outerFill;
        [Foldout("Repair Settings"), SerializeField] private AnimationData repairAnimation;
        [Foldout("Repair Settings"), SerializeField, Range(0, 1f)] private float repairDelay;
        [Foldout("Repair Settings"), SerializeField, Range(0, 5f)] private float repairRandomize;
        

        [Foldout("Destroy Settings"), SerializeField] private float impulse;
        [Foldout("Destroy Settings"), SerializeField, Range(0, 1f)] private float upRatio;
        [Foldout("Destroy Settings"), SerializeField, Range(0, 1f)] private float randomize;

        [Foldout("Break Settings"), SerializeField] private float breakImpulse;

        [Foldout("Bullet Destroy Settings"), SerializeField] private float topBulletHeight;
        [Foldout("Bullet Destroy Settings"), SerializeField] private float bulletHitRandomize;
        [Foldout("Bullet Destroy Settings"), SerializeField] private AnimationCurve bulletFlyCurve;


        [Foldout("Restart Settings"), SerializeField] private AnimationData restartAnimation;
        [Foldout("Restart Settings"), SerializeField] private RestartParams.ActionTypes restartType;
        [Foldout("Restart Settings"), SerializeField,
         ShowIf(nameof(restartType), RestartParams.ActionTypes.Jump)] private float restartJumpPower;
        [Foldout("Restart Settings"), SerializeField, Range(0, 2),
        ShowIf(nameof(restartType), RestartParams.ActionTypes.Jump)]  private float restartJumpRandomize;


        [Foldout("Effects"), SerializeField] private ParticleSystem breakParticle;


        [Foldout("Listeners"), SerializeField] private List<HitTrigger> repairTriggers;
        [Foldout("Listeners"), SerializeField] private List<HitTrigger> demolishTriggers;
        [Foldout("Listeners"), SerializeField] private List<BreakTrigger> breakTriggers;


        [Foldout("Links"), SerializeField] private WaveMaker waveMaker;
        [Foldout("Links"), SerializeField] private ColorSetter colorSetter;
        [Foldout("Links"), SerializeField] private PaintData paintData;


        [Foldout("Data"), SerializeField] private int groupCount;

        [Foldout("States"), SerializeField] private GameStates state;
        [Foldout("States"), SerializeField] private GameTypes gameType;


        [Foldout("Test"), SerializeField] private int testRepairCount;
        #endregion


        private List<IGameVoxel> allVoxels;
        private Dictionary<Vector3, IGameVoxel> voxelDict;
        private ISoundPlayer soundPlayer;
        private IHaptic haptic;


        private int height;
        private int width;
        private Vector3 minBounds;
        private Vector3 maxBounds;

        public float Fill
        {
            get
            {
                switch(gameType)
                {
                    case GameTypes.Game:
                        return (float)allVoxels.Count(x => x.State == VoxelStates.Repaired) / (float)allVoxels.Count;
                    case GameTypes.Break:
                        return (float)allVoxels.Count(x => x.State == VoxelStates.Broken) / (float)allVoxels.Count;
                    default:
                        return 0;
                }

            }
        }

        public Vector3 MassCenter
        {
            get
            {
                Vector3 center = Vector3.zero;
                int count = 0;
                allVoxels.Where(x => x.State == VoxelStates.Repaired).ToList().ForEach(x => { center += x.Position; count++; });
                return center / count;
            }
        }
        public Vector3 Center
        {
            get
            {
                return transform.position + new Vector3(0, repairCenter.y * height, 0);
            }
        }
        public int GroupCount
        {
            get => groupCount;
            private set => groupCount = value;
        }
        public GameTypes GameType
        {
            get => gameType;
            private set => gameType = value;
        }
        public GameStates State
        {
            get => state;
            set
            {
                state = value;
                switch(state)
                {
                    case GameStates.Done:
                        OnDone?.Invoke();
                        break;
                    case GameStates.Failed:
                        OnFailed?.Invoke();
                        break;
                }
            }
        }


        public event System.Action<float> OnFilled;

        public event System.Action OnFailed;
        public event System.Action OnDone;

        public event System.Action<MoneyValue> OnMoney;


        #region Initilize

        public void Initialize(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;
            haptic = info.Components.Haptic;

            if (info.SceneContext != null)
            {
                GameType = info.SceneContext.GameType;
            }

            switch (GameType)
            {
                case GameTypes.Game:
                    InitializeMain();
                    break;
                case GameTypes.Paint:
                    InitializePaint();
                    break;
                case GameTypes.Break:
                    InitializeBreak();
                    break;
            }

        }
        public void InitializeShelf(SavedData.VoxelModelInfo info)
        {
            allVoxels = new List<IGameVoxel>(GetComponentsInChildren<IGameVoxel>(true));
            allVoxels.ForEach(x =>
            {
                x.Initilize(this, GameTypes.ShelfWatch);               
            });

            if (!info.Opened)
            {
                allVoxels.ForEach(x =>
                {
                    x.Material = info.PaintData.LocketMaterial;
                });
            }
            else if(info.Painted)
            {
                allVoxels.ForEach(x =>
                {
                    try
                    {
                        x.Material = info.Colors[x.ColorIndex];
                    }
                    catch(System.IndexOutOfRangeException)
                    {
                        Debug.LogError("Painted colors error!");
                    }
                });
            }
        }
        public void InitializePainted(SavedData.VoxelModelInfo info)
        {
            allVoxels = new List<IGameVoxel>(GetComponentsInChildren<IGameVoxel>(true));
            allVoxels.ForEach(x =>
            {
                x.Initilize(this, GameTypes.Paint);
            });

            waveMaker.Initilize(allVoxels, soundPlayer, haptic);

            allVoxels.ForEach(x =>
            {
                try
                {
                    x.Material = info.Colors[x.ColorIndex];
                }
                catch (System.IndexOutOfRangeException)
                {
                    Debug.LogError("Painted colors error!");
                }
            });
        }
        public void InitializePuzzle(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;
            haptic = info.Components.Haptic;


            allVoxels = new List<IGameVoxel>(GetComponentsInChildren<IGameVoxel>(true));
            allVoxels.ForEach(x =>
            {
                x.Initilize(this, GameTypes.Puzzle);
            });

            waveMaker.Initilize(allVoxels, soundPlayer, haptic);
        }
        public void InitializeRoad()
        {
            allVoxels = new List<IGameVoxel>(GetComponentsInChildren<IGameVoxel>(true));
            allVoxels.ForEach(x => x.Initilize(this, GameTypes.EventRoad));

            waveMaker.Initilize(allVoxels, soundPlayer, haptic);

            minBounds = new Vector3(allVoxels.Min(x => x.Position.x), allVoxels.Min(x => x.Position.y), 0);
            maxBounds = new Vector3(allVoxels.Max(x => x.Position.x), allVoxels.Max(x => x.Position.y), 0);

            height = Mathf.RoundToInt(maxBounds.y - minBounds.y);
            width = Mathf.RoundToInt(maxBounds.x - minBounds.x);
        }
        public void InitializeStatic()
        {
            allVoxels = new List<IGameVoxel>(GetComponentsInChildren<IGameVoxel>(true));
            allVoxels.ForEach(x =>
            {
                x.Initilize(this, GameTypes.ShelfWatch);
            });
        }

        private void InitializeMain()
        {
            allVoxels = new List<IGameVoxel>(GetComponentsInChildren<IGameVoxel>(true));
            allVoxels.ForEach(x =>
            {
                x.Initilize(this, gameType);
                x.Bind(soundPlayer);
            });

            waveMaker.Initilize(allVoxels, soundPlayer, haptic);

            if (transform.parent != null)
            {
                repairTriggers = FindObjectsOfType<HitTrigger>()
                    .Where(x => x.HitType == RepairHitInfo.Types.Repair).ToList();
                repairTriggers.ForEach(x => x.OnBulletEnter += OnRepairBullet);

                demolishTriggers = FindObjectsOfType<HitTrigger>()
                    .Where(x => x.HitType == RepairHitInfo.Types.Break).ToList();
                demolishTriggers.ForEach(x => x.OnBulletEnter += OnDestroyBullet);
            }

            minBounds = new Vector3(allVoxels.Min(x => x.Position.x), allVoxels.Min(x => x.Position.y), 0);
            maxBounds = new Vector3(allVoxels.Max(x => x.Position.x), allVoxels.Max(x => x.Position.y), 0);

            height = Mathf.RoundToInt(maxBounds.y - minBounds.y);
            width = Mathf.RoundToInt(maxBounds.x - minBounds.x);

            foreach (IGameVoxel voxel in allVoxels)
            {
                voxel.Break(new ActionParams());
            }
        }
        private void InitializeBreak()
        {
            allVoxels = new List<IGameVoxel>(GetComponentsInChildren<IGameVoxel>(true));

            breakTriggers = new List<BreakTrigger>();
            allVoxels.ForEach(x =>
            {
                x.Initilize(this, gameType);
                x.Bind(soundPlayer);

                BreakTrigger trigger = x.GameObject.AddComponent<BreakTrigger>();
                trigger.OnBulletEnter += OnBreakBullet;

                breakTriggers.Add(trigger);
            });

            waveMaker.Initilize(allVoxels, soundPlayer, haptic);


            minBounds = new Vector3(allVoxels.Min(x => x.Position.x), allVoxels.Min(x => x.Position.y), 0);
            maxBounds = new Vector3(allVoxels.Max(x => x.Position.x), allVoxels.Max(x => x.Position.y), 0);

            height = Mathf.RoundToInt(maxBounds.y - minBounds.y);
            width = Mathf.RoundToInt(maxBounds.x - minBounds.x);

            voxelDict = new Dictionary<Vector3, IGameVoxel>();
            foreach(IGameVoxel voxel in allVoxels)
            {
                if (voxelDict.ContainsKey(voxel.Position))
                    continue;
                voxelDict.Add(voxel.Position, voxel);
            }
        }
        private void InitializePaint()
        {
            allVoxels = new List<IGameVoxel>(GetComponentsInChildren<IGameVoxel>(true));
            allVoxels.ForEach(x => x.Initilize(this, GameTypes.Paint));

            waveMaker.Initilize(allVoxels, soundPlayer, haptic);

            minBounds = new Vector3(allVoxels.Min(x => x.Position.x), allVoxels.Min(x => x.Position.y), 0);
            maxBounds = new Vector3(allVoxels.Max(x => x.Position.x), allVoxels.Max(x => x.Position.y), 0);

            height = Mathf.RoundToInt(maxBounds.y - minBounds.y);
            width = Mathf.RoundToInt(maxBounds.x - minBounds.x);

            colorSetter.Initilize(allVoxels.OfType<IVoxel>().ToList(), GroupCount);
            colorSetter.SetGrayscaleColors();
        }


        public void InitilizeOnCreate(int groupCount) //fix
        {
            GroupCount = groupCount;
        }




        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnStateChanged += OnStateChanged;

            eventsHandler.OnClearScene += (x) =>
            {
                eventsHandler.OnStateChanged -= OnStateChanged;
            };
        }

        #endregion

        #region Events

        public void Restart(System.Action onDone)
        {
            Restart();

            this.Delayed(1.5f, onDone);
        }

        private void OnStateChanged(GameStates state)
        {
            this.state = state;
        }

        #endregion

        #region Repair Game

        private void OnRepairBullet(RepairHitInfo info)
        {
            DemolishTypes type = DemolishTypes.Repair;
            if(outerFill)
            {
                type = DemolishTypes.OuterRepair;
            }

            info.Bullet.Demolish(type);

            Repair(info.Info);
        }
        private void OnDestroyBullet(RepairHitInfo info)
        {
            if (info.Bullet.Settings.IsSuper)
                return;
            Demolish(info.Info);
        }


        public void Restart()
        {
            switch(gameType)
            {
                case GameTypes.Game:
                    RestartParams parameter = new RestartParams(restartAnimation, restartType, restartJumpPower, restartJumpRandomize);

                    foreach (IGameVoxel voxel in allVoxels)
                    {
                        voxel.Restart(parameter);
                    }
                    break;
            }
        }
        private void Repair(RepairBulletInfo info)
        {
            if (State != GameStates.Game)
                return;

            if (info.IsSuper)
            {
                waveMaker.MakeWave(new WaveParams(WaveParams.Types.Hit));
            }

            int repaired = 0;
            Vector3 center = new Vector3(repairCenter.x * width, repairCenter.y * height, 0);
            IEnumerable<IGameVoxel> voxels = allVoxels.Where(x => x.State == VoxelStates.Hidden);
            if(outerFill)
            {
                voxels = voxels.OrderByDescending(x => (x.Position - center).magnitude + Random.Range(0, repairRandomize));
            }
            else
            {
                voxels = voxels.OrderBy(x => (x.Position - center).magnitude + Random.Range(0, repairRandomize));
            }

            foreach(IGameVoxel voxel in voxels)
            {
                if (repaired >= info.RepairCount)
                    break;
                AnimationData animation = repairAnimation.Clone(repairAnimation);
                animation.Delay = repaired * repairDelay;

                voxel.Repair(new RepairParams(Vector3.zero, speed, animation));

                repaired++;

                this.Delayed(animation.Delay, () =>
                {
                    OnMoney?.Invoke(new MoneyValue(1));
                });
            }
            if(repaired > 0)
            {
                OnFilled?.Invoke(Fill);
            }

            if(voxels.Count() < 1)
            {
                State = GameStates.Done;
            }


            soundPlayer.PlaySound("voxel_create", info.Volume, info.Tone);
            haptic.VibrateHaptic();
        }
        private void Demolish(RepairBulletInfo info)
        {
            if (State != GameStates.Game)
                return;
            State = GameStates.Failed;


            Vector3 position = transform.TransformPoint(MassCenter) +
                new Vector3(Random.Range(-bulletHitRandomize, bulletHitRandomize),
                            Random.Range(-bulletHitRandomize, bulletHitRandomize),
                            0);

            soundPlayer.PlaySound("voxel_hit");
            haptic.VibratePop();

            info.Bullet.Redirect(new RicochetInfo(position, topBulletHeight, bulletFlyCurve, () =>
            {
                waveMaker.StopAllWaves();

                //Instantiate(breakParticle, position, Quaternion.Euler(-90, 0, 0), transform.parent).Play();

                BreakParams parameter = new BreakParams(MassCenter, impulse, upRatio, 1, 1, randomize);

                IEnumerable<IGameVoxel> voxels = allVoxels.Where(x => x.State == VoxelStates.Repaired);
                foreach (IGameVoxel voxel in voxels)
                {
                    voxel.Break(parameter);
                }

                soundPlayer.PlaySound("model_destroy");
                haptic.VibratePeek();
            }));
        }


        #endregion

        #region Paint Game

        public void Paint(PaintInfo info)
        {
            switch(info.Type)
            {
                case PaintInfo.Types.Instantly:
                    PaintInstantly(info);
                    break;
                case PaintInfo.Types.WaveGroup:
                    PaintWave(info);
                    break;
            }
        }

        public void SetGrayScale()
        {

            if (TryGetComponent(out ColorSetter colorSetter))
            {
                colorSetter.Initilize(allVoxels.OfType<IVoxel>().ToList(), groupCount);
                colorSetter.SetGrayscaleColors();
            }
        }

        private void PaintInstantly(PaintInfo info)
        {
            colorSetter.PaintGroup(info.Group, info.Material);
        }
        private void PaintWave(PaintInfo info)
        {
            waveMaker.MakeWave(new PaintWaveParams(info.Center, (x) =>
            {
                colorSetter.PaintVoxel(x, info.Material);
            }, info.Group, WaveParams.Types.Paint));
        }

        #endregion

        #region Break Game

        private void OnBreakBullet(BreakBulletInfo info)
        {
            Break(info);
        }

        
        private (int, int) NeighborCount(IGameVoxel voxel)
        {
            int count = 0;
            int alive = 0;
            if(voxelDict.TryGetValue(voxel.Position + Vector3.left, out IGameVoxel left))
            {
                count++;
                if(left.State == VoxelStates.Repaired)
                {
                    alive++;
                }
            }
            if (voxelDict.TryGetValue(voxel.Position + Vector3.up, out IGameVoxel up))
            {
                count++;
                if (up.State == VoxelStates.Repaired)
                {
                    alive++;
                }
            }
            if (voxelDict.TryGetValue(voxel.Position + Vector3.right, out IGameVoxel right))
            {
                count++;
                if (right.State == VoxelStates.Repaired)
                {
                    alive++;
                }
            }
            if (voxelDict.TryGetValue(voxel.Position + Vector3.down, out IGameVoxel down))
            {
                count++;
                if (down.State == VoxelStates.Repaired)
                {
                    alive++;
                }
            }

            return (count, alive);
        }

        private void Break(BreakBulletInfo info)
        {
            if (State != GameStates.Game || info == null || info.Bullet == null || info.Bullet.Destroyed)
                return;

            BreakParams parameter = new BreakParams(info.Bullet.transform.position, 7 * info.Power, 0.5f, 2f, 1, 1);

            int reward = 0;

            allVoxels
                .Where(x =>
                {
                    Vector3 vector = (x.GameObject.transform.position - info.Bullet.transform.position);
                    vector.z = 0;

                    return x.State == VoxelStates.Repaired && vector.magnitude < info.Radius;
                })
                .ToList()
                .ForEach(x =>
                {
                    x.Break(parameter);
                    reward++;
                });

            for (int i = 0; i < 3; i++)
            {
                allVoxels
                    .Where(x => x.State == VoxelStates.Repaired)
                    .ToList()
                    .ForEach(x =>
                    {
                        (int, int) count = NeighborCount(x);

                        switch (count.Item1)
                        {
                            case 1:
                                if (count.Item2 < 1)
                                {
                                    x.Break(parameter);
                                    reward++;
                                }
                                break;
                            case 2:
                                if (count.Item2 < 2)
                                {
                                    x.Break(parameter);
                                    reward++;
                                }
                                break;
                            case 3:
                                if (count.Item2 < 3)
                                {
                                    x.Break(parameter);
                                    reward++;
                                }
                                break;
                            case 4:
                                if (count.Item2 < 2)
                                {
                                    x.Break(parameter);
                                    reward++;
                                }
                                break;
                        }
                    });
            }
            info.Bullet.Demolish(BreakBullet.BreakTypes.Hit);

            IEnumerable<IGameVoxel> remaining = allVoxels.Where(x => x.State == VoxelStates.Repaired);
            if ((float)remaining.Count() / (float)allVoxels.Count() < 0.1f)
            {
                foreach (IGameVoxel voxel in remaining)
                {
                    voxel.Break(parameter);
                    reward++;
                }
                State = GameStates.Done;
            }

            OnMoney?.Invoke(new MoneyValue(reward));
            OnFilled?.Invoke(Fill);

            soundPlayer.PlaySound("voxel_hit");
            haptic.VibrateHaptic();
        }

        public void BoomEffect(Vector3 force, float angular, float randomize = 0f)
        {
            Vector3 direction = force.normalized;
            float power = force.magnitude;

            allVoxels
                .Where(x => x.State == VoxelStates.Broken)
                .ToList()
                .ForEach(x =>
                {
                    x.AddImpulse((direction + Random.onUnitSphere * randomize) * power, Random.onUnitSphere * angular);
                });
        }

        public void Demolish()
        {
            Vector3 position = transform.TransformPoint(MassCenter) +
                new Vector3(Random.Range(-bulletHitRandomize, bulletHitRandomize),
                            Random.Range(-bulletHitRandomize, bulletHitRandomize),
                            0);

            waveMaker.StopAllWaves();


            BreakParams parameter = new BreakParams(MassCenter, impulse, upRatio, 1, 1, randomize);

            IEnumerable<IGameVoxel> voxels = allVoxels.Where(x => x.State == VoxelStates.Repaired);
            foreach (IGameVoxel voxel in voxels)
            {
                voxel.Break(parameter);
            }

            soundPlayer.PlaySound("model_destroy");
            haptic.VibrateHaptic();
        }


        #endregion

        #region Internal

        [Button("Restore")]
        public void Restore()
        {
            Restart();
        }

        [Button("Break")]
        public void BREAK_TEST()
        {
            IEnumerable<IGameVoxel> voxels = allVoxels.Where(x => x.State == VoxelStates.Repaired);
            foreach (IGameVoxel voxel in voxels)
            {
                voxel.Break(new BreakParams(MassCenter, impulse, upRatio, 1, 1, randomize));
            }
        }


        #endregion
    }
}