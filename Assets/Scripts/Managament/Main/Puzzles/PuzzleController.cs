using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Data;
using Puzzles;
using Services;
using UI;
using Voxel;
using DG.Tweening;
using Animations;


namespace Managament
{
    public class PuzzleController : MonoBehaviour, Initializable<GameInfo>
    {
        [Header("State")]
        [SerializeField] private States state;
        [Header("Settings")]
        [SerializeField] private float minDistance;
        [Header("Links")]
        [SerializeField] private PuzzleUI puzzleUI;
        [SerializeField] private PuzzleInput puzzleInput;
        [SerializeField] private PuzzleModelsData puzzleData;
        [Header("Components")]
        [SerializeField] private List<ParticleSystem> particles;
        [SerializeField] private ScaleReturnAnimation radial;
        [Header("Placement")]
        [SerializeField] private Transform puzzlePlace;
        [SerializeField] private Transform itemsPlace;
        [Header("Points")]
        [SerializeField] private Vector3 pointsOffset;
        [SerializeField] private Transform pointsPlace;
        

        private Puzzle puzzle;
        private SavedData.PuzzleInfo puzzleInfo;
        private GameInfo gameInfo;
        private List<PuzzleItem> currantItems;
        private IHaptic haptic;
        private ISoundPlayer soundPlayer;


        public States State
        {
            get => state;
            set
            {
                if (state == value)
                    return;
                switch(state)
                {
                    case States.Select:
                        puzzleInput.OnSelect -= SelectPuzzleItem;
                        break;
                    case States.Install:
                        puzzleInput.OnDrag -= CheckForCollision;
                        puzzleInput.OnPlaced -= ItemPlaced;
                        puzzleInput.OnDrop -= ItemDropped;
                        break;
                }
                state = value;
                switch(state)
                {
                    case States.Select:
                        puzzleInput.OnSelect += SelectPuzzleItem;

                        StartCoroutine(SelectHintCour());
                        puzzleUI.State = PuzzleUI.States.Select;
                        break;
                    case States.Install:
                        puzzleInput.OnDrag += CheckForCollision;
                        puzzleInput.OnPlaced += ItemPlaced;
                        puzzleInput.OnDrop += ItemDropped;
                        CreateItems();

                        StartCoroutine(InstallHintCour());
                        puzzleUI.State = PuzzleUI.States.Install;
                        break;
                    case States.Done:
                        StartCoroutine(FinalCour());
                        break;
                }
            }
        }


        public event System.Action<PuzzleId, ModelId> OnSelectItem;
        public event System.Action OnExit;


        public void Initialize(GameInfo gameInfo)
        {
            
            this.gameInfo = gameInfo;

            haptic = gameInfo.Components.Haptic;
            soundPlayer = gameInfo.Components.SoundPlayer;

            puzzleUI.Initialize(gameInfo);
            puzzleUI.OnExit += Exit;
            puzzleUI.OnSelectRandom += SelectRandom;

            puzzle = puzzlePlace.GetComponentInChildren<Puzzle>();
            puzzle.Initialize(gameInfo);
            puzzle.SetItems(puzzleData.GetModels.First(x => x.Group == puzzle.PuzzleId.Group), PuzzleItem.InitTypes.Puzzle);

            puzzleInput.Initialize();


            puzzleInfo = new SavedData.PuzzleInfo(puzzle.PuzzleId, puzzleData);
            if(puzzleInfo.Items.Count(x => x.Opened && !x.Placed) > 0)
            {
                State = States.Install;
            }
            else if(puzzleInfo.Items.Count(x => !x.Opened) > 0)
            {
                State = States.Select;
            }
            else
            {
                State = States.Done;
            }
        }

        private void SelectRandom()
        {
            List<PuzzlePlace> places = puzzle.Items
                .Where(x => x.Item == null)
                .ToList();

            SelectPuzzleItem(places[Random.Range(0, places.Count)]);
        }
        private void SelectPuzzleItem(PuzzlePlace obj)
        {
            if (obj.Item != null)
                return;

            ModelId modelId = puzzleData
                            .GetModels.First(x => x.PuzzleId.Group == puzzle.PuzzleId.Group)
                            .Items.First(x => x.Index == obj.Index).Model;

            OnSelectItem?.Invoke(puzzle.PuzzleId, modelId);
        }


        private void ItemPlaced(PuzzleItem obj)
        {
            puzzleInfo.Items
                .First(x => x.Index == obj.Index)
                .SetPlaced();
        }
        private void ItemDropped(PuzzleItem obj)
        {
            PuzzlePlace place = puzzle.Items
                .Where(x =>
                {
                    Vector3 vector = (obj.transform.position - x.transform.position);
                    vector.y = 0;
                    return vector.magnitude < minDistance * 2;
                })
                .OrderBy(x => (x.Place.position - obj.transform.position).magnitude)
                .FirstOrDefault();
            if(place)
            {
                if(place.Index != obj.Index)
                {
                    soundPlayer.PlaySound("denied", 0.5f);

                    Vector3 point = pointsPlace.position +  new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
                    obj.FailInstall(point);
                }
            }
            else
            {
                soundPlayer.PlaySound("trap_hide", 0.5f);
            }
        }
        private void CheckForCollision(PuzzleItem obj)
        {
            PuzzlePlace place = puzzle.Items.FirstOrDefault(x =>
            {
                Vector3 vector = (obj.transform.position - x.transform.position);
                vector.y = 0;
                return x.Index == obj.Index && vector.magnitude < minDistance;
            });
            if(place)
            {
                place.PlaceItem(obj);
                puzzleInput.OnItemPlaced();
                haptic.VibrateHaptic();
                soundPlayer.PlaySound("trap_show", 0.75f);

                CheckForEnd();
            }
        }
        private void CheckForEnd()
        {
            if (currantItems.Count(x => !x.Placed) == 0)
            {
                if (puzzleInfo.Items.Count(x => !x.Placed) == 0)
                {
                    State = States.Done;
                }
                else
                {
                    State = States.Select;
                }
            }
        }
        private void CreateItems()
        {
            List<Vector3> points = new List<Vector3>();

            for (float x = 0; x < 3; x++)
            {
                for (float y = 0; y < 3; y++)
                {
                    points.Add(new Vector3((x - 1) * pointsOffset.x, 0, (y - 1) * pointsOffset.y));
                }
            }
            List<ModelId> models = puzzleInfo.Items
                .Where(x => x.Opened && !x.Placed)
                .ToList()
                .ConvertAll(x => x.ModelId);
            currantItems = new List<PuzzleItem>();

            foreach (ModelId model in models)
            {
                int rand = Random.Range(0, points.Count);
                Vector3 offset = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
                Vector3 position = pointsPlace.position + points[rand] + offset;
                Quaternion rotation = Quaternion.Euler(90, 0, Random.Range(-25, 25));
                points.RemoveAt(rand);

                PuzzleItem item = Instantiate(model.GetModel, position, rotation, itemsPlace).AddComponent<PuzzleItem>();

                item.Initialize(gameInfo, PuzzleItem.InitTypes.Puzzle);

                currantItems.Add(item);
            }
        }


        private IEnumerator SelectHintCour()
        {
            while (State == States.Select)
            {
                yield return new WaitForSeconds(1f);

                List<PuzzlePlace> places = puzzle.Items
                    .Where(x => x.Item == null)
                    .ToList();
                Extentions.Shuffle(new System.Random(Time.frameCount), places);

                foreach(PuzzlePlace place in places)
                {
                    place.PlaySelectHintAnimation();

                    yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
                }
            }

            yield break;
        }
        private IEnumerator InstallHintCour()
        {
            if (currantItems.Count <= 0)
                yield break;
            PuzzlePlace place = puzzle.Items.First(x => x.Index == currantItems.First().Index);

            while (State == States.Install)
            {
                yield return new WaitForSeconds(2f);

                place.PlayInstallHintAnimation();
            }

            yield break;
        }
        private IEnumerator FinalCour()
        {
            puzzleUI.State = PuzzleUI.States.None;

            yield return new WaitForSeconds(0.5f);

            soundPlayer.PlaySound("win", 0.75f);

            radial.Play();
            particles.ForEach(x => x.Play());

            puzzle.HideBackground();

            puzzle.transform.DOLocalRotate(Vector3.up * 330, 2f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            puzzleUI.State = PuzzleUI.States.Done;


            yield break;
        }


        private void Exit()
        {
            OnExit?.Invoke();
        }


        public enum States
        {
            None,
            Select,
            Install,
            Done,
        }
    }
}
