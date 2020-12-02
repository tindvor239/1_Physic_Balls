using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Container : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Shooter.Instance.ContainBalls.Add(collision.gameObject);
        Shooter.Instance.ContainBalls = Shooter.Instance.ContainBalls.Distinct().ToList();
    }
}
