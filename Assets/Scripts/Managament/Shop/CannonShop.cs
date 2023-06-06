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
    public class CannonShop : MonoBehaviour, IShopAnimation
    {
        [Foldout("Animations"), SerializeField] private float moveTime;
        [Foldout("Animations"), SerializeField] private AnimationCurve moveCurve;
        [Foldout("Animations"), SerializeField] private AnimationCurve flyCurve;
        [Foldout("Animations"), SerializeField] private float topHeight;
        [Space]
        [Foldout("Animations"), SerializeField] private Transform rightPoint;
        [Foldout("Animations"), SerializeField] private Transform centerPoint;
        [Foldout("Animations"), SerializeField] private Transform leftPoint;


        private CannonAnimation currant;
        private List<Tween> idleTweens = new List<Tween>();


        public ShopItemsTypes ShopType
        {
            get => ShopItemsTypes.Cannons;
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

            currant = Instantiate(prefab, rightPoint.position, rightPoint.rotation, transform).GetComponent<CannonAnimation>();
            currant.Main.position = rightPoint.position;

            currant.Main.DOLocalRotate(Vector3.up * 180, moveTime, RotateMode.LocalAxisAdd);


            while (time < moveTime)
            {
                float ratio = moveCurve.Evaluate(time / moveTime);
                float height = flyCurve.Evaluate(ratio) * topHeight;

                currant.Main.position = Vector3.Lerp(rightPoint.position, centerPoint.position, ratio) + Vector3.up * height;

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            yield break;
        }
        private IEnumerator RemoveCour()
        {
            float time = 0;
            var wait = new WaitForFixedUpdate();

            CannonAnimation prev = currant;
            prev.Main.DOLocalRotate(Vector3.up * -180, moveTime, RotateMode.LocalAxisAdd);

            Vector3 prevPosition = prev.Main.position;

            while (time < moveTime)
            {
                float ratio = moveCurve.Evaluate(time / moveTime);
                float height = flyCurve.Evaluate(ratio) * topHeight;

                prev.Main.position = Vector3.Lerp(prevPosition, leftPoint.position, ratio) + Vector3.up * height;

                time += Time.fixedDeltaTime;
                yield return wait;
            }

            Destroy(prev.gameObject);
        }

        private void StartAnimation()
        {
            if(idleTweens.Count > 0)
            {
                foreach(Tween tween in idleTweens)
                {
                    tween.Kill();
                }
                idleTweens.Clear();
            }
            if (currant == null)
                return;
            Tween jump = currant.Main.DOPunchPosition(Vector3.up * Random.Range(1f, 2f), 1, Random.Range(2, 4))
                .SetEase(Ease.InOutSine)
                .SetDelay(3)
                .OnComplete(() => StartAnimation());



            idleTweens.Add(jump);
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