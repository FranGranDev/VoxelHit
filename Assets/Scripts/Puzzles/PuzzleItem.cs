using UnityEngine;
using Voxel;
using DG.Tweening;
using System.Collections.Generic;
using Voxel.Waves;
using Services;
using Data;

namespace Puzzles
{
    public class PuzzleItem : MonoBehaviour
    {
        public bool Placed { get; private set; }
        public bool Taken { get; private set; }
        public int Index { get; private set; }


        private VoxelObject voxelObject;
        private WaveMaker waveMaker;
        private List<Tween> tweens = new List<Tween>();

        private float offset;

        private ISoundPlayer soundPlayer;
        private IHaptic haptic;

        public void Initialize(GameInfo info, InitTypes initType)
        {
            haptic = info.Components.Haptic;
            soundPlayer = info.Components.SoundPlayer;

            Index = GetComponent<ModelId>().Index;
            voxelObject = GetComponent<VoxelObject>();
            waveMaker = GetComponent<WaveMaker>();

            switch(initType)
            {
                case InitTypes.Static:
                    voxelObject.InitializeStatic();
                    break;
                case InitTypes.Puzzle:
                    voxelObject.InitializePuzzle(info);
                    break;
            }

        }
        public void SetOffset(float offset)
        {
            this.offset = offset;
        }

        public void Interact(IVoxel voxel)
        {
            if (Placed)
            {
                waveMaker.MakeWave(new ModifyWaveParams(voxel.Position));
            }
        }


        public void SetItem(PuzzlePlace place)
        {
            Placed = true;

            transform.SetParent(place.transform);

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        public void PlaceItem(PuzzlePlace place)
        {
            Placed = true;

            StopAnimations();

            transform.SetParent(place.transform);

            tweens.Add(transform.DOLocalMove(Vector3.zero, 0.25f)
                .SetEase(Ease.InOutSine));
            tweens.Add(transform.DOLocalRotate(Vector3.zero, 0.25f)
                .SetEase(Ease.InOutSine));


            waveMaker.MakeWave(new WaveParams(WaveParams.Types.Center));
        }

        public void Throw(Vector3 point)
        {
            Taken = false;

            StopAnimations();

            point += Vector3.up * offset;

            tweens.Add(transform.DOMove(point, 0.25f)
                .SetEase(Ease.InOutSine));
            tweens.Add(transform.DOLocalRotate(new Vector3(90, Random.Range(-5, 5), 0), 0.25f)
                .SetEase(Ease.InOutSine));
        }
        public void FailInstall(Vector3 point)
        {
            Taken = false;

            StopAnimations();

            float moveTime = 0.5f;
            float failTime = 0.5f;

            point += Vector3.up * offset;

            tweens.Add(transform.DOPunchRotation(Vector3.forward * 25, failTime, 9)
                .SetEase(Ease.InOutSine));
            tweens.Add(transform.DOMove(point, moveTime)
                .SetDelay(failTime)
                .SetEase(Ease.InOutSine));
            tweens.Add(transform.DOLocalRotate(new Vector3(90, Random.Range(-5, 5), 0), 0.25f)
                .SetDelay(failTime + moveTime - 0.25f)
                .SetEase(Ease.InOutSine));
        }
        public void Take()
        {
            Taken = true;

            StopAnimations();
        }
 


        public void PlayDone()
        {
            StopAnimations();

            tweens.Add(transform.DOPunchPosition(-Vector3.forward * 3, 0.5f, 5)
                .SetEase(Ease.InOutSine));

            waveMaker.MakeWave(new WaveParams(WaveParams.Types.Center));
        }
        private void StopAnimations()
        {
            foreach (Tween tween in tweens)
            {
                tween.Kill();
            }
            tweens.Clear();
        }


        public enum InitTypes
        {
            Static, Puzzle
        }
    }
}
