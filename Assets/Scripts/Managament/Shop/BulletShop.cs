using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cannons.Animations;
using Services;
using Data;
using DG.Tweening;
using NaughtyAttributes;

namespace Shop
{
    public class BulletShop : MonoBehaviour, IShopAnimation
    {
        [Foldout("Animations"), SerializeField] private float moveTime;
        [Foldout("Animations"), SerializeField] private AnimationCurve moveCurve;
        [Foldout("Animations"), SerializeField] private AnimationCurve flyCurve;
        [Foldout("Animations"), SerializeField] private AnimationCurve scaleCurve;
        [Foldout("Animations"), SerializeField] private float topHeight;
        [Space]
        [Foldout("Animations"), SerializeField] private Transform rightPoint;
        [Foldout("Animations"), SerializeField] private Transform centerPoint;
        [Foldout("Animations"), SerializeField] private Transform leftPoint;


        private GameObject currant;
        private List<Tween> idleTweens = new List<Tween>();


        public ShopItemsTypes ShopType
        {
            get => ShopItemsTypes.Bullets;
        }


        public void Create(GameObject prefab)
        {
            StartCoroutine(CreateCour(prefab));
            StartAnimation();
        }
        public void Remove()
        {
            if (!currant)
                return;

            StartCoroutine(RemoveCour());
            StopAnimation();
        }



        private IEnumerator CreateCour(GameObject prefab)
        {
            float time = 0;
            var wait = new WaitForFixedUpdate();

            currant = Instantiate(prefab, rightPoint.position, rightPoint.rotation, transform);
            currant.transform.position = rightPoint.position;

            currant.transform.DOLocalRotate(Vector3.up * 180, moveTime, RotateMode.LocalAxisAdd);


            while (time < moveTime)
            {
                float ratio = moveCurve.Evaluate(time / moveTime);
                float height = flyCurve.Evaluate(ratio) * topHeight;

                currant.transform.position = Vector3.Lerp(rightPoint.position, centerPoint.position, ratio) + Vector3.up * height;

                currant.transform.localScale = new Vector3(1, scaleCurve.Evaluate(time / moveTime), 1);

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            yield break;
        }
        private IEnumerator RemoveCour()
        {
            float time = 0;
            var wait = new WaitForFixedUpdate();

            GameObject prev = currant;
            prev.transform.DOLocalRotate(Vector3.up * -180, moveTime, RotateMode.LocalAxisAdd);

            Vector3 prevPosition = prev.transform.position;

            while (time < moveTime)
            {
                float ratio = moveCurve.Evaluate(time / moveTime);
                float height = flyCurve.Evaluate(ratio) * topHeight;

                prev.transform.position = Vector3.Lerp(prevPosition, leftPoint.position, ratio) + Vector3.up * height;

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            Destroy(prev.gameObject);
        }

        private void StartAnimation()
        {
            if (currant == null)
                return;
            if (idleTweens.Count > 0)
            {
                foreach (Tween tween in idleTweens)
                {
                    tween.Kill();
                }
                idleTweens.Clear();
            }
            Tween turn = currant.transform.DOLocalRotate(Vector3.up * 360, 7, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1)
                .SetDelay(moveTime + 0.25f);

            idleTweens.Add(turn);
        }
        private void StopAnimation()
        {
            if (currant == null)
                return;
            foreach (Tween tween in idleTweens)
            {
                tween.Kill();
            }
            idleTweens.Clear();
        }
    }
}