using UnityEngine;

public class Boost : MonoBehaviour
{
    // Rotation speed for Boost object
    [SerializeField] float rotationSpeed = 2f;

    void Update()
    {
        // Rotation Boost object
        transform.Rotate(new Vector3(0f, rotationSpeed, 0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        // If player collect boost
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Scale();
            other.GetComponent<Player>().InreaseScore();
            other.GetComponent<Player>().RemoveTarget(gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("AI"))  // If AI collect boost
        {
            other.GetComponent<EnemyAI>().Scale();
            other.GetComponent<EnemyAI>().InreaseScore();
            other.GetComponent<EnemyAI>().RemoveTarget(gameObject);
            Destroy(gameObject);
        }
    }

}
