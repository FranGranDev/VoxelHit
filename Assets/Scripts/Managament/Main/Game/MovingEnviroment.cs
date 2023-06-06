using System.Collections;
using System.Collections.Generic;
using Animations;
using Services;
using UnityEngine;

namespace Managament
{
    public class MovingEnviroment : MonoBehaviour, Initializable<GameInfo>
    {
        [Header("Animation")]
        [SerializeField] private new AnimationData animation;
        [Header("Points")]
        [SerializeField] private Transform movingPoint;
        [Space]
        [SerializeField] private Transform idlePoint;
        [SerializeField] private Transform gamePoint;
        [SerializeField] private Transform failedPoint;
        [SerializeField] private Transform compleatePoint;

        private Dictionary<GameStates, Transform> pointDict;


        public void Initialize(GameInfo info)
        {
            pointDict = new Dictionary<GameStates, Transform>()
            {
                { GameStates.Idle, gamePoint},
                { GameStates.Game, gamePoint},
                { GameStates.Failed, gamePoint},
                { GameStates.Done, compleatePoint},
                { GameStates.Final, compleatePoint},
            };
        }

        public void MoveToPoint(GameStates from, GameStates to, System.Action onCompleate)
        {
            if(pointDict[from] == pointDict[to])
            {
                onCompleate?.Invoke();
                return;
            }
            StartCoroutine(MoveCour(pointDict[from], pointDict[to], onCompleate));
        }

        public IEnumerator MoveCour(Transform prev, Transform next, System.Action onCompleate)
        {
            yield return new WaitForSeconds(animation.Delay);

            var wait = new WaitForFixedUpdate();

            float time = 0;
            while (time < animation.Time)
            {
                float ratio = time / animation.Time;

                movingPoint.localPosition = Vector3.Lerp(prev.localPosition, next.localPosition, animation.AnimationCurves[0].Evaluate(ratio));
                movingPoint.localRotation = Quaternion.Lerp(prev.localRotation, next.localRotation, animation.AnimationCurves[1].Evaluate(ratio));

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            movingPoint.localPosition = next.localPosition;
            movingPoint.localRotation = next.localRotation;

            onCompleate?.Invoke();

            yield break;
        }
    }
}
