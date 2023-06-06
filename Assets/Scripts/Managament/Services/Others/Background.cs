using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Managament
{
    public class Background : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image pattern;

        public void Initialize(Settings settings)
        {
            if (settings == null)
                return;


            background.color = settings.BackgroundColor;
            pattern.color = settings.PatternColor;

            if(settings.PatternSprite)
            {
                pattern.sprite = settings.PatternSprite;
            }
        }

        [System.Serializable]
        public class Settings
        {
            [SerializeField] private Sprite patternSprite;
            [Space]
            [SerializeField] private Color backgroundColor;
            [SerializeField] private Color patternColor;


            public Sprite PatternSprite => patternSprite;
            public Color BackgroundColor => backgroundColor;
            public Color PatternColor => patternColor;
        }
    }
}
