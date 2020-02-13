using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed = 100f;
    public float bulletTimeLife = 5f;
    private float bulletTimeCount;
    private Rigidbody2D bulletRigidbody2D;
    public float bulletDamage = 10f;

	void Start ()
    {
        bulletRigidbody2D = GetComponent<Rigidbody2D>();
        bulletRigidbody2D.AddForce(transform.up * bulletSpeed, ForceMode2D.Force);
	}
	
	void Update ()
    {
		if(bulletTimeCount >= bulletTimeLife)
        {
            Destroy(this.gameObject);
        }

        bulletTimeCount += Time.deltaTime;
	}

    [PunRPC]
    void BulletDestroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") && collider.GetComponent<PlayerController>() && collider.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("PlayerId: " + collider.GetComponent<PhotonView>().Owner.ActorNumber + " PlayerName: " + collider.GetComponent<PhotonView>().Owner.NickName);
            collider.GetComponent<PlayerController>().TakeDamage(-bulletDamage);

            this.GetComponent<PhotonView>().RPC("BulletDestroy", RpcTarget.AllViaServer);
        }

        Destroy(this.gameObject);
    }
}