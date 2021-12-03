using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Agent AI for movement
    private NavMeshAgent agent;
    // Enemy rigidbody
    private Rigidbody rb;
    // Enemy have a target?
    private bool haveTarget = false;
    // Enemy current target transform
    private Transform currentTarget;
    // Time to check if the enemy is inside the playing field after the collision
    private float checkTimer = 1.5f;

    [SerializeField] private float attackForce = 3f;  
    [SerializeField] private float backForce = 3f;
    // Enemy Speed
    [SerializeField] private float movementSpeed = 2f;
    // Max Scale
    [SerializeField] private float maxScale = 8f;
    // Check the enemy's ground contact after collision
    [HideInInspector] public bool checkIsGround;
    // Check the enemy's destroyed from player object
    [HideInInspector] public bool isDestroyed;
    // Enemy Score
    [HideInInspector] public int score;
    // Enemy ground layer
    public LayerMask layer;

    // Start is called before the first frame update
    void Start()
    {
        // Get Agent Compenent and set speed
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;    
        
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check game started
        if (GameManager.Instance.isGameStarted)
        {
            // Check boost count in the scene and enemy have a target
            if (BoostManager.Instance.boostList.Count > 0 && !haveTarget)
            {
                // Get Nearest Target Boost
                currentTarget = GetClosestBoost(BoostManager.Instance.boostList.ToArray());
                // Set agent Target Boost
                agent.SetDestination(currentTarget.transform.position);
                haveTarget = true;

            }
            // If there is a target but the target disappeared before reaching the target set enemy no target
            if (haveTarget && !currentTarget)
            {
                haveTarget = false;
            }
        }
        // If the enemy is inside the playing field after the collision
        if (checkIsGround)
        {
            checkTimer -= Time.deltaTime;
            if (!isGrounded()) // If not the enemy is inside field send score to table and destroy
            {
                isDestroyed = true;
                GameManager.Instance.AddScoreToList(gameObject.name, score);
                // Decrease enemy count number
                GameManager.Instance.enemyCount--;
                Destroy(gameObject);
            }
            // Set default to timer
            if (checkTimer <= 0)
            {
                checkIsGround = false;
                checkTimer = 1.5f;
            }
        }
        else // If the enemy leaves the field of play without collision
        {
            if(!isGrounded())
            {
                GameManager.Instance.AddScoreToList(gameObject.name, score);
                // Decrease enemy count number
                GameManager.Instance.enemyCount--;
                Destroy(gameObject);
            }

        }
        // If real player is dead destroy gameobject
        if(GameManager.Instance.isPlayerDead)
        {                      
            Destroy(gameObject);
        }
    }

    // If AI collect boost remove in the list
    public void RemoveTarget(GameObject boost)
    {
        if(boost)
        {
            BoostManager.Instance.boostList.Remove(BoostManager.Instance.boostList.Find(x => x.name == boost.gameObject.name));
            haveTarget = false;
        }        
    }

    // Check the enemy's ground contact
    bool isGrounded()
    {
        Ray checkGround = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(checkGround, out hit, 10f, layer))
        {
            return true;
        }

        return false;
    }

    // Scale the enemy when you collect Boosts
    public void Scale()
    {
        if (transform.localScale.x < maxScale)
            transform.localScale += new Vector3(.25f, .25f, .25f);
    }

    // Inrease Score to AI
    public void InreaseScore()
    {
        score += 100;       
    }

    // Get closest boost in the scene
    Transform GetClosestBoost(Transform[] boosts)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in boosts)
        {
            if (potentialTarget)
            {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }

        }

        return bestTarget;
    }

    // If player collision to enemy
    private void OnCollisionEnter(Collision collision)
    {
        // If player collision to enemy
        if (collision.gameObject.CompareTag("AI") || collision.gameObject.CompareTag("Player"))
        {
            // After collision push to forward enemy
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * attackForce, ForceMode.Impulse);
            // After collision push to back player
            rb.AddForce(-transform.forward * (backForce * 2), ForceMode.Impulse);
            
            
        }
    }  

}
