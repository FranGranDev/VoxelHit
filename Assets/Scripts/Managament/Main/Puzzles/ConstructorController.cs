using Animations;
using Data;
using DG.Tweening;
using Puzzles;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Managament
{
    public class ConstructorController : MonoBehaviour, Initializable<GameInfo>
    {
        [Header("Settings")]
        [SerializeField] private float minDistance;
        [Header("Links")]
        [SerializeField] private PuzzleUI puzzleUI;
        [SerializeField] private PuzzleInput puzzleInput;
        [SerializeField] private PuzzleModelsData modelsData;
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
        private SavedData.PuzzleInfo constructorInfo;
        private GameInfo gameInfo;
        private List<PuzzleItem> currantItems;
        private IHaptic haptic;
        private ISoundPlayer soundPlayer;


        public event System.Action<PuzzleId, ModelId> OnSelectItem;
        public event System.Action OnExit;


        public void Initialize(GameInfo gameInfo)
        {
            this.gameInfo = gameInfo;

            haptic = gameInfo.Components.Haptic;
            soundPlayer = gameInfo.Components.SoundPlayer;

            puzzleUI.Initialize(gameInfo);
            puzzleUI.OnExit += Exit;


            puzzle = puzzlePlace.GetComponentInChildren<Puzzle>();
            puzzle.Initialize(gameInfo);
            puzzle.SetItems(modelsData.GetModels.First(x => x.Group == puzzle.PuzzleId.Group), PuzzleItem.InitTypes.Puzzle);

            puzzleInput.Initialize();


            constructorInfo = new SavedData.PuzzleInfo(puzzle.PuzzleId, modelsData);

            puzzleInput.OnDrag += CheckForCollision;
            puzzleInput.OnPlaced += ItemPlaced;
            puzzleInput.OnDrop += ItemDropped;

            CreateItems();

            puzzleUI.State = PuzzleUI.States.Install;
        }


        private void ItemPlaced(PuzzleItem obj)
        {
            constructorInfo.Items
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
                    return vector.magnitude < minDistance;
                })
                .OrderBy(x => (x.Place.position - obj.transform.position))
                .FirstOrDefault();
            if (place)
            {
                if (place.Index != obj.Index)
                {
                    soundPlayer.PlaySound("denied", 0.5f);

                    Vector3 point = pointsPlace.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
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
            if (place)
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
                StartCoroutine(FinalCour());
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
            List<ModelId> models = constructorInfo.Items
                .Where(x => x.Opened && !x.Placed)
                .ToList()
                .ConvertAll(x => x.ModelId);
            currantItems = new List<PuzzleItem>();

            float offsetY = 0;

            foreach (ModelId model in models)
            {
                int rand = Random.Range(0, points.Count);
                Vector3 offset = new Vector3(Random.Range(-2, 2), offsetY, Random.Range(-2, 2));
                Vector3 position = pointsPlace.position + points[rand] + offset;
                Quaternion rotation = Quaternion.Euler(90, 0, Random.Range(-25, 25));
                points.RemoveAt(rand);

                PuzzleItem item = Instantiate(model.GetModel, position, rotation, itemsPlace).AddComponent<PuzzleItem>();

                item.Initialize(gameInfo, PuzzleItem.InitTypes.Puzzle);
                item.SetOffset(offsetY);

                currantItems.Add(item);

                offsetY += 1.5f;
            }
        }

        private IEnumerator FinalCour()
        {
            puzzleUI.State = PuzzleUI.States.None;

            yield return new WaitForSeconds(0.5f);

            soundPlayer.PlaySound("win", 0.75f);

            radial.Play();
            particles.ForEach(x => x.Play());

            puzzle.HideBackground();

            puzzle.transform.DOLocalRotate(Vector3.up * 360, 2f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(1f);

            puzzleUI.State = PuzzleUI.States.Done;


            yield break;
        }


        private void Exit()
        {
            OnExit?.Invoke();
        }
    }
}