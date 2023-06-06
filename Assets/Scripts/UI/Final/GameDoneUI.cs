using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class GameDoneUI : MonoBehaviour
    {
        [SerializeField] private UIModifyPanel panel;

        public event System.Action OnContinue;

        public void Show()
        {
            panel.Initilize();
            panel.IsShown = true;
        }
        public void ContinueClick()
        {
            panel.IsShown = false;
            OnContinue?.Invoke();
        }
    }
}
