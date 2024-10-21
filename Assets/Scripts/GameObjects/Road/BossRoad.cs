using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameObjects.Road
{
    public class BossRoad : Road
    {
        public FinishLine finishLine;
        public Boss.Boss boss;
        public GameObject optimizer;
        
        public void StartBossMatch(GameObject recruitment)
        {
            SetPlayerOnFinish();

            boss.PlayerArrived(this);
        }

        private void SetPlayerOnFinish()
        {
            gameManager.playerManager.ResetInput();
            gameManager.playingState.isOnFinish = true;
            gameManager.cameraController.SwitchToBossCam();
            gameManager.soundManager.GameMusic(gameManager.gamePropertiesInSave.bossFightMusic);
            SetActiveReloadButton(false);
        }

        public void GameFinished() // player finished game, add score in here
        {
            Debug.Log("Game Is Finished");
            gameManager.soundManager.PlayASound(gameManager.gamePropertiesInSave.onGameWinSound);
            SetActiveReloadButton(false);
            CheckForRecords();
            gameManager.playerManager.SetWin();
            
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
                var currentScore = gameManager.gamePropertiesInSave.levelRecords[findRecord];
                var recordIndexBonus = Random.Range(currentScore, currentScore * 100);
                var extraBonus = gameManager.playingState.extraBonus;
                StartCoroutine(IncreaseBonusOverTime(recordIndexBonus, extraBonus,"WAOW Record! Bonus: ", 1f));
                
            }
            var scoreText = gameManager.playingState.extraComboBonus;
            StartCoroutine(IncreaseBonusOverTime(score, scoreText,"Extra Bonus: ", 1f));
            
        }
        private IEnumerator IncreaseBonusOverTime(int targetValue, TextMeshProUGUI meshProUGUI, string message, float duration)
        {
            var elapsed = 0f;
            var startValue = 0; // Start from 0 and increase over time

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
        
                int currentValue = (int)(Mathf.Lerp(startValue, targetValue, elapsed / duration));

                meshProUGUI.text = message + currentValue;
                yield return null;
            }
            meshProUGUI.text = message + targetValue;
        }

        private void SetActiveReloadButton(bool b)
        {
            gameManager.playingState.reloadButton.enabled = b;
        }
    }
}