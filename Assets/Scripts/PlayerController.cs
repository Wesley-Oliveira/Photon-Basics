using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    public float playerSpeed = 5f;

    private Rigidbody2D rigidBody2D;
    private PhotonView photonView;

    private Vector2 playerRB;
    private Vector3 playerPosition;
    private Quaternion playerRotation;
    private float lag;

    [Header("Health")]
    public Image playerHealthFill;
    public float playerHealthMax = 100f;
    private float playerHealthCurrent;

    [Header("Bullet")]
    public GameObject bulletGO;
    public GameObject bulletGOPhotonView;
    public GameObject spawnBullet;

    // Use this for initialization
    void Start ()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();

        HealthManager(playerHealthMax);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(photonView.IsMine)
        {
            PlayerMove();
            PlayerTurn();

            Shooting();
        }
        /*if (Input.GetMouseButtonDown(0))
            HealthManager(-10f);
        */
    }

    void Shooting()
    {
        if(Input.GetMouseButtonDown(0))
        {
            photonView.RPC("Shoot", RpcTarget.All);
        }
        /*if(Input.GetMouseButtonDown(1))
        {
            PhotonNetwork.Instantiate(bulletGOPhotonView.name, spawnBullet.transform.position, spawnBullet.transform.rotation, 0);
        }*/
    }
    
    [PunRPC] //Função pode ser vista na rede e consome menos recursos
    void Shoot()
    {
        Instantiate(bulletGO, spawnBullet.transform.position, spawnBullet.transform.rotation);
    }

    public void TakeDamage(float value)
    {
        photonView.RPC("TakeDamageNetwork", RpcTarget.AllBuffered, value);
    }

    [PunRPC]
    void TakeDamageNetwork(float value)
    {
        HealthManager(value);
    }

    void HealthManager(float value)
    {
        playerHealthCurrent += value;
        playerHealthFill.fillAmount = playerHealthCurrent / 100;
    }

    void PlayerMove()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        rigidBody2D.velocity = new Vector2(x * playerSpeed, y * playerSpeed);
    }

    void PlayerTurn()
    {
        Vector3 mousePosition = Input.mousePosition;

        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(
                                mousePosition.x - transform.position.x,
                                mousePosition.y - transform.position.y);
        transform.up = direction;
    }
}