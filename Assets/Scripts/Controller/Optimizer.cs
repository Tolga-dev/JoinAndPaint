using System;
using GameObjects.Base;
using Save.GameObjects.Obstacle;
using UnityEngine;

namespace Controller
{
    public class Optimizer : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Obstacle"))
            {
                other.GetComponent<GameObjectBase>().enabled = true;

                var checkMe = other.GetComponent<RotatingPlatform>();
                if (checkMe != null)
                    checkMe.enabled = true;
            }
            
        }
        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Obstacle"))
            {
                other.GetComponent<GameObjectBase>().enabled = false;
                
                var checkMe = other.GetComponent<RotatingPlatform>();
                if(checkMe != null)
                    checkMe.enabled = false;
            }
        }
    }
}