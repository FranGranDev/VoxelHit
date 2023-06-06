using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cannons.Catapults;
using Services;

namespace Cannons
{
    [RequireComponent(typeof(Catapult), typeof(IScreenInput))]
    public class CatapultController : MonoBehaviour, Initializable<GameInfo>, IBindable<IGameEventsHandler>
    {
        [Header("Values")]
        [SerializeField] private States state;
        [Header("Components")]
        [SerializeField] private Collider zone;


        private Catapult catapult;
        private GameStates gameState;

        public Vector3 Point { get; private set; }


        public void Initialize(GameInfo gameInfo)
        {
            catapult = GetComponent<Catapult>();
            catapult.SetNormal(zone.transform.up);

            IScreenInput input = GetComponent<IScreenInput>();

            input.OnTap += OnTap;
            input.OnDrag += OnDrag;
            input.OnTapEnded += OnTapEnded;
        }
        public void Bind(IGameEventsHandler events)
        {
            events.OnStateChanged += OnStateChanged;

            events.OnClearScene += (x) =>
            {
                x.OnStateChanged -= OnStateChanged;
            };
        }

        private void OnStateChanged(GameStates obj)
        {
            gameState = obj;
        }

        private void OnTap(Vector3 point, GameObject obj)
        {
            if (gameState != GameStates.Game)
                return;
            state = States.Scope;
        }
        private void OnDrag(Vector3 point, GameObject obj)
        {
            if (gameState != GameStates.Game)
                return;
            Point = zone.ClosestPoint(point);
        }
        private void OnTapEnded(Vector3 point, GameObject obj)
        {
            if (gameState != GameStates.Game)
                return;

            state = States.Idle;
            catapult.Fire(point);
        }



        private void FixedUpdate()
        {
            if(state == States.Scope)
            {
                catapult.Scope(Point);
            }
        }

        private enum States
        {
            Idle,
            Scope,
        }
    }
}
