using Services;
using UnityEngine;
using DG.Tweening;
using Cannons;

namespace EventRoad
{
    public class MovePoint : MonoBehaviour, Initializable<GameInfo>
    {
        [Header("Settings")]
        [SerializeField] private float moveTime = 1f;
        [SerializeField] private float rotateTime = 0.25f;
        [SerializeField] private Transform movePoint;


        public Transform Point => movePoint;


        public void Initialize(GameInfo info)
        {
            movePoint = GetComponentInChildren<Cannon>().Point;
        }

        public void MoveToPoint(RoadPoint point, float delay, System.Action onStart, System.Action onDone)
        {
            movePoint.DOJump(point.position, 5, 1, moveTime)
                .SetDelay(delay)
                .OnStart(() => onStart?.Invoke())
                .SetEase(Ease.OutSine)
                .SetUpdate(UpdateType.Fixed)
                .OnKill(() =>
                {
                    if (point.NextPoint)
                    {
                        Vector3 direction = (point.NextPoint.position - movePoint.position).normalized;
                        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                        movePoint.DORotate(rotation.eulerAngles, rotateTime)
                            .SetEase(Ease.InOutSine)
                            .SetUpdate(UpdateType.Fixed);

                    }
                    onDone?.Invoke();
                });       
        }
        public void SetPoint(RoadPoint point)
        {
            transform.position = point.position;
            movePoint.position = point.position;

            if(point.NextPoint)
            {
                movePoint.forward = (point.NextPoint.position - movePoint.position).normalized;
            }
        }

        public void Dance()
        {
            movePoint.DOPunchPosition(Vector3.up * 2, 0.5f, 5)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1);
            movePoint.DOLocalRotate(Vector3.up * 360, 4f, RotateMode.WorldAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }

    }
}
