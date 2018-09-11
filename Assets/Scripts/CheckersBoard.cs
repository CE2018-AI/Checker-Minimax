﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour {

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    private Vector3 boardOffset = new Vector3(-4.0f,0,-4.0f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0, 0);

    private Vector2 mouseOver;
    private Vector2 startDarg,endDrag;

    private bool isWhite;
    private bool isWhiteTurn;
    private bool haskilled;

    private Piece selectedPiece;
    private List<Piece> forcePiece;
    private bool jumpAgain;
    // Use this for initialization
    void Start() {
        GenerateBoard();
        isWhiteTurn = true;
        isWhite = true;
        forcePiece = new List<Piece>();
        jumpAgain = false;
    }

    // Update is called once per frame
    void Update() {
        updateMouseOver();
        //Debug.Log(mouseOver);
        if ((isWhite)?isWhiteTurn:!isWhiteTurn)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (selectedPiece != null)
                UpdatePieceDrag(selectedPiece);

            if (Input.GetMouseButtonDown(0))
                SelectPiece(x,y);

            if (Input.GetMouseButtonUp(0))
                TryMove((int)startDarg.x, (int)startDarg.y, x, y);
        }
    }
    private void TryMove(int x1, int y1,int x2,int y2)
    {
        //forcePiece = ScanForPossibleMove();

        startDarg = new Vector2(x1,y1);
        endDrag = new Vector2(x2,y2);
        selectedPiece = pieces[x1,y1];

        if(x2<0||x2>=pieces.Length|| y2<0||y2>=pieces.Length)
        {
            if (selectedPiece != null)
                MovePiece(selectedPiece,x1,y1);

            startDarg = Vector2.zero;
            selectedPiece = null;
            return;
        }

        if(selectedPiece != null)
        {
            //If it has not move
            if(endDrag == startDarg)
            {
                MovePiece(selectedPiece, x1, y1);
                startDarg = Vector2.zero;
                selectedPiece = null;
                return;
            }
            //check if its a valid move
            if(selectedPiece.ValidMove(pieces,x1,y1,x2,y2))
            {
                // Did we kill anything
                // jump
                if(Mathf.Abs(x2-x1)==2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if(p!=null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        Destroy(p.gameObject);
                        haskilled = true;
                      
                    }
                }

                // were we supposed to kill anything?
                /* if(forcePiece.Count!=0 && !haskilled)
                {
                    MovePiece(selectedPiece, x1, y1);
                    startDarg = Vector2.zero;
                    selectedPiece = null;
                    return;
                }*/

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2); 
                // can jump again?
                if(haskilled)
                {
                    //white
                    if(pieces[x2, y2].isWhite)
                    {
                        // chk has another to kill ?
                        if (x2 > 1 && y2 < 6 && x2 < 6)
                        {
                            
                            if (pieces[x2 - 1, y2 + 1] != null && !pieces[x2 - 1, y2 + 1].isWhite && pieces[x2 - 2, y2 + 2] == null)
                            {
                                Debug.Log("Jump Left Again.");
                                jumpAgain = true;


                            }
                            else if (pieces[x2 + 1, y2 + 1] != null && !pieces[x2 + 1, y2 + 1].isWhite && pieces[x2 + 2, y2 + 2] == null)
                            {
                                Debug.Log("Jump Right Again.");
                                jumpAgain = true;
                            }
                            else
                            {
                                haskilled = false;
                            }
                        }
                        else
                            haskilled = false;
                    }
                    else
                        haskilled = false;
                    //king

                }
                Endturn();
            }
            else
            {
                MovePiece(selectedPiece, x1, y1);
                startDarg = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
    }
    private bool JumpAgain(int x,int y,int x2,int y2)
    {
        if (x == x2 && y == y2)
        {
            Piece p = pieces[(x + x2) / 2, (y + y2) / 2];
            if (p != null)
            {
                pieces[(x + x2) / 2, (y + y2) / 2] = null;
                Destroy(p.gameObject);
            }
            pieces[x2, y2] = selectedPiece;
            pieces[(int)startDarg.x, (int)startDarg.y] = null;
            MovePiece(selectedPiece, x2, y2);
            return true;
        }
        else
            return false;
     
    }
    private void UpdatePieceDrag(Piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }
    private void updateMouseOver()
    {
        // if white turn
        if(!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,25.0f,LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x-boardOffset.x);
            mouseOver.y = (int)(hit.point.z-boardOffset.z);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }
    private void SelectPiece(int x,int y)
    {
        // Out of bounds
        if (x < 0 || x >= pieces.Length || y < 0 || y >= pieces.Length) return;

        Piece p = pieces[x, y];
        if(p!=null && p.isWhite == isWhite) 
        {
            if(forcePiece.Count ==0)
            {
                selectedPiece = p;
                startDarg = mouseOver;
            }
            else
            {
                //Look for the piece under our fourced pieces list
                if (forcePiece.Find(fp => fp == p) == null)
                    return;

                selectedPiece = p;
                startDarg = mouseOver;

            }
            selectedPiece = p;
            startDarg = mouseOver;
            Debug.Log(selectedPiece.name);
        }
    }
    private void GenerateBoard()
    {
        for (int y = 0; y < 3; y++)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                //Generate Piece
                GeneratePiece((oddRow)?x:x+1, y);
            }
        }

        for (int y = 7; y >4; y--)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                //Generate Piece
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }
    }
    private void GeneratePiece(int x, int y)
    {
        bool isPieceWhite = (y > 3) ? false : true;
        GameObject go = Instantiate((isPieceWhite)?whitePiecePrefab:blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }
    private void Endturn()
    {
        int y = (int)endDrag.y;

        if(selectedPiece != null)
        {
            if(selectedPiece.isWhite && !selectedPiece.isKing && y==7)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right*180);
            }
            else if(!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
        }
        selectedPiece = null;
        startDarg = Vector2.zero;

        //if(ScanForPossibleMove(selectedPiece,x,y).Count!=0 && haskilled)

        if (!jumpAgain)
        {
            isWhiteTurn = !isWhiteTurn;
            isWhite = !isWhite;
        }
        jumpAgain = false;
        CheckVictory();
        haskilled = false;
    }
    private void CheckVictory()
    {
        var ps = FindObjectsOfType<Piece>();
        bool hasWhite = false, hasBlack = false;
        for(int i=0; i< ps.Length;i++)
        {
            if (ps[i].isWhite)
                hasWhite = true;
            else
                hasBlack = true;
        }

        if (!hasWhite)
            Victory(false);
        if(!hasBlack)
            Victory(true);
    }
    private void Victory(bool iswhite)
    {
        if (!isWhite)
            Debug.Log("White Win");
        else
            Debug.Log("Black Win");

    }
    private List<Piece> ScanForPossibleMove(Piece p,int x,int y)
    {
        forcePiece = new List<Piece>();

        if (pieces[x, y].IsforceToMove(pieces, x, y))
            forcePiece.Add(pieces[x,y]);

        return forcePiece;
    }
    private List<Piece> ScanForPossibleMove()
    {
        forcePiece = new List<Piece>();

        // chk all the pieces
        for(int i =0;i<8;i++)
        {
            for (int j = 0; j < 8;j++)
            {
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn)
                    if (pieces[i, j].IsforceToMove(pieces, i, j))
                        forcePiece.Add(pieces[i,j]);
            }
        }

        return forcePiece;
    }
    private void MovePiece(Piece p,int x,int y)
    {
        p.transform.position = (Vector3.right * x)+(Vector3.forward*y)+boardOffset+ pieceOffset;
    }
}
