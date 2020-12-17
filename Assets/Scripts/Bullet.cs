using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 25;

    [HideInInspector]
    public bool shotByPlayer = false;

    [HideInInspector]
    public Vector2 direction = new Vector2();

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = direction * speed;
        Destroy(gameObject, 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (shotByPlayer && collision.transform.CompareTag("Enemy"))
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemyManager>().EnemyTakeDamage(collision.gameObject.GetComponent<Enemy>());
            Destroy(gameObject);
        }
        else if(!shotByPlayer && collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage();
            Destroy(gameObject);
        }
        else if (!collision.transform.CompareTag("Player") && !collision.transform.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
