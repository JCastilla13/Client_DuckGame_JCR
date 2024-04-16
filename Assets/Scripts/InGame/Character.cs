using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviourPun, IPunObservable
{
    private float speed;
    private float jumpForce;
    private Rigidbody2D rb;
    public SpriteRenderer sr;
    public Animator anim;
    private float desiredMovementAxis = 0f;
    private PhotonView pv;
    private Vector3 enemyPosition = Vector3.zero;

    private static float PlayerSpeed;
    private static float PlayerJumpForce;

    [SerializeField] private TextMesh playerNameText;

    private float timeToShoot = 1f;
    private float lastTimeShoot;
    private bool cooldownReady = false;

    private int initHp = 3;
    private int playerHp = 3;

    private int playerLifes = 3;

    [SerializeField] private TextMesh playerHpText;
    [SerializeField] private TextMesh playerLifesText;

    [SerializeField] private GameObject bulletSpawner;

    [SerializeField] private AudioSource shootCollisionSound;
    [SerializeField] private AudioSource shootFinalCollisionSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = rb.GetComponent<PhotonView>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 20;

        //Obtiene los valores de speed y jumpforce
        speed = PlayerPrefs.GetFloat("speed");
        jumpForce = PlayerPrefs.GetFloat("jumpforce");

        int localPlayerNum = PhotonNetwork.LocalPlayer.ActorNumber;

        if (localPlayerNum == 1)
        {
            if (pv.IsMine)
            {
                sr.color = Color.yellow;
            }
            else
            {
                sr.color = Color.red;
            }
        }
        else
        {
            if (pv.IsMine)
            {
                sr.color = Color.red;
            }
            else
            {
                sr.color = Color.yellow;
            }
        }
    }

    private void Start()
    {
        //Asigna los valores de speed y jumpforce a las variables de instancia
        speed = PlayerSpeed;
        jumpForce = PlayerJumpForce;

        playerNameText.text = PhotonNetwork.CurrentRoom.Players[photonView.OwnerActorNr].NickName;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            CheckInputs();
            if (cooldownReady)
            {
                lastTimeShoot -= Time.deltaTime;
            }
            if (lastTimeShoot < 0)
            {
                cooldownReady = false;
                lastTimeShoot = 0;
            }
        }
        else
        {
            SmoothReplicate();
        }

        playerLifesText.text = "Lifes: " + playerLifes.ToString();
        playerHpText.text = "Health: " + playerHp.ToString();

    }

    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            float horizontalInput = Input_Manager._INPUT_MANAGER.GetLeftAxisUpdate().x;

            float horizontalVelocity = horizontalInput * speed * Time.fixedDeltaTime;

            rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);

            if (horizontalVelocity > 0)
            {
                sr.flipX = false;
                bulletSpawner.transform.localPosition = new Vector3(1f, 0f, 0f);
            }
            else if (horizontalVelocity < 0)
            {
                sr.flipX = true;
                bulletSpawner.transform.localPosition = new Vector3(-1f, 0f, 0f);
            }

            anim.SetBool("IsMoving", horizontalInput != 0);
        }
    }


    private void CheckInputs()
    {
        desiredMovementAxis = Input.GetAxisRaw("Horizontal");

        if (Input_Manager._INPUT_MANAGER.GetJumpButton() && Mathf.Approximately(rb.velocity.y, 0f))
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            anim.SetBool("IsJumping", true);
        }

        if (rb.velocity.y < 0)
        {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", true);
        }
        else
        {
            anim.SetBool("IsFalling", false);
        }


        if (Input_Manager._INPUT_MANAGER.GetShootButton() && lastTimeShoot <= 0 && !cooldownReady)
        {
            Shoot();
            lastTimeShoot = timeToShoot;
            cooldownReady = true;
        }
    }

    private void Shoot()
    {
        GameObject bullet = PhotonNetwork.Instantiate("Bullet", bulletSpawner.transform.position, Quaternion.identity);
        Bullet bulletRef = bullet.GetComponent<Bullet>();

        if (sr.flipX)
        {
            bulletRef.SetDirection(false);
        }
        else
        {
            bulletRef.SetDirection(true);
        }
    }


    public void Damage()
    {
        if (!pv.IsMine)
        {
            pv.RPC("NetworkDamage", RpcTarget.All);
        }
    }

    [PunRPC]
    private void NetworkDamage()
    {
        playerHp--;

        if (playerHp <= 0)
        {
            if (playerLifes <= 0)
            {
                PhotonNetwork.LoadLevel("Matchmaking");
            }
            else
            {
                playerLifes--;
                playerHp = initHp;
                shootFinalCollisionSound.Play();
                transform.position = Vector3.zero;
            }
        }
        else
        {
            shootCollisionSound.Play();
        }
    }


    private void SmoothReplicate()
    {
        transform.position = Vector3.Lerp(transform.position, enemyPosition, Time.fixedDeltaTime * 20);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            enemyPosition = (Vector3)stream.ReceiveNext();
        }
    }

    public static void SetPlayerSpeed(float speed)
    {
        PlayerSpeed = speed;
    }

    public static void SetPlayerJumpForce(float jumpForce)
    {
        PlayerJumpForce = jumpForce;
    }
}
