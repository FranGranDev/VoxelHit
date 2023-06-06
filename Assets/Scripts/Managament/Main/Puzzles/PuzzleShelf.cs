using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Puzzles;
using Data;
using UI.Items;

namespace Managament
{
    public class PuzzleShelf : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform puzzlePlace;
        [SerializeField] private Canvas canvas;
        [Header("Buttons")]
        [SerializeField] private ButtonUI startButton;
        [SerializeField] private ButtonUI continueButton;
        [SerializeField] private ButtonUI restartButton;
        [SerializeField] private GameObject closedButton;
        [Header("UI")]
        [SerializeField] private ChangeableLevel changeableLevel;
        
        private States State
        {
            set
            {
                switch(value)
                {
                    case States.Start:
                        startButton.gameObject.SetActive(true);
                        restartButton.gameObject.SetActive(false);
                        continueButton.gameObject.SetActive(false);
                        closedButton.gameObject.SetActive(false);
                        break;
                    case States.Restart:
                        startButton.gameObject.SetActive(false);
                        restartButton.gameObject.SetActive(true);
                        continueButton.gameObject.SetActive(false);
                        closedButton.gameObject.SetActive(false);
                        break;
                    case States.Continue:
                        startButton.gameObject.SetActive(false);
                        restartButton.gameObject.SetActive(false);
                        continueButton.gameObject.SetActive(true);
                        closedButton.gameObject.SetActive(false);
                        break;
                    case States.Closed:
                        startButton.gameObject.SetActive(false);
                        restartButton.gameObject.SetActive(false);
                        continueButton.gameObject.SetActive(false);
                        closedButton.gameObject.SetActive(true);
                        break;
                }
            }
        }

        public Puzzle Puzzle { get; private set; }

        public event System.Action<PuzzleShelf> OnStartClick;
        public event System.Action<PuzzleShelf> OnRestartClick;


        public void Initialize(GameInfo gameInfo, PuzzleModelsData.Collection puzzle, SavedData.PuzzleInfo puzzleInfo)
        {            
            Puzzle = Instantiate(puzzle.PuzzleId, puzzlePlace.position, Quaternion.identity, puzzlePlace).GetComponent<Puzzle>();
            Puzzle.Initialize(gameInfo);
            Puzzle.SetItems(puzzle);

            canvas.worldCamera = Camera.main;

            startButton.OnClick += StartClick;
            continueButton.OnClick += StartClick;
            restartButton.OnClick += RestartClick;

            changeableLevel.SetValue(new LevelNumber(puzzle.MinLevel));


            if(SavedData.LevelsDone < puzzle.MinLevel - 1)
            {
                State = States.Closed;
            }
            else if(puzzleInfo.Done)
            {
                State = States.Restart;
            }
            else if(puzzleInfo.Items.Count(x => x.Opened) > 0)
            {
                State = States.Continue;
            }
            else
            {
                State = States.Start;
            }
        }


        private void StartClick()
        {
            OnStartClick?.Invoke(this);
        }
        private void RestartClick()
        {
            OnRestartClick?.Invoke(this);
        }

        private enum States
        {
            Start,
            Continue,
            Restart,
            Closed
        }
    }
}
