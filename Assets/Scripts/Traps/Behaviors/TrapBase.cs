using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animations;
using System.Linq;

namespace Traps
{
    public abstract class TrapBase : ITrap
    {
        public TrapBase(Transform transform, MonoBehaviour monoBehaviour, AnimationData animationData, BaseSettings settings)
        {
            this.transform = transform;
            this.animationData = animationData;
            this.monoBehaviour = monoBehaviour;
            this.settings = settings;

            startScale = transform.localScale;
            traps = transform.GetComponentsInChildren<Trap>(true).ToList();

            zeroScaleY = new Vector3(startScale.x, 0, startScale.z);
            zeroScaleZ = new Vector3(startScale.x, startScale.y, 0);
            zeroScaleX = new Vector3(0, startScale.y, startScale.z);

            zeroScales = new Dictionary<Axis, Vector3>()
            {
                {Axis.Up, zeroScaleY },
                {Axis.Forward, zeroScaleZ },
                {Axis.Right, zeroScaleX },
            };
            rotationAxis = new Dictionary<Axis, Vector3>()
            {
                {Axis.Up, Vector3.up },
                {Axis.Forward, Vector3.forward },
                {Axis.Right, Vector3.right },
            };

            Scale = 0;
        }

        private BaseSettings settings;
        private MonoBehaviour monoBehaviour;
        protected Coroutine moveCoroutine;
        private List<Trap> traps;

        protected Transform transform;
        protected AnimationData animationData;

        private float scale;
        private bool isShown;

        private Vector3 startScale;

        private Dictionary<Axis, Vector3> rotationAxis;

        private Dictionary<Axis, Vector3> zeroScales;
        private Vector3 zeroScaleY;
        private Vector3 zeroScaleZ;
        private Vector3 zeroScaleX;

        protected float Scale
        {
            get => scale;
            set
            {
                if (settings.NoScale)
                    return;
                scale = Mathf.Clamp01(value);


                transform.localScale = Vector3.Lerp(zeroScales[settings.ScaleAxis], startScale, scale);
            }
        }
        protected Vector3 RotationAxis
        {
            get => rotationAxis[settings.Axis];
        }
        public bool IsShown
        {
            get => isShown;
            private set
            {
                isShown = value;
                if (!settings.NoScale)
                {
                    traps.ForEach(x => x.Disabled = !isShown);
                }
            }
        }

        protected Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return monoBehaviour.StartCoroutine(coroutine);
        }
        protected void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine == null)
                return;
            monoBehaviour.StopCoroutine(coroutine);
        }


        public virtual void Enable()
        {
            IsShown = true;
            Show();
        }
        public virtual void Disable()
        {
            IsShown = false;
            Hide();
        }

        protected void Show()
        {
            StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(ShowCour());
        }
        protected void Hide()
        {
            StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(HideCour());
        }

        private IEnumerator ShowCour()
        {
            yield return new WaitForSeconds(animationData.Delay);

            float time = 0;
            var wait = new WaitForFixedUpdate();

            float startScale = Scale;

            while(time < animationData.Time)
            {
                float ratio = animationData.AnimationCurves[0].Evaluate(time / animationData.Time);
                Scale = Mathf.Lerp(startScale, 1, ratio);

                time += Time.fixedDeltaTime;
                yield return wait;
            }
            Scale = 1;

            yield break;
        }
        private IEnumerator HideCour()
        {
            yield return new WaitForSeconds(animationData.Delay);

            float time = 0;
            var wait = new WaitForFixedUpdate();

            float startScale = Scale;

            while (time < animationData.Time)
            {
                float ratio = animationData.AnimationCurves[0].Evaluate(time / animationData.Time);
                Scale = Mathf.Lerp(startScale, 0, ratio);

                time += Time.fixedDeltaTime;
                yield return wait;
            }
            Scale = 0;

            yield break;
        }

        public enum Axis
        {
            Up,
            Forward,
            Right,
        }


        [System.Serializable]
        public class BaseSettings
        {
            [SerializeField] private Axis rotationAxis = Axis.Up;
            [Space]
            [SerializeField] private Axis scaleAxis = Axis.Up;
            [SerializeField] private bool noScale = false;

            public Axis Axis => rotationAxis;
            public Axis ScaleAxis => scaleAxis;
            public bool NoScale => noScale;
        }
    }

    public interface ITrap
    {
        public bool IsShown { get; }

        public void Enable();
        public void Disable();
    }
}
