using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;


public class FishRescueResult : MonoBehaviour
{
    public GameObject net;        // the net object to hide
    public GameObject fishFree;   // the free swimming fish to show
    public float swimSpeed = 2f;

    public GameObject sphereCollider; 


 void Start()
{
    int result = PlayerPrefs.GetInt("MinigameResult", -1);
    Debug.Log($"MinigameResult on load: {result}");
    Debug.Log($"ComingFromMinigame set, PlayerX: {PlayerPrefs.GetFloat("PlayerX")}");
    PlayerPrefs.SetInt("MinigameResult", -1);
    if (result == 1)
    {
        Debug.Log("Starting release coroutine");
        StartCoroutine(ReleaseDelay());
        sphereCollider.SetActive(false);
        

    }
}

    IEnumerator ReleaseDelay()
    {
        yield return new WaitForSeconds(0.1f);
        net.SetActive(false);
        fishFree.SetActive(true);
        fishFree.AddComponent<SwimAway>().speed = swimSpeed;
        FindObjectOfType<HUDManager>().AddRescued();
    }
}