using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletPooling bulletPooling;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject objectBullet;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private ParticleSystem spark;
    [SerializeField] public void TriggerBulletObject() => objectBullet.SetActive(true);
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
        yield return new WaitUntil(() => explosion.isStopped || spark.isStopped);
        yield return new WaitForSeconds(0.5f);
        rb.isKinematic = false;
        bulletPooling.ReturnBullet(gameObject);
    }
}
