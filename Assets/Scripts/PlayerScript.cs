using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{
    #region Variables
    [SerializeField] private float maxSpeed;
    [SerializeField] private Animator playerAnime;
    [SerializeField] private float maxHP;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private float cameraThreshold;

    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject settingsUI;

    [SerializeField] private AudioClip deathClip;
    [SerializeField] private FixedJoystick fixedJoystick;
    public bool autoRunner;
    private AudioSource audioSource;
    private bool hasPlayedAudio;

    private float HP;
    public bool isDead;
    [HideInInspector] public float movSpeed;
    public int attachedObjs;
    public bool isPaused;
    public List<GameObject> attachedLst;
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();

        HP = maxHP;
        movSpeed = maxSpeed;

        attachedLst = new List<GameObject>();

        gameUI.SetActive(true);
        deathUI.SetActive(false);
        pauseUI.SetActive(false);
        settingsUI.SetActive(false);

#if UNITY_STANDALONE_WIN || UNITY_WIN || UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX || UNITY_LINUX || UNITY_WEBGL
        if (fixedJoystick != null)
            fixedJoystick.gameObject.SetActive(false);
#elif UNITY_EDITOR_ANDROID || UNITY_STANDALONE_ANDROID || UNITY_ANDROID
        if (fixedJoystick != null)
        {
            if (!autoRunner)
                fixedJoystick.gameObject.SetActive(true);
            else
                fixedJoystick.gameObject.SetActive(false);
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (!isPaused)
            {
                Move();
                if (autoRunner)
                    MoveCamera();
                ObjectsAttached();
                gameUI.SetActive(true);
                deathUI.SetActive(false);
            }
            PauseGame();
        }
        else
        {
            playerAnime.SetTrigger("isDead");
            if (playerAnime.GetCurrentAnimatorStateInfo(0).IsName("DeathTurtle"))
            {
                if (!hasPlayedAudio)
                {
                    audioSource.PlayOneShot(deathClip);
                    hasPlayedAudio = true;
                }
                gameUI.SetActive(false);
                deathUI.SetActive(true);
            }
        }
    }
    void Move()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(this.transform.position);
        Vector3 movVec = Vector3.zero;
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
#if UNITY_STANDALONE_WIN || UNITY_WIN || UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX || UNITY_LINUX || UNITY_WEBGL
        if (screenPoint.y < 1 && Input.GetKey(KeyCode.UpArrow))
        {
            movVec.y += movSpeed;
            this.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        if (screenPoint.y > 0 && Input.GetKey(KeyCode.DownArrow))
        {
            movVec.y -= movSpeed;
            this.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        if (!autoRunner)
        {
            if (screenPoint.x < 1 && Input.GetKey(KeyCode.RightArrow))
            {
                Vector3 scale = this.transform.localScale;
                //scale.x = Mathf.Abs(scale.x);
                this.transform.localScale = scale;
                movVec.x += movSpeed;
                //this.transform.rotation = Quaternion.Euler(new Vector3 (0f, 0f, 90f));
            }
            if (screenPoint.x > 0 && Input.GetKey(KeyCode.LeftArrow))
            {
                Vector3 scale = this.transform.localScale;
                //scale.x = -Mathf.Abs(scale.x);
                this.transform.localScale = scale;
                movVec.x -= movSpeed;
                //this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
            }
        }
#elif UNITY_EDITOR_ANDROID || UNITY_STANDALONE_ANDROID || UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            if (autoRunner || fixedJoystick == null)
            {
                Vector3 inputPos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                inputPos.z = this.transform.position.z;
                //Vector3 threshold = (inputPos - this.transform.position).normalized;
                inputPos -= this.transform.position;
                //inputPos -= threshold*0.1f;
                if (Vector3.Distance(inputPos, this.transform.position) < 0.1f)
                {
                    movVec.y = 0;
                    movVec.x = 0f;
                }
                else
                {
                    movVec.y = inputPos.y * movSpeed;
                    movVec.y = Mathf.Clamp(movVec.y, -movSpeed, movSpeed);
                    if (!autoRunner)
                    {
                        movVec.x = inputPos.x * movSpeed;
                        movVec.x = Mathf.Clamp(movVec.x, -movSpeed, movSpeed);
                    }
                }
            }
            else
            {
                movVec.x = fixedJoystick.Direction.x * movSpeed;
                if (screenPoint.x > 1 && movVec.x > 0)
                    movVec.x = 0;
                if (screenPoint.x < 0 && movVec.x < 0)
                    movVec.x = 0;

                movVec.y = fixedJoystick.Direction.y * movSpeed;
                if (screenPoint.y > 1 && movVec.y > 0)
                    movVec.y = 0;
                if (screenPoint.y < 0 && movVec.y < 0)
                    movVec.y = 0;
            }

        }
#endif
        if (autoRunner)
            movVec.x += movSpeed;
        else
        {
            if (movVec != Vector3.zero)
            {
                float angle = Vector3.Angle(Vector3.right, movVec.normalized);
                if (movVec.y < 0)
                    angle = -angle;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        this.transform.position += movVec;

        if (movVec != Vector3.zero)
            playerAnime.SetBool("isSwimming", true);
        else
            playerAnime.SetBool("isSwimming", false);
    }

    void MoveCamera()
    {
        if (this.transform.position.x > Camera.main.gameObject.transform.position.x - cameraThreshold)
        {
            Vector3 newPos = Camera.main.gameObject.transform.position;
            newPos.x = this.transform.position.x + cameraThreshold;
            Camera.main.gameObject.transform.position = newPos;
        }
    }

    public void ChangeHP(float amount)
    {
        HP += amount;
        HP = Mathf.Clamp(HP, -20, maxHP);
        hpSlider.value = HP / maxHP;
        if (HP <= 0)
            isDead = true;
    }

    public void ObjectsAttached()
    {
        if (attachedObjs > 0)
        {
            movSpeed = maxSpeed / ((attachedObjs + 1) / 1.5f);
            ChangeHP(-attachedObjs / 100f);
        }
        else
            movSpeed = maxSpeed;
    }

    void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                isPaused = true;
                gameUI.SetActive(false);
                pauseUI.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                isPaused = false;
                gameUI.SetActive(true);
                pauseUI.SetActive(false);
                settingsUI.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    public void PauseBT()
    {
        if (!isPaused)
        {
            isPaused = true;
            gameUI.SetActive(false);
            pauseUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            gameUI.SetActive(true);
            pauseUI.SetActive(false);
            settingsUI.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f;
        }
    }

    public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

