using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Cinemachine;

public class HUDManager : MonoBehaviour
{
    [Header("Hearts")]
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    [Header("Timer")]
    public Image timerRadial;
    public float totalTime = 300f;
    public TMP_Text timerText;

    [Header("Rescued Counter")]
    public Image animalIcon;
    public TMP_Text rescuedText;
    public int totalAnimals = 2; // fish + manatee

    [Header("Plastic Counter")]
    public Image plasticIcon;
    public TMP_Text plasticText;
    public int totalPlastic = 6;

    public float timeLeft;
    public int currentHearts = 6;
    public int rescued = 0;
    public int plasticCleaned = 0;
    private bool timerRunning = true;
    private Image[] heartImages;

    [Header("Win Panel")]
    public GameObject winPanel;
    public GameObject losePanel;
    public TMP_Text loseTitleText;

    public GameObject introPanel;


   void Start()
{
    timeLeft = totalTime;
    heartImages = new Image[] { heart1, heart2, heart3 };

    if (PlayerPrefs.GetInt("ComingFromMinigame", 0) == 1)
    {
        // coming back from minigame, no intro
        introPanel.SetActive(false);
        timerRunning = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        timeLeft = PlayerPrefs.GetFloat("SavedTime", totalTime);
        currentHearts = PlayerPrefs.GetInt("SavedHearts", 6);
        rescued = PlayerPrefs.GetInt("SavedRescued", 0);
        plasticCleaned = PlayerPrefs.GetInt("SavedPlastic", 0);
        GameObject.FindWithTag("Player").transform.position = new Vector3(-144.7252f, 5.584428f, 22.77357f);
        PlayerPrefs.SetInt("ComingFromMinigame", 0);

        if (PlayerPrefs.GetInt("MinigameDied", 0) == 1)
        {
            PlayerPrefs.SetInt("MinigameDied", 0);
            currentHearts = 0;
            UpdateHearts();
            OnDeath();
            return;
        }
    }
    else
    {
        FindObjectOfType<ManateeController>().enabled = false;
        CinemachineCamera cam = FindObjectOfType<CinemachineCamera>();
        cam.enabled = false;
        // fresh start, show intro
        introPanel.SetActive(true);
        timerRunning = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerPrefs.SetInt("ManateeRescued", 0);
        for (int i = 1; i <= 6; i++)
            PlayerPrefs.DeleteKey($"Plastic_plastic{i}");
    }

    UpdateHearts();
    rescuedText.text = $"{rescued}/{totalAnimals}";
    plasticText.text = $"{plasticCleaned}/{totalPlastic}";
    timerText.text = FormatTime(timeLeft);
}

public void StartGame()
{
    FindObjectOfType<ManateeController>().enabled = true;
    FindObjectOfType<CinemachineCamera>().enabled = true;
    introPanel.SetActive(false);
    timerRunning = true;
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}

    void Update()
    {
        if (!timerRunning) return;
        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(0, timeLeft);
        timerRadial.fillAmount = timeLeft / totalTime;
        timerText.text = FormatTime(timeLeft);
        if (timeLeft <= 0)
        {
            timerRunning = false;
            OnTimerEnd();
        }

    }

    public void LoseHalfHeart()
    {
        currentHearts = Mathf.Max(0, currentHearts - 1);
        UpdateHearts();
        if (currentHearts <= 0) OnDeath();
    }

    public void LoseFullHeart()
    {
        currentHearts = Mathf.Max(0, currentHearts - 2);
        UpdateHearts();
        if (currentHearts <= 0) OnDeath();
    }


    public void AddRescued()
{
    rescued++;
    rescuedText.text = $"{rescued}/{totalAnimals}";
    CheckWin();
}

public void AddPlasticCleaned()
{
    plasticCleaned++;
    plasticText.text = $"{plasticCleaned}/{totalPlastic}";
    CheckWin();
}

void CheckWin()
{
    if (rescued >= totalAnimals && plasticCleaned >= totalPlastic)
        OnWin();
}


    void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            int heartValue = currentHearts - (i * 2);
            if (heartValue >= 2)
                heartImages[i].sprite = fullHeart;
            else if (heartValue == 1)
                heartImages[i].sprite = halfHeart;
            else
                heartImages[i].sprite = emptyHeart;
        }
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }

void OnWin()
{
    timerRunning = false;
    ShowWinPanel();

    PlayerPrefs.SetInt("RescuedManatees", 
        PlayerPrefs.GetInt("RescuedManatees", 0) + 1);
    PlayerPrefs.SetInt("RescuedFish", 
        PlayerPrefs.GetInt("RescuedFish", 0) + 1);
}

void OnDeath()
{
    timerRunning = false;
    loseTitleText.text = "Oh no! You ran out of health...";
    losePanel.SetActive(true);
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

void OnTimerEnd()
{
    timerRunning = false;
    loseTitleText.text = "Time's up! The river needs more help...";
    losePanel.SetActive(true);
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

    public void ShowWinPanel()
{
    winPanel.SetActive(true);
    timerRunning = false;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

public void SwimFreely()
{
    winPanel.SetActive(false);
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}

public void GoToMenu()
{
    PlayerPrefs.SetInt("RescuedCount", 
        PlayerPrefs.GetInt("RescuedCount", 0) + rescued);
    SceneManager.LoadScene("MainMenu");
}

public void RestartGame()
{
    SceneManager.LoadScene("MainScene");
}
}