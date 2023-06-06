using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Puzzles;
using Services;
using Data;
using UI;
using System.Collections;

namespace Managament
{
    public class PuzzleShelfController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float scrollSpeed;
        [SerializeField] private float offsetY;
        [SerializeField] private float minDrag;
        [Header("Links")]
        [SerializeField] private SavedData savedData;
        [SerializeField] private PuzzleModelsData puzzleData;
        [SerializeField] private PuzzleShelfUI puzzleUI;
        [Header("Prefabs")]
        [SerializeField] private PuzzleShelf shelfPrefab;
        [Header("Components")]
        [SerializeField] private new Transform camera;
        [SerializeField] private Transform shelfContainer;
        [Header("Values")]
        [SerializeField] private float currantOffset;
        [SerializeField] private float maxOffset;
        [SerializeField] private float minOffset;

        private GameInfo gameInfo;
        private ISoundPlayer soundPlayer;
        private IAdsController adsController;
        private IHaptic haptic;


        public List<PuzzleShelf> PuzzleShelfs { get; private set; }

        public event System.Action<PuzzleId> OnPlayPuzzle;
        public event System.Action OnExit;

        public void Initialize(GameInfo gameInfo)
        {
            this.gameInfo = gameInfo;
            soundPlayer = gameInfo.Components.SoundPlayer;
            adsController = gameInfo.Components.Ads;
            haptic = gameInfo.Components.Haptic;

            puzzleUI.Initialize(gameInfo);
            puzzleUI.OnExit += Exit;

            StartCoroutine(CreateShelfsCour());

            GetComponentInChildren<IDragInput>().OnDrag += Drag;


            GetComponentsInChildren<Initializable<ISoundPlayer>>(true)
                .ToList()
                .ForEach(x => x.Initialize(soundPlayer));

            GetComponentsInChildren<Initializable<IAdsController>>(true)
                .ToList()
                .ForEach(x => x.Initialize(adsController));


            maxOffset = 0;
            minOffset = -(PuzzleShelfs.Count - 1) * offsetY;
        }



        private IEnumerator CreateShelfsCour()
        {
            float offset = 0f;
            int index = 0;

            maxOffset = 0;
            minOffset = offsetY;

            PuzzleShelfs = new List<PuzzleShelf>();

            foreach (PuzzleModelsData.Collection puzzle in puzzleData.GetModels)
            {
                PuzzleShelfs.Add(CreateShelf(puzzle, offset));

                offset -= offsetY;


                minOffset -= offsetY;
                if (index > 2)
                {
                    yield return new WaitForSeconds(0.5f);
                }
                index++;
            }

            yield break;
        }
        private void CreateShelfs()
        {
            float offset = 0f;


            PuzzleShelfs = new List<PuzzleShelf>();

            foreach (PuzzleModelsData.Collection puzzle in puzzleData.GetModels)
            {
                PuzzleShelfs.Add(CreateShelf(puzzle, offset));

                offset -= offsetY;
            }

            maxOffset = 0;
            minOffset = -(PuzzleShelfs.Count - 1) * offsetY;
        }
        private PuzzleShelf CreateShelf(PuzzleModelsData.Collection puzzle, float offset)
        {
            Vector3 position = new Vector3(0, offset, 0);

            PuzzleShelf shelf = Instantiate(shelfPrefab, position, Quaternion.identity, shelfContainer);

            SavedData.PuzzleInfo info = new SavedData.PuzzleInfo(puzzle.PuzzleId, puzzleData);
            shelf.Initialize(gameInfo, puzzle, info);

            shelf.OnStartClick += PlayPuzzle;
            shelf.OnRestartClick += RestartPuzzle;

            return shelf;
        }
        private void UpdateVisible()
        {
            foreach(PuzzleShelf shelf in PuzzleShelfs)
            {
                bool enabled = shelf.transform.position.y + offsetY * 2 > currantOffset && shelf.transform.position.y - offsetY * 2 < currantOffset;
                shelf.gameObject.SetActive(enabled);
            }
        }

        private void PlayPuzzle(PuzzleShelf obj)
        {
            OnPlayPuzzle?.Invoke(obj.Puzzle.PuzzleId);
        }
        private void RestartPuzzle(PuzzleShelf obj)
        {
            //ads => restart

            SavedData.PuzzleInfo info = new SavedData.PuzzleInfo(obj.Puzzle.PuzzleId, puzzleData);
            info.Reset();

            OnPlayPuzzle?.Invoke(obj.Puzzle.PuzzleId);
        }
        private void Exit()
        {
            OnExit?.Invoke();
        }

        private void Drag(Vector2 obj)
        {
            float drag = obj.y;
            if (Mathf.Abs(drag) < minDrag)
                return;

            currantOffset -= drag * Time.deltaTime * scrollSpeed;
        }

        private void FixedUpdate()
        {
            UpdateVisible();

            currantOffset = Mathf.Lerp(currantOffset, Mathf.Clamp(currantOffset, minOffset, maxOffset), 0.25f);

            Vector3 position = new Vector3(camera.position.x, currantOffset, camera.position.z);
            camera.position = Vector3.Lerp(camera.position, position, 0.25f);
        }
    }
}
