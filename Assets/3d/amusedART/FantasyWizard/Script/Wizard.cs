using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Wizard : MonoBehaviour
{

	public const string IDLE	= "Wizard_Idle";
	public const string RUN		= "Wizard_Run";
	public const string ATTACK	= "Wizard_Attack";
	public const string SKILL	= "Wizard_Skill";
	public const string DAMAGE	= "Wizard_Damage";
	public const string STUN	= "Wizard_Stun";
	public const string DEATH	= "Wizard_Death";
    
    Animator anim;

	void Start ()
    {
		anim = GetComponent<Animator>();
        for(int i = 0; i < 6; i++)
        {
            gameObject.GetComponent<NetworkAnimator>().SetParameterAutoSend(i, true);
        }
    }

	public void IdleAni ()
    {
        anim.SetBool("Casting", false);
        anim.SetBool("Moving", false);
        anim.SetBool("CastAoe", false);
        anim.SetBool("Damage", false);
        anim.SetBool("Stun", false);
        anim.SetBool("Dead", false);
        //anim.CrossFade (IDLE);
    }

	public void RunAni ()
    {
        anim.SetBool("Moving", true);
        anim.SetBool("Casting", false);
        anim.SetBool("CastAoe", false);
        anim.SetBool("Damage", false);
        anim.SetBool("Stun", false);
        anim.SetBool("Dead", false);
        //anim.CrossFade (RUN);
    }

	public void AttackAni ()
    {
        anim.SetBool("Casting", true);
        anim.SetBool("Moving", false);
        anim.SetBool("CastAoe", false);
        anim.SetBool("Damage", false);
        anim.SetBool("Stun", false);
        anim.SetBool("Dead", false);
        //anim.CrossFade (ATTACK);
        //      anim[ATTACK].speed = 1.0f;
    }

	public void SkillAni ()
    {
        anim.SetBool("CastAoe", true);
        anim.SetBool("Casting", false);
        anim.SetBool("Moving", false);
        anim.SetBool("Damage", false);
        anim.SetBool("Stun", false);
        anim.SetBool("Dead", false);
        //anim.CrossFade (SKILL);
        //      anim[SKILL].speed = 1.8f;
    }

	public void DamageAni ()
    {
        anim.SetBool("Damage", true);
        anim.SetBool("Casting", false);
        anim.SetBool("Moving", false);
        anim.SetBool("CastAoe", false);
        anim.SetBool("Stun", false);
        anim.SetBool("Dead", false);
        //anim.CrossFade (DAMAGE);
    }

	public void StunAni ()
    {
        anim.SetBool("Stun", true);
        anim.SetBool("Casting", false);
        anim.SetBool("Moving", false);
        anim.SetBool("CastAoe", false);
        anim.SetBool("Damage", false);
        anim.SetBool("Dead", false);
        //anim.CrossFade (STUN);
    }

	public void DeathAni ()
    {
        anim.SetBool("Dead", true);
        anim.SetBool("Casting", false);
        anim.SetBool("Moving", false);
        anim.SetBool("CastAoe", false);
        anim.SetBool("Damage", false);
        anim.SetBool("Stun", false);
        //anim.CrossFade (DEATH);
    }
		
}


















