using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ReflectedBulletDamage : MonoBehaviour
{
    public float damage = 1;
    public float lifetime = 10f;
    private float timer = 0f;
    private PlayerBulletExplosion bulletExplosion;

    private void Start() 
    {
        bulletExplosion = GetComponent<PlayerBulletExplosion>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            // Assuming the enemy has a script called "EnemyHealth" with a "TakeDamage" method
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
            StartCoroutine(bulletExplosion.Explode());
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("S_Enemies"))
        {
            Destroy(collision.gameObject);  // Destroy the object with "S_Enemies" layer
            StartCoroutine(bulletExplosion.Explode());
            return;  // Exit the method to avoid further checks and actions
        }

        
        
    }
}
