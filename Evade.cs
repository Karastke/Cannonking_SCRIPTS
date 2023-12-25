using System.Collections;
using UnityEngine;

public class Evade : MonoBehaviour
{
    public float evadeDistance;
    public float evadeDuration;
    public float evadeCooldown;

    private Rigidbody2D rigid;
    private float evadeTimer;
    private bool isEvading;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (evadeTimer < evadeCooldown)
        {
            evadeTimer += Time.deltaTime;
        }
    }

    

    public void ExecuteEvade(Vector2 evadeDirection)
    {
        if (evadeTimer >= evadeCooldown && evadeDirection != Vector2.zero)
        {
            StartCoroutine(EvadeCoroutine(evadeDirection));
        }
    }

    IEnumerator EvadeCoroutine(Vector2 evadeDirection)
    {
        isEvading = true;
        evadeTimer = 0;

        float evadeSpeed = evadeDistance / evadeDuration;
        float elapsedTime = 0;

        // Make the player temporarily invulnerable here, e.g., disable collisions

        while (elapsedTime < evadeDuration)
        {
            Vector2 nextVec = evadeDirection * evadeSpeed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Make the player vulnerable again, e.g., enable collisions

        isEvading = false;
    }
}