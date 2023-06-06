using System.Collections;
using Services;
using UnityEngine;
using TMPro;
using System;

namespace UI.Items
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ChangableText : MonoBehaviour, IChangeable<string>
    {
        [SerializeField] private string text;
        [SerializeField] private string replaceItem = "@";

        private TextMeshProUGUI textMesh;

        public void Bind(ref Action<string> onChanged)
        {
            onChanged += UpdateText;

            textMesh = GetComponent<TextMeshProUGUI>();
        }

        private void UpdateText(string value)
        {
            if (string.IsNullOrEmpty(replaceItem))
                return;
            string procent = value.ToString();
            string finalText = text.Replace(replaceItem, procent);

            textMesh.text = finalText;
        }


        public void SetValue(string value)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            UpdateText(value);
        }
    }
}
