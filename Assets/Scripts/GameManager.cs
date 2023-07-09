using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager: Singleton<GameManager>
    {
        
        public CursorLockMode cursorLockMode = CursorLockMode.Locked;
        public DeliveryBox deliveryBox;
        public bool isOnBoarding = true;
        public BoxController boxController;
        public MailmanController mailmanController;
        
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            boxController.DisableMoving();
            mailmanController.DisableMoving();
        }

        private void Update()
        {
            Cursor.lockState = cursorLockMode;
            if (isOnBoarding && Input.anyKey)
            {
                isOnBoarding = false;
                GameUI.Instanse.SetVisibleForOnboardingBlock(false);
                boxController.EnableMoving();
                mailmanController.EnableMoving();
            }
        }
    }
}