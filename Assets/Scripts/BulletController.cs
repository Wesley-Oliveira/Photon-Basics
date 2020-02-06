using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 100f;
    public float bulletTimeLife = 5f;
    private float bulletTimeCount;
    private Rigidbody2D bulletRigidbody2D;

    
    
	// Use this for initialization
	void Start ()
    {
        bulletRigidbody2D = GetComponent<Rigidbody2D>();
        bulletRigidbody2D.AddForce(transform.up * bulletSpeed, ForceMode2D.Force);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(bulletTimeCount >= bulletTimeLife)
        {
            Destroy(this.gameObject);
        }

        bulletTimeCount += Time.deltaTime;
	}
}
