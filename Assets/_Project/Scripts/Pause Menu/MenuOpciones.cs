using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SilverWing
{
    public class MenuOpciones : MonoBehaviour
    {
        public AudioMixer audioMixer;

        public void SetVolume (float volume){
           
           audioMixer.SetFloat("MasterVolume", volume);
        }

        public void SetFullScreen(bool isFullScreen){

            Screen.fullScreen= isFullScreen;
        }
    }
}
