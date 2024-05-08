using System;
using System.Collections;
using DG.Tweening;
using Script;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Animator leftWing;
    [SerializeField] private Animator rightWing;
    [SerializeField] private Vector2 uiOffset;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private Transform VFX;
    [SerializeField] private GameObject normalSkill;

    private AudioClip clip;
    private AudioSource soundComponent;
    private AudioSource soundComponentCast;
    
    [FormerlySerializedAs("_shield")] public ParticleSystem Shield;

    public void AttackNormal(Transform target, Action action, int number)
    {
        Skill3(target, action, number);
    }

    public void AttackSkill(int characterType, InGamePlayerObject target, Action action)
    {
        switch (characterType)
        {
            case 0:
                Skill1(characterType, target, action);
                break;
            case 1:
                Skill1(characterType, target, action, 0.2f, 0.35f);
                break;
            case 2:
                Skill1(characterType, target, action, userFirePoint: true);
                break;
            case 3:
                Skill1(characterType, target, action, userFirePoint: true);
                break;
            case 5:
                Skill1(characterType, target, action, userFirePoint: true, skillEffectDelay: 0.5f);
                break;
            case 6:
                Skill1(characterType, target, action, weaponEffectDelay: 0.5f, skillEffectDelay: 1f);
                break;
            case 7:
                Skill1(characterType, target, action, weaponEffectDelay: 0.8f, skillEffectDelay: 0.8f);
                break;
        }
    }


    public void Skill1(int EffectNumber, InGamePlayerObject target, Action action, float weaponEffectDelay = 0,
        float skillEffectDelay = 0, bool userFirePoint = false)
    {
        soundComponentCast = null;
        ParticleSystem weaponEffect;
        if (userFirePoint)
        {
            weaponEffect = InGameManager.Instance.effectManager.CreateParticle(EffectNumber, false, FirePoint);
        }
        else
        {
            weaponEffect = InGameManager.Instance.effectManager.CreateParticle(EffectNumber, false, VFX);
        }
        

        ParticleSystem skillEffect;
        if (EffectNumber == 5 && target.playerAnimation.Shield != null)
        {
            skillEffect = target.playerAnimation.Shield;
        }
        else
        {
            skillEffect = InGameManager.Instance.effectManager.CreateParticle(EffectNumber, true, target.transform);
            if (EffectNumber == 5)
            {
                target.playerAnimation.Shield = skillEffect;
            }
        }

        soundComponent = weaponEffect.GetComponent<AudioSource>();
        soundComponentCast = skillEffect.GetComponent<AudioSource>();

        if (EffectNumber == 3)
        {
            playerAnim.SetTrigger("Attack1");
        }

        if (EffectNumber == 1 || EffectNumber == 6)
        {
            playerAnim.SetTrigger("Attack3");
        }


        if (EffectNumber == 0 || EffectNumber == 2 || EffectNumber == 5)
        {
            playerAnim.SetTrigger("Attack2");
        }

        if (EffectNumber == 7)
        {
            playerAnim.SetTrigger("Attack4");
            DOVirtual.DelayedCall(0.3f, () =>
            {
                leftWing.SetTrigger("Attack");
                rightWing.SetTrigger("Attack");
            });
        }

        DOVirtual.DelayedCall(weaponEffectDelay, () =>
        {
            InGameManager.Instance.effectManager.Play(weaponEffect, EffectNumber, false);
            action?.Invoke();
            if (soundComponent)
            {
                MainSoundPlay();
            }
        });

        DOVirtual.DelayedCall(skillEffectDelay, () =>
        {
            InGameManager.Instance.effectManager.Play(skillEffect, EffectNumber, true, EffectNumber != 5);
            if (soundComponentCast)
            {
                CastSoundPlay();
            }
        });
    }

    public void Skill3(Transform target, Action action, int number = 1)
    {
        playerAnim.SetTrigger("Attack2");
        ParticleSystem weaponEffect = InGameManager.Instance.effectManager.CreateParticle(4, false, FirePoint);

        InGameManager.Instance.effectManager.Play(weaponEffect, 4, false);

        StartCoroutine(NormalAttack(target, action, number));
    }

    private IEnumerator NormalAttack(Transform target, Action action, int number = 1)
    {
        yield return new WaitForSeconds(0.3f);
        float time = 0.7f / number;
        for (int i = 0; i < number; ++i)
        {
            GameObject projectile = Instantiate(normalSkill, FirePoint.position, FirePoint.rotation);
            projectile.GetComponent<HS_TargetProjectile>().UpdateTarget(target, (Vector3)uiOffset, action);
            soundComponent = normalSkill.GetComponent<AudioSource>();
            if (soundComponent)
            {
                clip = soundComponent.clip;
                soundComponent.PlayOneShot(clip);
            }

            yield return new WaitForSeconds(time);
        }
    }

    public void MainSoundPlay()
    {
        clip = soundComponent.clip;
        soundComponent.PlayOneShot(clip);
    }

    public void CastSoundPlay()
    {
        soundComponentCast.Play(0);
    }

    public void Dead()
    {
        playerAnim.SetTrigger("Die");
    }

    public void Revival()
    {
        playerAnim.SetTrigger("Idle");
    }

    public void ActiveShield()
    {
        if (Shield != null)
        {
            Shield.gameObject.SetActive(true);
        }
        else
        {
            Shield = InGameManager.Instance.effectManager.CreateParticle(5, true, transform);
        }

        InGameManager.Instance.effectManager.Play(Shield, 5, true, false);
    }

    public void InActiveShield()
    {
        InGameManager.Instance.effectManager.ReturnToPool(5, true, Shield);
        Shield = null;
    }
}