using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public CheckerData Data;
    public bool isWhite;
    public bool isKing;

    Checker(bool isWhite, bool isKing = false)
    {
        this.isWhite = isWhite;
        this.isKing = isKing;
        Data = new CheckerData(this);
    }

    public void MakeKing()
    {
        transform.eulerAngles = transform.eulerAngles + new Vector3(180, 0, 0);
    }

    [System.Serializable]
    public class CheckerData
    {
        public Vector2 position;
        public bool isWhite;
        public bool isKing;

        public CheckerData(Checker checker)
        {
            position = new Vector2(checker.gameObject.transform.position.x, checker.gameObject.transform.position.z);
            isWhite = checker.isWhite;
            isKing = checker.isKing;
        }
    }

}

