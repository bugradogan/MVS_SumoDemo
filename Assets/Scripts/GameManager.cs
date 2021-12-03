using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region SINGLETON PATTERN
    public static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("GameManager");
                    _instance = container.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    #endregion
    // Start game timer
    private float startTimer = 3f;
    // Dictionary List for Score Table
    private Dictionary<string, int> scoreList = new Dictionary<string, int>();
    private Player player;
    // Total AI enemy
    public int enemyCount;

    // Some game logic
    [HideInInspector] public bool isGameStarted = false;
    [HideInInspector] public bool isPlayerDead = false;
    [HideInInspector] public bool isGameOver = false;

    // Start Timer Text Object
    public Text timerText;
    //Player Score Text Object
    public Text playerScoreText;
    // Panel object to list scores
    public Transform scoreListPanel;
    // Score table text object
    public GameObject scoreListText;

   
    void Start()
    {
        // Get AI count in the scene
        enemyCount = GameObject.FindObjectsOfType<EnemyAI>().Length;
        // Get player
        player = GameObject.FindObjectOfType<Player>();
    }

   
    void Update()
    {
        // Start Timer
        startTimer -= Time.deltaTime;
        timerText.text = startTimer.ToString("0");
        if (startTimer <= 0)
        {
            isGameStarted = true;
            startTimer = 0;
            timerText.gameObject.SetActive(false);
        }
        // End Timer

        // If all enemies are destroyed
        if (enemyCount == 0 && !isGameOver)
        {
            isGameOver = true;
            // Adding the player's name and score to the list
            scoreList.Add(player.name, player.score);
            // Set active gameover panel
            scoreListPanel.parent.gameObject.SetActive(true);
            int i = 1;
            // Sorting players by score
            foreach (KeyValuePair<string, int> player in scoreList.OrderByDescending(key => key.Value))
            {
                GameObject textObject = Instantiate(scoreListText, scoreListPanel);
                textObject.GetComponent<Text>().text = i + ". " + player.Key + " " + player.Value;
                i++;
            }
        }
        // Show score and endgame panel if player dies before game is over
        if (isPlayerDead)
        {
            scoreListPanel.parent.gameObject.SetActive(true);
            scoreListPanel.gameObject.SetActive(false);
            playerScoreText.text = "Your Score:\n" + player.score.ToString();
            playerScoreText.gameObject.SetActive(true);
        }

    }

    // Adding the name and score to the list
    public void AddScoreToList(string key, int score)
    {
        scoreList.Add(key, score);
    }

    // Restart game
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
