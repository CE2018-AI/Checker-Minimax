using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
   
    private Side side;
    private string name;
    public Player()
    { }

    public enum Side
    {
        BLACK, WHITE
    }
    public Player(string name, Side side)
    {
        this.name = name;
        this.side = side;
    }
    public Player(Side side)
    {
        this.name = side.ToString();
        this.side = side;
    }

    public Side getSide()
    {
        return side;
    }

    public Board.Decision makeMove(Move m, Board b)
    {
        return b.makeMove(m, side);
    }

   /*public Board.Decision makeRandomMove(Board b)
    {
        List<Move> moves = b.getAllValidMoves(side);
        Random rand = new Random();
        return b.makeMove(moves.get(rand.nextInt(moves.size())), side);
    }*/
    public string toString()
    {
        return name + "/" + side;
    }

}
