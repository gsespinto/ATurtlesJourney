using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField] private float hpChange;
    [SerializeField] private float speed;
    [SerializeField] private bool isAttachable;
    [SerializeField] private bool isScubaDiver;
    [Space(10)]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip attachClip;
    [Space(10)]
    public MyEnums.Direction orientation;

    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public PlayerScript playerScript;
    private Vector3 direction;

    private bool hasHelped;
    private float attachTimer;
    private bool attached;

    void Start()
    {
        attachTimer = Random.Range(5f, 20f);
        SetOrientation();
    }

    // Update is called once per frame
    void Update()
    {
        // when the game's paused don't run Update
        if (playerScript.isPaused)
        {
            return;
        }
        // if the item isn't attached
        // moves the item and destroys it when offscreen
        if (!attached)
        {
            Move();
            DestroyItemOffscreen();
        }
        // if the item's attached
        // while the attachtimer is running
        // moves the item to the turtle's shell center
        else
        {
            if (attachTimer > 0)
            {
                MoveToShellCenter();
                attachTimer--;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerScript.isDead)
        {
            if (!isAttachable && !isScubaDiver)
            {
                if (other.gameObject.tag == "HeadBox")
                {
                    audioSource.PlayOneShot(hitClip);
                    playerScript.ChangeHP(hpChange);
                    Destroy(this.gameObject);
                }
            }
            // if it's an attachable item and isn't attached
            else if (isAttachable && !attached)
            {
                // if it hits the head, destroys the item and deals 125%
                // of the item's damage to the player
                if (other.gameObject.tag == "HeadBox")
                {
                    audioSource.PlayOneShot(hitClip);
                    playerScript.ChangeHP(hpChange * 1.25f);
                    Destroy(this.gameObject);
                }
                // if it hits the body, deals the normal amount of damage to the player
                // and attachs the item to the turtle's shell
                if (other.gameObject.tag == "BodyBox")
                {
                    audioSource.PlayOneShot(attachClip);
                    if (hpChange > 0)
                    {
                        playerScript.ChangeHP(hpChange);
                        playerScript.attachedObjs++;
                        playerScript.attachedLst.Add(this.gameObject);
                    }
                    this.transform.SetParent(other.gameObject.transform);
                    attached = true;
                }
            }
            // if it's a scuba diver that hasn't helped, hits the turtle 
            // and it has garbage attached, then it clear the turtle's shell
            if (isScubaDiver && !hasHelped)
            {
                if (other.gameObject.tag == "HeadBox" || other.gameObject.tag == "BodyBox")
                {
                    audioSource.PlayOneShot(hitClip);
                    for (int i = 0; i < playerScript.attachedLst.Count; i++)
                    {
                        if (playerScript.attachedLst[i] != null)
                        {
                            Destroy(playerScript.attachedLst[i].gameObject);
                            break;
                        }
                    }
                    playerScript.attachedObjs--;
                    hasHelped = true;
                }
            }
        }
    }

    /// <summary> Moves item along given direction with given speed </summary>
    void Move()
    {
        Vector3 movement = this.transform.position;
        movement += direction.normalized * speed;
        this.transform.position = movement;
    }

    /// <summary> When the item moves
    /// offscreen destroys it </summary>
    void DestroyItemOffscreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(this.transform.position);

        switch (orientation)
        {
            case MyEnums.Direction.UP:
                if (screenPoint.y > 2)
                    Destroy(this.gameObject);
                break;            
            case MyEnums.Direction.LEFT:
                if (screenPoint.x < -1)
                    Destroy(this.gameObject);
                break;
            case MyEnums.Direction.DOWN:
                if (screenPoint.y > -1)
                    Destroy(this.gameObject);
                break;
            case MyEnums.Direction.RIGHT:
                if (screenPoint.x > 2)
                    Destroy(this.gameObject);
                break;
        }
    }

    /// <summary> Lerps the item to the turtle's shell center </summary>
    void MoveToShellCenter()
    {
        Vector3 movement = Vector3.Lerp(this.transform.position, transform.parent.position, 0.05f);
        this.transform.position = movement;
    }

    /// <summary> Sets movement direction, and in the case of scubadiver it's direction </summary>
    void SetOrientation()
    {
        switch (orientation)
        {
            case (MyEnums.Direction.UP):
                direction = Vector3.up;
                if (isScubaDiver)
                    this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 270f));
                break;
            case (MyEnums.Direction.LEFT):
                direction = Vector3.left;
                if (isScubaDiver)
                    this.transform.rotation = Quaternion.Euler(Vector3.zero);
                break;
            case (MyEnums.Direction.DOWN):
                direction = Vector3.down;
                if (isScubaDiver)
                    this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
                break;
            case (MyEnums.Direction.RIGHT):
                direction = Vector3.right;
                if (isScubaDiver)
                    this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                break;
            default:
                direction = Vector3.left;
                if (isScubaDiver)
                    this.transform.rotation = Quaternion.Euler(Vector3.zero);
                break;
        }
    }
}
