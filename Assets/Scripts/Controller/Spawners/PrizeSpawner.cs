using System;
using System.Collections.Generic;
using GameObjects.Prizes;
using GameObjects.Road;
using Save.GameObjects.Prizes;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Controller.Spawners
{
    [Serializable]
    public class PrizeSpawner
    {
        [Header("Source")]
        public List<GameObject> prizes = new List<GameObject>();
        
        [Header("Created")]
        public List<GameObject> createdPrizes = new List<GameObject>();
        public List<GameObject> createdMembers = new List<GameObject>();
        
        public GameObject recruitment;
        
        private SpawnManager _spawnerManager;
        private GameManager _gameManager;

        private int _totalMaxRange;
        private int[] _prizeRanges;
        public int additionalPoint;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            
            var totalPrizes = prizes.Count;
            var maxRange = totalPrizes +1;
            _prizeRanges = new int[maxRange];
            
            for (int i = 0; i < maxRange; i++)
            {
                _totalMaxRange += (i + 1) * 20;
                
                _prizeRanges[i] = _totalMaxRange;
            }
        }
        public void SpawnObject(SpawnManager spawnerManager)
        {
            _spawnerManager = spawnerManager;
            SpawnPrizes(spawnerManager.roadSpawner.createdRoads);
        }

        private void SpawnPrizes(IEnumerable<Road> roads)
        {
            foreach (var road in roads)
            {
                for (int i = 0; i < road.spawnPoints.Count-1; i++)
                {
                    if (road.spawnPoints[i].isObjSpawned)
                        continue;

                    CreateMember(road.spawnPoints[i].spawnPoint);
                    
                    if (Random.Range(0, 2) == 1) // 50% chance to spawn
                    {
                        CreatePrize(road.spawnPoints[i].spawnPoint);
                    }
                }
            }
        }

       
        private void CreatePrize(Transform spawnPoint) 
        {
            var rate = Random.Range(0, _totalMaxRange); 

            int index = 0;
            for (int i = 0; i < _prizeRanges.Length-1; i++)
            {
                if (rate < _prizeRanges[i])
                {
                    index = i;
                    break;
                }
            } 
            
            var prizePrefab = prizes[index];

            
            int spawnCount = index switch
            {
                _ => Random.Range(1, 1 + (index * 3)) // Dynamic scaling: 1 -> 3 -> 6 -> 9
            };
            
            var positionOffsets = new float[spawnCount];

            for (int i = 0; i < spawnCount; i++)
            {
                positionOffsets[i] = (i % 2 == 0 ? 1 : -1) * (i / 4f);
            }
            
            for (int i = 0; i < spawnCount; i++)
            {
                var prize = Object.Instantiate(prizePrefab, spawnPoint, true);
                var position = spawnPoint.position;
              
                float randomXOffset = Random.Range(-0.5f, 0.5f);
                prize.transform.position = new Vector3(position.x + randomXOffset, prize.transform.position.y, position.z + positionOffsets[i]);
                SetRandValuePrize(prize.GetComponent<Prize>(), index+1);
                createdPrizes.Add(prize);
            }
        }
        
        public void SetRandValuePrize(Prize prize, int factor)
        {
            var save = _spawnerManager.GameManager.gamePropertiesInSave;
            var maxRange = save.currenLevel + additionalPoint;
            
            prize.prizeAmount = Random.Range(maxRange - (int)(maxRange/2),maxRange);
            prize.prizeAmount *= factor;
            prize.gameManager = _gameManager;
        }
        private void CreateMember(Transform spawnPoint)
        {
            Debug.Log("SpawnMan");
            
            var spawnCount = Random.Range(2, 5); 
 
            var positionOffsets = new float[spawnCount];

            for (int i = 0; i < spawnCount; i++)
            {
                positionOffsets[i] = (i % 2 == 0 ? 1 : -1) * (i / 4f);
            }
            
            for (int i = 0; i < spawnCount; i++)
            {
                var prize = Object.Instantiate(recruitment, spawnPoint, true);
                var createdRecruitment = prize.GetComponent<Recruitment>();
                createdRecruitment.Freeze();
                createdRecruitment.gameManager = _gameManager;

                var position = spawnPoint.position;
                var randomXOffset = Random.Range(-0.5f, 0.5f);
              
                prize.transform.position = new Vector3(position.x + randomXOffset, prize.transform.position.y, position.z + positionOffsets[i]);
                createdMembers.Add(prize);
            }
            
        }

 

        /*private void ConfigureSelector(GameObject prize)
        {
            var selector = prize.GetComponentInChildren<Selector>();
            var gameManager = _spawnerManager.GameManager;
            var selectorManager = gameManager.selectorManager;
            var operations = selectorManager.GetOperations();
            
            selector.selection = operations[Random.Range(0, operations.Count)];
    
            var currentLevel = gameManager.gamePropertiesInSave.currenLevel;

            selector.prizeAmount = selector.selection.selectionAction switch
            {
                SelectionAction.Sum => Random.Range(5, 10 + currentLevel),
                SelectionAction.Subtraction => Random.Range(5, 20 + currentLevel),
                SelectionAction.Multiply => Random.Range(2, 4 + currentLevel/10),
                SelectionAction.Divide => Random.Range(2, 4 + currentLevel),
                _ => selector.prizeAmount
            };
            selector.gateSprite.sprite = selector.selection.sprite;
            
            selector.SetText();
        }
        

        private void SpawnChest(SpawnManager spawnerManager)
        {
            var road = spawnerManager.roadSpawner.createdRoads[0]; // make it easy some times :)
            var spawnPoint = road.spawnPoint[Random.Range(0, road.spawnPoint.Count)];
            SpawnAtPoint(chest, spawnPoint);
        }*/
        
        private void SpawnAtPoint(GameObject prize, Transform spawnPoint)
        {
            var created = Object.Instantiate(prize, spawnPoint.position, Quaternion.identity);
            var chestPrize = created.GetComponent<Prize>();
            SetRandValuePrize(chestPrize,2);
            createdPrizes.Add(created);
        }

        public void ResetPrize()
        {
            foreach (var createdObstacle in createdPrizes)
            {
                Object.Destroy(createdObstacle);
            }
            createdPrizes.Clear();
            
            foreach (var member in createdMembers)
            {
                Object.Destroy(member);
            }
            createdMembers.Clear();
        }
    }
}