using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType
{
    NONE = -1,
    DOUBLEDAMAGE,
    HEALTH
}

public class Collectible : MonoBehaviour
{
    public Sprite[] collectibleVariants;
    //[HideInInspector]
    public CollectibleType type = CollectibleType.NONE;

    private void Awake()
    {
        if (type != CollectibleType.NONE)
            GetComponent<SpriteRenderer>().sprite = collectibleVariants[(int)type];
    }

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
