using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType
{
    NONE,
    TEST
}

public class Collectible : MonoBehaviour
{
    public CollectibleType type = CollectibleType.TEST;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        collision.GetComponent<Player>().activeCollectible = type;
        collision.GetComponent<Player>().collectibleDuration.Reset();

        Destroy(gameObject);
    }
}
