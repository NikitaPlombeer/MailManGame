using System;
using Nova;
using UnityEngine;

namespace DefaultNamespace
{
    public class BuffEffectUI: MonoBehaviour
    {

        public Color loseColor;
        public Color slowColor;
        public Color speedColor;
        public Color winColor;

        public UIBlock2D effectBlock;

        private void Start()
        {
            SetDefault();
        }

        public void SetDefault()
        {
            effectBlock.gameObject.SetActive(false);
            
        }

        public void SetLose()
        {
            effectBlock.gameObject.SetActive(true);
            effectBlock.Shadow.Color = loseColor;
        }
        
        public void SetSlow()
        {
            effectBlock.gameObject.SetActive(true);
            effectBlock.Shadow.Color = slowColor;
        }
        
        public void SetSpeed()
        {
            effectBlock.gameObject.SetActive(true);
            effectBlock.Shadow.Color = speedColor;
        }
        
        public void SetWin()
        {
            effectBlock.gameObject.SetActive(true);
            effectBlock.Shadow.Color = winColor;
        }
        
    }
}