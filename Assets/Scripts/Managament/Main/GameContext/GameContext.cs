using DG.Tweening;
using System.Linq;
using UnityEngine;
using Audio;
using Data;
using Services;
using Ads;
using NaughtyAttributes;
using Managament.Shelfs;
using Managament.Levels;
using System.Collections.Generic;
using Vibration;


namespace Managament
{
    [DefaultExecutionOrder(-1)]
    public class GameContext : MonoBehaviour, ISceneVisitor
    {
        private const int LOADING_SCREEN_SCENE_INDEX = 0;

        private const int GAME_SCENE_INDEX = 1;
        private const int INFINITY_SCENE_INDEX = 2;

        private const int SHELF_SCENE_INDEX = 3;
        private const int SHOP_SCENE_INDEX = 4;

        private const int PAINT_SCENE_INDEX = 5;
        private const int PAINTED_SCENE_INDEX = 6;

        private const int PUZZLE_SHELF_SCENE_INDEX = 7;
        private const int PUZZLE_SCENE_INDEX = 8;
        private const int PUZZLE_REPAIR_SCENE_INDEX = 9;

        private const int EVENT_ROAD_SCENE_INDEX = 10;
        private const int BREAK_SCENE_INDEX = 11;

        #region Singletone

        private static GameContext Active { get; set; }


        #endregion

        [Header("Settings/Ads")]
        [SerializeField] private float interReloadTime = 25f; 
        [Header("Resources")]
        [SerializeField] private LocationsList locationsList;
        [Header("Links")]
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private SoundPlayer soundPlayer;
        [SerializeField] private AdsController adsController;
        [Space]


        private Haptic haptic;
        private MoneyController moneyController;
        private MoneyController gemController;

        private EventsTracker eventsTracker;
        private ComponentsInfo components;


        private SceneContext.SceneData sceneData;
        private SceneContext.SceneTypes currantScene;
        private Dictionary<SceneContext.SceneTypes, int> scenesIndex = new Dictionary<SceneContext.SceneTypes, int>()
        {
            {SceneContext.SceneTypes.Game, GAME_SCENE_INDEX },
            {SceneContext.SceneTypes.Paint, PAINT_SCENE_INDEX },
            {SceneContext.SceneTypes.Shelf, SHELF_SCENE_INDEX },
            {SceneContext.SceneTypes.Painted, PAINTED_SCENE_INDEX },
            {SceneContext.SceneTypes.Shop, SHOP_SCENE_INDEX },
            {SceneContext.SceneTypes.Infinity, INFINITY_SCENE_INDEX },
            {SceneContext.SceneTypes.LoadingScreen, LOADING_SCREEN_SCENE_INDEX },
            {SceneContext.SceneTypes.Puzzle, PUZZLE_SCENE_INDEX },
            {SceneContext.SceneTypes.PuzzleShelf, PUZZLE_SHELF_SCENE_INDEX },
            {SceneContext.SceneTypes.PuzzleRepair, PUZZLE_REPAIR_SCENE_INDEX },
            {SceneContext.SceneTypes.EventRoad, EVENT_ROAD_SCENE_INDEX },
            {SceneContext.SceneTypes.Break, BREAK_SCENE_INDEX },
        };


        private void Awake()
        {
            if (Active != null)
            {
                Destroy(gameObject);
                return;
            }
            Active = this;
            DontDestroyOnLoad(gameObject);


            ComponentsInitilize();
            SetupSceneContext();
        }

        private void ComponentsInitilize()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
            DOTween.SetTweensCapacity(1000, 50);


            moneyController = new MoneyController(this, MoneyController.Types.Coin);
            gemController = new MoneyController(this, MoneyController.Types.Gem);
            eventsTracker = new EventsTracker();
            haptic = new Haptic();
            components = new ComponentsInfo
                (moneyController,
                 gemController,
                 soundPlayer, 
                 eventsTracker, 
                 adsController, 
                 haptic);

            adsController.Initialize(interReloadTime);
            sceneLoader.Initialize(scenesIndex);


            TurnSound(SavedData.SoundEnabled);
        }
        private void SetupSceneContext()
        {
            SceneContext.OnLoaded += OnSceneLoaded;
            SceneContext.OnAwake += OnSceneAwake;
            SceneContext.OnCallPreload += OnCalledScenePreload;
        }

        private void OnCalledScenePreload(SceneContext scene, SceneContext.SceneTypes preload)
        {
            sceneLoader.PreloadScene(preload);
        }
        private void OnSceneAwake(SceneContext scene)
        {
            adsController.ClearEvents();

            scene.Components = components;

            scene.OnLocalInitialize += OnSceneInitialized;
            scene.OnTurnSound += TurnSound;
            scene.OnTurnVibro += TurnVibro;

            scene.Visit(this);
        }

        //Scene Awake
        public void Visited(LoadingSceneContext loadingScene)
        {
            currantScene = SceneContext.SceneTypes.LoadingScreen;
        }
        public void Visited(RepairSceneContext repairScene)
        {
            repairScene.OnShelfClick += GoSimpleShelfScene;
            repairScene.OnShopClick += GoShopScene;
            repairScene.OnPuzzleClick += GoPuzzleShelfScene;
            repairScene.OnFinalEnd += GoShelfScene;
            repairScene.OnEventClick += GoEventRoadScene;

            currantScene = SceneContext.SceneTypes.Game;
        }
        public void Visited(InfinitySceneContext infinityScene)
        {
            infinityScene.OnShelfClick += GoSimpleShelfScene;
            infinityScene.OnShopClick += GoShopScene;
            infinityScene.OnPuzzleClick += GoPuzzleShelfScene;

            currantScene = SceneContext.SceneTypes.Game;
        }

        public void Visited(PaintSceneContext paintScene)
        {
            paintScene.OnDone += GoPaintedScene;
            paintScene.OnExit += GoShelfScene;

            paintScene.SetData(sceneData);

            currantScene = SceneContext.SceneTypes.Paint;
        }
        public void Visited(PaintedSceneContext paintedScene)
        {
            paintedScene.OnDone += GoShelfScene;

            paintedScene.SetData(sceneData);

            currantScene = SceneContext.SceneTypes.Painted;
        }

        public void Visited(ShelfSceneContext shelfScene)
        {
            shelfScene.OnSelectItem += GoPaintScene;
            shelfScene.OnExit += GoGameScene;

            shelfScene.SetData(sceneData);

            currantScene = SceneContext.SceneTypes.Shelf;
        }

        public void Visited(ShopSceneContext shopScene)
        {
            shopScene.OnDone += GoGameScene;

            currantScene = SceneContext.SceneTypes.Shop;
        }

        public void Visited(PuzzleShelfSceneContext puzzleScene)
        {
            puzzleScene.OnSelectPuzzle += GoPuzzleScene;
            puzzleScene.OnExit += GoGameScene;

            currantScene = SceneContext.SceneTypes.PuzzleShelf;
        }
        public void Visited(PuzzleSceneContext puzzleScene)
        {
            puzzleScene.SetData(sceneData);

            puzzleScene.OnSelectPuzzle += GoRepairPuzzleScene;
            puzzleScene.OnExit += GoPuzzleShelfScene;

            currantScene = SceneContext.SceneTypes.Puzzle;
        }
        public void Visited(PuzzleRepairSceneContext puzzleScene)
        {
            puzzleScene.SetData(sceneData);
            puzzleScene.OnFinalEnd += GoPuzzleScene;
            puzzleScene.OnExitClick += GoPuzzleShelfScene;

            currantScene = SceneContext.SceneTypes.PuzzleRepair;
        }

        public void Visited(EventRoadSceneContext roadScene)
        {
            roadScene.OnPlay += GoBreakScene;
            roadScene.OnNextRoad += GoEventRoadScene;
            roadScene.OnExit += GoGameScene;

            currantScene = SceneContext.SceneTypes.EventRoad;
        }
        public void Visited(BreakSceneContext breakScene)
        {
            breakScene.SetData(sceneData);
            breakScene.OnExitClick += GoEventRoadScene;
            breakScene.OnFinalEnd += GoEventRoadScene;

            currantScene = SceneContext.SceneTypes.Break;
        }



        //Scene Awake

        private void OnSceneLoaded(SceneContext scene)
        {
            Debug.Log($"Scene Context Loaded | {scene.GameType}");

            this.Delayed(Time.fixedDeltaTime, () => moneyController.CallVisualUpdate());            

            switch (sceneLoader.CurrantScene)
            {
                case SceneContext.SceneTypes.Game:
                    sceneLoader.PreloadScene(SceneContext.SceneTypes.Shelf);
                    break;
                case SceneContext.SceneTypes.Infinity:
                    sceneLoader.PreloadScene(SceneContext.SceneTypes.Shelf);
                    break;
                case SceneContext.SceneTypes.Shelf:
                    PreloadGameScene();
                    break;
                case SceneContext.SceneTypes.Paint:
                    sceneLoader.PreloadScene(SceneContext.SceneTypes.Painted);
                    break;
                case SceneContext.SceneTypes.Painted:
                    sceneLoader.PreloadScene(SceneContext.SceneTypes.Shelf);
                    break;
                case SceneContext.SceneTypes.Shop:
                    PreloadGameScene();
                    break;
                case SceneContext.SceneTypes.PuzzleShelf:
                    sceneLoader.PreloadScene(SceneContext.SceneTypes.Puzzle);
                    break;
                case SceneContext.SceneTypes.Puzzle:
                    sceneLoader.PreloadScene(SceneContext.SceneTypes.PuzzleRepair);
                    break;
                case SceneContext.SceneTypes.PuzzleRepair:
                    sceneLoader.PreloadScene(SceneContext.SceneTypes.Puzzle);
                    break;
                case SceneContext.SceneTypes.EventRoad:
                    //None;
                    break;
                case SceneContext.SceneTypes.Break:
                    sceneLoader.PreloadScene(SceneContext.SceneTypes.EventRoad);
                    break;
                case SceneContext.SceneTypes.LoadingScreen:
                    this.Delayed(Time.fixedDeltaTime, () =>
                    {
                        PreloadGameScene(ThreadPriority.High);
                        sceneLoader.ActivateOnLoad();
                    });
                    break; 
            }
        }
        private void OnSceneInitialized(GameInfo obj)
        {
            try
            {
                soundPlayer.Initilize(obj);
            }
            catch { }
        }


        private void TurnSound(bool value)
        {
            SavedData.SoundEnabled = value;

            AudioListener.volume = value ? 1 : 0;
        }
        private void TurnVibro(bool value)
        {
            SavedData.VibroEnabled = value;

            if(value)
            {
                haptic.VibratePop();
            }
        }


#region SceneTransition

        private void GoShelfScene()
        {            
            switch(currantScene)
            {
                case SceneContext.SceneTypes.Game:
                    sceneData = new ShelfSceneContext.ShelfSceneData(ShelfSceneContext.ShelfSceneData.ActionTypes.NewOpened, SavedData.LastOpenedModel);
                    break;
                case SceneContext.SceneTypes.Painted:
                    sceneData = new ShelfSceneContext.ShelfSceneData(ShelfSceneContext.ShelfSceneData.ActionTypes.Painted, SavedData.LastPaintedModel);
                    break;
                default:
                    sceneData = new ShelfSceneContext.ShelfSceneData();
                    break;
            }

            sceneLoader.GoScene(SceneContext.SceneTypes.Shelf);
        }
        private void GoSimpleShelfScene()
        {
            sceneData = new ShelfSceneContext.ShelfSceneData();

            sceneLoader.GoScene(SceneContext.SceneTypes.Shelf);
        }
        private void GoShopScene()
        {
            sceneLoader.GoScene(SceneContext.SceneTypes.Shop);
        }

        private void GoGameScene()
        {
            if(SavedData.LevelsDone < locationsList.LevelsCount)
            {
                sceneLoader.GoScene(SceneContext.SceneTypes.Game);
                SavedData.EndShown = false;
            }
            else
            {
                sceneLoader.GoScene(SceneContext.SceneTypes.Infinity);
            }
        }
        private void PreloadGameScene(ThreadPriority priority = ThreadPriority.Normal)
        {
            if (SavedData.LevelsDone < locationsList.LevelsCount)
            {
                sceneLoader.PreloadScene(SceneContext.SceneTypes.Game, priority);
            }
            else
            {
                sceneLoader.PreloadScene(SceneContext.SceneTypes.Infinity, priority);
            }
        }

        private void GoPaintScene(Shelf.Item item)
        {
            sceneData = new PaintSceneContext.PaintSceneData(item.Index);

            sceneLoader.GoScene(SceneContext.SceneTypes.Paint);
        }
        private void GoPaintedScene()
        {
            sceneLoader.GoScene(SceneContext.SceneTypes.Painted);
        }

        private void GoPuzzleShelfScene()
        {
            sceneLoader.GoScene(SceneContext.SceneTypes.PuzzleShelf);
        }
        private void GoPuzzleScene(PuzzleId obj)
        {
            sceneData = new PuzzleSceneData(obj);

            sceneLoader.GoScene(SceneContext.SceneTypes.Puzzle);
        }
        private void GoRepairPuzzleScene(PuzzleId puzzleId, ModelId modelId)
        {
            sceneData = new PuzzleModelSceneData(puzzleId, modelId);

            sceneLoader.GoScene(SceneContext.SceneTypes.PuzzleRepair);
        }


        private void GoEventRoadScene()
        {
            sceneLoader.GoScene(SceneContext.SceneTypes.EventRoad);
        }
        private void GoBreakScene(PuzzleId puzzleId, ModelId modelId, bool boss)
        {
            sceneData = new PuzzleModelSceneData(puzzleId, modelId, boss);

            sceneLoader.GoScene(SceneContext.SceneTypes.Break);
        }

#endregion

#region Internal

        [Button("Add 500 money")]
        private void Add_500()
        {
            moneyController?.Add(500);
        }
        [Button("Add 50000 money")]
        private void Add_50000()
        {
            moneyController?.Add(50000);
            gemController?.Add(50000);
        }

        [Button("Set All Levels Almost Done")]
        private void SetLevelsAlmostDone()
        {
            SavedData.LevelsDone = 65;
            SavedData.LocationIndex = 13;
            SavedData.LevelIndex = 0;
        }




#endregion
    }
}
