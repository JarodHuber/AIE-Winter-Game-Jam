using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public Timer health = new Timer(5);
    public float speed = 5;

    public GameObject bulletFab = null;
    public Timer reloadTimer = new Timer(.5f);

    public Sprite[] playerStates;

    #region collectible
    public CollectibleType activeCollectible = CollectibleType.NONE;
    public Timer collectibleDuration = new Timer(5);
    #endregion

    public bool paused = false;

    Rigidbody2D rb;
    Vector2 vel = new Vector2();

    SpriteRenderer sp;

    Timer invincibleTimer = new Timer(0.2f);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        reloadTimer.Reset(reloadTimer.Delay);

        sp = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (paused)
            return;

        Move();
        Gun();

        if (activeCollectible != CollectibleType.NONE && collectibleDuration.Check())
        {
            activeCollectible = CollectibleType.NONE;
        }

        if (!invincibleTimer.IsComplete(false))
            invincibleTimer.CountByTime();
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
            sp.sprite = playerStates[2];
            Shoot(transform.right);
            return;
        }
        if (Input.GetAxis("HorizontalBullet") < 0)
        {
            sp.sprite = playerStates[1];
            Shoot(-transform.right);
            return;
        }
        if (Input.GetAxis("VerticalBullet") > 0)
        {
            sp.sprite = playerStates[3];
            Shoot(transform.up);
            return;
        }
        if (Input.GetAxis("VerticalBullet") < 0)
        {
            sp.sprite = playerStates[0];
            Shoot(-transform.up);
            return;
        }
    }

    void Shoot(Vector2 direction)
    {
        if (!reloadTimer.IsComplete()) 
            return;

        Instantiate(bulletFab, transform.position, Quaternion.identity).GetComponent<Bullet>().Initialize(direction, true, (activeCollectible == CollectibleType.DOUBLEDAMAGE) ? 2 : 1);
    }

    public void Heal(int amount)
    {
        health.CountByValue(-amount);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameEngine>().RaiseHealthBar((int)health.TimeRemaining);
    }

    public void TakeDamage()
    {
        if (!invincibleTimer.IsComplete())
            return;

        health.CountByValue(1);

        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameEngine>().LowerHealthBar((int)health.TimeRemaining);

        if (health.IsComplete(false))
        {
            PlayerPrefs.SetInt("curLvl", SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(5);
        }
    }
}
