using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType
{
    NONE,
    DOUBLEDAMAGE,
    HEALTH
}

public class Collectible : MonoBehaviour
{
    public CollectibleType type = CollectibleType.DOUBLEDAMAGE;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if(type == CollectibleType.HEALTH)
        {
            collision.GetComponent<Player>().Heal(1);
        }
        else
        {
            collision.GetComponent<Player>().activeCollectible = type;
            collision.GetComponent<Player>().collectibleDuration.Reset();
        }

        Destroy(gameObject);
    }
}
