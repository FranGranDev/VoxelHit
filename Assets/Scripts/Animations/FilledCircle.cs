using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using UnityEngine.UI;


namespace Animations
{
    public class FilledCircle : MonoBehaviour, Initializable<GameInfo>
    {
        [SerializeField] private float fillSpeed = 0.1f;
        [Space, SerializeField] private Color nonFilledColor;
        [SerializeField] private Color filledColor;
        [SerializeField] private AnimationCurve fillCurve;

        [SerializeField] private Image image;

        private IFillable fillable;


        public void Initialize(GameInfo info)
        {
            if (transform.parent == null)
                return;
            fillable = transform.parent.GetComponentInChildren<IFillable>();
        }

        private void FixedUpdate()
        {
            if (fillable == null)
                return;
            float fill = fillable.Fill;

            Color color = Color.Lerp(nonFilledColor, filledColor, fillCurve.Evaluate(fill));

            image.fillAmount = Mathf.Lerp(image.fillAmount, fill, fillSpeed);
            image.color = Color.Lerp(image.color, color, fillSpeed);
        }
    }
}
