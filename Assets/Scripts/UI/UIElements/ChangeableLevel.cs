using System.Collections;
using Services;
using UnityEngine;
using TMPro;
using System;

namespace UI.Items
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ChangeableLevel : MonoBehaviour, IChangeable<LevelNumber>
    {
        [SerializeField] private string text;
        [SerializeField] private string replaceItem = "@";

        private TextMeshProUGUI textMesh;

        public void Bind(ref Action<LevelNumber> onChanged)
        {
            onChanged += UpdateText;

            textMesh = GetComponent<TextMeshProUGUI>();
        }

        private void UpdateText(LevelNumber value)
        {
            if (string.IsNullOrEmpty(replaceItem))
                return;
            string procent = value.ToString();
            string finalText = text.Replace(replaceItem, procent);

            textMesh.text = finalText;
        }


        public void SetValue(LevelNumber value)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            UpdateText(value);
        }
    }
}
