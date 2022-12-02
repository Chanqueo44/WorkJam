using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SilverWing
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool GamePaused =false;
        public GameObject PauseGameUI;

        void Start(){
            PauseGameUI.SetActive(false);
            GamePaused=false;
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape)){

                if(GamePaused){
                    Resume();
                }
                else{
                    Pause();
                }
            }
        
        }

        public void Resume(){
            PauseGameUI.SetActive(false);
            Time.timeScale=1f;
            GamePaused= false;
        }

        void Pause(){
            PauseGameUI.SetActive(true);
            Time.timeScale = 0f;
            GamePaused= true;
        }
        
        public void LoadMenu(){
            Time.timeScale= 1f;
            SceneManager.LoadScene("Menu");
        }

        public void QuitGame(){
            Application.Quit();
        }
    }
}
