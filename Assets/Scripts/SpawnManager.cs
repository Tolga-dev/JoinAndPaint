using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // spawn ground prefab
    public GameObject groundPrefab;
    public Vector3 offset;
    public int spawnAmount;
    public Transform playerInitialPosition;
    // spawn boss ground
    public GameObject bossPrefab;

    public List<GameObject> grounds = new List<GameObject>();
    private void Start()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            grounds.Add(Instantiate(groundPrefab, offset * i, Quaternion.identity));
        }
        grounds.Add(Instantiate(bossPrefab, offset * spawnAmount, Quaternion.identity));
    }
    
    
    public void CleanGround()
    {
        foreach (var ground in grounds)
        {
            Destroy(ground);
        }
        grounds.Clear();
    }
}