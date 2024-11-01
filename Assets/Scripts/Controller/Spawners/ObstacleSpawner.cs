using System;
using System.Collections.Generic;
using GameObjects.Boss;
using GameObjects.Obstacle;
using GameObjects.Road;
using Save.GameObjects.Obstacle;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Controller.Spawners
{
    [Serializable]
    public class ObstacleSpawner
    {
        public List<GameObject> obstacles = new List<GameObject>();
        public List<GameObject> createdObstacles = new List<GameObject>();

        public virtual void SpawnObject(SpawnManager spawnerManager)
        {
            var createdRoads = spawnerManager.roadSpawner.createdRoads;

            if (spawnerManager.GameManager.gamePropertiesInSave.currenLevel % 50 == 0)
            {
                SpawnBigBoom(createdRoads, spawnerManager);
            }
            else
            {
                var spawnPointAmount = createdRoads.Count * 4;
                var spawnAmount = 0;

                if (spawnerManager.roadSpawner.createdBossRoad.boss.GetType() == typeof(ChestBoss)) // chest spawns
                {
                    // % 10
                    spawnAmount = (spawnPointAmount * 10 / 100) + 1;

                }
                else // boss spawns
                {
                    // % 20
                    spawnAmount = (spawnPointAmount * 30 / 100) + 1;
                }
                
                Debug.Log(spawnAmount  + " " + spawnPointAmount);
                var maxSizeKindOfSpawner = GetMaxSize(spawnerManager);

                foreach (var road in createdRoads)
                {
                    if (spawnAmount <= 0)
                        break;

                    var spawnPoints = road.spawnPoints;

                    int randomIndex = Random.Range(0, 4); // 0 = spawnPoints[0], 1 = spawnPoints[1]

                    var selectedSpawnPoint1 = spawnPoints[randomIndex].spawnPoint; // Randomly selected spawn point

                    var spawn = obstacles[Random.Range(0, maxSizeKindOfSpawner)]; // You can adjust this to select a random obstacle if needed

                    var created1 = Object.Instantiate(spawn, selectedSpawnPoint1, true);
                    AdjustPositionToSpawnPoint(created1, selectedSpawnPoint1, spawnerManager);

                    createdObstacles.Add(created1);

                    road.spawnPoints[randomIndex].isObjSpawned = true;

                    spawnAmount--;
                }
            }
        }

        private void AdjustPositionToSpawnPoint(GameObject created, Transform spawnPoint, SpawnManager spawnerManager)
        {
            var obstacle = created.GetComponentInChildren<Obstacle>();
            obstacle.gameManager = spawnerManager.GameManager;

            var position = created.transform.position;
            var spawnPosition = spawnPoint.position;
    
            
            position.x = spawnPosition.x;
            position.z = spawnPosition.z;
            
            created.transform.position = position;

            obstacle.enabled = false;
            var rotatingPlatform = obstacle.GetComponent<RotatingPlatform>();
            if(rotatingPlatform != null)
                rotatingPlatform.enabled = false;

            // add additional rot to here thx
        }


        public void ResetObstacle()
        {
            foreach (var createdObstacle in createdObstacles)
            {
                Object.Destroy(createdObstacle);
            }
            createdObstacles.Clear();
        }

        public void SpawnBigBoom(List<Road> createdRoads, SpawnManager spawnerManager) // her 50 ile bolumunde bunu cagir?
        {
            var maxSizeKindOfSpawner = GetMaxSize(spawnerManager);

            foreach (var road in createdRoads)
            {
                var spawnPoints = road.spawnPoints;

                int randomIndex = Random.Range(0, 2); // 0 = spawnPoints[0], 1 = spawnPoints[1]
                int pairedIndex =
                    randomIndex == 0 ? 3 : 2; // If 0 is chosen, pair with 3 (index 3), else pair with 2 (index 2)

                var selectedSpawnPoint1 = spawnPoints[randomIndex].spawnPoint; // Randomly selected spawn point
                var selectedSpawnPoint2 = spawnPoints[pairedIndex].spawnPoint; // Paired spawn point

                var spawn = obstacles[Random.Range(0, maxSizeKindOfSpawner)]; // You can adjust this to select a random obstacle if needed

                var created1 = Object.Instantiate(spawn, selectedSpawnPoint1, true);
                AdjustPositionToSpawnPoint(created1, selectedSpawnPoint1, spawnerManager);

                spawn = obstacles[Random.Range(0, maxSizeKindOfSpawner)]; // You can adjust this to select a random obstacle if needed
                var created2 = Object.Instantiate(spawn, selectedSpawnPoint2, true);
                AdjustPositionToSpawnPoint(created2, selectedSpawnPoint2, spawnerManager);

                createdObstacles.Add(created1);
                createdObstacles.Add(created2);

                road.spawnPoints[randomIndex].isObjSpawned = true;
                road.spawnPoints[pairedIndex].isObjSpawned = true;
            }
            
        }

        private int GetMaxSize(SpawnManager spawnerManager)
        {
            var maxSizeKindOfSpawner = spawnerManager.GameManager.gamePropertiesInSave.currenLevel / 10 +1;
            if (maxSizeKindOfSpawner >= obstacles.Count)
            {
                maxSizeKindOfSpawner = obstacles.Count;
            }

            return maxSizeKindOfSpawner;
        }
    }
}