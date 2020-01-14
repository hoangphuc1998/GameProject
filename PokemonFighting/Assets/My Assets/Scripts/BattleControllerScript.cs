using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BattleControllerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    private GameObject pkmPlayer;
    public GameObject pkmOpponent = null;
    public Camera camera;
    private Slider mSlider;
    private bool sliderDir = true;
    public static GameObject LocalPlayerInstance = null;
    private float cameraTransition = 0.0f;
    private Rigidbody _body;
    private Animator _animator;
    private Vector3 _inputs = Vector3.zero;
    private float Speed = 5f;
    private bool isBoostSpeed = false;
    private bool isPower = false;
    private bool isColdDownAttack = false;



    // Start is called before the first frame update
    void Start()
    {
        Slider[] sliders = FindObjectsOfType<Slider>();
        foreach (Slider x in sliders)
            x.enabled = false;
        camera.enabled = photonView.IsMine;
        mSlider = gameObject.GetComponentInChildren<Slider>();
        pkmPlayer = gameObject;
        _body = pkmPlayer.GetComponent<Rigidbody>();
        _animator = pkmPlayer.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
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
            Debug.Log(mSlider.value);
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
        // Debug.Log(_inputs.x + " "+  _inputs.y + " "+ _inputs.z);
        if (_inputs != Vector3.zero)
        {
            pkmPlayer.transform.forward = _inputs;
            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }


        if (Input.GetButtonDown("Attack1"))
        {
            isPower = true;
        } 
        else if (Input.GetButtonUp("Attack1"))
        {
            isPower = false;
            // Attack1();
            photonView.RPC("Attack1", RpcTarget.All);
        }
        else if (Input.GetButtonDown("Attack2"))
        {
            isPower = true;
        } 
        else if (Input.GetButtonUp("Attack2"))
        {
            isPower = false;
            // Attack2();
            photonView.RPC("Attack2", RpcTarget.All);
        }
        else if (Input.GetButtonDown("Boost") && !isBoostSpeed)
        {
            isBoostSpeed = true;
            StartCoroutine(coolDownBoostSpeed());
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
        camera.transform.position = new Vector3(pkmPlayer.transform.position.x, pkmPlayer.transform.position.y + 1.5f, pkmPlayer.transform.position.z - 4f - cameraTransition);
        camera.transform.LookAt(pkmPlayer.transform);
    }
    public void Facing() {
        pkmPlayer.transform.LookAt(pkmOpponent.transform.position);
        pkmOpponent.transform.LookAt(pkmPlayer.transform.position);
        pkmPlayer.transform.localEulerAngles = new Vector3(0f, pkmPlayer.transform.localRotation.eulerAngles.y, 0f);
        pkmOpponent.transform.localEulerAngles = new Vector3(0f, pkmOpponent.transform.localRotation.eulerAngles.y, 0f);
    }

    void LateUpdate()
    {
        _body.MovePosition(_body.position + Speed * _inputs * (!isBoostSpeed ? 1 : 2) * Time.fixedDeltaTime);
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

    [PunRPC]
    private void Attack1()
    {
        int st = getPowerFromSlider();
        if (st == -1) return;
        pkmPlayer.GetComponent<animationPKM>().Attack1(st);
        StartCoroutine(coolDownAttack());
    }

    [PunRPC]
    private void Attack2()
    {
        int st = getPowerFromSlider();
        if (st == -1) return;
        pkmPlayer.GetComponent<animationPKM>().Attack2(st);
        StartCoroutine(coolDownAttack());
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }
    
    private IEnumerator coolDownAttack()
    {
        isColdDownAttack = true;
        yield return new WaitForSeconds(1.5f);
        isColdDownAttack = false;
    }
}
