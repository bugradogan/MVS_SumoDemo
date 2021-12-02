using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    Camera cam;

    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float lerp = 2f;
    [SerializeField] private float maxScale = 8f;
    [SerializeField] private float attackForce = 3f;
    [SerializeField] private int killedEnemyScore = 1000;
    private float backForce = 3f;
    private EnemyAI currentEnemy;

    public LayerMask layer;

    public int score;
    public Text scoreText;
    private bool checkIsGround;
    private float checkTimer = 1.5f;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (checkIsGround)
        {
            checkTimer -= Time.deltaTime;
            if (currentEnemy.isDestroyed)
            {
                score += currentEnemy.score + killedEnemyScore;
                UpdateUI();
                GameManager.Instance.enemyCount--;
                checkIsGround = false;
                checkTimer = 1.5f;
            }
            if (checkTimer <= 0)
            {
                checkIsGround = false;
                checkTimer = 1.5f;
            }
        }

        if (!isGrounded())
        {
            GameManager.Instance.isPlayerDead = true;
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
            Movement();

        if (GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            if (rb.velocity.magnitude < 5f)
                rb.AddRelativeForce(Vector3.forward * movementSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }

    }

    void Movement()
    {
        Vector3 mousepos = Input.mousePosition;
        mousepos.z = cam.transform.localPosition.z;
        Ray ray = cam.ScreenPointToRay(mousepos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
        {
            Vector3 newPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            rb.transform.rotation = (Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newPoint - transform.position), rotationSpeed * Time.deltaTime));
        }
    }

    public void RemoveTarget(GameObject boost)
    {
        if (boost)
            BoostManager.Instance.boostList.Remove(BoostManager.Instance.boostList.Find(x => x.name == boost.gameObject.name));
    }   

    public void Scale()
    {
        if (transform.localScale.x < maxScale)
            transform.localScale += new Vector3(.25f, .25f, .25f);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AI"))
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * attackForce, ForceMode.Impulse);
            rb.AddForce(-transform.forward * (backForce * 2), ForceMode.Impulse);
            collision.gameObject.GetComponent<EnemyAI>().checkIsGround = true;
            checkIsGround = true;
            currentEnemy = collision.gameObject.GetComponent<EnemyAI>();
        }
    }

    public void InreaseScore()
    {
        score += 100;
        attackForce += .5f;
        backForce -= .25f;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = score.ToString();
    }

}
