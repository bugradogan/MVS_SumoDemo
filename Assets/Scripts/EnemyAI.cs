using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private List<Transform> boostList = new List<Transform>();
    private NavMeshAgent agent;   
    private bool haveTarget = false;
    private Transform currentTarget;
    private float checkTimer = 1.5f;

    [SerializeField] private float movementSpeed = 2f;

    public LayerMask layer;
    [HideInInspector] public bool checkIsGround;
    [HideInInspector] public bool isDestroyed;
    [HideInInspector] public int score;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;       
        boostList = BoostManager.Instance.boostList;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameStarted)
        {
            if (boostList.Count > 0 && !haveTarget)
            {
                // Get Nearest Target Boost
                currentTarget = GetClosestBoost(boostList.ToArray());
                agent.SetDestination(currentTarget.transform.position);
                haveTarget = true;

            }
            if (haveTarget && !currentTarget)
            {
                haveTarget = false;
            }
        }        

        if (checkIsGround)
        {
            checkTimer -= Time.deltaTime;
            if (!isGrounded())
            {
                isDestroyed = true;
                GameManager.Instance.AddScoreToList(gameObject.name, score);
                Destroy(gameObject);
            }
            if (checkTimer <= 0)
            {
                checkIsGround = false;
                checkTimer = 1.5f;
            }
        }

        if(GameManager.Instance.isPlayerDead)
        {                      
            GameManager.Instance.AddScoreToList(gameObject.name, score);
            Destroy(gameObject);
        }
    }

    public void RemoveTarget(GameObject boost)
    {
        if(boost)
        {
            boostList.Remove(boostList.Find(x => x.name == boost.gameObject.name));
            haveTarget = false;
        }        
    }

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

    public void Scale()
    {
        if (transform.localScale.x < 7f)
            transform.localScale += new Vector3(.25f, .25f, .25f);
    }
    public void InreaseScore()
    {
        score += 100;       
    }

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



}
