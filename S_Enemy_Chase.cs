using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFollow : MonoBehaviour
{
    public GameObject player;
    public float speed = 2.0f;
    public float minDistance = 1.0f;

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > minDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }
}