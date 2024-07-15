using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private static ObjectPool objectPool;

    private Rigidbody2D rb;
    public float lifetime = 10f;
    private float timer;
    private float speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (objectPool == null)
        {
            objectPool = ObjectPool.Instance;
        }
    }

    private void OnEnable()
    {
        timer = 0;
    }

    public void Initialize(float bulletSpeed)
    {
        speed = bulletSpeed;
        if (PlayerManager.Instance != null)
        {
            Vector3 playerPosition = PlayerManager.Instance.GetPlayerPosition();
            Vector3 direction = (playerPosition - transform.position).normalized;
            rb.velocity = direction * speed;

            float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 90);
        }
        else
        {
            Debug.LogWarning("플레이어 매니저 못찾음");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > lifetime)
        {
            objectPool.ReturnToPool(gameObject);
        }
    }

    private void OnDisable()
    {
        ResetState();
    }

    private void ResetState()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
        speed = 0f;
    }
}