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
    }
}