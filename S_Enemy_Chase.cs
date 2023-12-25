using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFollow : MonoBehaviour
{
    public GameObject player;
    public float speed = 2.0f;
    public float minDistance = 1.0f;
    public float rotationSpeed = 5.0f;

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > minDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            // Rotate the monster to face the player, only around the Y-axis
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0; // Ignore the Y-axis difference
            Quaternion targetRotation = Quaternion.Euler(0, Quaternion.LookRotation(direction).eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}