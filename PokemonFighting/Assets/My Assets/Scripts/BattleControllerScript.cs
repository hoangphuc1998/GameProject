using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class BattleControllerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    float[] limit_attack = {30f, 60f};
    private FloatingJoystick joystick;
    public GameObject pkmOpponent = null;
    private Camera camera;
    private GameObject cameraWrapper;
    private Slider mSlider;
    private bool sliderDir = true;
    public static GameObject LocalPlayerInstance = null;
    private float cameraTransition = 0.0f;
    private Rigidbody _body;
    private Animator _animator;
    private Vector3 _inputs = Vector3.zero;
    private Vector3 _inputJoyStick = Vector3.zero;
    private float Speed = 5f;
    private bool isBoostSpeed = false;
    private bool isPower = false;
    private bool isColdDownAttack = false;
    public int health = 100;
    public int maxHealth = 100;

    private GameObject UIController;
    // Pokemon UI (health bar, name)
    public GameObject pkmUIPrefab;
    // Score
    public int score = 0;
    bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {

        cameraWrapper = transform.Find("CameraWrapper").gameObject;
        camera = transform.Find("CameraWrapper/ThirdPersonCamera").GetComponent<Camera>();
        transform.Find("CameraWrapper/ThirdPersonCamera").GetComponent<AudioListener>().enabled = photonView.IsMine;
        camera.enabled = photonView.IsMine;
        
        if (photonView.IsMine)
        {
            UIController = Instantiate(Resources.Load("UIController") as GameObject, gameObject.transform);
            UIController.name = "UIController";
            joystick = transform.Find("UIController/Joystick").GetComponent<FloatingJoystick>();
            mSlider = transform.Find("UIController/PowerBar").GetComponent<Slider>();

            Button attack1Btn = transform.Find("UIController/Attack1").GetComponent<Button>();
            attack1Btn.gameObject.SetActive(true);
            Button attack2Btn = transform.Find("UIController/Attack2").GetComponent<Button>();
            attack2Btn.gameObject.SetActive(true);

            EventTrigger.Entry emitingAttack = new EventTrigger.Entry();
            emitingAttack.eventID = EventTriggerType.PointerDown;
            emitingAttack.callback.AddListener((data) => { EmitingAttackDown(); });

            EventTrigger.Entry releaseAttack1 = new EventTrigger.Entry();
            releaseAttack1.eventID = EventTriggerType.PointerUp;
            releaseAttack1.callback.AddListener((data) => { ReleaseAttack1(); });

            EventTrigger.Entry releaseAttack2 = new EventTrigger.Entry();
            releaseAttack2.eventID = EventTriggerType.PointerUp;
            releaseAttack2.callback.AddListener((data) => { ReleaseAttack2(); });

            EventTrigger triggerAttack1Btn = attack1Btn.GetComponent<EventTrigger>();
            triggerAttack1Btn.triggers.Add(emitingAttack);
            triggerAttack1Btn.triggers.Add(releaseAttack1);

            EventTrigger triggerAttack2Btn = attack2Btn.GetComponent<EventTrigger>();
            triggerAttack2Btn.triggers.Add(emitingAttack);
            triggerAttack2Btn.triggers.Add(releaseAttack2);
            
            joystick.gameObject.SetActive(true);
            mSlider.gameObject.SetActive(true);

            gameObject.tag = "Player";
        } else
        {
            //camera.name = "UnusedCamera";
            Destroy(cameraWrapper);
            Destroy(UIController);

            gameObject.tag = "OtherPlayer";
        }
        
        _body = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        // Set UI
        GameObject pkmUI = Instantiate(pkmUIPrefab, gameObject.transform.Find("Canvas").transform);
        pkmUI.SendMessage("SetTarget", gameObject.GetComponent<BattleControllerScript>(), SendMessageOptions.RequireReceiver);


    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        //Leave Room when die
        if (this.health <= 0)
        {
            isDead = true;
            PlayerPrefs.SetInt("score", this.score);
            GameObject.Find("BattleManager").GetComponent<BattleManager>().ProcessDeath(this.gameObject);
        }
        ControlPlayer();
        UpdateCameraPosition();
        UpdateSlider();
        // Facing();
    }
    private void UpdateSlider()
    {
        if (isPower) {
            mSlider.value += (float)(0.05 * (sliderDir ? 1 : -1));
            // Debug.Log(mSlider.value);
            if (mSlider.value >= 1) sliderDir = false;
            else if (mSlider.value <= 0) sliderDir = true;
        } else
        {
            StartCoroutine(resetSlider());
        }
    }
    void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void ControlPlayer()
    {
        if (isColdDownAttack) return;
        _inputs = Vector3.zero;
        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");
        _inputJoyStick = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
        // Debug.Log(_inputs.x + " "+  _inputs.y + " "+ _inputs.z);
        if (_inputs != Vector3.zero)
        {
            transform.forward = _inputs;
            _animator.SetBool("isMoving", true);
        }
        else if (_inputJoyStick != Vector3.zero)
        {
            transform.forward = _inputJoyStick;
            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }


        if (Input.GetButtonDown("Attack1"))
        {
            EmitingAttackDown();
        } 
        else if (Input.GetButtonUp("Attack1"))
        {
            ReleaseAttack1();
            //photonView.RPC("Attack1", RpcTarget.All);
        }
        else if (Input.GetButtonDown("Attack2"))
        {
            EmitingAttackDown();
        } 
        else if (Input.GetButtonUp("Attack2"))
        {
            ReleaseAttack2();
           // photonView.RPC("Attack2", RpcTarget.All);
        }
        else if (Input.GetButtonDown("Boost") && !isBoostSpeed)
        {
            isBoostSpeed = true;
            StartCoroutine(coolDownBoostSpeed());
        } 
        else if (Input.GetButtonDown("RotateLeft"))
        {
            //cameraWrapper.transform.eulerAngles += new Vector3(0, 1, 0);
        } else if (Input.GetButtonDown("RotateRight"))
        {
            //cameraWrapper.transform.eulerAngles -= new Vector3(0, 1, 0);
        }
    }
    private void UpdateCameraPosition()
    {
        //camera.transform.position = new Vector3(pkmPlayer.transform.position.x, pkmPlayer.transform.position.y + 1.5f, pkmPlayer.transform.position.z - 3f);
        if ((_animator.GetBool("isMoving") == true || isPower) && cameraTransition <= 3)
        {
            cameraTransition += 0.02f;
        } else if (!(_animator.GetBool("isMoving") == true || isPower) && cameraTransition > 0)
        {
            cameraTransition -= 0.05f;
        }
        if (isBoostSpeed && camera.fieldOfView < 65)
        {
            camera.fieldOfView += 1;
        } else if (!isBoostSpeed && camera.fieldOfView > 55)
        {
            camera.fieldOfView -= 2;
        }
        camera.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z - 4f - cameraTransition);
        camera.transform.LookAt(transform);
    }

    public void EmitingAttackDown()
    {
        isPower = true;
    }

    public void ReleaseAttack1()
    {
        isPower = false;
        Attack1();
    }

    public void ReleaseAttack2()
    {
        isPower = false;
        Attack2();
    }
    public void Facing() {
        /*
        pkmPlayer.transform.LookAt(pkmOpponent.transform.position);
        pkmOpponent.transform.LookAt(transform.position);
        pkmPlayer.transform.localEulerAngles = new Vector3(0f, pkmPlayer.transform.localRotation.eulerAngles.y, 0f);
        pkmOpponent.transform.localEulerAngles = new Vector3(0f, pkmOpponent.transform.localRotation.eulerAngles.y, 0f);
        */
    }

    void LateUpdate()
    {
        _body.MovePosition(_body.position + Speed * _inputs * (!isBoostSpeed ? 1 : 2) * Time.fixedDeltaTime);
        _body.MovePosition(_body.position + Speed * _inputJoyStick * (!isBoostSpeed ? 1 : 2) * Time.fixedDeltaTime);
    }
    private IEnumerator resetSlider()
    {
        yield return new WaitForSeconds(.15f);
        mSlider.value = 0;
        sliderDir = true;
    }
    private IEnumerator coolDownBoostSpeed()
    {

        yield return new WaitForSeconds(3);
        isBoostSpeed = false;

    }

    private int getPowerFromSlider()
    {
        if (0.7f < mSlider.value && mSlider.value < 1f)
        {
            return 1;
        } else if (0.4f < mSlider.value && mSlider.value < 0.7f)
        {
            return 0;
        }
        return -1;
    }

    private void Attack1()
    {
        if (!photonView.IsMine) return;
        int st = getPowerFromSlider();
        if (st == -1) return;
        GameObject target = FindPray(limit_attack[st]);
        if (target == null)
        {
            return;
        }
        GetComponent<animationPKM>().Attack1(target, st);
        StartCoroutine(coolDownAttack());
    }

    private void Attack2()
    {
        if (!photonView.IsMine) return;
        int st = getPowerFromSlider();
        if (st == -1) return;
        GameObject target = FindPray(limit_attack[st]);
        if (target == null)
        {
            return;
        }
        GetComponent<animationPKM>().Attack2(target, st);
        StartCoroutine(coolDownAttack());
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.health);
            stream.SendNext(this.score);
        }
        else
        {
            this.health = (int)stream.ReceiveNext();
            this.score = (int)stream.ReceiveNext();
        }
    }
    
    private IEnumerator coolDownAttack()
    {
        isColdDownAttack = true;
        yield return new WaitForSeconds(1f);
        isColdDownAttack = false;
    }
    [PunRPC]
    public void DecreaseHealth(int amount)
    {
        this.health -= amount;
        if (this.health < 0)
        {
            this.health = 0;
        }
    }

    [PunRPC]
    public void IncreaseScore(int amount)
    {
        this.score += amount;
    }

    private GameObject FindPray(float limit)
    {
        GameObject[] otherPlayers = GameObject.FindGameObjectsWithTag("OtherPlayer");
        if (otherPlayers.Length == 0) return null;
        GameObject ans = null;
        float min_angle = 180;
        foreach (GameObject otherPlayer in otherPlayers)
        {
            Vector2 v1 = new Vector2(otherPlayer.transform.position.x - gameObject.transform.position.x, otherPlayer.transform.position.z - gameObject.transform.position.z);
            Vector2 v2 = new Vector2(transform.forward.x, transform.forward.z);
            /*
            Debug.DrawLine(
                new Vector3(otherPlayer.transform.position.x,1, otherPlayer.transform.position.z),
                new Vector3(gameObject.transform.position.x, 1, gameObject.transform.position.z)
                , Color.black, 10000);
            Debug.DrawLine(
            new Vector3(v2.x + gameObject.transform.position.x, 1, v2.y + gameObject.transform.position.z),
            new Vector3(gameObject.transform.position.x, 1, gameObject.transform.position.z)
            , Color.black, 10000);
            
            Debug.Log(_angle);
            */
            float _angle = Mathf.Abs(Vector2.Angle(v1, v2));
            if (_angle <= limit && min_angle > _angle)
            {
                ans = otherPlayer;
                min_angle = _angle;
            }
        }
        return ans;
    }
}
