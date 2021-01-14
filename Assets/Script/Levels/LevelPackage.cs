using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Levels")]
public class LevelPackage : ScriptableObject
{
    public new string name;
    [SerializeField]
    private byte stars = 3;
    [SerializeField]
    private int balls;
    [SerializeField]
    private List<string> items;
    [SerializeField]
    private int row, column;
    [SerializeField]
    private byte[] points = new byte[3];
    [SerializeField]
    private byte turnCount;
    [SerializeField]
    private bool canMoveUp;
    #region Properties
    public byte Stars { get => stars; }
    public int Balls { get => balls; }
    public int Row { get => row; }
    public int Column { get => column; }
    public List<string> Items { get => items; }
    public byte TurnCount { get => turnCount; }
    public bool CanMoveUp { get => canMoveUp; }
    #endregion
    #region Method
    public void Pack(BaseLevel baseLevel, List<string> items)
    {
        name = baseLevel.name;
        stars = baseLevel.stars;
        balls = baseLevel.balls;
        this.items = items;
        row = baseLevel.row;
        column = baseLevel.column;
        points[0] = baseLevel.star1;
        points[1] = baseLevel.star2;
        points[2] = baseLevel.star3;
        turnCount = baseLevel.turnCount;
        canMoveUp = baseLevel.canMoveUp;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
    public void Unpack(Level level)
    {
        level.Name = name;
        level.Stars = stars;
        level.Balls = new List<Ball>(balls);
        level.Items = new List<Item>(items.Count);
        level.Points = points;
        level.TurnCount = turnCount;
        level.Row = row;
        level.Column = column;
        level.CanMoveUp = canMoveUp;
    }
    public void Save(Level level)
    {
        stars = level.Stars;
    }
    #endregion
}
