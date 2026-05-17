using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

public class BoatRescue : MonoBehaviour
{
    public GameObject trappedManatee;
    public float releaseDistance = 4f;
    public HUDManager hud;

    public GameObject sphereCollider;


    private Vector3 startPos;
    private bool released = false;

void Start() {
    startPos = transform.position;
    if (PlayerPrefs.GetInt("ManateeRescued", 0) == 1)
    {
        released = true;
        sphereCollider.SetActive(false);
        trappedManatee.SetActive(false);
        transform.position = new Vector3(
            PlayerPrefs.GetFloat("BoatX", startPos.x),
            PlayerPrefs.GetFloat("BoatY", startPos.y),
            PlayerPrefs.GetFloat("BoatZ", startPos.z)
        );
        StartCoroutine(FreezeAfterSettle());
    }
}

IEnumerator FreezeAfterSettle()
{
    yield return new WaitForSeconds(0.5f);
    GetComponent<Rigidbody>().isKinematic = true;
}

    void Update()
    {
        if (released) return;
        if (Vector3.Distance(transform.position, startPos) > releaseDistance)
        {
            StartCoroutine(ReleaseManatees());
        }
    }

    IEnumerator ReleaseManatees()
    {
        released = true;
        float t = 0f;
        Vector3 startPos = trappedManatee.transform.position;
        Vector3 endPos = startPos + Vector3.up * 4f;
        
        while (t < 1f)
        {
            t += Time.deltaTime;
            trappedManatee.transform.position = 
                Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        hud.AddRescued();
        GetComponent<Rigidbody>().isKinematic = true;
        sphereCollider.SetActive(false);
        InteractionUI.Instance?.ShowPrompt("Manatee rescued!");
        yield return new WaitForSeconds(2f);
        InteractionUI.Instance?.HidePrompt();
        PlayerPrefs.SetInt("ManateeRescued", 1);
        // rememebr where the boat was
        PlayerPrefs.SetFloat("BoatX", transform.position.x);
        PlayerPrefs.SetFloat("BoatY", transform.position.y);
        PlayerPrefs.SetFloat("BoatZ", transform.position.z);
    }
}