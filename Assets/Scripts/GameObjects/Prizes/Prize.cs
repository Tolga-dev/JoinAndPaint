using System.Collections;
using GameObjects.Base;
using TMPro;
using UnityEngine;

namespace GameObjects.Prizes
{
    public class Prize : GameObjectBase
    {
        public ParticleSystem emoji;
        public Canvas prizeCanvas;
        public int prizeAmount;
        
        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (isHitPlayer)
            {
                FoundPlayerHit();
                return;
            }

            if (other.CompareTag("Recruitment"))
            {
                var recruitment = other.GetComponent<Recruitment>();
                if (recruitment.isHitPlayer == false)
                    return;
                
                FoundPlayerHit();
                CallPlayerGotHit();
            }
        }

        public virtual void FoundPlayerHit()
        {
            gameManager.playingState.score += prizeAmount;
        }
        
        protected override void PlayAdditionalEffects(PlayerManager playerController)
        {
            if (emoji != null)
                SetParticlePosition(emoji,emoji.transform);

            if(prizeCanvas != null)
                    ShowCanvas(prizeCanvas.transform);
        }

        public void ShowCanvas(Transform playerPos)
        {
            var text = prizeCanvas.GetComponentInChildren<TextMeshProUGUI>();
            text.text = prizeAmount.ToString();
    
            prizeCanvas.gameObject.SetActive(true);
            StartCoroutine(AnimateCanvas(prizeCanvas.transform));
        }
         

        protected virtual IEnumerator AnimateCanvas(Transform canvasTransform)
        {
            float duration = 1f; // Total duration of the animation
            float elapsed = 0f;

            Vector3 startPos = canvasTransform.localPosition; // Starting position (Vector3.zero)
            Vector3 endPos = startPos + new Vector3(0, 0.1f, 0); // Move up by 1 unit on the Y axis
    
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasTransform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
        
                yield return null;
            }
            prizeCanvas.gameObject.SetActive(false);
            Destroy(prizeCanvas.gameObject);
        }

        public void OnDestroy()
        {
            if(prizeCanvas != null )
                Destroy(prizeCanvas.gameObject);
        }
    }
}