using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletPooling bulletPooling;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject objectBullet;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private ParticleSystem spark;
    private string tagCol;
    void OnTriggerEnter(Collider other)
    {
        tagCol = other.gameObject.tag;

        if (tagCol == "Reward")
        {
            Debug.Log("Đạn đã chạm vào Reward!");
            spark.Play();
        }
        else if (tagCol == "Obstacle")
        {
            Debug.Log("Đạn đã chạm vào Obstacle!");
            objectBullet.SetActive(false);
            rb.isKinematic = true;
            explosion.Play();
            StartCoroutine(ReturnBulletToPool());
        }
    }

    private IEnumerator ReturnBulletToPool()
    {
        yield return new WaitForSeconds(3f);
        bulletPooling.ReturnBullet(gameObject);
    }
}
