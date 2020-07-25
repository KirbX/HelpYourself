using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporteurSimple : Transformation
{
    public GameObject linkedTeleport;
    [HideInInspector]
    public bool already_use = false;
    Rigidbody2D player_rb2d;
    GameObject currentPlayer;

    internal Material m_Material;
    [HideInInspector]
    public int defaultNumber = 999;
    [HideInInspector]
    public int numberOfUse;
    private Color col;
    // Start is called before the first frame update
    void Awake()
    {
        m_Material = GetComponent<Renderer>().material;
        col = m_Material.color;
    }

    void Start()
    {

        currentPlayer = GameObject.Find("currentPlayer");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!already_use)
        {
            currentPlayer.transform.position = linkedTeleport.transform.position;
            setUsed(true);
            changeMaterialOpacity(0.5f);

            StartCoroutine(waitAndActive(2));

        }
    }

    private IEnumerator waitAndActive(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        setUsed(false);
        changeMaterialOpacity(1f);
    }


    private void setUsed(bool b)
    {
        already_use = b;
        linkedTeleport.GetComponent<TeleporteurSimple>().already_use = b;
    }

    private void changeMaterialOpacity(float alpha)
    {
        m_Material.color = new Color(col.r, col.g, col.b, alpha);
        var other = linkedTeleport.GetComponent<TeleporteurSimple>();
        other.m_Material.color = new Color(other.col.r, other.col.g, other.col.b, alpha);
    }

    public override void reset()
    {
        m_Material.color = new Color(col.r, col.g, col.b, 1.0f);
        already_use = false;
    }
}

