using System.Collections.Generic;
using UnityEngine;

public class BoostManager : MonoBehaviour
{
    #region SINGLETON PATTERN
    public static BoostManager _instance;
    public static BoostManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BoostManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("BoostManager");
                    _instance = container.AddComponent<BoostManager>();
                }
            }
            return _instance;
        }
    }
    #endregion
    // Boost Prefab object
    public GameObject boostPrefab;
    // Maximum field to instantiate
    public float radius;
    // First spawn boost count
    public int firstSpawnCount = 10;   
  
    //  list in the scene boost object
    public List<Transform> boostList = new List<Transform>();
 

    void Awake()
    {
        // Spawn firstSpawnCount boost object in the field
        BoostSpawn(firstSpawnCount);
    }

    // Spawn firstSpawnCount boost object in the field
    void BoostSpawn(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Random position in the field
            Vector3 randomPosition = Random.insideUnitSphere * radius + transform.position;
            // Set random position Y axis to 1
            randomPosition.y = 1f;
            // Spawn boost object random position
            GameObject boost = Instantiate(boostPrefab, randomPosition, Quaternion.identity,transform);
            // Set boost name random
            boost.name = "Boost" + Random.Range(0,10000);
            // Add  boost transform to list
            boostList.Add(boost.transform);
        }
    }

    // If boost count below to 5 spawn 6 new objects
    private void Update()
    {
        if(boostList.Count < 5)
        {
            BoostSpawn(10);
        }
    }

    // For field visualization in the scene
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
