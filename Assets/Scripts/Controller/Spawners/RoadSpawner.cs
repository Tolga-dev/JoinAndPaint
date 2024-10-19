using System;
using System.Collections.Generic;
using GameObjects.Boss;
using GameObjects.Road;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Controller.Spawners
{
    [Serializable]
    public class RoadSpawner
    {
        public GameManager gameManager;
        public GameObject road;
        
        public GameObject bossRoad;
        public GameObject chestBoss;
    
        [Header("Positions")]
        public Vector3 offset;

        [Header("Created")]
        public List<Road> createdRoads = new List<Road>();
        public BossRoad createdBossRoad;

        public int initAmount;
        public int startAmountOfRoad;
        public int spawnAmount;
        
        public void Init(GameManager gameManagerInGame)
        {
            gameManager = gameManagerInGame;
            startAmountOfRoad = initAmount;
        }
        
        public void SpawnNormalRoad()
        {
            var created = Object.Instantiate(road, startAmountOfRoad * offset,road.transform.rotation);
            var createdRoad = created.GetComponent<Road>();
            createdRoad.gameManager = gameManager;
            
            createdRoads.Add(createdRoad);
            
            SetNewPos();
        }

        public void SpawnBossObject()
        {
            GameObject[] bossOptions = { bossRoad }; // 5 turda bir chestBoss, her turda farkli sapkali boss, bazilrinda ozel chestler olacak
            
            var randomIndex = Random.Range(0, bossOptions.Length);
            var selectedBoss = bossOptions[randomIndex];

            var created = Object.Instantiate(selectedBoss, startAmountOfRoad * offset, selectedBoss.transform.rotation);
            
            createdBossRoad = created.GetComponent<BossRoad>();
            createdBossRoad.gameManager = gameManager;
            SpawnAccessories(createdBossRoad.boss);

            gameManager.cameraController.SetTarget(createdBossRoad.boss.transform, gameManager.cameraController.bossCam);
            SetNewPos();
        }

        private void SpawnAccessories(Boss boss)
        {
            var accessorTypes = gameManager.spawnerManager.prizeSpawner.accessorTypes;
            
            var randomAccessorType = accessorTypes.accessorTypes[Random.Range(0, accessorTypes.accessorTypes.Count)];
            var accessor = randomAccessorType.accessories[0];
            var currentAccessor = accessor.accessor;
            boss.damageAmount += accessor.power;

            switch (randomAccessorType.accessorType)
            {
                case AccessorTypesEnum.Head:
                    var accessorHead = boss.accessorHead;
                    var createdAccessor = Object.Instantiate(currentAccessor, accessorHead);
                    createdAccessor.transform.localPosition = Vector3.zero;
                    createdAccessor.GetComponent<MeshRenderer>().material = accessorTypes.materials[Random.Range(0, accessorTypes.materials.Count)];
                    break;
                case AccessorTypesEnum.LeftHand:
                    var accessorLeftHand = boss.accessorLeftHand;
                    var createdAccessorLeftHand = Object.Instantiate(currentAccessor, accessorLeftHand);
                    createdAccessorLeftHand.transform.localPosition = Vector3.zero;
                    createdAccessorLeftHand.GetComponent<MeshRenderer>().material = accessorTypes.materials[Random.Range(0, accessorTypes.materials.Count)];
                    break;
                case AccessorTypesEnum.RightHand:
                    var accessorRightHand = boss.accessorRightHand;
                    var createdAccessorRightHand = Object.Instantiate(currentAccessor, accessorRightHand);
                    createdAccessorRightHand.transform.localPosition = Vector3.zero;
                    createdAccessorRightHand.GetComponent<MeshRenderer>().material = accessorTypes.materials[Random.Range(0, accessorTypes.materials.Count)];
                    break;
            }
        }

        public void ResetRoads()
        {
            foreach (var createdRoad in createdRoads)
            {
                Object.Destroy(createdRoad.gameObject);
            }
            createdRoads.Clear();
            
            Object.Destroy(createdBossRoad.gameObject);
            createdBossRoad = null;
            
            startAmountOfRoad = initAmount;
        }
        
        public int GetNumberOfRoad()
        {
            var level = gameManager.gamePropertiesInSave.currenLevel;
            var numberOfRoadsToSpawn = spawnAmount;

            numberOfRoadsToSpawn += level / 10;
            
            return numberOfRoadsToSpawn;
        }
        
        public void SetNewPos()
        {
            startAmountOfRoad++;
        }
    }
}