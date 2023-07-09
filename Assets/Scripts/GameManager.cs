using System;
using DefaultNamespace.City;
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
        public CityGenerator cityGenerator;
        
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            boxController.DisableMoving();
            mailmanController.DisableMoving();
            
            cityGenerator.GenerateCity();
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