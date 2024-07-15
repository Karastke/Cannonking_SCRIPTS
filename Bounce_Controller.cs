using UnityEngine;
using System.Collections;

public class Bounce_Controller : MonoBehaviour
{
    [SerializeField] public int bounceCount = 0;
    [SerializeField] public int maxBounceCount = 3;
    [SerializeField] public float cooldownTime = 0.1f;
    [SerializeField] public Sprite[] bounceSprites;
    public bool isInCooldown = false;
    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        ResetState();
    }

    private void OnDisable()
    {
        // 코루틴 중지
        StopAllCoroutines();
    }

    public void ResetState()
    {
        bounceCount = 0;
        isInCooldown = false;
        if (bounceSprites.Length > 0)
        {
            spriteRenderer.sprite = bounceSprites[0];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !isInCooldown)
        {
            Vector2 bounceDirection = Vector2.Reflect(rb.velocity, collision.contacts[0].normal);
            float speed = rb.velocity.magnitude;
            rb.velocity = bounceDirection.normalized * speed;

            bounceCount++;

            // Change the sprite based on bounce count
            if (bounceCount < bounceSprites.Length)
            {
                spriteRenderer.sprite = bounceSprites[bounceCount];
            }

            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        isInCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isInCooldown = false;
    }
}