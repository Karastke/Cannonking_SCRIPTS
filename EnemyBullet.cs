using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    
    private GameObject player;
    private Rigidbody2D rb;
    public float force;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = player.transform.position - transform.position;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;

        //적이 쏘는 총알이 원이 아닌 다른 모양일 경우 타겟 방향으로 향하는 모양으로 고쳐주는 코드
        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 10)
        {
            Destroy(gameObject);
        }
    }
    
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         ParryingSystem parryingSystem = collision.gameObject.GetComponent<ParryingSystem>();
    //         if (parryingSystem.IsParrying())
    //         {
    //             parryingSystem.TakeDamageWithParry(1, this);
    //             parryingSystem.ReflectBullet(this);  // This line replaces the Reflect call
    //         }
    //         else
    //         {
    //             collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
    //             BallCollision ballCollision = GetComponent<BallCollision>();
    //             if (ballCollision != null)
    //             {
    //                 ballCollision.TriggerExplosion();
    //             }
    //             else
    //             {
    //                 Debug.LogError("BallCollision script is missing.");
    //             }
    //         }
    //     }
    //     else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && gameObject.CompareTag("ReflectedBullet"))
    //     {
    //         // Add code to deal damage to the enemy
    //         Destroy(gameObject);
    //     }
    // }

    // public void ChangeToReflectedBullet()
    // {
    //     gameObject.tag = "ReflectedBullet";
    //     gameObject.layer = LayerMask.NameToLayer("PlayerProjectiles");
    // }


}

