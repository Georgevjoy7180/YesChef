using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{   
    public static GameManager Instance { get; private set; }
    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Game Timer")]
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private TMP_Text gameTimerText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalHighScoreText;

    [Header("Start Screen")]
    [SerializeField] private GameObject startPanel;

    [SerializeField] private ChefMove chefMove;
    [SerializeField] private CustomerWindow[] customerWindows;

    [Header("Pause")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;

    private bool isPaused = false;
    private float timeRemaining;
    private bool gameActive;
    private int currentScore;
    private int highScore;
    private void Awake(){
        if(Instance!=null && Instance!= this){
            Destroy(gameObject);
            return;
        }
        Instance=this;
        highScore = PlayerPrefs.GetInt("HighScore",0);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {  
        currentScore = 0;

        timeRemaining = gameDuration;

        gameActive = false;
        isPaused = false;

        Time.timeScale = 1f;

        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        pauseButton.SetActive(false);
        scoreText.gameObject.SetActive(false);
        highScoreText.gameObject.SetActive(false);
        gameTimerText.gameObject.SetActive(false);

        UpdateScoreUI();
        UpdateTimerUI();

        chefMove.enabled = false;
    }
    public void StartGame()
    {
        startPanel.SetActive(false);

        timeRemaining = gameDuration;

        gameActive = true;
        isPaused = false;

        Time.timeScale = 1f;

        chefMove.enabled = true;

        pauseButton.SetActive(true);
        pausePanel.SetActive(false);
        scoreText.gameObject.SetActive(true);
        highScoreText.gameObject.SetActive(true);
        gameTimerText.gameObject.SetActive(true);

        foreach (CustomerWindow window in customerWindows)
        {
            window.StartFirstOrder();
        }

        UpdateTimerUI();
    }
    public void AddScore(int amount){
        if(!gameActive){
            return;
        }
        currentScore+=amount;
        if (currentScore > highScore)
        {
            highScore = currentScore;

            PlayerPrefs.SetInt("HighScore",highScore);

            PlayerPrefs.Save();
        }
        UpdateScoreUI();
    }
    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + currentScore;
        highScoreText.text ="High Score: " + highScore;
    }


    // Update is called once per frame
    private void Update()
    {
        if (!gameActive)
        return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame();
        }

        UpdateTimerUI();
    }
    private void UpdateTimerUI()
    {
        int totalSeconds = Mathf.CeilToInt(timeRemaining);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        gameTimerText.text =$"{minutes:00}:{seconds:00}";
    }
    public void PauseGame()
    {
        if (!gameActive || isPaused)
            return;

        isPaused = true;

        Time.timeScale = 0f;

        pausePanel.SetActive(true);
        pauseButton.SetActive(false);
    }
    public void ResumeGame()
    {
        if (!gameActive || !isPaused)
            return;

        isPaused = false;

        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
    }
    private void EndGame()
    {
        gameActive = false;
        isPaused = false;

        Time.timeScale = 1f;

        chefMove.StopMovement();

        pausePanel.SetActive(false);
        pauseButton.SetActive(false);

        gameOverPanel.SetActive(true);

        finalScoreText.text =
        "Final Score: " + currentScore;

        finalHighScoreText.text =
        "High Score: " + highScore;
    }
    public bool IsGameActive()
    {
        return gameActive;
    }
    public void RestartGame()
    {   Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitGame()
    {   Time.timeScale = 1f;

        Application.Quit();
    }
}
