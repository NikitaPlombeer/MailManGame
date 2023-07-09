using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class MainMenu: MonoBehaviour
    {

        public void OnStartClick()
        {
            SceneManager.LoadScene("CityGeneration");
        }

        public void OnExitClick()
        {
            Application.Quit();
        }
    }
}