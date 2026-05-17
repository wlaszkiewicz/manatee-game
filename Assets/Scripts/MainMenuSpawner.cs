using UnityEngine;

public class MainMenuSpawner : MonoBehaviour
{
    public GameObject manatee;
    public GameObject fish;

    void Start()
    {
        manatee.SetActive(PlayerPrefs.GetInt("RescuedManatees", 0) > 0);
        fish.SetActive(PlayerPrefs.GetInt("RescuedFish", 0) > 0);
    }
}