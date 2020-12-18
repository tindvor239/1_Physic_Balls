using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level
{
    [SerializeField]
    private string name;
    [SerializeField]
    private byte star;
    [SerializeField]
    private List<Ball> balls;
    [SerializeField]
    private List<Obstacle> obstacles;
    [SerializeField]
    private StorageJson storage = new StorageJson();
    #region Properties
    public List<Ball> Balls { get => balls; }
    public List<Obstacle> Obstacles { get => obstacles; }
    public StorageJson Storage { get => storage; }
    #endregion
}
