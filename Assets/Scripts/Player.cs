using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Rigidbody compenent player
    private Rigidbody rb;
    // For main camera
    private Camera cam;

    //Player settings 
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float maximumVelocity = 5f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float maxScale = 8f;
    [SerializeField] private float attackForce = 3f;
    [SerializeField] private int killedEnemyScore = 1000;
    [SerializeField] private float backForce = 3f;

    // Player's target enemy
    private EnemyAI currentEnemy;
    // Ground check layer for mouse input
    public LayerMask layer;

    // Player score and text object
    public int score;
    public Text scoreText;
    // Check the enemy's ground contact after collision
    private bool checkIsGround;
    // Check time the enemy's ground contact after collision
    private float checkTimer = 1.5f;


    void Start()
    {
        // Get Compenents
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    private void Update()
    {
        // Check the enemy's ground contact after collision
        if (checkIsGround)
        {
            checkTimer -= Time.deltaTime;
            // If current enemy destroyed
            if (currentEnemy.isDestroyed)
            {
                // Add score to player 
                score += currentEnemy.score + killedEnemyScore;
                // Update Score
                UpdateUI();
                checkIsGround = false;
                checkTimer = 1.5f;
            }
            if (checkTimer <= 0)
            {
                checkIsGround = false;
                checkTimer = 1.5f;
            }
        }
        // If the player leaves the field of play destroy and game over
        if (!isGrounded())
        {
            GameManager.Instance.isPlayerDead = true;
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        // If player click mouse left button
        if (Input.GetMouseButton(0))
            Movement();

        // If the game starts and does not end, add force the player in a straight direction until the speed is 5.
        if (GameManager.Instance.isGameStarted && !GameManager.Instance.isGameOver)
        {
            if (rb.velocity.magnitude < maximumVelocity)
                rb.AddRelativeForce(Vector3.forward * movementSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }

    }

    // For mouse position
    void Movement()
    {
        // Get mouse position
        Vector3 mousepos = Input.mousePosition;
        // Set mouse position z to camera local position z
        mousepos.z = cam.transform.localPosition.z;
        // Return a ray going from camera through a screen point.
        Ray ray = cam.ScreenPointToRay(mousepos);

        RaycastHit hit;
        // If ray touch to ground object
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
        {
            // Get touch point position x and z
            Vector3 newPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            // Set rotation to player 
            rb.transform.rotation = (Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newPoint - transform.position), rotationSpeed * Time.deltaTime));
        }
    }

    // If player collects boost, remove boost from list
    public void RemoveTarget(GameObject boost)
    {
        if (boost)
            BoostManager.Instance.boostList.Remove(BoostManager.Instance.boostList.Find(x => x.name == boost.gameObject.name));
    }
    // Scale up player
    public void Scale()
    {
        if (transform.localScale.x < maxScale)
            transform.localScale += new Vector3(.25f, .25f, .25f);
    }

    // Check player touch ground
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

    // If player collision to enemy
    private void OnCollisionEnter(Collision collision)
    {
        // If player collision to enemy
        if (collision.gameObject.CompareTag("AI"))
        {
            // After collision push to forward enemy
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * attackForce, ForceMode.Impulse);
            // After collision push to back player
            rb.AddForce(-transform.forward * (backForce * 2), ForceMode.Impulse);
            //  Check enemy touch ground
            collision.gameObject.GetComponent<EnemyAI>().checkIsGround = true;
            //  Check enemy touch ground
            checkIsGround = true;
            // set collided target as enemy
            currentEnemy = collision.gameObject.GetComponent<EnemyAI>();
        }
    }

    // Increase score and attack power after boost collection
    public void InreaseScore()
    {
        score += 100;
        attackForce += .5f;
        backForce -= .25f;
        UpdateUI();
    }

    // Update Score UI
    void UpdateUI()
    {
        scoreText.text = score.ToString();
    }

}
