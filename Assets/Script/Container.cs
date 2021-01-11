using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Container : Singleton<Container>
{
    [SerializeField]
    private Collider2D[] colliders;
    #region Properties
    public Collider2D[] Colliders { get => colliders; }
    #endregion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Shooter.Instance.ContainBalls.Add(collision.gameObject);
        Shooter.Instance.ContainBalls = Shooter.Instance.ContainBalls.Distinct().ToList();
    }
}
