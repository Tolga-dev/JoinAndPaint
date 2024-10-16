using System.Collections;
using UnityEngine;

namespace GameObjects.Road
{
    public class BossRoad : Road
    {
        public FinishLine finishLine;
        
        [Header("Boss")] 
        public GameObject boss;
        public float scaleDuration = 0.5f; // Duration of the scaling animation for each pile
        public float moveDuration = 0.5f; // Duration for moving the money pile to the boss
        private readonly Vector3 _maxScale = new Vector3(27.023634f, 810.709106f, 27.023634f);
       
        public void PlayerArrived() // player finished game, add score in here
        {
            Debug.Log("Game Is Finished");
            gameManager.soundManager.PlayASound(gameManager.gamePropertiesInSave.onGameWinSound);
            SetActiveReloadButton(false);
            CheckForRecords();
            
            gameManager.playerManager.SetWin();
            gameManager.cameraController.SwitchToWinCam();
            
            StartCoroutine(SetGameMainMenu());
        }
        
        private IEnumerator SetGameMainMenu()
        {
            Debug.Log("Called");
            yield return new WaitForSeconds(3);
            
            gameManager.ChangeState(gameManager.menuState);
            SetActiveReloadButton(true);
        }

        private void CheckForRecords()
        {
            var score = gameManager.playingState.score;

            var findRecord = -1;
            for (int i = 0; i < gameManager.gamePropertiesInSave.levelRecords.Count; i++)
            {
                if (score > gameManager.gamePropertiesInSave.levelRecords[i])
                {
                    findRecord = i;
                }
            }

            if (findRecord != -1)
            {
                MadeAScore(gameManager.gamePropertiesInSave.levelRecords[findRecord]);
            }
        }
        private void MadeAScore(int recordIndexBonus)
        {
            StartCoroutine(IncreaseBonusOverTime(recordIndexBonus, 1f));
        }
        private IEnumerator IncreaseBonusOverTime(int targetValue, float duration)
        {
            var playingState = gameManager.playingState;
            var elapsed = 0f;
            var startValue = 0; // Start from 0 and increase over time

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
        
                int currentValue = (int)(Mathf.Lerp(startValue, targetValue, elapsed / duration));

                playingState.extraBonus.text = $"WAOW Record! Bonus: {currentValue}";
                yield return null;
            }
            playingState.extraBonus.text = $"WAOW Record! Bonus: {targetValue}";
        }

        private void SetActiveReloadButton(bool b)
        {
            gameManager.playingState.reloadButton.enabled = b;
        }

    }
}