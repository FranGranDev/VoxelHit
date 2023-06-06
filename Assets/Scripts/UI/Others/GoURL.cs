using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class GoURL : MonoBehaviour
    {
        [TextArea, SerializeField] private string url;

        public void GoUrl()
        {
            Application.OpenURL(url);
        }
    }
}
