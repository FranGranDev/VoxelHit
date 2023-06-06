using System.Collections;
using System.Collections.Generic;
using Animations;
using Services;
using UnityEngine;


namespace Managament
{
    public class GameCamera : MonoBehaviour, Initializable<GameInfo>, IBindable<IGameEventsHandler>
    {
        [Header("Link")]
        [SerializeField] private MovingEnviroment movingEnviroment;

        private GameStates gameState;


        public void Initialize(GameInfo info)
        {
            gameState = GameStates.Idle;
        }
        public void Bind(IGameEventsHandler eventsHandler)
        {
            eventsHandler.OnStateChanged += MoveToPoint;

            eventsHandler.OnClearScene += (x) =>
            {
                eventsHandler.OnStateChanged -= MoveToPoint;
            };
        }

        private void MoveToPoint(GameStates state)
        {
            if (this == null)
                return;

            movingEnviroment?.MoveToPoint(gameState, state, null);
            
            gameState = state;
        }
    }
}
