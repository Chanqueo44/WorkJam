using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilverWing
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Vector2 parallaxEffectMultiplier;
        private Transform cameraTransform;
        private Vector3 lastCameraPosition;

        // Start is called before the first frame update
        private void Start()
        {   
            cameraTransform= Camera.main.transform;
            lastCameraPosition= cameraTransform.position;
        }

        private void LateUpdate()
        {
            Vector3 cameraMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3(cameraMovement.x*parallaxEffectMultiplier.x , cameraMovement.y*parallaxEffectMultiplier.y, 0);
            lastCameraPosition= cameraTransform.position;
        }
    }
}