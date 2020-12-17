using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 25;

    [HideInInspector]
    public Vector2 direction = new Vector2();

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = direction * speed;
        Destroy(gameObject, 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
