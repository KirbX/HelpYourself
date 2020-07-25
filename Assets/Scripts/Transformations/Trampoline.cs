using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le personnage bloqué joue le role de trampolline, permettant au personnage actuel de rebondir (de plus en plus haut?)
public class Trampoline : Transformation
{
    bool bounce = false;
    float bounceAmount = 5;
    [SerializeField]
    GameObject currentPlayer;

    Rigidbody2D rb2d;


    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = GameObject.Find("currentPlayer");
        rb2d = currentPlayer.GetComponent<Rigidbody2D>();
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "currentPlayer")
        {
            bounce = true;
        }
    }
    // Update is called once per frame
     void Update()
    {
        if (bounce)
        {
            currentPlayer.GetComponent<PlayerPlatformerController>().setVelocity(new Vector2(0, 1), bounceAmount);
            bounce = false;
        }
    }

    public override void reset()
    {
    }
}
