using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ControllerPKM : MonoBehaviour, IPunObservable
{
	private Rigidbody _body;
	private Animator _animator;
	private Vector3 _inputs = Vector3.zero;
	private float Speed = 3f;
    private bool isBoostSpeed = false;
    private bool isPower = false;
    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _inputs = Vector3.zero;
        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");
        // Debug.Log(_inputs.x + " "+  _inputs.y + " "+ _inputs.z);
        if (_inputs != Vector3.zero)
        {
            transform.forward = _inputs;
            _animator.SetBool("isMoving", true);
        } else
        {
            _animator.SetBool("isMoving", false);
        }
            

        if (Input.GetButtonDown("Attack1"))
        {

        } else if (Input.GetButtonDown("Attack2"))
        {
            GetComponent<animationPKM>().Attack2(0);
        } else if (Input.GetButtonDown("Boost") && !isBoostSpeed)
        {
            isBoostSpeed = true;
            StartCoroutine(coolDownBoostSpeed());
        }
        Debug.Log(isBoostSpeed);
    }

    private IEnumerator coolDownBoostSpeed()
    {
        
        yield return new WaitForSeconds(3);
        isBoostSpeed = false;

    }
	void FixedUpdate()
    {
        _body.MovePosition(_body.position + Speed * _inputs * (!isBoostSpeed ? 1 : 2) * Time.fixedDeltaTime);
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
}
