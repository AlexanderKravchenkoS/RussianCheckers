using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Checker : MonoBehaviour
{
    public CheckerData Data;

    public void MakeKing()
    {
        if (!Data.isKing)
        {
            Data.isKing = true;
            transform.eulerAngles += new Vector3(180, 0);
        }
    }
}

[Serializable]
public struct CheckerData
{
    public Vector2Int position;
    public bool isWhite;
    public bool isKing;

    public CheckerData(Vector2Int position, bool isWhite, bool isKing)
    {
        this.position = position;
        this.isWhite = isWhite;
        this.isKing = isKing;
    }
}