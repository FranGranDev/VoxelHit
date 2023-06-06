using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;


namespace Cannons.Bullets
{
    public class RepairBullet : RepairBulletBase
    {
        [Header("Animation")]
        [SerializeField] private float time;
        [SerializeField] private Ease ease;
        [SerializeField] private int rotationCount;
        [Header("Components")]
        [SerializeField] private new Rigidbody rigidbody;
        [Header("Effects")]
        [SerializeField] private TrailRenderer fireTrail;
        [SerializeField] private GameObject superTrail;
        [SerializeField] private ParticleSystem outerParticle;

        private Coroutine flyCoroutine;
        private bool isRedirected;

        public override void Run(RepairBulletInfo settings)
        {
            base.Run(settings);

            flyCoroutine = StartCoroutine(FlyCour());
            GetComponentsInChildren<AudioSource>()
                .ToList()
                .ForEach(x => x.volume *= settings.Volume);

            if(settings.IsSuper)
            {
                superTrail.gameObject.SetActive(true);
            }
        }
        public override void Demolish(DemolishTypes type)
        {
            if (isRedirected)
                return;
            switch(type)
            {
                case DemolishTypes.Repair:
                    if(flyCoroutine != null)
                    {
                        StopCoroutine(flyCoroutine);
                        flyCoroutine = null;

                        transform.DOScale(0, time)
                            .SetEase(ease);
                        transform.DOLocalRotate(new Vector3(0, rotationCount * 360, 0), time, RotateMode.LocalAxisAdd)
                            .SetEase(ease);
                        transform.DOLocalMoveY(transform.localPosition.y + 2, time)
                            .SetEase(ease)
                            .OnKill(() => Destroy(gameObject));
                    }
                    break;
                case DemolishTypes.OuterRepair:
                    if (flyCoroutine != null)
                    {
                        StopCoroutine(flyCoroutine);
                        flyCoroutine = null;


                        outerParticle.transform.forward = Vector3.forward;
                        outerParticle.gameObject.SetActive(true);
                        outerParticle.transform.SetParent(null);
                        outerParticle.Play();

                        transform.DOScale(0, time)
                            .SetEase(ease);
                        transform.DOLocalRotate(new Vector3(0, 0, rotationCount * 360), time, RotateMode.LocalAxisAdd)
                            .SetEase(ease)
                            .OnKill(() => Destroy(gameObject));
                    }
                    break;
                case DemolishTypes.None:
                    Destroy(gameObject);
                    break;
            }
        }
        public override void Redirect(RicochetInfo path)
        {
            if(flyCoroutine != null)
            {
                StopCoroutine(flyCoroutine);
            }
            flyCoroutine = StartCoroutine(RedirectCour(path));
            isRedirected = true;
        }

        private IEnumerator FlyCour()
        {
            var wait = new WaitForFixedUpdate();

            float distance = 0;

            while(distance < 100)
            {
                Vector3 addPosition = transform.forward * Settings.Speed * Time.fixedDeltaTime;
                transform.position += addPosition;

                distance += addPosition.magnitude;
                yield return wait;
            }
        }
        private IEnumerator RedirectCour(RicochetInfo path)
        {
            fireTrail.emitting = true;

            Vector3 startPosition = transform.position;
            Vector3 randomDirection = Random.onUnitSphere;

            float pathTime = (startPosition - path.Target).magnitude * 3.5f / Settings.Speed;
            float time = 0;

            var wait = new WaitForFixedUpdate();


            while (time < pathTime * 0.95f)
            {
                float ratio = time / pathTime;

                Vector3 height = Vector3.up * path.TopHeight * path.Curve.Evaluate(ratio);

                transform.position = Vector3.Lerp(startPosition, path.Target, ratio) + height;
                transform.Rotate(randomDirection, 30 * Random.Range(0.75f, 1.25f));

                time += Time.fixedDeltaTime;
                yield return wait;
            }


            path.OnCompleate?.Invoke();

            isRedirected = false;
            Demolish(DemolishTypes.None);
            yield break;
        }

    }
}
