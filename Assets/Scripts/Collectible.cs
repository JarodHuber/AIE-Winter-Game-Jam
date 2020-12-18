using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType
{
    TEST
}

public class Collectible : MonoBehaviour
{
    public CollectibleType type = CollectibleType.TEST;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        switch (type)
        {
            case CollectibleType.TEST:
                print("Collected");
                break;
        }
        Destroy(gameObject);
    }
}
