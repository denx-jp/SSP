using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{

    [SerializeField] AudioClip sound;

    public void PlayeSE()
    {
        AudioSource.PlayClipAtPoint(sound, Vector3.zero);
    }
}
