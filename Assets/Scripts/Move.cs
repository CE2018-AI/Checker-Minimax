using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    private Vector2 start;
    private Vector2 end;

    public Move(int startRow, int startCol, int endRow, int endCol)
    {
        start = new Vector2(startRow, startCol);
        end = new Vector2(endRow, endCol);
    }

    public Move(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }

    public Vector2 getStart()
    {
        return start;
    }

    public Vector2 getEnd()
    {
        return end;
    }

    public string toString()
    {
        return "Start: " + start.x + ", " + start.y + " End: " + end.x + ", " + end.y;
    }

    public bool equals(Object m)
    {
        //if (!(m instanceof Move))
        if(m.GetType() != typeof(Move))
            return false;
        Move x = (Move)m;
        if (this.getStart()==x.getStart() && this.getEnd()==x.getEnd())
            return true;

        return false;
    }

}
