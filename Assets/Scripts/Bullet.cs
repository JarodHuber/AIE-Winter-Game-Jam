using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 25;

    public GameObject[] sounds;

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
            GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemyManager>().EnemyTakeDamage(collision.gameObject.GetComponentInParent<Enemy>());
            Instantiate(sounds[0], collision.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if(!shotByPlayer && collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage();
            Instantiate(sounds[1], collision.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (!collision.transform.CompareTag("Player") && !collision.transform.CompareTag("Enemy") && !collision.transform.CompareTag("EnemyBase"))
        {
            Destroy(gameObject);
        }
    }
}
