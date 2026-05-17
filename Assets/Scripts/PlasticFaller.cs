using UnityEngine;

public class PlasticFaller : MonoBehaviour
{
    public float speed = 3f;


        void Update()
    {
        if (FindObjectOfType<MinigameManager>().gameOver) return;
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < -6f) Destroy(gameObject);
    }
    

    void OnTriggerEnter2D(Collider2D other)
{
    
    if (other.CompareTag("Player"))
    {
        if (gameObject.CompareTag("Food"))
            FindObjectOfType<MinigameManager>().EatFood();
        else
            FindObjectOfType<MinigameManager>().HitByPlastic();
        Destroy(gameObject);
    }
}
}