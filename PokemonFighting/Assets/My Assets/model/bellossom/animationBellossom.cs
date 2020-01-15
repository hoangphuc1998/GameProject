using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        particleAttack1.Stop();
        particleAttack2.Stop();
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


    public override void Attack1(GameObject target=null, int st = 0) {
        anim.SetTrigger("isJumping");
        particleAttack1.Play();
    }

    public override void Attack2(GameObject target=null, int st = 0) {
        anim.SetTrigger("isJumping");
        particleAttack2.Play();
    }

    public override string moveName1() {
        return "Mega Drain";
    }

    public override string moveName2() {
        return "Magical Leaf";
    }
}
