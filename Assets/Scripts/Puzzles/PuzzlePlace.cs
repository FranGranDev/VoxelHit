using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Voxel;
using Data;
using DG.Tweening;

namespace Puzzles
{
    public class PuzzlePlace : MonoBehaviour
    {
        [SerializeField] private int index;
        [Space]
        [SerializeField] private bool permanent;
        [SerializeField] private bool useGrayscale;
        [Space]
        [SerializeField] private Transform modelPlace;
        [SerializeField] private Transform backPlace;

        public int Index => index;
        public Transform Place => modelPlace;
        public PuzzleItem Item { get; private set; }


        public void Initialize(GameInfo gameInfo)
        {
            if(permanent)
            {
                Item = modelPlace.GetComponentInChildren<PuzzleItem>();
                Item.Initialize(gameInfo, PuzzleItem.InitTypes.Static);
                backPlace.gameObject.SetActive(false);
                return;
            }
            backPlace.GetComponentsInChildren<VoxelObject>()
                .ToList()
                .ForEach(x =>
                {
                    x.InitializeStatic();
                    if (useGrayscale)
                    {
                        x.SetGrayScale();
                    }
                });
        }

        public void SetItem(PuzzleItem item)
        {
            Item = item;

            item.SetItem(this);

            backPlace.gameObject.SetActive(false);
        }
        public void PlaceItem(PuzzleItem item)
        {
            Item = item;

            item.PlaceItem(this);

            this.Delayed(0.25f, () => backPlace.gameObject.SetActive(false));
        }

        public void PlaySelectHintAnimation()
        {
            backPlace.DOPunchScale(new Vector3(0, 0f, 5f), 0.75f, 3)
                .SetEase(Ease.InOutSine);
        }
        public void PlayInstallHintAnimation()
        {
            backPlace.DOPunchScale(new Vector3(0, 0f, 3f), 1f, 0)
                .SetEase(Ease.InOutSine);
        }
    }
}
