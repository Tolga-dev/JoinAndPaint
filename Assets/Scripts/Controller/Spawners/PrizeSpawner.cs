using System;
using System.Collections.Generic;
using GameObjects.Prizes;
using GameObjects.Road;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace Controller.Spawners
{
    public enum AccessorTypesEnum
    {
        Head,
        LeftHand,
        RightHand
    }
    [Serializable]
    public class Accessor
    {
        public GameObject accessor;
        public int power;
    }

    [Serializable]
    public class AccessorType
    {
        public AccessorTypesEnum accessorType;
        public List<Accessor> accessories = new List<Accessor>();
    }
    
    
    [Serializable]
    public class AccessorTypes
    {
        public List<Material> materials = new List<Material>();
        public List<AccessorType> accessorTypes = new List<AccessorType>();
        public AccessorType GetType(AccessorTypesEnum accessorTypesEnum)
        {
            return accessorTypes.Find(x => x.accessorType == accessorTypesEnum);
        }
        
    }

    [Serializable]
    public class PrizeSpawner
    {
        [Header("Source")]
        public List<GameObject> prizes = new List<GameObject>();
        
        [Header("Created")]
        public List<GameObject> createdPrizes = new List<GameObject>();
        public List<GameObject> createdMembers = new List<GameObject>();
        
        public GameObject recruitment;
        
        public GameObject goodSelector;
        public Sprite badSprite;
        
        private SpawnManager _spawnerManager;
        private GameManager _gameManager;

        private int _totalMaxRange;
        private int[] _prizeRanges;
        public int additionalPoint;
        
        public AccessorTypes accessorTypes;

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

                    var index = Random.Range(0, 3);
                    
                    if (index == 1) // 50% chance to spawn
                    {
                        CreatePrize(road.spawnPoints[i].spawnPoint);
                    }
                    else if (index == 2)
                    {
                        CreateSelector(road.spawnPoints[i].spawnPoint);
                    }
                }
            }
        }

        private void CreateSelector(Transform spawnPoint)
        {
            var selector = goodSelector;
            
            var prize = Object.Instantiate(selector, spawnPoint, true);
            var position = spawnPoint.position;
            var transform = prize.transform;
            transform.position = new Vector3(position.x, transform.position.y, position.z);
            createdPrizes.Add(prize.gameObject);

            var selectorController = prize.GetComponentInChildren<Selector>();
            selectorController.gameManager = _gameManager;
            selectorController.prizeAmount = 20;
            selectorController.SetText();
            
            var randomCurrentSelector = Random.Range(0, 2);
            
            if(randomCurrentSelector == 1)
                selectorController.selectorEnum = SelectorEnum.Good;
            else
            {
                selectorController.selectorEnum = SelectorEnum.Bad;
                selectorController.gateSprite.sprite = badSprite;
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

        public void CreateMemberFromSelector(int prizeAmount, Transform spawnPoint)
        {
            CreateMember(spawnPoint,prizeAmount);   
        }
        
        private void CreateMember(Transform spawnPoint, int spawnCount = 0)
        {
            Debug.Log("SpawnMan");

            var canIAddToNewMembers = true;
            if (spawnCount == 0)
            {
                spawnCount = Random.Range(2, 5);
                canIAddToNewMembers = false;
            }
 
            var positionOffsets = new float[spawnCount];

            for (int i = 0; i < spawnCount; i++)
            {
                positionOffsets[i] = (i % 2 == 0 ? 1 : -1) * (i / 16f);
            }
            
            for (int i = 0; i < spawnCount; i++)
            {
                var prize = Object.Instantiate(recruitment, spawnPoint, true);
                var createdRecruitment = prize.GetComponent<Recruitment>();
                createdRecruitment.Freeze();
                createdRecruitment.gameManager = _gameManager;
                
                var randomAccessorType = accessorTypes.accessorTypes[Random.Range(0, accessorTypes.accessorTypes.Count)];
                var accessor = randomAccessorType.accessories[0];
                var currentAccessor = accessor.accessor;
                createdRecruitment.damageAmount += accessor.power;

                switch (randomAccessorType.accessorType)
                {
                    case AccessorTypesEnum.Head:
                        var accessorHead = createdRecruitment.accessorHead;
                        var createdAccessor = Object.Instantiate(currentAccessor, accessorHead);
                        createdAccessor.transform.localPosition = Vector3.zero;
                        createdAccessor.GetComponent<MeshRenderer>().material = accessorTypes.materials[Random.Range(0, accessorTypes.materials.Count)];
                        break;
                    case AccessorTypesEnum.LeftHand:
                        var accessorLeftHand = createdRecruitment.accessorLeftHand;
                        var createdAccessorLeftHand = Object.Instantiate(currentAccessor, accessorLeftHand);
                        createdAccessorLeftHand.transform.localPosition = Vector3.zero;
                        createdAccessorLeftHand.GetComponent<MeshRenderer>().material = accessorTypes.materials[Random.Range(0, accessorTypes.materials.Count)];
                        break;
                    case AccessorTypesEnum.RightHand:
                        var accessorRightHand = createdRecruitment.accessorRightHand;
                        var createdAccessorRightHand = Object.Instantiate(currentAccessor, accessorRightHand);
                        createdAccessorRightHand.transform.localPosition = Vector3.zero;
                        createdAccessorRightHand.GetComponent<MeshRenderer>().material = accessorTypes.materials[Random.Range(0, accessorTypes.materials.Count)];
                        break;
                }

                var position = spawnPoint.position;
                var randomXOffset = Random.Range(-0.5f, 0.5f);
                
                if (canIAddToNewMembers)
                    prize.transform.position = new Vector3(position.x + randomXOffset, prize.transform.position.y, position.z);
                else
                   prize.transform.position = new Vector3(position.x + randomXOffset, prize.transform.position.y, position.z + positionOffsets[i]);
                    
                createdMembers.Add(prize);
                if (canIAddToNewMembers)
                    _gameManager.memberManager.AddNewMember(prize.transform);
            }
        }
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

        public void RemoveMember(int prizeAmount)
        {
            for (int i = 0; i < prizeAmount; i++)
            {
                if (_gameManager.playerManager.members.Count == 1) // 1 is player ehe
                    break;
                
                _gameManager.memberManager.DestroyNewMember(_gameManager.playerManager.members[1].transform);
            }
        }
    }
}