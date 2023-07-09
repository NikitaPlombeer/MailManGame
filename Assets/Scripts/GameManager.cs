using System;
using DefaultNamespace.City;
using DwarfTrains.Sound;
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

        public float startTime;
        public bool isDelivered = false;
        
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            GameUI.Instanse.HideWinUI();
            boxController.DisableMoving();
            mailmanController.DisableMoving();
            
            cityGenerator.GenerateCity();
            GameUI.Instanse.HideTime();
        }

        public void Delivered()
        {
            isDelivered = true;
            boxController.DisableMoving();
            mailmanController.DisableMoving();
            
            GameUI.Instanse.ShowWinBlock(Time.timeSinceLevelLoad - startTime);
            GameUI.Instanse.SetVisibleForDeliverLabel(false);
            
            cursorLockMode = CursorLockMode.None;
            Cursor.visible = true;
            
            GlobalSoundManager.Instanse.StopSound("Ambient");
        }

        private void Update()
        {
            Cursor.lockState = cursorLockMode;
            if (isOnBoarding && Input.anyKey)
            {
                startTime = Time.timeSinceLevelLoad;
                isOnBoarding = false;
                GameUI.Instanse.SetVisibleForOnboardingBlock(false);
                boxController.EnableMoving();
                mailmanController.EnableMoving();
                
                GlobalSoundManager.Instanse.PlaySound("Ambient");
                
            }

            if (!isOnBoarding && !isDelivered)
            {
                GameUI.Instanse.DisplayTime(Time.timeSinceLevelLoad - startTime);
            }
        }
    }
}