using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SendMessage("Damage",SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
