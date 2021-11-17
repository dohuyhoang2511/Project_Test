using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell_Viz : MonoBehaviour
{
    public GridCell Cell { get; set; }

    [HideInInspector] public bool isVisited = false;
    [HideInInspector] public GameObject northWall, southWall, eastWall, westWall, floor;

    [SerializeField] private SpriteRenderer innerBox;

    public void SetInnerColor(Color col)
    {
        innerBox.color = col;
    }
}
