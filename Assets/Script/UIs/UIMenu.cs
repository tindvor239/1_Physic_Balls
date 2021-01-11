using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField]
    protected List<UIMenu> sections;
    [SerializeField]
    protected List<Image> images;
    [SerializeField]
    protected List<Text> menuInfos;

    #region Properties
    public List<UIMenu> Sections { get => sections; }
    public List<Image> Images { get => images; }
    public List<Text> MenuInfos { get => menuInfos; }
    #endregion
}
