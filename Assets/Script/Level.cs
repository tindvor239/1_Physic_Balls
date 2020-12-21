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
    private List<Item> items;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private StorageJson storage = new StorageJson();
    #region Properties
    public string Name { get => name; set => name = value; }
    public byte Star { get => star; set => star = value; }
    public List<Ball> Balls { get => balls; set => balls = value; }
    public List<Item> Items { get => items; set => items = value; }
    public StorageJson Storage { get => storage; }
    public GameObject BallPrefab { get => ballPrefab; }
    #endregion
}
