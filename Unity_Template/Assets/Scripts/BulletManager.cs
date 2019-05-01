using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;

public class BulletManager : MonoBehaviour, ISerializable
{
    public Player player; //IM

    public int maxBullets; //IM
    //https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netframework-4.8
    private List<Bullet> bullets; //IM
    private int bulletIndex; //IM

    public float fireRate; //IM
    private float nextFire;

    public GlobalUI deathHandler; //IM

    /* Start is called before the first frame update,
     * Initializes time variables and bullet array
     */
    void Start()
    {
        InitTime();

        InitBullets();
    }

    /*
     * InitTime - sets nextFire
     */
    private void InitTime()
    {
        nextFire = Time.time;
    }

    /*
     * InitBullets - adds all the existing bullets to an array
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


    /* Update - is called once per frame,
     * Checks to see if player is shooting
     */
    void Update()
    {
        // http://wiki.unity3d.com/index.php?title=Xbox360Controller
        if ((Input.GetKey(KeyCode.F) ||
             Input.GetKey(KeyCode.J) ||
             Input.GetKey(KeyCode.JoystickButton1) || // B button on xbox 360 controller
             Input.GetAxisRaw("RightTrigger") == 1
            )  

             && Time.time > nextFire
             && deathHandler.IsAlive
           )
        {
            nextFire = Time.time + fireRate;
            Fire();
        }

    }

    /*
     * Fire - starts a new bullet in the player world, moving in the correct direction
     */
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
        currentBul.InPlay(player.MovingRight);
        
        if (player.MovingRight)
        {
            currentBul.transform.localScale = new Vector3(1, 1, 1);
        } else
        {
            currentBul.transform.localScale = new Vector3(-1, 1, 1);
        }

        bulletIndex = (bulletIndex + 1) % maxBullets;
    }

    /// Serial Methods, see Serial Namespace 
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
