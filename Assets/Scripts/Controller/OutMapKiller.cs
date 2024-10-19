using System;
using UnityEngine;

namespace Controller
{
    public class OutMapKiller : MonoBehaviour
    {
        public Transform outMapNewPos;
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
                other.gameObject.transform.position = outMapNewPos.position;
            else if (other.CompareTag("Recruitment"))
            {
                var recruitment = other.GetComponent<Recruitment>();
                recruitment.gameManager.memberManager.DestroyNewMember(recruitment);
            }
            
        }
    }
}