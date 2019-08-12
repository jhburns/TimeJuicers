using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;
using InputMapping;


public class BulletManager : MonoBehaviour, ISerializable
{
    public Player player; //IM

    public int maxBullets; //IM
    private List<Bullet> bullets; //IM
    private int bulletIndex; //IM

    public float fireRate; //IM
    private float nextFire;

    public GlobalUI deathHandler; //IM

    private UserInput input; //IM

    void Start()
    {
        Init();

        InitBullets();
    }

    /*
     * Init - setup non-bullet vars
     */
    private void Init()
    {
        nextFire = 0f;
        input = new UserInput();
    }

    /*
     * InitBullets - finds then adds all the existing bullets to an array
     */
    private void InitBullets()
    {
        bullets = new List<Bullet>();

        Bullet[] tempBullets = FindObjectsOfType<Bullet>();

        for (int i = 0; i < tempBullets.Length; i++)
        {
            bullets.Add(tempBullets[i]);
        }


        bulletIndex = 0;
    }


    void Update()
    {
        if (input.FireDown() && nextFire < 0 && deathHandler.IsAlive)
        {
            nextFire = fireRate;
            Fire();
        }
        else
        {
            nextFire -= Time.deltaTime;
        }

    }

    /*
     * Fire - moves a new bullet in the player world, in the appropriate direction
     * Or prevents a bullet from being fired during cool-down
     */
    void Fire()
    {
        Bullet currentBul = bullets[bulletIndex];

        float offset = 0.6f;

        if (!player.IsMovingRight)
        {
            offset *= -1;
        }

        float xPos = player.transform.position.x + offset; // offset used to place bullet in front (not middle) of player

        currentBul.InPlay(player.IsMovingRight, new Vector2(xPos, player.transform.position.y));

        // Looping through all bullets shouldn't be an issue even with a high fire rate, with about 20 right now
        bulletIndex = (bulletIndex + 1) % maxBullets;
    }

    ////////////////////////////////////////////////////
    // Serial implementation, see Libraries/Serial.cs //
    ////////////////////////////////////////////////////
    public ISerialDataStore GetCurrentState()
    {
        return new SaveBulletMan(bulletIndex, nextFire);
    }

    public void SetState(ISerialDataStore state)
    {
        SaveBulletMan past = (SaveBulletMan) state;
        bulletIndex = past.bulletIndex;
        nextFire = past.nextFire;
    }
}

internal class SaveBulletMan : ISerialDataStore
{
    public int bulletIndex { get; private set; }
    public float nextFire { get; private set; }

    public SaveBulletMan(int bulletI, float nf)
    {
        bulletIndex = bulletI;
        nextFire = nf;
    }
}
