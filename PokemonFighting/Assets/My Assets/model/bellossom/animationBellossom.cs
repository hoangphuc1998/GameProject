using UnityEngine;
using Photon.Pun;
// BELLOSSOM
public class animationBellossom : animationPKM
{
    public ParticleSystem particleAttack1;
    public ParticleSystem particleAttack2;
    int cur = 0;
    int perFrame = 20;
    float[] eyeOffsetX = {0, (float) 0.5, (float) 0.5, (float) 0.5};
    float[] eyeOffsetY = {0, (float) -0.25, 0, (float) -0.75};
    Animator anim;
    GameObject pkm;
    int blink1 = 0, blink2 = 1;

    
    // Start is called before the first frame update
    void Start()
    {
        if (particleAttack1) particleAttack1.Stop();
        if (particleAttack2) particleAttack2.Stop();
    }
    void Awake()
    {
        pkm = transform.Find("Bellossom").gameObject;
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
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("isJumping")) {
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
    public override void Attack1(GameObject target, int power)
    {
        var x = particleAttack1.main;
        int damage = 0;
        if (power == 0)
        {
            damage = 10;
            GetComponent<BattleControllerScript>().health += 5;
        }
        else if (power == 1)
        {
            damage = 10;
            GetComponent<BattleControllerScript>().health += 10;
        }
        anim.SetTrigger("isJumping");
        this.target = target;
        photonView.RPC("Attack1Particle", RpcTarget.All);
        calculateDamageAndScore(damage, power);
    }


    public override void Attack2(GameObject target, int power)
    {
        var x = particleAttack2.main;
        var y = particleAttack2.main;
        int damage = 0;
        if (power == 0)
        {
            damage = 10;
        }
        else if (power == 1)
        {
            damage = 20;
        }
        anim.SetTrigger("isJumping");
        this.target = target;
        photonView.RPC("Attack2Particle", RpcTarget.All);
        calculateDamageAndScore(damage, power);
    }

    [PunRPC]
    public void Attack1Particle()
    {
        particleAttack1.Play();
    }
    [PunRPC]
    public void Attack2Particle()
    {
        particleAttack2.GetComponent<ParticleTowardObject>().Play();
        particleAttack2.GetComponent<ParticleTowardObject>().Play();
    }

    public override string moveName1() {
        return "Mega Drain";
    }

    public override string moveName2() {
        return "Magical Leaf";
    }
}
