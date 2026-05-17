using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using IEnumerator = System.Collections.IEnumerator;

public class MinigameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float playerSpeed = 5f;
    public float leftLimit = -10f;
    public float rightLimit = 10f;

    [Header("Plastic Spawning")]
    public GameObject[] plasticPrefabs;
    public float spawnInterval = 1.5f;
    public float fallSpeed = 3f;

    [Header("Food")]
    public GameObject[] foodPrefabs;
    public float foodSpawnInterval = 2f;
    public int foodRequired = 6;
    public TMP_Text foodText;

    [Header("Game Settings")]
    public float gameDuration = 20f;
    public TMP_Text timerText;

    [Header("Hearts UI")]
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Image timerRadial;

    public Transform spawnParent;

    [Header("Panels")]
    public GameObject successPanel;
    public GameObject failPanel;

    private float timeLeft;
    private int lives = 3;
    public bool gameOver = false;
    private float spawnTimer;
    private float foodSpawnTimer;
    private int foodEaten = 0;



    [Header("Intro")]
    public GameObject introPanel;
    public TMP_Text introCountdownText;
    public float introDelay = 10f;
    public TMP_Text failHeartText;

    private bool introActive = true;

    void Start()
    {

        introPanel.SetActive(true);
        successPanel.SetActive(false);
        failPanel.SetActive(false);
        UpdateHearts();
        if (foodText) foodText.text = $"Food: 0/{foodRequired}";
        timeLeft = gameDuration;
        StartCoroutine(IntroCountdown());

    }

    void Update()
    {
        if (introActive) return;
        if (gameOver) return;

        timeLeft -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        if (timeLeft <= 0) { Fail(); return; }

        float input = 0f;
        if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed) input = -1f;
        if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed) input = 1f;
        Vector3 pos = player.position;
        pos.x += input * playerSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        player.position = pos;

        if (input != 0)
        {
            Vector3 scale = player.localScale;
            scale.x = input > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            player.localScale = scale;
        }

        timerRadial.fillAmount = timeLeft / gameDuration;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0) { SpawnPlastic(); spawnTimer = spawnInterval; }

        foodSpawnTimer -= Time.deltaTime;
        if (foodSpawnTimer <= 0) { SpawnFood(); foodSpawnTimer = foodSpawnInterval; }
    }

    void SpawnPlastic()
    {
        int index = Random.Range(0, plasticPrefabs.Length);
        float x = Random.Range(leftLimit, rightLimit);
        GameObject p = Instantiate(plasticPrefabs[index],
            new Vector3(x, 6f, 0f), Quaternion.identity,spawnParent);
            
        p.tag = "Plastic";
        p.AddComponent<PlasticFaller>().speed = fallSpeed;
    }

    void SpawnFood()
    {
        int index = Random.Range(0, foodPrefabs.Length);
        float x = Random.Range(leftLimit, rightLimit);
        GameObject f = Instantiate(foodPrefabs[index],
            new Vector3(x, 6f, 0f), Quaternion.identity,spawnParent);
        f.tag = "Food";
        f.AddComponent<PlasticFaller>().speed = fallSpeed * 0.7f;
    }

    public void HitByPlastic()
    {
        lives--;
        UpdateHearts();
        if (lives <= 0) Fail();
    }

    public void EatFood()
    {
        foodEaten++;
        if (foodText) foodText.text = $"Food: {foodEaten}/{foodRequired}";
        if (foodEaten >= foodRequired) Success();
    }

    void UpdateHearts()
    {
        heart1.sprite = lives >= 1 ? fullHeart : emptyHeart;
        heart2.sprite = lives >= 2 ? fullHeart : emptyHeart;
        heart3.sprite = lives >= 3 ? fullHeart : emptyHeart;
    }

    void Success()
    {
        gameOver = true;
        successPanel.SetActive(true);
        PlayerPrefs.SetInt("MinigameResult", 1);
    }


void Fail()
{
    gameOver = true;
    int hearts = PlayerPrefs.GetInt("SavedHearts", 6);
    hearts = Mathf.Max(0, hearts - 1);
    PlayerPrefs.SetInt("SavedHearts", hearts);
    PlayerPrefs.SetInt("MinigameResult", 0);

    float heartsRemaining = hearts / 2f;
string heartDisplay = heartsRemaining % 1 == 0 
    ? $"{(int)heartsRemaining}" 
    : $"{(int)heartsRemaining + 0.5f}";
failHeartText.text = $"This cost you half a heart!\nHearts remaining: {heartDisplay}";

    if (hearts <= 0)
    {
        PlayerPrefs.SetInt("MinigameDied", 1);
        ExitMinigame();
        return;
    }

    failPanel.SetActive(true);
}

    public void ExitMinigame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void RestartMinigame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator IntroCountdown()
    {
        float t = introDelay;
        while (t > 0)
        {
            introCountdownText.text = $"Game starts in {Mathf.CeilToInt(t)}...";
            t -= Time.deltaTime;
            yield return null;
        }
        introPanel.SetActive(false);
        introActive = false;
    }
}