using UnityEngine;
using Services;
using DG.Tweening;


namespace Traps
{
    public class TrapPart : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Min(0)] private float power;
        [SerializeField, Min(0)] private float angularPower;
        [Space]
        [SerializeField, Range(0, 1)] private float centerRatio;
        [SerializeField, Range(0, 1)] private float velocityRatio;
        [SerializeField, Range(0, 1)] private float upRatio;
        [SerializeField, Range(0, 1)] private float randomizeRatio;

        private ISoundPlayer soundPlayer;
        private GameObject origin;

        private Vector3 prevPosition;
        private Vector3 velocity;
        private GameInfo gameInfo;
        private bool demolished;
        private bool playHit;


        public void Initialize(GameInfo gameInfo)
        {
            this.gameInfo = gameInfo;
            soundPlayer = gameInfo.Components.SoundPlayer;

            origin = gameObject;
            if (transform.childCount == 1)
            {
                origin = transform.GetChild(0).gameObject;
            }
        }
        public void Demolish(bool playSound = true)
        {
            if (demolished || !gameObject.activeInHierarchy || !gameObject.activeSelf)
                return;
            demolished = true;


            origin.transform.SetParent(gameInfo.SceneContext.LevelTransform);

            Rigidbody rigidbody = origin.AddComponent<Rigidbody>();
            Vector3 direction = (origin.transform.position - transform.position).normalized;

            Vector3 force = (direction * centerRatio + velocity.normalized * velocityRatio + Vector3.up * upRatio + Random.onUnitSphere * randomizeRatio).normalized * power;
            Vector3 angular = Random.onUnitSphere * angularPower;

            rigidbody.AddForce(force, ForceMode.VelocityChange);
            rigidbody.AddTorque(angular, ForceMode.VelocityChange);

            if (playSound)
            {
                soundPlayer.PlaySound("trap_break");
            }
        }
        public void PlayHit()
        {
            if (playHit)
                return;
            playHit = true;

            Vector3 prev = origin.transform.localRotation.eulerAngles;
            origin.transform.DOShakeRotation(0.5f, 30, 8, 90)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    origin.transform.DOLocalRotate(prev, 0.5f)
                        .SetEase(Ease.InOutSine);
                    playHit = false;
                });
        }

        private void FixedUpdate()
        {
            velocity = origin.transform.position - prevPosition;
            prevPosition = origin.transform.position;
        }
    }
}
