using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleBullet : MonoBehaviour
{
    private float Lifetime = 5f;
    private Rigidbody rb;
    private float time;
    private float BulletDamage;

    public GameObject LightHitEffect;
    public GameObject MediumHitEffect;
    public GameObject HeavyHitEffect;
    public void SetLifetime(float newLifetime)
    {
        Lifetime = newLifetime;
    }

    public void SetBulletDamage(float bulletDamage)
    {
        BulletDamage = bulletDamage;
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time >= Lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyController>())
        {
            var enemy = other.gameObject.GetComponent<EnemyController>();
            enemy.ReceiveDamage(AttackType.Light, BulletDamage);

            Instantiate(HeavyHitEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
