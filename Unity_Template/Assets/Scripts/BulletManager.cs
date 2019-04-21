using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public Bullet bulletTemplate;
    public Player player;

    public int maxBullets;
    private List<Bullet> bullets; //https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netframework-4.8
    private int bulletIndex;

    public float fireRate;
    private float nextFire;

    // Start is called before the first frame update
    void Start()
    {
        InitTime();

        InitBullets();
    }

    private void InitTime()
    {
        nextFire = Time.time;
    }

    private void InitBullets()
    {
        bullets = new List<Bullet>();

        for (int i = 0; i < maxBullets; i++) {
            Vector3 startingPos = new Vector3(player.transform.position.x + i, -30f, 0); // i is used to offset bullets

            Bullet currentBul = Instantiate(bulletTemplate, startingPos, Quaternion.identity); //https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
            currentBul.transform.SetParent(gameObject.transform); //Puts bullet under manager

            bullets.Add(currentBul);
        }

        bulletIndex = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.J)) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Fire();
        }
    }
    
    void Fire()
    {
        Bullet currentBul = bullets[bulletIndex];

        float offset = 0.6f;

        if (!player.MovingRight)
        {
            offset *= -1;
        }

        float xPos = player.transform.position.x + offset;


        currentBul.transform.position = new Vector2(xPos, player.transform.position.y);
        currentBul.IsMovingRight = player.MovingRight;
        currentBul.InPlay();

        bulletIndex = (bulletIndex + 1) % maxBullets;
    }
}
