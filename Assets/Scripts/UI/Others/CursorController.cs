using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;

namespace UI
{
    public class CursorController : MonoBehaviour
    {
        private ICursor cursor;

        private void Start()
        {
            cursor = GetComponentInChildren<ICursor>();

            if(cursor == null)
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            cursor.Position = Input.mousePosition;

            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                cursor.Down();
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                cursor.Up();
            }
        }
    }
}
