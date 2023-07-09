using Nova;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameUI: Singleton<GameUI>
    {

        public TextBlock deliverLabel;
        public UIBlock2D onboardingBlock;
        
        public void Start()
        {
            SetVisibleForOnboardingBlock(true);
            SetVisibleForDeliverLabel(false);
        }
        
        public void SetVisibleForOnboardingBlock(bool isVisible)
        {
            onboardingBlock.gameObject.SetActive(isVisible);
        }
        
        public void SetVisibleForDeliverLabel(bool isVisible)
        {
            deliverLabel.gameObject.SetActive(isVisible);
        }
        
    }
}