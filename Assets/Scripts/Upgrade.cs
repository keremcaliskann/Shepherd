using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public static event System.Action OnCollected;

    Player player;
    GunController gunController;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        gunController = FindObjectOfType<GunController>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (OnCollected != null)
            {
                OnCollected();
            }

            Destroy(gameObject);
        }
    }
}
