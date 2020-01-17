using UnityEngine;
using Photon.Pun;
public class animationPikachu : animationPKM
{
    public ParticleSystem GroundElectric;
    public ParticleSystem FaceElectricL;
    public ParticleSystem FaceElectricR;
    int cur = 0;
    int perFrame = 20;
    float[] eyeOffsetX = {0, 0, (float) 0.5, (float) 0.5};
    float[] eyeOffsetY = {0, (float) -0.75, 0, (float) -0.75};
    Animator anim;
    GameObject pkm;
    int blink1 = 0, blink2 = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (GroundElectric) GroundElectric.Stop();
        if (FaceElectricL) FaceElectricL.Stop();
        if (FaceElectricR) FaceElectricR.Stop();
    }
    void Awake()
    {
        
        pkm = transform.Find("PikachuM").gameObject;
        anim = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        cur = (++cur) % perFrame;
        Blink();
    }

    void Blink()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")) {
            blink1 = 2;
            blink2 = 0;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            blink1 = 1;
            blink2 = 0;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die")) {
            blink1 = 3;
            blink2 = 3;
        }

        if (cur <= perFrame / 2)
        {
            pkm.GetComponent<Renderer>().materials[3].mainTextureOffset = new Vector2(eyeOffsetX[blink1], eyeOffsetY[blink1]);
        }
        else
        {
            pkm.GetComponent<Renderer>().materials[3].mainTextureOffset = new Vector2(eyeOffsetX[blink2], eyeOffsetY[blink2]);
        }
    }

    public override void Attack1(GameObject target, int power) {
        var x = GroundElectric.main;
        int damage = 50;
        if (power == 0)
        {
        } else if (power == 1)
        {
        }
        anim.SetTrigger("isAttack");
        this.target = target;

        // Attack
        photonView.RPC("Attack1Particle", RpcTarget.All);

        calculateDamageAndScore(damage, power);
        
    }


    public override void Attack2(GameObject target, int power) {
        var x = FaceElectricL.main;
        var y = FaceElectricR.main;
        int damage = 50;
        if (power == 0)
        {
        } else if (power == 1)
        {
        }
        anim.SetTrigger("isJumping");
        this.target = target;

        // Attack
        photonView.RPC("Attack2Particle", RpcTarget.All);
        calculateDamageAndScore(damage, power);
    }

    public override string moveName1() {
        return "Tail Whip";
    }

    public override string moveName2() {
        return "Thunder Shock";
    }
    [PunRPC]
    public void Attack1Particle()
    {
        GroundElectric.Play();
    }
    [PunRPC]
    public void Attack2Particle()
    {
        FaceElectricL.GetComponent<ParticleTowardObject>().Play();
        FaceElectricR.GetComponent<ParticleTowardObject>().Play();
    }
       
}
