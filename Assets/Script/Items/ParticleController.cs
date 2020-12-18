using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        GameManager.Instance.PoolParty.GetPool("Particles Pool").GetBackToPool(gameObject, GameManager.Instance.transform.position);
    }
}
