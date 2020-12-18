using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDrop : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, GetComponent<AudioSource>().clip.length);
    }
}
