using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Services;
using NaughtyAttributes;


namespace Data
{
    [CreateAssetMenu(fileName = "Saved Data", menuName = "Saved Data/Saved Data")]
    public class SavedData : ScriptableObject
    {
        #region Constant

        private const string LEVELS_DONE_KEY = "CurrentLevelIndex";
        private const string LOCATIONS_DONE_KEY = "LocationsDoneIndex";

        private const string INF_LEVELS_DONE_KEY = "Inf_CurrentLevelIndex";
        private const string INF_LOCATIONS_DONE_KEY = "Inf_LocationsDoneIndex";

        private const string LOCATION_INDEX_KEY = "CurrentLocationIndex";
        private const string LEVEL_INDEX_KEY = "CurrentLocationLevelIndex";

        private const string INF_LOCATION_INDEX_KEY = "Inf_CurrentLocationIndex";
        private const string INF_LEVEL_INDEX_KEY = "Inf_CurrentLocationLevelIndex";

        private const string SEED_KEY = "SeedKey";

        private const string LAST_OPENED_MODEL_KEY = "last_opened_model";
        private const string LAST_PAINTED_MODEL_KEY = "last_painted_model";

        private const string LAST_SHELF_INDEX_KEY = "last_shelf_index";

        private const string CURRANT_BULLET_KEY = "currant_bullet";
        private const string CURRANT_CANNON_KEY = "currant_cannon";


        private const string SOUND_ENABLED = "sound_enabled";
        private const string VIBRO_ENABLED = "vibro_enabled";


        private const string END_SHOWN = "end_shown";

        #endregion

        [Foldout("Links"), SerializeField] private VoxelModelsData modelsData;
        [Foldout("Links"), SerializeField] private PaintData paintData;
        [Foldout("Links"), SerializeField] private CannonsData cannonsData;
        [Foldout("Links"), SerializeField] private BulletData bulletData;
        [Foldout("Links"), SerializeField] private PuzzleModelsData puzzlesData;
        [Foldout("Links"), SerializeField] private PuzzleModelsData eventsData;
        [Space]
        [Foldout("Visualize"), SerializeField] private List<ModelInfoVisualize> modelItems;
        [Foldout("Visualize"), SerializeField] private List<ModelInfoVisualize> puzzlesItems;
        [Space]
        [Foldout("Utils"), SerializeField] private string utilModelGroupOpen;


        #region General

        public static bool EndShown { get => PlayerPrefs.GetInt(END_SHOWN, 0) == 1; set => PlayerPrefs.SetInt(END_SHOWN, value ? 1 : 0); }

        public static int LevelsDone { get => PlayerPrefs.GetInt(LEVELS_DONE_KEY, 0); set => PlayerPrefs.SetInt(LEVELS_DONE_KEY, value); }
        public static int LocationsDone { get => PlayerPrefs.GetInt(LOCATIONS_DONE_KEY, 0); set => PlayerPrefs.SetInt(LOCATIONS_DONE_KEY, value); }

        public static int InfinityLevelsDone { get => PlayerPrefs.GetInt(INF_LEVELS_DONE_KEY, 0); set => PlayerPrefs.SetInt(INF_LEVELS_DONE_KEY, value); }
        public static int InfinityLocationsDone { get => PlayerPrefs.GetInt(INF_LOCATIONS_DONE_KEY, 0); set => PlayerPrefs.SetInt(INF_LOCATIONS_DONE_KEY, value); }

        public static int TrackedLevelsIndex
        {
            get => LevelsDone + InfinityLevelsDone + 1;
        }


        public static int Seed { get => PlayerPrefs.GetInt(SEED_KEY, 1337); set => PlayerPrefs.SetInt(SEED_KEY, value); }

        public static int LevelIndex { get => PlayerPrefs.GetInt(LEVEL_INDEX_KEY, 0); set => PlayerPrefs.SetInt(LEVEL_INDEX_KEY, value); }
        public static int LocationIndex { get => PlayerPrefs.GetInt(LOCATION_INDEX_KEY, 0); set => PlayerPrefs.SetInt(LOCATION_INDEX_KEY, value); }

        public static int InfinityLevelIndex { get => PlayerPrefs.GetInt(INF_LEVEL_INDEX_KEY, 0); set => PlayerPrefs.SetInt(INF_LEVEL_INDEX_KEY, value); }
        public static int InfinityLocationIndex
        {
            get
            {
                if(!PlayerPrefs.HasKey(INF_LOCATION_INDEX_KEY))
                {
                    InfinityLocationIndex = Random.Range(0, 14);
                }
                return PlayerPrefs.GetInt(INF_LOCATION_INDEX_KEY);
            }
            set => PlayerPrefs.SetInt(INF_LOCATION_INDEX_KEY, value);
        }



        public static bool SoundEnabled { get => PlayerPrefs.GetInt(SOUND_ENABLED, 1) == 1; set => PlayerPrefs.SetInt(SOUND_ENABLED, value ? 1 : 0); }
        public static bool VibroEnabled { get => PlayerPrefs.GetInt(VIBRO_ENABLED, 1) == 1; set => PlayerPrefs.SetInt(VIBRO_ENABLED, value ? 1 : 0); }

        #endregion

        #region Shop

        public int GetCurrantItemIndex(ShopItemsTypes itemsType)
        {
            switch(itemsType)
            {
                case ShopItemsTypes.Bullets:
                    return CurrantBulletIndex;
                case ShopItemsTypes.Cannons:
                    return CurrantCannonIndex;
                default:
                    return 0;
            }
        }
        public void SetCurrantItemIndex(ShopItemsTypes itemsType, int value)
        {
            switch (itemsType)
            {
                case ShopItemsTypes.Bullets:
                    CurrantBulletIndex = value;
                    break;
                case ShopItemsTypes.Cannons:
                    CurrantCannonIndex = value;
                    break;
            }
        }
        public IEnumerable<IShopItem> GetShopItems(ShopTypes shopTypes, ShopItemsTypes itemsType)
        {
            switch(itemsType)
            {
                case ShopItemsTypes.Bullets:
                    return GetBulletsInfo(shopTypes).OfType<IShopItem>();
                case ShopItemsTypes.Cannons:
                    return GetCannonsInfo(shopTypes).OfType<IShopItem>();
                default:
                    return null;
            }
        }
        public CostInfo GetCurrantItemCost(ShopTypes shopType, ShopItemsTypes itemsType)
        {
            switch(itemsType)
            {
                case ShopItemsTypes.Bullets:
                    return BulletsCost(shopType);
                case ShopItemsTypes.Cannons:
                    return CannonCost(shopType);
                default:
                    return null;
            }
        }


        #endregion

        #region Voxel Models

        public static int LastOpenedModel
        {
            get => PlayerPrefs.GetInt(LAST_OPENED_MODEL_KEY, -1);
            set => PlayerPrefs.SetInt(LAST_OPENED_MODEL_KEY, value);
        }
        public static int LastPaintedModel
        {
            get => PlayerPrefs.GetInt(LAST_PAINTED_MODEL_KEY, -1);
            set => PlayerPrefs.SetInt(LAST_PAINTED_MODEL_KEY, value);
        }
        public bool IsModelGroupOpened(string group)
        {
            List<ModelId> models = modelsData.GetItems(group);
            foreach (ModelId model in models)
            {
                if (!new VoxelModelInfo(model, paintData).Opened)
                    return false;
            }
            return true;
        }

        public class VoxelModelInfo : ModelInfo
        {
            protected const string PAINTED_KEY = "_model_painted";//{group}_{index}
            protected const string PAINT_COLORS_COUNT_KEY = "_model_colors_count";//{group}_{index}
            protected const string PAINT_COLORS_INDEX_KEY = "_model_colors_index_";//{group}_{index}


            public VoxelModelInfo(ModelId modelId, PaintData paintData) : base(modelId)
            {
                PaintData = paintData;
                Painted = PlayerPrefs.GetInt(Group + Index + PAINTED_KEY, 0) == 1;
                Colors = new List<Material>();

                if (Painted)
                {
                    int count = PlayerPrefs.GetInt(Group + Index + PAINT_COLORS_COUNT_KEY, 0);
                    for (int i = 0; i < count; i++)
                    {
                        int index = PlayerPrefs.GetInt(Group + Index + PAINT_COLORS_INDEX_KEY + i, 0);
                        Colors.Add(PaintData.GetColor(index).Material);
                    }
                }
            }


            public static void SetPainted(ModelId model, Dictionary<int, PaintData.Item> colors, bool setLastPainted = true)
            {
                PlayerPrefs.SetInt(model.Group + model.Index + PAINTED_KEY, 1);

                IEnumerable<int> keys = colors.Keys.OrderBy(x => x);
                PlayerPrefs.SetInt(model.Group + model.Index + PAINT_COLORS_COUNT_KEY, colors.Count);
                foreach (int key in keys)
                {
                    PlayerPrefs.SetInt(model.Group + model.Index + PAINT_COLORS_INDEX_KEY + key, colors[key].Index);
                }

                if (setLastPainted)
                {
                    LastPaintedModel = model.Index;
                }
            }


            public bool Painted { get; }
            public List<Material> Colors { get; }
            public PaintData PaintData { get; }
        }

        public class ModelInfo
        {
            protected const string OPENED_KEY = "_model_opened"; //{group}_{index}

            public int Index { get; }
            public string Group { get; }
            public bool Opened { get => PlayerPrefs.GetInt(Group + Index + OPENED_KEY, 0) == 1; }
            public ModelId ModelId { get; }

            public ModelInfo(ModelId modelId)
            {
                Index = modelId.Index;
                Group = modelId.Group;

                ModelId = modelId;
            }
            
            public void SetOpened()
            {
                PlayerPrefs.SetInt(Group + Index + OPENED_KEY, 1);
            }
            public static void SetOpened(ModelId model, bool setLastOpened = true)
            {
                PlayerPrefs.SetInt(model.Group + model.Index + OPENED_KEY, 1);

                if (setLastOpened)
                {
                    LastOpenedModel = model.Index;
                }
            }
        }

        #endregion

        #region Puzzles&Events

        public List<EventInfo> GetEvents()
        {
            List<EventInfo> events = new List<EventInfo>();
            foreach (PuzzleModelsData.Collection puzzle in eventsData.GetModels)
            {
                EventInfo info = new EventInfo(puzzle.PuzzleId, eventsData);
                events.Add(info);
            }

            return events;
        }
        public EventInfo GetActiveEvent()
        {
            List<PuzzleModelsData.Collection> avaliable = eventsData.GetModels
                .Where(x => LevelsDone >= x.MinLevel)
                .ToList();
            foreach (PuzzleModelsData.Collection puzzle in avaliable)
            {
                EventInfo info = new EventInfo(puzzle.PuzzleId, eventsData);
                if (!info.Done)
                    return info;
            }

            PuzzleModelsData.Collection last = avaliable.LastOrDefault();
            if (last == null)
                return null;

            return new EventInfo(last.PuzzleId, eventsData);
        }
        public EventInfo GetNextEvent()
        {
            EventInfo prev = GetActiveEvent();

            int index = eventsData.GetModels.IndexOf(prev.Collection) + 1;
            if (index >= eventsData.GetModels.Count)
            {
                return null;
            }


            return new EventInfo(eventsData.GetModels[index].PuzzleId, eventsData);           
        }

        
        public List<PuzzleInfo> GetPuzzles()
        {
            List<PuzzleInfo> puzzles = new List<PuzzleInfo>();
            foreach(PuzzleModelsData.Collection collection in puzzlesData.GetModels)
            {
                puzzles.Add(new PuzzleInfo(collection.PuzzleId, puzzlesData));
            }

            return puzzles;
        }


        public class EventInfo : PuzzleInfo
        {
            public EventInfo(PuzzleId puzzleId, PuzzleModelsData puzzledData) : base(puzzleId, puzzledData)
            {
                
            }


            public int Remaining
            {
                get => Items.Count(x => !x.Opened);
            }
            public PuzzleItemInfo Currant
            {
                get
                {
                    return Items.FirstOrDefault(x => !x.Opened);
                }
            }
        }

        public class PuzzleItemInfo : ModelInfo
        {
            protected const string PLACED_KEY = "_model_placed"; //{group}_{index}

            public PuzzleItemInfo(ModelId modelId) : base(modelId)
            {                
            }

            public bool Placed 
            { 
                get => PlayerPrefs.GetInt(Group + Index + PLACED_KEY, 0) == 1;
            }

            public void SetPlaced()
            {
                PlayerPrefs.SetInt(Group + Index + PLACED_KEY, 1);
            }
            public void Reset()
            {
                PlayerPrefs.SetInt(Group + Index + OPENED_KEY, 0);
                PlayerPrefs.SetInt(Group + Index + PLACED_KEY, 0);
            }
            public static void Reset(ModelId model)
            {
                PlayerPrefs.SetInt(model.Group + model.Index + OPENED_KEY, 0);
                PlayerPrefs.SetInt(model.Group + model.Index + PLACED_KEY, 0);
            }
        }
        public class PuzzleInfo
        {
            private const string WATCHED_KEY = "_wathced";

            public PuzzleInfo(PuzzleId puzzleId, PuzzleModelsData puzzledData)
            {
                PuzzleId = puzzleId;

                Opened = true;
                Collection = puzzledData.GetModels.First(x => x.Group == puzzleId.Group);

                Items = new List<PuzzleItemInfo>();
                foreach(ModelId model in puzzledData.GetItems(puzzleId))
                {
                    Items.Add(new PuzzleItemInfo(model));
                }
                foreach(PuzzleItemInfo item in Items)
                {
                    if(!item.Placed)
                    {
                        Opened = false;
                        return;
                    }
                }

                Done = Items.Count(x => !x.Placed) == 0;
            }


            public PuzzleId PuzzleId { get; }
            public PuzzleModelsData.Collection Collection { get; }
            public bool Opened { get; set; }
            public bool Done { get; }

            public List<PuzzleItemInfo> Items { get; }

            public Notflication Notflication
            {
                get => Collection.Notflication;
            }
            public bool NotflicationWatched
            {
                get => PlayerPrefs.GetInt(PuzzleId.Group + WATCHED_KEY, 0) == 1;
                set => PlayerPrefs.SetInt(PuzzleId.Group + WATCHED_KEY, value ? 1 : 0);
            }


            public int MinLevel
            {
                get => Collection.MinLevel;
            }
            public bool Avaliable
            {
                get => LevelsDone + 1 >= MinLevel;
            }


            public void Reset()
            {
                foreach (PuzzleItemInfo item in Items)
                {
                    item.Reset();
                }
            }
        }
        #endregion

        #region Paint

        public ColorInfo GetColorInfo(int index)
        {
            return new ColorInfo(index, paintData);
        }
        public class ColorInfo : IShopItem
        {
            private const string OPENED_KEY = "_color_opened";

            public ColorInfo(int index, PaintData paintData)
            {
                Index = index;
                ColorItem = paintData.GetColor(index);

            }

            public int Index { get; }
            public PaintData.Item ColorItem { get; }
            public ItemBase ItemUI => ColorItem;
            public bool Opened
            {
                get
                {
                    if (ColorItem.Type == PaintData.Item.Types.Base)
                    {
                        return true;
                    }

                    if(ColorItem.Type == PaintData.Item.Types.Vip)
                    {
                        return PlayerPrefs.GetInt($"{Index}{OPENED_KEY}", 0) == 1;
                    }

                    return false;
                }
            }


            public void SetOpened()
            {
                if(ColorItem.Type == PaintData.Item.Types.Vip)
                {
                    PlayerPrefs.SetInt($"{Index}{OPENED_KEY}", 1);
                }
            }
        }

        #endregion

        #region Shelf

        public static int LastShelfIndex
        {
            get => PlayerPrefs.GetInt(LAST_SHELF_INDEX_KEY, 0);
            set => PlayerPrefs.SetInt(LAST_SHELF_INDEX_KEY, value);
        }

        #endregion

        #region Cannons

        public static int CurrantCannonIndex
        {
            get => PlayerPrefs.GetInt(CURRANT_CANNON_KEY, 0);
            set => PlayerPrefs.SetInt(CURRANT_CANNON_KEY, value);
        }

        public CostInfo CannonCost(ShopTypes shopType)
        {
            List<CannonInfo> cannons = GetCannonsInfo(shopType);
            int count = cannons.Count(x => x.Opened);

            if (count >= cannons.Count)
                return null;

            return cannonsData.GetCost(shopType, count);
        }
        public CannonInfo GetCannonInfo(int index)
        {
            return new CannonInfo(index, cannonsData);
        }
        public List<CannonInfo> GetCannonsInfo(ShopTypes shopType)
        {
            List<CannonInfo> info = new List<CannonInfo>();

            bulletData
                .Where(x => x.Prefab != null && x.ShopType == shopType)
                .ToList()
                .ForEach(x => info.Add(new CannonInfo(x.Index, cannonsData)));


            return info;
        }
        public class CannonInfo : IShopItem
        {
            private const string OPENED_KEY = "_cannon_opened";

            public CannonInfo(int index, CannonsData cannonsData)
            {
                Index = index;
                CannonItem = cannonsData.GetCannon(index);

            }

            public int Index { get; }
            public CannonsData.Item CannonItem { get; }
            public ItemBase ItemUI => CannonItem;
            public bool Opened
            {
                get
                {
                    if (Index == 0)
                        return true;
                    return PlayerPrefs.GetInt($"{Index}{OPENED_KEY}", 0) == 1;
                }
            }


            public void SetOpened()
            {
                PlayerPrefs.SetInt($"{Index}{OPENED_KEY}", 1);
            }
        }

        #endregion

        #region Bullets

        public static int CurrantBulletIndex
        {
            get => PlayerPrefs.GetInt(CURRANT_BULLET_KEY, 0);
            set => PlayerPrefs.SetInt(CURRANT_BULLET_KEY, value);
        }
        public CostInfo BulletsCost(ShopTypes shopType)
        {
            List<BulletInfo> bullets = GetBulletsInfo(shopType);
            int count = bullets.Count(x => x.Opened);

            if (count >= bullets.Count)
                return null;

            return bulletData.GetCost(shopType, count);
        }
        public BulletInfo GetBulletInfo(int index)
        {
            return new BulletInfo(index, bulletData);
        }
        public List<BulletInfo> GetBulletsInfo(ShopTypes shopType)
        {
            List<BulletInfo> info = new List<BulletInfo>();

            bulletData
                .Where(x => x.Prefab != null && x.ShopType == shopType)
                .ToList()
                .ForEach(x => info.Add(new BulletInfo(x.Index, bulletData)));

            return info;
        }
        public class BulletInfo : IShopItem
        {
            private const string OPENED_KEY = "_bullets_opened";

            public BulletInfo(int index, BulletData bulletData)
            {
                Index = index;
                BulletItem = bulletData.GetBullet(index);

            }

            public int Index { get; }
            public BulletData.Item BulletItem { get; }
            public ItemBase ItemUI => BulletItem;
            public bool Opened
            {
                get
                {
                    if (Index == 0)
                        return true;
                    return PlayerPrefs.GetInt($"{Index}{OPENED_KEY}", 0) == 1;
                }
            }
            public void SetOpened()
            {
                PlayerPrefs.SetInt($"{Index}{OPENED_KEY}", 1);
            }
        }

        #endregion

        #region Internal


        [Button("Show Items")]
        private void UpdateItems()
        {
            modelItems = new List<ModelInfoVisualize>();

            modelsData.GetItems().ForEach(x =>
            {
                modelItems.Add(new ModelInfoVisualize(x, paintData));
            });
            puzzlesData.GetAllItems().ForEach(x =>
            {
                modelItems.Add(new ModelInfoVisualize(x, paintData));
            });
        }

        [Button("Open Model Group")]
        private void OpenGroup()
        {
            modelsData.GetItems(utilModelGroupOpen)
                .ForEach(x =>
                {
                    VoxelModelInfo.SetOpened(x);
                });
        }

        [System.Serializable]
        private class ModelInfoVisualize
        {
            public ModelInfoVisualize(ModelId item, PaintData paintData)
            {
                name = item.GetModel.name;
                group = item.Group;
                index = item.Index;

                VoxelModelInfo info = new VoxelModelInfo(item, paintData);

                opened = info.Opened;
                painted = info.Painted;

                materials = info.Colors;
            }

            [SerializeField] private string name;
            [SerializeField] private string group;
            [SerializeField] private int index;
            [Space]
            [SerializeField] private bool opened;
            [SerializeField] private bool painted;
            [SerializeField] private List<Material> materials;
        }

        #endregion
    }
}
