using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 2f;   
    
   
    void Update()
    {
        transform.Rotate(new Vector3(0f, rotationSpeed, 0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Scale();
            other.GetComponent<Player>().InreaseScore();
            other.GetComponent<Player>().RemoveTarget(gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("AI"))
        {
            other.GetComponent<EnemyAI>().Scale();
            other.GetComponent<EnemyAI>().InreaseScore();
            other.GetComponent<EnemyAI>().RemoveTarget(gameObject);
            Destroy(gameObject);
        }
    }
}
