using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UI.Items;
using Services;
using System;

namespace UI
{
    public class PuzzleShelfUI : MonoBehaviour
    {
        [Foldout("Buttons"), SerializeField] private ButtonUI exitButton;


        private ISoundPlayer soundPlayer;
        private IAdsController adsController;



        public event Action OnExit;

        public void Initialize(GameInfo info)
        {
            soundPlayer = info.Components.SoundPlayer;

            GetComponentsInChildren<UIPanel>(true)
                .ToList()
                .ForEach(x => x.Initilize(true));

            GetComponentsInChildren<Initializable<ISoundPlayer>>(true)
                .ToList()
                .ForEach(x => x.Initialize(soundPlayer));


            GetComponentsInChildren<Initializable<IAdsController>>(true)
                .ToList()
                .ForEach(x => x.Initialize(adsController));

            exitButton.OnClick += ExitClick;
        }



        private void ExitClick()
        {
            OnExit?.Invoke();
        }
    }
}
