using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] weaponParticlePrefabs;
    [SerializeField] private ParticleSystem[] skillParticlePrefabs;

    private List<Queue<ParticleSystem>> _poolWeaponParticle = new List<Queue<ParticleSystem>>();
    private List<Queue<ParticleSystem>> _poolSkillParticle = new List<Queue<ParticleSystem>>();

    private void Awake()
    {
        for (int i = 0; i < weaponParticlePrefabs.Length; ++i)
        {
            _poolWeaponParticle.Add(new Queue<ParticleSystem>());
        }

        for (int i = 0; i < skillParticlePrefabs.Length; ++i)
        {
            _poolSkillParticle.Add(new Queue<ParticleSystem>());
        }
    }


    public ParticleSystem CreateParticle(int index, bool isSkillParticle,Transform trans)
    {
        ParticleSystem particle;
        var prefabs = weaponParticlePrefabs;
        var pool = _poolWeaponParticle;
        if (isSkillParticle)
        {
            prefabs = skillParticlePrefabs;
            pool = _poolSkillParticle;
        }

        if (pool[index].Count > 0)
        {
            particle = pool[index].Dequeue();
            particle.gameObject.SetActive(true);
            particle.transform.parent = trans;
            particle.transform.localPosition=Vector3.zero;
            particle.transform.localRotation=Quaternion.identity;
        }
        else
        {
            particle = Instantiate(prefabs[index],trans);
        }

        return particle;
    }

    public void ReturnToPool(int index, bool isSkillParticle, ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
        var pool = _poolWeaponParticle;
        if (isSkillParticle)
        {
            pool = _poolSkillParticle;
        }

        pool[index].Enqueue(particle);
    }

    public void Play(ParticleSystem effect, int index, bool isSkillParticle, bool destroyWhenFinish = true)
    {
        effect.Play();

        if (destroyWhenFinish)
        {
            var main = effect.main;
            var totalDuration = main.duration + main.startLifetimeMultiplier;
            DOVirtual.DelayedCall(totalDuration, () => { ReturnToPool(index, isSkillParticle, effect); });
        }
        
    }
}