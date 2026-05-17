using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlasticCleanupGame : MonoBehaviour
{
    public static PlasticCleanupGame Instance;

    [Header("Overlay Panel")]
    public GameObject cleanupPanel;

    [Header("Skill Check")]
    public GameObject skillCheckPanel;
    public Image skillNeedle;
    public Image skillGreenZone;

    [Header("Button Mash")]
    public GameObject buttonMashPanel;
    public Image mashProgressBar;
    public TMP_Text mashText;

    [Header("Wobble")]
    public GameObject wobblePanel;
    public RectTransform wobbleDot;
    public RectTransform wobbleCircle;

    private PlasticInteractable currentPlastic;
    private int currentGame;
    private bool gameActive = false;

    // skill check
    private float needleAngle = 0f;
    private float needleSpeed = 120f;
    private int skillHits = 0;

    // button mash
    private float mashProgress = 0f;
    private float mashRequired = 10f;
    private float mashDrain = 1.5f;

    // wobble
    private Vector2 dotPos;
    private float wobbleTimer = 0f;
    private float wobbleDuration = 3f;

    void Awake() => Instance = this;

    void Start()
    {
        cleanupPanel.SetActive(false);
    }

    public void StartCleanup(PlasticInteractable plastic)
    {
        currentPlastic = plastic;
        currentGame = Random.Range(0, 3);
        cleanupPanel.SetActive(true);
        gameActive = true;

        skillCheckPanel.SetActive(currentGame == 0);
        buttonMashPanel.SetActive(currentGame == 1);
        wobblePanel.SetActive(currentGame == 2);

        // reset states
        skillHits = 0;
        needleAngle = 0f;
        mashProgress = 0f;
        dotPos = Vector2.zero;
        wobbleTimer = wobbleDuration;

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!gameActive) return;

        if (currentGame == 0) UpdateSkillCheck();
        else if (currentGame == 1) UpdateButtonMash();
        else UpdateWobble();
    }

    // --- SKILL CHECK ---
    void UpdateSkillCheck()
    {
        needleAngle += needleSpeed * Time.deltaTime;
        if (needleAngle > 360f) needleAngle -= 360f;
        skillNeedle.rectTransform.localRotation = Quaternion.Euler(0, 0, -needleAngle);

        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // green zone is roughly 60-120 degrees
            float normalized = needleAngle % 360f;
            if (normalized > 60f && normalized < 120f)
            {
                skillHits++;
                if (skillHits >= 3) CleanupSuccess();
            }
            else
            {
                CleanupFail();
            }
        }
    }

    // --- BUTTON MASH ---
    void UpdateButtonMash()
    {
        if (UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
            mashProgress += 2f;

        mashProgress -= mashDrain * Time.deltaTime;
        mashProgress = Mathf.Clamp(mashProgress, 0, mashRequired);
        mashProgressBar.fillAmount = mashProgress / mashRequired;
        mashText.text = $"Mash E! {Mathf.RoundToInt(mashProgress)}/{Mathf.RoundToInt(mashRequired)}";

        if (mashProgress >= mashRequired) CleanupSuccess();
    }

    // --- WOBBLE STABILIZER ---
    void UpdateWobble()
    {
        wobbleTimer -= Time.deltaTime;

        // drift dot randomly
        dotPos += new Vector2(
            Mathf.Sin(Time.time * 2.3f) * 80f,
            Mathf.Cos(Time.time * 1.7f) * 80f) * Time.deltaTime;

        // player controls with WASD
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb.aKey.isPressed) dotPos.x -= 150f * Time.deltaTime;
        if (kb.dKey.isPressed) dotPos.x += 150f * Time.deltaTime;
        if (kb.wKey.isPressed) dotPos.y += 150f * Time.deltaTime;
        if (kb.sKey.isPressed) dotPos.y -= 150f * Time.deltaTime;

        dotPos = Vector2.ClampMagnitude(dotPos, 120f);
        wobbleDot.anchoredPosition = dotPos;

        // check if dot is inside circle (radius ~40)
        if (dotPos.magnitude < 40f)
            wobbleTimer -= Time.deltaTime; // count down faster when inside!

        if (wobbleTimer <= 0) CleanupSuccess();

        // fail if dot hits edge
        if (dotPos.magnitude >= 119f) CleanupFail();
    }

    void CleanupSuccess()
    {
        gameActive = false;
        cleanupPanel.SetActive(false);
        currentPlastic.gameObject.SetActive(false);
        InteractionUI.Instance.HidePrompt();
        FindObjectOfType<HUDManager>()?.AddPlasticCleaned();
    }

    void CleanupFail()
    {
        gameActive = false;
        cleanupPanel.SetActive(false);
        FindObjectOfType<HUDManager>()?.LoseHalfHeart();
        InteractionUI.Instance.HidePrompt();
    }
}