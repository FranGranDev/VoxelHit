using Factory;
using NaughtyAttributes;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Managament
{
    public abstract class SceneContext : MonoBehaviour, ISceneContext, IGameEventsHandler
    {
        [Foldout("DI"), SerializeField] protected Zenject.SceneContext sceneContext;
        [Foldout("Music"), SerializeField] protected AudioClip baseMusic;
        [Foldout("States"), SerializeField] private GameTypes gameType;
        [Foldout("States"), SerializeField] private GameStates gameState;
        [Foldout("Settings"), SerializeField] protected bool autoStart;
        [Foldout("Background"), SerializeField] private bool staticBackground = true;
        [Foldout("Background"), SerializeField] protected Background.Settings background;

        protected List<ITargetEventHandler> targetEventHandlers;


        public ComponentsInfo Components { get; set; }

        public GameStates GameState
        {
            get => gameState;
            protected set
            {
                OnStateChanged?.Invoke(value);

                GameStates prev = gameState;
                gameState = value;

                switch (gameState)
                {
                    case GameStates.Game:
                        if (prev == GameStates.Failed)
                        {
                            if (targetEventHandlers.Count > 0)
                            {
                                targetEventHandlers.ForEach(x => x.Restart(() => OnRestart?.Invoke()));
                            }//fix
                            else
                            {
                                OnRestart?.Invoke();
                            }
                        }
                        else
                        {
                            OnStarted?.Invoke();
                        }
                        break;
                    case GameStates.Failed:
                        OnFailed?.Invoke();
                        break;
                    case GameStates.Done:
                        OnDone?.Invoke();
                        break;
                }
            }
        }
        public GameTypes GameType
        {
            get => gameType;
        }
        public virtual Transform LevelTransform
        {
            get => transform;
        }
        protected SceneData Data
        {
            get; set;
        }


        public event Action<GameStates> OnStateChanged;

        public event Action OnStarted;
        public event Action OnRestart;
        public event Action OnDone;
        public event Action OnFailed;
        public event Action<IGameEventsHandler> OnClearScene;

        public event Action<GameInfo> OnLocalInitialize;

        public event Action<bool> OnTurnSound;
        public event Action<bool> OnTurnVibro;

        public static event Action<SceneContext> OnAwake;
        public static event Action<SceneContext> OnLoaded;
        public static event Action<SceneContext, SceneTypes> OnCallPreload;



        private void Awake()
        {
            OnAwake?.Invoke(this);

            Initialize();
        }
        private void Start()
        {
            OnLoaded?.Invoke(this);
        }


        private void Initialize()
        {
            SceneInitilize();
            SetLayerMatrix();
            SetGravity();

            if (staticBackground)
            {
                BackgroundInitialize(background);
            }
        }
        public void SetData(SceneData sceneData)
        {
            Data = sceneData;
        }
        public abstract void Visit(ISceneVisitor visitor);


        protected abstract void SceneInitilize();
        protected virtual void SetLayerMatrix()
        {
            Physics.IgnoreLayerCollision(6, 6, true);
        }
        protected virtual void SetGravity()
        {
            Physics.gravity = new Vector3(0, -20, 0);
        }

        protected void UseFactories()
        {
            transform.GetComponentsInChildren<IFactory>()
                .ToList()
                .ForEach(x => x.Create());
        }
        protected T UseFactory<T>(T obj)
        {
            IFactory<T> factory = transform.GetComponentInChildren<IFactory<T>>();
            factory.Create(obj);

            return factory.Created;
        }
        protected void UseTargetEvents()
        {
            targetEventHandlers = transform.GetComponentsInChildren<ITargetEventHandler>(true).
                Where(x => !x.Equals(this)).ToList();
            targetEventHandlers.ForEach(x =>
            {
                x.OnDone += OnLevelCompleate;
                x.OnFailed += OnLevelFailed;
                x.OnMoney += OnCollectMoney;
            });
        }


        protected void AutoBind<T>()
        {
            T target = transform.GetComponentInChildren<T>();
            if (target == null)
                return;

            transform.GetComponentsInChildren<IBindable<T>>(true)
                .ToList()
                .ForEach(x => x.Bind(target));
        }
        protected void InitializeComponents<T>(T info)
        {
            transform.GetComponentsInChildren<Initializable<T>>(true)
                .ToList()
                .ForEach(x => x.Initialize(info));
        }
        protected void BackgroundInitialize(Background.Settings settings)
        {
            Background back = GetComponentInChildren<Background>();
            if (back)
            {
                back.Initialize(settings);
            }
        }



        protected void ResetDI()
        {
            sceneContext.Resolve();
        }


        protected void ClearEvents()
        {
            OnStarted = null;
            OnRestart = null;
            OnDone = null;
            OnFailed = null;

            OnStateChanged = null;
        }
        protected void CallScenePreload(SceneTypes sceneType)
        {
            OnCallPreload?.Invoke(this, sceneType);
        }
        protected void CallOnLocalInitialize(GameInfo info)
        {
            OnLocalInitialize?.Invoke(info);
        }


        protected virtual void OnLevelCompleate()
        {
            if (GameState == GameStates.Done)
                return;
            GameState = GameStates.Done;
        }
        protected virtual void OnLevelFailed()
        {
            if (GameState == GameStates.Failed)
                return;
            GameState = GameStates.Failed;
        }
        protected virtual void OnCollectMoney(MoneyValue obj)
        {

        }

        protected virtual void ClearScene()
        {
            OnClearScene?.Invoke(this);
        }


        protected void TurnSound(bool value)
        {
            OnTurnSound?.Invoke(value);
        }
        protected void TurnVibro(bool value)
        {
            OnTurnVibro?.Invoke(value);
        }


        public class SceneData
        {

        }
        public enum SceneTypes 
        { 
            Game,
            Shelf,

            Paint,
            Painted,

            Shop, 
            Infinity, 
            LoadingScreen, 

            Puzzle,
            PuzzleShelf,
            PuzzleRepair,

            EventRoad,
            Break,
        }

        #region Internal

        [Button]
        private void Internal_Restart()
        {
            if (GameState != GameStates.Failed)
                return;
            GameState = GameStates.Game;
        }

        [Button]
        private void Internal_Start()
        {
            if (GameState != GameStates.Idle)
                return;
            GameState = GameStates.Game;
        }

        [Button]
        private void Internal_Failed()
        {
            if (GameState != GameStates.Game)
                return;
            GameState = GameStates.Failed;
        }

        [Button]
        private void Internal_Done()
        {
            OnLevelCompleate();
        }

        private void OnValidate()
        {
            Background back = GetComponentInChildren<Background>();
            if(back)
            {
                back.Initialize(background);
            }
        }
        #endregion
    }
}
