using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTransformation : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    private PlayerPlatformerController currentPlayerController;

    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    RectTransform UITransformation;

    Vector3 camPosition;

    private GameObject currentPlayer;
 
    private bool canCreate = true;

    private List<GameObject> exMe;
    private Vector2 _screenSize;

    private bool waitForClick = false;
    private bool hasClicked = false;

    LineRenderer line;
    [HideInInspector]
    public enum ACTION
    {
        DEFAULT = -1, TAPIS = 0, TRAMPOLINE, ROQUETTE, GRAVITY
    }
    ACTION actionType = ACTION.DEFAULT;


    // Start is called before the first frame update
    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(1f, 0f, 0f, 1f);
        line.endColor = new Color(1f, 0f, 0f, 1f);


        exMe = new List<GameObject>();

        _screenSize = new Vector2(Screen.width, Screen.height);

        createNewPlayer();
    }


    private void createNewPlayer()
    {
        currentPlayer = Instantiate(player, player.transform.position, Quaternion.identity);
        currentPlayer.tag = "Player";
        currentPlayer.name = "currentPlayer";

        mainCamera.transform.parent = currentPlayer.transform;
        mainCamera.transform.localPosition = new Vector3(0, 0.85f, -10);
        var DeadZoneCam = mainCamera.GetComponent<DeadzoneCamera>();
        DeadZoneCam.target = currentPlayer.GetComponent<SpriteRenderer>();
        UITransformation.gameObject.SetActive(false);

        currentPlayerController = currentPlayer.GetComponent<PlayerPlatformerController>();
    }



    private bool is_inverted()
    {
        return currentPlayerController.GetComponent<PlayerPlatformerController>().inverted();
    }

    private void resetAll()
    {
        if (is_inverted())
        {
            currentPlayerController.changegravity();
        }
        currentPlayer.transform.position = player.transform.position;
        currentPlayer.transform.rotation = Quaternion.identity;

        currentPlayerController.setVelocity(new Vector2(0, 0), 0);
        

        resetExs();
        resetTeleport();
    }




    // Update is called once per frame
    void Update()
    {
        if (waitForClick)
        {

            if (actionType == ACTION.ROQUETTE)
            {
                Vector2 vecFrom = currentPlayer.GetComponent<Renderer>().bounds.center;
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 offset = mousePos - vecFrom;
                offset.Normalize();


                Vector2 vecTo = new Vector2(offset.x + vecFrom.x, offset.y + vecFrom.y);
                line.SetPosition(0, vecFrom);
                line.SetPosition(1, vecTo); 
                if (Input.GetButtonUp("Fire1"))
                    hasClicked = true;
                if (hasClicked)
                {
                    GameObject playerTemp = blockActualPlayerWithScript("Rocket", "Untagged", "Default");

                    playerTemp.GetComponent<Rocket>().createArrow(vecFrom, vecTo);
                    unpauseGame();
                    hasClicked = false;
                    waitForClick = false;
                    actionType = ACTION.DEFAULT;
                    line.positionCount = 0;
                }
            }
        }
        else {
            if (Input.GetButtonDown("Fire1") && canCreate)
            {
                pauseGame();
                showUITransformation();
            }
            if (Input.GetButtonUp("Fire1"))
            {

                closeUiTransformation();
                switch (actionType)
                {
                    case ACTION.TAPIS:
                        blockActualPlayerWithScript("Carpet", "Untagged", "Default");
                        actionType = ACTION.DEFAULT;
                        unpauseGame();
                        break;
                    case ACTION.TRAMPOLINE:
                        blockActualPlayerWithScript("Trampoline", "Untagged", "Trampoline");
                        actionType = ACTION.DEFAULT;
                        unpauseGame();
                        break;
                    case ACTION.ROQUETTE:
                        line.positionCount = 2;
                        waitForClick = true;
                        break;

                    case ACTION.GRAVITY:
                        blockActualPlayerWithScript("InversionGravite", "Untagged", "Default");
                        actionType = ACTION.DEFAULT;
                        unpauseGame();
                        break;
                    default:
                        unpauseGame();
                        break;
                }

            }
        }

    }
   
   
    private GameObject blockActualPlayerWithScript(string script, string tag, string layer )
    {
        var currentPos = currentPlayer.transform.position;
        var currentRot = currentPlayer.transform.rotation;


        resetAll();
        var constructed = Instantiate(player, currentPos, currentRot);

        constructed.tag = tag;
        constructed.layer = LayerMask.NameToLayer(layer);
        constructed.name = script;
        Destroy(constructed.GetComponent<PlayerPlatformerController>());
        constructed.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        constructed.AddComponent (System.Type.GetType(script));
        exMe.Add(constructed);

        return constructed;

    }


    private void pauseGame()
    {
        Time.timeScale = 0.0f;
    }

    private void unpauseGame()
    {
        Time.timeScale = 1.0f;
    }
    private void showUITransformation()
    {
        Vector2 mousePosition = Input.mousePosition;
        UITransformation.position = Camera.main.ScreenToWorldPoint(mousePosition);
        UITransformation.localPosition = new Vector3(UITransformation.localPosition.x, UITransformation.localPosition.y, -77);
        
        UITransformation.gameObject.SetActive(true);
    }
    private void closeUiTransformation()
    {
        UITransformation.gameObject.SetActive(false);
    }

    public void setButtonNumber(int action)
    {
        actionType = (ACTION)action;
    }
    
    private void resetExs()
    {
        foreach(GameObject ex in exMe)
        {
            if (ex.GetComponent<Transformation>() as Transformation != null)
            {
                ex.GetComponent<Transformation>().reset();
            }
        }
    }

    private void resetTeleport()
    {
        TeleporteurSimple[] ts = FindObjectsOfType<TeleporteurSimple>();
        foreach(var t in ts)
        {
            t.reset();
        }
    }

   
}
