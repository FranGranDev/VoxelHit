using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using DG.Tweening;


namespace Animations
{
    public class RepairEffect : MonoBehaviour
    {
        [SerializeField] private float maxAlpha;
        [SerializeField] private float alphaPerShot;
        [Space]
        [SerializeField] private float delayTime;
        [SerializeField] private float hideSpeed;
        [Space]
        [SerializeField] private float punchScale;
        [SerializeField] private float punchTime;
        [SerializeField] private int punchVibro;
        [SerializeField] private Ease punchEase;
        [Space]
        [SerializeField] private MeshRenderer meshRenderer;

        private Material material;
        private Tween tween;

        private Color color;
        private float alpha;
        private float currantAlpha;
        private float lastShotTime;

        private float Alpha
        {
            get => alpha;
            set
            {
                alpha = Mathf.Clamp(value, 0, maxAlpha);
            }
        }
        private Color GetColor(float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }


        public void Initilalize(IFillEvent fillEvent, Color baseColor)
        {
            if (fillEvent == null)
                return;

            material = new Material(meshRenderer.sharedMaterial);
            meshRenderer.material = material;

            color = baseColor;
            material.color = GetColor(0);

            fillEvent.OnFilled += AddAlpha;
        }

        private void AddAlpha(float obj)
        {
            Alpha += alphaPerShot;

            lastShotTime = Time.time;

            if(tween == null && currantAlpha < maxAlpha * 0.75f)
            {
                tween = transform.DOPunchScale(Vector3.one * punchScale, punchTime, punchVibro)
                    .SetEase(punchEase)
                    .OnKill(() => tween = null);
            }
        }

        private void FixedUpdate()
        {
            if(Time.time + delayTime > lastShotTime)
            {
                Alpha -= hideSpeed * Time.fixedDeltaTime;
            }

            currantAlpha = Mathf.Lerp(currantAlpha, Alpha, 0.1f);
            material.color = GetColor(currantAlpha);
        }
    }
}
