using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SilverWing
{
    public class MenuPrincipal : MonoBehaviour
    {
        public void PlayGame()
        {
            SceneManager.LoadScene(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

    }
}
