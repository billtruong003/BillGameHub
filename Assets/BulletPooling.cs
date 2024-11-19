using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooling : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Queue<GameObject> bulletPool = new Queue<GameObject>();

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            Bullet bulletBehaviour = bullet.GetComponent<Bullet>();
            bulletBehaviour.bulletPooling = this;
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }
    public GameObject GetBullet()
    {
        if (bulletPool.Count == 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
        GameObject bulletFromPool = bulletPool.Dequeue();
        bulletFromPool.SetActive(true);
        return bulletFromPool;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}
