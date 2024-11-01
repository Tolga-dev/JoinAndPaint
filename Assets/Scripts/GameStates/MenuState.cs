using System;
using System.Globalization;
using GameStates.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameStates
{
    [Serializable]
    public class MenuState : GameState
    { 
        // panel
        public Transform menuPanel;
        public GameObject clickAvoid;
        
        // game properties
        public TextMeshProUGUI paraAmount;
            
        // no ads
        [Header("Shop UI")]
        public Button startMarket;
        public Button exitMarket;
        public Button buyNoAds;
        public TextMeshProUGUI description;
        public TextMeshProUGUI shopResult;
        public Transform marketPanel; 
        
        // updates
        // damage
        public Button damageUpdateButton;
        public TextMeshProUGUI damagePrice;
        public TextMeshProUGUI damageUpdateAmount;
        // health
        public Button healthUpdateButton;
        public TextMeshProUGUI healthPrice;
        public TextMeshProUGUI healthUpdateAmount;
        
        // setting
        [Header("Setting UI")]
        public Button startSettings;
        public Button exitSettings;
        public Transform settingPanel;
        
        public Button changeStatusMusicButton;
        public Button changeStatusSoundButton;
        public TextMeshProUGUI clickToStart;

        public override void Init(GameManager gameManager)
        {
            base.Init(gameManager);
            SetUI();
        }

        public override void Enter()
        {
            var save = GameManager.gamePropertiesInSave;
            var sound = GameManager.soundManager;
            
            sound.GameMusic(save.onMenuStateSound);
            GameManager.cameraController.SwitchToMenuStateCam();
            
            menuPanel.gameObject.SetActive(true);
        }
        public override void Update()
        {
            GameManager.playerManager.inputController.HandleMouseInput();
            if (GameManager.playerManager.inputController.isMouseDown)
            { 
                GameManager.ChangeState(GameManager.playingState);
            }
        }
        public override void Exit()
        {
            menuPanel.gameObject.SetActive(false);
            Debug.Log("MenuState Exit");
        }
        
        private void SetUI()
        {
            SetSaveUI();

            var save = GameManager.gamePropertiesInSave;
            var soundManager = GameManager.soundManager;
            
            // settings
            changeStatusMusicButton.onClick.AddListener(() =>
            {
                soundManager.ButtonClickSound();
                
                if (save.isGameMusicOn)
                {
                    soundManager.GameMusic(null);
                    save.isGameMusicOn = false;
                }
                else
                {
                    save.isGameMusicOn = true;
                    soundManager.GameMusic(save.onMenuStateSound);
                }
                
                ToggleButtonPosition(changeStatusMusicButton, save.isGameMusicOn);
            });
            
            changeStatusSoundButton.onClick.AddListener(() =>
            {
                soundManager.ButtonClickSound();
                save.isGameSoundOn = !save.isGameSoundOn;
                ToggleButtonPosition(changeStatusSoundButton, save.isGameSoundOn);
            });
            
            startSettings.onClick.AddListener(() =>
            {
                settingPanel.gameObject.SetActive(true);
                soundManager.ButtonClickSound();
            }); 
            exitSettings.onClick.AddListener(() =>
            {
                settingPanel.gameObject.SetActive(false);
                soundManager.ButtonClickSound();
            });
            
            // markets
            startMarket.onClick.AddListener(() =>
            {
                soundManager.ButtonClickSound();
                soundManager.GameMusic(save.onMarketSound);
                marketPanel.gameObject.SetActive(true);
                
            });

            exitMarket.onClick.AddListener(() =>
            {
                soundManager.ButtonClickSound();
                soundManager.GameMusic(save.onMenuStateSound);
                marketPanel.gameObject.SetActive(false);
                
                shopResult.text = "";
            });
            
            buyNoAds.onClick.AddListener(() =>
            {
                var success = new UnityAction<string>((string result) =>
                {
                    soundManager.ButtonClickSound();
                    soundManager.PlayASound(save.purchasedSound);
                    GameManager.gamePropertiesInSave.isNoAds = true;
                    
                    SetShopUI();
                    
                    GameManager.serviceManager.adsManager.CleanUp();
                    shopResult.text = result;
                });
                
                var failed = new UnityAction<string>((string result) =>
                {
                    soundManager.PlayASound(save.failedClickSound);
                    shopResult.text = result;
                });
                GameManager.serviceManager.inAppPurchase.BuyItem(GameManager.gamePropertiesInSave.noAdsProductId,success,failed);
            }); 
            
            // updates
            damageUpdateButton.onClick.AddListener((() =>
            {
                soundManager.ButtonClickSound();
                UpdateDamage();
                SetUpdateUI();
            }));
            
            healthUpdateButton.onClick.AddListener((() =>
            {
                soundManager.ButtonClickSound();
                UpdateHealth();
                SetUpdateUI();
            }));
        }

        public void UpdateDamage()
        {
            var save = GameManager.gamePropertiesInSave;
            var money = save.totalMoney;
            var price = save.damagePrice;

            if (!save.damageIsNewPriceCalculated)
            {
                var priceMinIncreaseAmount = save.damagePriceMinIncreaseAmount * save.damagePriceLevel;
                var priceMaxIncreaseAmount = save.damagePriceMaxIncreaseAmount * save.damagePriceLevel;

                if (priceMinIncreaseAmount > 0 && priceMaxIncreaseAmount > priceMinIncreaseAmount)
                {
                    save.damageNewAdditionalPrice = Random.Range(priceMinIncreaseAmount, priceMaxIncreaseAmount);
                }
                else
                {
                    save.damageNewAdditionalPrice = priceMinIncreaseAmount; // Fallback to minimum increase
                }

                save.damageIsNewPriceCalculated = true;
            }

            
            if (money >= price && save.damageIsNewPriceCalculated)
            {
                save.totalMoney -= price;
                
                var newPrice = price + save.damageNewAdditionalPrice;
                save.damagePrice = newPrice;

                save.damageUpdate += save.damageIncreaseComboAmount * Random.Range(1, 4);

                save.damagePriceLevel++;
                save.damageIsNewPriceCalculated = false;

                GameManager.soundManager.PlayASound(save.purchasedSound);

                // Optionally start saving and play ads
                GameManager.StartCoroutine(GameManager.saveManager.Save());
                GameManager.serviceManager.adsManager.PlayComboTransitionAds();
                
                SetMenuStateUI();
            }
            else
            {
                GameManager.soundManager.PlayASound(save.failedClickSound);
            }
        }
        
        public void UpdateHealth()
        {
            var save = GameManager.gamePropertiesInSave;
            var money = save.totalMoney;
            var price = save.healthPrice;

            if (!save.healthIsNewPriceCalculated)
            {
                var priceMinIncreaseAmount = save.healthPriceMinIncreaseAmount * save.healthPriceLevel;
                var priceMaxIncreaseAmount = save.healthPriceMaxIncreaseAmount * save.healthPriceLevel;

                if (priceMinIncreaseAmount > 0 && priceMaxIncreaseAmount > priceMinIncreaseAmount)
                {
                    save.healthNewAdditionalPrice = Random.Range(priceMinIncreaseAmount, priceMaxIncreaseAmount);
                }
                else
                {
                    save.healthNewAdditionalPrice = priceMinIncreaseAmount; // Fallback to minimum increase
                }

                save.healthIsNewPriceCalculated = true;
            }


            if (money >= price && save.healthIsNewPriceCalculated)
            {
                save.totalMoney -= price;
                var newPrice = price + save.healthNewAdditionalPrice;
                save.healthPrice = newPrice;

                save.healthUpdate += save.healthIncreaseComboAmount * Random.Range(1, 4);

                save.healthPriceLevel++;
                save.healthIsNewPriceCalculated = false;

                GameManager.soundManager.PlayASound(save.purchasedSound);

                // Optionally start saving and play ads
                GameManager.StartCoroutine(GameManager.saveManager.Save());
                 GameManager.serviceManager.adsManager.PlayComboTransitionAds();
                 
                SetMenuStateUI();
            }
            else
            {
                GameManager.soundManager.PlayASound(save.failedClickSound);
            }
        }

        private void ToggleButtonPosition(Button button, bool isOn)
        {
            var rectTransform = button.GetComponent<RectTransform>();
            var width = rectTransform.rect.width;
            var vector2 = rectTransform.anchoredPosition;
            
            if (isOn)
            {
                vector2.x = 0;
            }
            else
            {
                vector2.x = width;
            }
            rectTransform.anchoredPosition = vector2;
        }
        private void SetSaveUI()
        {
            var save = GameManager.gamePropertiesInSave;
            
            // settings
            ToggleButtonPosition(changeStatusMusicButton, save.isGameMusicOn);
            ToggleButtonPosition(changeStatusSoundButton, save.isGameSoundOn);
            
            SetShopUI();
            SetMoney();
            SetUpdateUI();
        }


        private void SetShopUI()
        {
            var save = GameManager.gamePropertiesInSave;
            if (save.isNoAds)
            {
                buyNoAds.gameObject.SetActive(false);
                description.text = "You have bought No Ads!";
            }
            else
            {
                description.text = $"Remove ads with {GameManager.serviceManager.inAppPurchase.ReturnLocalizedPrice(GameManager.gamePropertiesInSave.noAdsProductId)}"; // money might be changed
            }   
            menuPanel.gameObject.SetActive(true);
        }

        public void SetNewMotivationString(string message)
        {
            if (string.IsNullOrEmpty(message) == false)
            {
                clickToStart.text = message;
                return;
            }

            var gamePropertiesInSave = GameManager.gamePropertiesInSave;

            if (GameManager.playingState.isGameWon)
            {
                clickToStart.text = gamePropertiesInSave.winText[Random.Range(0, gamePropertiesInSave.winText.Length)];
            }
            else
            {
                clickToStart.text = gamePropertiesInSave.lostText[Random.Range(0, gamePropertiesInSave.lostText.Length)];
            }

            GameManager.playingState.isGameWon = false;
        }

        public void SetMenuStateUI()
        {
            SetMoney();
            SetUpdateUI();
        }
        private void SetMoney()
        {
            paraAmount.text = GameManager.gamePropertiesInSave.totalMoney + "$";
        }

        private void SetUpdateUI()
        {
            damagePrice.text = GameManager.gamePropertiesInSave.damagePrice + "$";
            damageUpdateAmount.text = "x" +GameManager.gamePropertiesInSave.damageUpdate.ToString("F1");
            
            healthPrice.text = GameManager.gamePropertiesInSave.healthPrice + "$";
            healthUpdateAmount.text = "x" + GameManager.gamePropertiesInSave.healthUpdate.ToString("F1");
        }
        
    }
}