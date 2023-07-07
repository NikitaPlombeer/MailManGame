using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager: MonoBehaviour
    {
        
        public CursorLockMode cursorLockMode = CursorLockMode.Locked;
        
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Cursor.lockState = cursorLockMode;

        }
    }
}