using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private void Update()
    {
        if(GetComponent<ParticleSystem>().isStopped)
            GameManager.Instance.PoolParty.GetPool("Particles Pool").GetBackToPool(gameObject, GameManager.Instance.transform.position);
    }
}
