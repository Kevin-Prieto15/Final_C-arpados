using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herramientas : MonoBehaviour
{
    public float colldown = 1f;
    private float timer = 0;
    public bool isAxe = false;
    public bool isPickaxe = false;
    public bool isLance = false;
    public float damage = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rock") && isPickaxe)
        {
            DropsObjetos DP = collision.gameObject.GetComponent<DropsObjetos>();
            DP.takeDamage(damage);
            timer = 0;
        }
        else if (collision.gameObject.CompareTag("Tree") && isAxe)
        {
            DropsObjetos DP = collision.gameObject.GetComponent<DropsObjetos>();
            DP.takeDamage(damage);
            timer = 0;
        }
    }
}
