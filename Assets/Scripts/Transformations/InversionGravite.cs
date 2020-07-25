using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class InversionGravite : Transformation
{
    Rigidbody2D player_rb2d;

    CapsuleCollider2D object_bc2d;
    [SerializeField]
    GameObject currentPlayer;
    PlayerPlatformerController currentPlayerController;
    [HideInInspector]
    public int defaultNumber = 1;
    [HideInInspector]
    public int numberOfUse;

    
    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = GameObject.Find("currentPlayer");
        currentPlayerController = currentPlayer.GetComponent<PlayerPlatformerController>();
        player_rb2d = currentPlayer.GetComponent<Rigidbody2D>();
        numberOfUse = defaultNumber;
        object_bc2d = GetComponent<CapsuleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "currentPlayer"  && numberOfUse > 0)
        {
            Inversion();
            numberOfUse--;
            if (numberOfUse == 0)
            {
                putToSleep();
            }
        }
    }

    

    public void Inversion()
    {
        currentPlayerController.changegravity();
    }

    private void putToSleep()
    {
        object_bc2d.isTrigger = true;
        GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 0.5f);
    }


    public override void reset()
    {
        numberOfUse = defaultNumber;
        object_bc2d = GetComponent<CapsuleCollider2D>();
        object_bc2d.isTrigger = false;
        GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 1f);

    }
}
