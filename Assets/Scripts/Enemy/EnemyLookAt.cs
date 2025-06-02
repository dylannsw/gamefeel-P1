using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAiming : MonoBehaviour
{
    public Transform player;
    public float turnSpeed = 5f;

    void Update()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f; //Keep only horizontal rotation

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }
}

