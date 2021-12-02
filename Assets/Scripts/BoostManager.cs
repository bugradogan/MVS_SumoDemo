using System.Collections;
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
    public GameObject boostPrefab;
    public float radius;
    public int firstSpawnCount = 10;
    public List<Transform> boostList = new List<Transform>();
    // Start is called before the first frame update
    void Awake()
    {
        BoostSpawn(10);
    }

    void BoostSpawn(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * radius;
            randomPosition.y = 1f;
            GameObject boost = Instantiate(boostPrefab, randomPosition, Quaternion.identity,transform);
            boost.name = "Boost" + Random.Range(0,1000);
            boostList.Add(boost.transform);
        }
    }


    private void Update()
    {
        if(boostList.Count < 5)
        {
            BoostSpawn(6);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
