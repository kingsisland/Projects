using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
    public List<GameObject> pieces;
    public List<GameObject> capturedPieces;
    public string name;
    public int forward;

    public Player (string name, bool positive2Movement)
    {
        this.name = name;
        pieces = new List<GameObject>();
        capturedPieces = new List<GameObject>();

        if (positive2Movement == true)
            forward = 1;
        else
            forward = -1;
    }
}
