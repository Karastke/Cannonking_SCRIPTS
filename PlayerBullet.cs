using System.Collections;
using UnityEngine;

public class ReflectedBulletDamage : MonoBehaviour
{
    public float damage = 1;
    public float lifetime = 10f;
    private float timer = 0f;
    private PlayerBulletExplosion bulletExplosion;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletExplosion = GetComponent<PlayerBulletExplosion>();
    }

    private void OnEnable()
    {
        timer = 0f;
    }

    public void ResetBullet()
    {
        timer = 0f;
        // 추가적인 초기화가 필요한 경우 여기에 구현
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > lifetime)
        {
            ReturnToPool();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            collision.gameObject.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            StartCoroutine(ExplodeAndReturn());
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("S_Enemies"))
        {
            ObjectPool.Instance.ReturnToPool(collision.gameObject);
            StartCoroutine(ExplodeAndReturn());
        }
    }

    private IEnumerator ExplodeAndReturn()
    {
        yield return StartCoroutine(bulletExplosion.Explode());
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        timer = 0f;
        rb.velocity = Vector2.zero;
        ObjectPool.Instance.ReturnToPool(gameObject);
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
}