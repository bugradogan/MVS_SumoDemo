using System.Collections;
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
    private float startTimer = 3f;
    private Dictionary<string, int> scoreList = new Dictionary<string, int>();
    private Player player;
    public int enemyCount;

    [HideInInspector] public bool isGameStarted = false;
    [HideInInspector] public bool isPlayerDead = false;
    [HideInInspector] public bool isGameOver = false;
    public Text timerText;
    public Text playerScoreText;
    public Transform scoreListPanel;
    public GameObject scoreListText;

    // Start is called before the first frame update
    void Start()
    {
        enemyCount = GameObject.FindObjectsOfType<EnemyAI>().Length;
        player = GameObject.FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        startTimer -= Time.deltaTime;
        timerText.text = startTimer.ToString("0");
        if (startTimer <= 0)
        {
            isGameStarted = true;
            startTimer = 0;
            timerText.gameObject.SetActive(false);
        }

        if (enemyCount == 0 && !isGameOver)
        {
            isGameOver = true;
            scoreList.Add(player.name, player.score);
            scoreListPanel.parent.gameObject.SetActive(true);
            int i = 1;
            foreach (KeyValuePair<string, int> player in scoreList.OrderByDescending(key => key.Value))
            {
                GameObject textObject = Instantiate(scoreListText, scoreListPanel);
                textObject.GetComponent<Text>().text = i + ". " + player.Key + " " + player.Value;
                i++;
            }


        }

        if (isPlayerDead)
        {
            scoreListPanel.parent.gameObject.SetActive(true);
            scoreListPanel.gameObject.SetActive(false);
            playerScoreText.text = "Your Score:\n" + player.score.ToString();
            playerScoreText.gameObject.SetActive(true);

        }

    }

    public void AddScoreToList(string key, int score)
    {
        scoreList.Add(key, score);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
