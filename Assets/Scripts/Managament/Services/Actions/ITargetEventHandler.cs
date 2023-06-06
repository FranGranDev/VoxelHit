using UnityEngine;
using System;

namespace Services
{
    public interface IGameEventsHandler
    { 
        public event Action<GameStates> OnStateChanged;

        public event Action OnStarted;
        public event Action OnRestart;
        public event Action OnDone;
        public event Action OnFailed;

        public event Action<IGameEventsHandler> OnClearScene;
    }
    public interface ITargetEventHandler
    {
        public void Restart(Action onDone);

        public event Action OnDone;
        public event Action OnFailed;

        public event Action<MoneyValue> OnMoney;
    }

    public enum GameStates { Idle, Game, Done, Failed, Final }
    public enum GameTypes { Game, Paint, ShelfWatch, Painted, Shop, Puzzle, EventRoad, Break }
}