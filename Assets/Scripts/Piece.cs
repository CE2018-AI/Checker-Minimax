﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    public bool isWhite;
    public bool isKing;

    public bool ValidMove(Piece[,] board, int x1,int y1, int x2, int y2)
    {
        // If u r moving on top of another piece
        if (board[x2, y2] != null) return false;

        int deltaMove = Mathf.Abs(x1 - x2);
        int deltaMoveY = y2 - y1;
        if (isWhite || isKing)
        {
            if(deltaMove ==1)
            {
                if (deltaMoveY == 1)
                    return true;
            }
            else if(deltaMove == 2)
            {
                if(deltaMoveY == 2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }

        if (!isWhite || isKing)
        {
            if (deltaMove == 1)
            {
                if (deltaMoveY == -1)
                    return true;
            }
            else if (deltaMove == 2)
            {
                if (deltaMoveY == -2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }
        return false;
    }

    public bool IsforceToMove(Piece[,] board,int x,int y)
    {
        if(isWhite || isKing)
        {
            //Top left
            if(x>=2 && y<=5)
            {
                Piece p = board[x - 1, y + 1];
                // If there is a piece, and it is not the same color as our
                if(p!=null && p.isWhite != isWhite)
                {
                    //chk if its possible to land after the jump
                    if (board[x - 2, y + 2] == null)
                        return true;
                }
            }

            //Top Right
            if (x <= 5 && y <= 5)
            {
                Piece p = board[x + 1, y + 1];
                // If there is a piece, and it is not the same color as our
                if (p != null && p.isWhite != isWhite)
                {
                    //chk if its possible to land after the jump
                    if (board[x + 2, y + 2] == null)
                        return true;
                }
            }
        }
        
        if(!isWhite||isKing)
        {
            //Bottom left
            if (x >= 2 && y >= 2)
            {
                Piece p = board[x - 1, y - 1];
                // If there is a piece, and it is not the same color as our
                if (p != null && p.isWhite != isWhite)
                {
                    //chk if its possible to land after the jump
                    if (board[x - 2, y - 2] == null)
                        return true;
                }
            }

            //Bottom Right
            if (x <= 5 && y >= 2)
            {
                Piece p = board[x + 1, y - 1];
                // If there is a piece, and it is not the same color as our
                if (p != null && p.isWhite != isWhite)
                {
                    //chk if its possible to land after the jump
                    if (board[x + 2, y - 2] == null)
                        return true;
                }
            }
        }
        return false;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
    }

}
