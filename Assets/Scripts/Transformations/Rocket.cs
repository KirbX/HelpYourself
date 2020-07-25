using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Faire apparaitre une fleche pour la direction
//Le personnage bloqué joue le rôle de lanceur de fusée, dans une direction précise. Lorsque le personnage actuel passe dessus, il est propulsé dans la direction 
//avec une grande vélocité, 1 seule fois
public class Rocket : Transformation
{
    
    [SerializeField]
    GameObject currentPlayer;
    Rigidbody2D player_rb2d;
    CapsuleCollider2D object_bc2d;
    public int force = 15 ;

    [HideInInspector]
    public int defaultNumber = 1;
    [HideInInspector]
    public int numberOfUse;

    [HideInInspector]
    public Vector2 origin;
    [HideInInspector]
    public Vector2 destination;
    
    private Vector2 direction;
    private Color col;

    private LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = GameObject.Find("currentPlayer");
        numberOfUse = defaultNumber;
        player_rb2d = currentPlayer.GetComponent<Rigidbody2D>();
        object_bc2d = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "currentPlayer" && numberOfUse > 0)
        {
            expulse();
            numberOfUse--;
            if (numberOfUse == 0)// on enleve la colision
            {
                putToSleep();
            }
        }

    }

    private void expulse()
    {
        //GameObject.Find("currentPlayer").GetComponent<PlayerController2D>().externalForce = true;
        gameObject.SetActive(false);
        //player_rb2d.velocity = new Vector2(direction.x * force, direction.y * force);
        var playerController = currentPlayer.GetComponent<PlayerPlatformerController>();

        playerController.externalForce = true;
        playerController.setVelocity(direction, force);
        

        
        gameObject.SetActive(true);
    }

   
    public override void reset()
    {
        numberOfUse = defaultNumber;
        object_bc2d = GetComponent<CapsuleCollider2D>();
        object_bc2d.isTrigger = false;
        GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 1f);
    }

    private void putToSleep()
    {
        object_bc2d.isTrigger = true;
        GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 0.5f);
    }

    public void createArrow(Vector2 _origin, Vector2 _destination)
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        Color colorDirection = new Color(1.0f, 0, 0, 0.5f);
        line.startColor = colorDirection;
        line.endColor = colorDirection;
        origin = _origin;
        destination = _destination;

        line.SetPosition(0, origin);
        line.SetPosition(1, destination);

        direction = destination - origin;
        direction.Normalize();
    }
}
