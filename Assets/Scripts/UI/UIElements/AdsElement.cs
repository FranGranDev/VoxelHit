using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Items
{
    public class AdsElement : MonoBehaviour
    {
        private static bool Enabled;

        private void Start()
        {
            if (!Enabled)
            {
                gameObject.SetActive(false);
            }
        }
        private void OnEnable()
        {
            if(!Enabled)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
