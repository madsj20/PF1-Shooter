using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Dummy : MonoBehaviour
{
    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Hit by bullet");
            //--health;
            NetworkServer.Destroy(other.gameObject);
            /*if (health == 0)
                NetworkServer.Destroy(gameObject);*/
        }
    }
}
