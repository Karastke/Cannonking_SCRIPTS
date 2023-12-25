using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce_Controller : MonoBehaviour
{
    [SerializeField] public int bounceCount = 0;
    [SerializeField] public int maxBounceCount = 3;
    [SerializeField] public float cooldownTime = 0.1f;
    [SerializeField] public Sprite[] bounceSprites;
    public bool isInCooldown = false;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //2023-09-13 오후 12시에 수정한 코드 projectile 무시
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !isInCooldown)
        {
            Debug.Log("Collided with Wall!");
            Vector2 bounceDirection = Vector2.Reflect(GetComponent<Rigidbody2D>().velocity, collision.contacts[0].normal);
            float speed = GetComponent<Rigidbody2D>().velocity.magnitude;
            GetComponent<Rigidbody2D>().velocity = bounceDirection.normalized * speed;

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