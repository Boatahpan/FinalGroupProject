using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawn_point;

    private AudioSource audioSource;
    public AudioClip ZombieSound;

    void Start()
    {
    audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        player.transform.position = respawn_point.transform.position;
        audioSource.PlayOneShot(ZombieSound);
    }
}