using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agua : MonoBehaviour
{
    public float collDown = 2f;
    private float contado = 0;
    public float cantidad=10;
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && contado>=collDown)
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.TomarAgua(cantidad);
            contado=0;
        }
        else
            contado+=Time.deltaTime;
    }
}
