using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Transform bulletPos;
    public float shootInterval = 2f;
    public float bulletSpeed = 5f; // 총알 속도

    private float timer;
    private ObjectPool objectPool;
    private PlayerManager playerManager;

    void Start()
    {
        objectPool = ObjectPool.Instance;
        playerManager = PlayerManager.Instance;
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager not found in the scene!");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > shootInterval)
        {
            timer = 0;
            Shoot();
        }
    }

    void Shoot()
{
    GameObject bullet = objectPool.GetFromPool("EnemyBall", bulletPos.position, Quaternion.identity);
    if (bullet != null)
    {
        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
        Bounce_Controller bounceController = bullet.GetComponent<Bounce_Controller>();
        
        if (enemyBullet != null)
        {
            enemyBullet.Initialize(bulletSpeed);
        }
        
        if (bounceController != null)
        {
            bounceController.ResetState();
        }
    }
}
}