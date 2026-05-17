using UnityEngine;
using UnityEngine.SceneManagement;

public class FishRescueTrigger : MonoBehaviour
{
    public string minigameScene = "MinigameScene";

   private bool triggered = false;

    void OnTriggerEnter(Collider other){
            if (triggered) return;
            if (other.CompareTag("Player")){
            triggered = true;
    
            PlayerPrefs.SetInt("MinigameResult", -1);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PlayerPrefs.SetInt("ComingFromMinigame", 1);
            PlayerPrefs.SetFloat("SavedTime", FindObjectOfType<HUDManager>().timeLeft);
            PlayerPrefs.SetInt("SavedHearts", FindObjectOfType<HUDManager>().currentHearts);
            PlayerPrefs.SetInt("MinigameDied", 0);
            PlayerPrefs.SetInt("SavedRescued", FindObjectOfType<HUDManager>().rescued);
            PlayerPrefs.SetInt("SavedPlastic", FindObjectOfType<HUDManager>().plasticCleaned);
            
            SceneManager.LoadScene(minigameScene);
        }
    }
}