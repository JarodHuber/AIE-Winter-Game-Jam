using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public enum PlayerDir
    {
        None,
        Left,
        Right,
        Up,
        Down
    }

    public float speed = 5;

    Rigidbody2D rb;
    Vector2 vel = new Vector2();

    public PlayerDir directionState = PlayerDir.Left;
    public GameObject bulletFab = null;

    public Timer reloadTimer = new Timer(.5f);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        reloadTimer.Reset(reloadTimer.Delay);
    }

    private void FixedUpdate()
    {
        Move();
        Gun();
    }

    void Move()
    {
        vel.x = Input.GetAxis("Horizontal");
        vel.y = Input.GetAxis("Vertical");
        vel = Vector2.ClampMagnitude(vel, 1);

        rb.velocity = vel * speed;
    }

    void Gun()
    {
        if (!reloadTimer.IsComplete(false))
            reloadTimer.CountByTime();

        if (Input.GetAxis("HorizontalBullet") > 0)
        {
            directionState = PlayerDir.Right;
            Shoot(transform.right);
            return;
        }
        if (Input.GetAxis("HorizontalBullet") < 0)
        {
            directionState = PlayerDir.Left;
            Shoot(-transform.right);
            return;
        }
        if (Input.GetAxis("VerticalBullet") > 0)
        {
            directionState = PlayerDir.Up;
            Shoot(transform.up);
            return;
        }
        if (Input.GetAxis("VerticalBullet") < 0)
        {
            directionState = PlayerDir.Down;
            Shoot(-transform.up);
            return;
        }
    }

    void Shoot(Vector2 direction)
    {
        if (!reloadTimer.IsComplete()) 
            return;

        Bullet tmpBullet = Instantiate(bulletFab, transform.position, Quaternion.identity).GetComponent<Bullet>();

        tmpBullet.direction = direction;
    }
}
