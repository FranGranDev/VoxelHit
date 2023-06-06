using System.Collections;
using Services;
using UnityEngine;
using TMPro;

namespace UI.Items
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FilledText : MonoBehaviour, IBindable<IFillEvent>
    {
        [SerializeField] private string text;
        [SerializeField] private string replaceItem = "@";

        private TextMeshProUGUI textMesh;

        public void Bind(IFillEvent fillEvent)
        {
            fillEvent.OnFilled += UpdateText;

            textMesh = GetComponent<TextMeshProUGUI>();

            UpdateText(0);
        }

        private void UpdateText(float value)
        {
            string procent = Mathf.RoundToInt(value * 100).ToString();
            string finalText = text.Replace(replaceItem, procent);

            textMesh.text = finalText;
        }
    }
}
