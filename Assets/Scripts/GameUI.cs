using Nova;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class GameUI: Singleton<GameUI>
    {

        public TextBlock deliverLabel;
        public UIBlock2D onboardingBlock;
        public UIBlock2D winBlock;
        public TextBlock winTimeLabel;
        public TextBlock timeText;
        
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

        public void OnMainMenuClick()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        
        public void ShowWinBlock(float time)
        {
            winBlock.gameObject.SetActive(true);
            winTimeLabel.Text = $"Time: {(int)time} sec";
        }

        public void HideWinUI()
        {
            winBlock.gameObject.SetActive(false);
        }

        public void DisplayTime(float time)
        {
            timeText.gameObject.SetActive(true);
            timeText.Text = $"Time: {(int)time} sec.";
        }
        
        public void HideTime()
        {
            timeText.gameObject.SetActive(false);
        }
    }
}