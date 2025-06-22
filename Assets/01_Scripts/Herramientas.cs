using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herramientas : MonoBehaviour
{
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
        }
        else if (collision.gameObject.CompareTag("Tree") && isAxe)
        {
            DropsObjetos DP = collision.gameObject.GetComponent<DropsObjetos>();
            DP.takeDamage(damage);
        }
        else if (collision.gameObject.CompareTag("Spider") && isLance)
        {
            Spider DP = collision.gameObject.GetComponent<Spider>();
            DP.takeDamage(damage * 5);
        }
        else if (collision.gameObject.CompareTag("Serpiente") && isLance)
        {
            Serpiente DP = collision.gameObject.GetComponent<Serpiente>();
            DP.takeDamage(damage * 5);
        }
        else if (collision.gameObject.CompareTag("Jabali") && isLance)
        {
            JavaliCristal DP = collision.gameObject.GetComponent<JavaliCristal>();
            DP.takeDamage(damage*5);
        }
        else if (collision.gameObject.CompareTag("Owl") && isLance)
        {
            EchoOwl DP = collision.gameObject.GetComponent<EchoOwl>();
            DP.takeDamage(damage * 5);
        }
    }
}
