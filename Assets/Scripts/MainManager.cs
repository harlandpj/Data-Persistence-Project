using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance; // shared by all instances

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighScoreText; // field to display high score name and high score
    public Text CurrentLevel; // level number

    public GameObject GameOverText;
    public static string m_NameEntered; // name entered in main gui menu

    private bool m_Started = false;
    
    private int m_Points; // current points
    private int m_HighScore; // high score 

    private string m_HighScoreName;

    private TMP_InputField userName; // user name input on GUI

    private int m_Level; // current level
    private int m_HighLevel; // highest level 

    private bool m_GameOver = false;

    public AudioClip levelCompleted;

    [System.Serializable]
    class SaveData
    {
        public string Name;
        public int HighScore;
        public int LevelReached;
    }

    // setup the static object and retain data on loading
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject); // ok was going to implement a return to main screen to change player!
        LoadSaveData(); // load in saved game data
        CurrentLevel.text = "Level: " + m_Level.ToString();
        ScoreText.text = $"{m_NameEntered}: {m_Points}";
    }

    public void SaveUserData()
    {
        // save game data if it's a new high score
        SaveData data = new SaveData();

        if (m_Points > m_HighScore)
        {
            // save the new high score to file
            data.HighScore = m_Points;
            data.Name = m_NameEntered;
            data.LevelReached = 1; // for now

            // convert to JSON format and save to file
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        }
    }

    public void LoadSaveData()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        Debug.Log(path);

        if (File.Exists(path))
        {
            // we have played before so load saved data
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            m_HighScore = data.HighScore;
            m_HighScoreName = data.Name;
            m_Level = data.LevelReached;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        DrawBricks();
        DisplayHighScore();
        DisplayLevel();
    }

    private void DrawBricks()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void DisplayLevel()
    {
        CurrentLevel.text = "Level: " + m_Level.ToString();
    }

    private void UpdateLevelNumber()
    {
        m_Level += 1;
        DisplayLevel();
    }

    private void DisplayHighScore()
    {
        HighScoreText.text = $"Best Score: {m_HighScoreName}, {m_HighScore}";
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"{m_NameEntered}: {m_Points}";
        CheckHighScore();
        CheckAllDestroyed();
    }

    private void CheckAllDestroyed()
    {
        // check if all bricks destroyed
        GameObject[] bricksLeft = GameObject.FindGameObjectsWithTag("Brick");
        int visibleBricks = 0;

        foreach (GameObject brick in bricksLeft)
        {
            if (brick.activeInHierarchy)
            {
                visibleBricks++;
            }
        }

        if (visibleBricks == 1)
        {
            // as called after last is hit, so will be one left at this point
            Debug.Log("All destroyed");

            gameObject.GetComponent<AudioSource>().PlayOneShot(levelCompleted);
            DrawBricks();
            UpdateLevelNumber();
        }
        else 
        {
            Debug.Log($"bricks left {bricksLeft.Length}");
        }
    }

    public void GameOver()
    {
        CheckHighScore();
        m_GameOver = true;
        GameOverText.SetActive(true);
        SaveUserData();
    }

    private void CheckHighScore()
    {
        if (m_Points > m_HighScore)
        {
            HighScoreText.text = $"Best Score: {m_NameEntered} : {m_Points}";
            m_HighScoreName = m_NameEntered;
        }
    }
}
