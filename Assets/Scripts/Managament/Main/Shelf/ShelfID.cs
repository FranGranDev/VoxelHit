using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;


namespace Managament.Shelfs
{
    public class ShelfID : MonoBehaviour
    {       
        [Foldout("Opened Animation"), SerializeField] private float delay;
        [Space]
        [Foldout("Opened Animation"), SerializeField] private float scaleTime;
        [Foldout("Opened Animation"), SerializeField] private Ease scaleEase;
        [Space]
        [Foldout("Opened Animation"), SerializeField] private float rotationTime;
        [Foldout("Opened Animation"), SerializeField] private Ease rotationEase;
        [Space]
        [Foldout("Opened Animation"), SerializeField] private Vector3 shineScale;
        [Foldout("Opened Animation"), SerializeField] private float shineScaleTime;
        [Foldout("Opened Animation"), SerializeField] private Ease shineScaleEase;

        [Foldout("Painted Aniamtion"), SerializeField] private float paintedDelay;
        [Foldout("Painted Aniamtion"), SerializeField] private float paintedRotationTime;

        [Foldout("All Collected Animation"), SerializeField] private float allCollectedTime;
        [Foldout("All Collected Animation"), SerializeField] private float allCollectedJump;
        [Foldout("All Collected Animation"), SerializeField] private Ease allCollectedJumpEase;
        [Foldout("All Collected Animation"), SerializeField] private int allCollectedVibro;


        [Header("Components")]
        [SerializeField] private Transform modelPlace;
        [SerializeField] private Transform modelParent;
        [SerializeField] private Transform radialShine;


        public Transform ModelParent
        {
            get => modelParent;
        }
        public Shelf.Item Item
        {
            get; private set;
        }

        public void Initialize(Shelf.Item item)
        {
            Item = item;
        }

        public void PlayOpened(System.Action<Shelf.Item> onDone)
        {
            modelPlace.transform.localScale = Vector3.zero;

            modelPlace.DOScale(Vector3.one, scaleTime)
                .SetDelay(delay)
                .SetEase(scaleEase)
                .OnComplete(() =>
                {
                    modelPlace.DORotate(Vector3.up * 360, rotationTime, RotateMode.LocalAxisAdd)
                        .SetEase(rotationEase)
                        .OnComplete(() =>
                        {
                            onDone?.Invoke(Item);
                        });
                });
        }
        public void PlayPainted()
        {
            modelPlace.DORotate(Vector3.up * 360, paintedRotationTime, RotateMode.LocalAxisAdd)
                .SetDelay(paintedDelay)
                .SetEase(rotationEase);
            radialShine.DOScale(shineScale * 0.75f, shineScaleTime)
                .SetDelay(paintedDelay * 0.5f)
                .SetEase(shineScaleEase)
                .OnComplete(() =>
                {
                    radialShine.DOScale(Vector3.zero, shineScaleTime)
                        .SetDelay(paintedRotationTime)
                        .SetEase(shineScaleEase);
                });
        }
        public void PlayAllCollected()
        {
            modelPlace.DORotate(Vector3.up * 360, allCollectedTime, RotateMode.LocalAxisAdd)
                .SetEase(rotationEase);
            modelPlace.DOPunchPosition(Vector3.up * allCollectedJump, allCollectedTime, allCollectedVibro)
                .SetDelay(0.25f)
                .SetEase(allCollectedJumpEase);
        }

        public void StartShine()
        {
            radialShine.DOScale(shineScale, shineScaleTime)
                .SetEase(shineScaleEase);
        }
        public void StopShine()
        {
            radialShine.DOScale(Vector3.zero, 0.25f);
        }
    }
}
