using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cannons.Catapults;
using DG.Tweening;

namespace Cannons.Bullets
{
    public class BreakBullet : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;
        [Header("Effects")]
        [SerializeField] private List<ParticleSystem> particles;
        [SerializeField] private TrailRenderer trail;


        private Coroutine coroutine;
        private bool destroyed;
        private bool hitted;
        private Vector3 velocity;

        public BreakBulletInfo Info { get; private set; }
        public bool Destroyed => destroyed;
        

        public void Run(BreakBulletInfo info)
        {
            Info = info;
            Info.Path.StartPoint = transform.position;

            coroutine = StartCoroutine(RunCour());
            PlayEffects();
        }
        private IEnumerator RunCour()
        {
            float flyTime = Info.Path.GetDistance() / Info.Speed;
            float time = 0f;
            var wait = new WaitForFixedUpdate();

            Vector3 prevPosition = transform.position;


            while(time < flyTime)
            {
                float ratio = time / flyTime;

                Vector3 point = Info.Path.GetPoint(ratio);
                Vector3 vector = (point - prevPosition).normalized;
                prevPosition = point;
                velocity = vector / Time.fixedDeltaTime;

                if (vector.magnitude > 0)
                {
                    transform.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
                }
                transform.position = point;


                time += Time.fixedDeltaTime;
                yield return wait;
            }


            Demolish(BreakTypes.Miss);
            yield break;
        }
        private void PlayEffects()
        {
            foreach(ParticleSystem particle in particles)
            {
                particle.Play();
            }

            trail.emitting = true;
        }

        public void Demolish(BreakTypes breakType)
        {
            if (destroyed)
                return;
            StopCoroutine(coroutine);

            switch(breakType)
            {
                case BreakTypes.Instant:
                    Destroy(gameObject);
                    break;
                case BreakTypes.Miss:
                    ToDynamic();
                    Destroy(gameObject, 10);
                    break;
                case BreakTypes.Hit:
                    Richochet();
                    Destroy(gameObject, 10);
                    break;
            }
            destroyed = true;
        }


        public void PlaySpawn()
        {
            Vector3 scale = transform.localScale;
            transform.localScale = Vector3.zero;

            transform.DOScale(scale, 0.25f)
                .SetEase(Ease.InOutSine);
        }
        public void ToDynamic()
        {
            rigidbody.isKinematic = false;
            rigidbody.velocity = velocity * 0.55f;
        }
        public void Richochet()
        {
            transform.DOPunchScale(-Vector3.one * 0.4f, 1f, 7)
                .SetEase(Ease.InOutSine);

            rigidbody.isKinematic = false;
            Vector3 random = Random.onUnitSphere;
            random.z = 0;
            Vector3 direction = (velocity.normalized + random.normalized + Vector3.up).normalized;

            rigidbody.velocity = direction * velocity.magnitude * 0.25f;
            rigidbody.AddTorque(Random.onUnitSphere * 270, ForceMode.VelocityChange);

            hitted = true;
        }

        private void FixedUpdate()
        {
            if (!rigidbody.isKinematic && !hitted)
            {
                transform.rotation = Quaternion.LookRotation(rigidbody.velocity.normalized, Vector3.up);
            }
        }

        public enum BreakTypes
        {
            Miss,
            Hit,
            Instant,
        }
    }
    public class BreakBulletInfo
    {
        public BreakBulletInfo(BreakBullet bullet, Path path, float speed, float power, float radius)
        {
            Bullet = bullet;
            Path = path;
            Speed = speed;
            Power = power;
            Radius = radius;
        }

        public BreakBullet Bullet { get; }
        public Path Path { get; }
        public float Speed { get; }
        public float Power { get; }
        public float Radius { get; }
    }
}
