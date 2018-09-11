using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Board : MonoBehaviour {
    
    private Type[,] board;
    public  int SIZE = 8;

    private int numWhiteNormalPieces;
    private int numBlackNormalPieces;
    private int numBlackKingPieces;
    private int numWhiteKingPieces;

    public enum Type
    {
        EMPTY, WHITE, BLACK, WHITE_KING, BLACK_KING
    }

    public enum Decision
    {
        COMPLETED,
        FAILED_MOVING_INVALID_PIECE,
        FAILED_INVALID_DESTINATION,
        ADDITIONAL_MOVE,
        GAME_ENDED
    }

    public Board()
    {
        setUpBoard();
    }

    public Board(Type[,] board)
    {
        numWhiteNormalPieces = 0;
        numBlackNormalPieces = 0;
        numBlackKingPieces = 0;
        numWhiteKingPieces = 0;

        this.board = board;
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                Type piece = getPiece(i, j);
                if (piece == Type.BLACK)
                    numBlackNormalPieces++;
                else if (piece == Type.BLACK_KING)
                    numBlackKingPieces++;
                else if (piece == Type.WHITE)
                    numWhiteNormalPieces++;
                else if (piece == Type.WHITE_KING)
                    numWhiteKingPieces++;
            }
        }
    }

    private void setUpBoard()
    {
        numWhiteNormalPieces = 12;
        numBlackNormalPieces = 12;
        numBlackKingPieces = 0;
        numWhiteKingPieces = 0;
        board = new Type[SIZE,SIZE];
        for (int i = 0; i < board.Length; i++)
        {
            int start = 0;
            if (i % 2 == 0)
                start = 1;

            Type pieceType = Type.EMPTY;
            if (i <= 2)
                pieceType = Type.WHITE;
            else if (i >= 5)
                pieceType = Type.BLACK;

            for (int j = start; j < board.Length; j += 2)
            {
                board[i,j] = pieceType;
            }
        }

        populateEmptyOnBoard();
    }

    private void setUpTestBoard()
    {
        numBlackKingPieces = 1;
        numWhiteKingPieces = 1;
        board = new Type[SIZE,SIZE];
        board[6,1] = Type.WHITE_KING;
        board[4,3] = Type.BLACK_KING;
        populateEmptyOnBoard();

    }

    private void populateEmptyOnBoard()
    {
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < SIZE; j++) //board[i].length
            {
                if (board[i,j] == null)
                    board[i,j] = Type.EMPTY;
            }
        }
    }

    public Type getPiece(int row, int col)
    {
        return board[row,col];
    }

    public Type getPiece(Vector2 point)
    {
        return board[(int)point.x,(int)point.y];
    }

    public Type[,] getBoard()
    {
        return board;
    }

    public int getNumWhitePieces()
    {
        return numWhiteKingPieces + numWhiteNormalPieces;
    }

    public int getNumBlackPieces()
    {
        return numBlackKingPieces + numBlackNormalPieces;
    }

    public int getNumWhiteKingPieces()
    {
        return numWhiteKingPieces;
    }
    public int getNumBlackKingPieces()
    {
        return numBlackKingPieces;
    }
    public int getNumWhiteNormalPieces()
    {
        return numWhiteNormalPieces;
    }
    public int getNumBlackNormalPieces()
    {
        return numBlackNormalPieces;
    }

    // returns true if move successful
    public Decision makeMove(Move move, Player.Side side)
    {
        if (move == null)
        {
            return Decision.GAME_ENDED;
        }
        Vector2 start = move.getStart();
        int startRow = (int)start.x;
        int startCol = (int)start.y;
        Vector2 end = move.getEnd();
        int endRow = (int)end.x;
        int endCol = (int)end.y;

        //can only move own piece and not empty space
        if (!isMovingOwnPiece(startRow, startCol, side) || getPiece(startRow, startCol) == Type.EMPTY)
            return Decision.FAILED_MOVING_INVALID_PIECE;

        List<Move> possibleMoves = getValidMoves(startRow, startCol, side);
        //System.out.println(possibleMoves);

        Type currType = getPiece(startRow, startCol);

        if (possibleMoves.Contains(move))
        {
            bool jumpMove = false;
            //if it contains move then it is either 1 move or 1 jump
            if (startRow + 1 == endRow || startRow - 1 == endRow)
            {
                board[startRow,startCol] = Type.EMPTY;
                board[endRow,endCol] = currType;
            }
            else
            {
                jumpMove = true;
                board[startRow,startCol] = Type.EMPTY;
                board[endRow,endCol] = currType;
                Vector2 mid = findMidSquare(move);

                Type middle = getPiece(mid);
                if (middle == Type.BLACK)
                    numBlackNormalPieces--;
                else if (middle == Type.BLACK_KING)
                    numBlackKingPieces--;
                else if (middle == Type.WHITE)
                    numWhiteNormalPieces--;
                else if (middle == Type.WHITE_KING)
                    numWhiteKingPieces--;
                board[(int)mid.x,(int)mid.y] = Type.EMPTY;
            }

            if (endRow == 0 && side == Player.Side.BLACK)
            {
                board[endRow,endCol] = Type.BLACK_KING;
                numBlackNormalPieces--;
                numBlackKingPieces++;
            }

            else if (endRow == SIZE - 1 && side == Player.Side.WHITE)
            {
                board[endRow,endCol] = Type.WHITE_KING;
                numWhiteNormalPieces--;
                numWhiteKingPieces++;
            }
            if (jumpMove)
            {
                List<Move> additional = getValidSkipMoves(endRow, endCol, side);
                if (additional == null) // additional.isEmpty
                    return Decision.COMPLETED;
                return Decision.ADDITIONAL_MOVE;
            }
            return Decision.COMPLETED;
        }
        else
            return Decision.FAILED_INVALID_DESTINATION;
    }

    public List<Move> getAllValidMoves(Player.Side side)
    {

        Type normal = side == Player.Side.BLACK ? Type.BLACK : Type.WHITE;
        Type king = side == Player.Side.BLACK ? Type.BLACK_KING : Type.WHITE_KING;

        List<Move> possibleMoves = new List<Move>();
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                Type t = getPiece(i, j);
                if (t == normal || t == king)
                    possibleMoves.AddRange(getValidMoves(i, j, side)); //---------------------------------*AddAll
            }
        }


        return possibleMoves;
    }

    // requires there to actually be a mid square
    private Vector2 findMidSquare(Move move)
    {

        Vector2 ret = new Vector2((move.getStart().x + move.getEnd().x) / 2,
                (move.getStart().y + move.getEnd().y) / 2);

        return ret;
    }

    private bool isMovingOwnPiece(int row, int col, Player.Side side)
    {
        Type pieceType = getPiece(row, col);
        if (side == Player.Side.BLACK && pieceType != Type.BLACK && pieceType != Type.BLACK_KING)
            return false;
        else if (side == Player.Side.WHITE && pieceType != Type.WHITE && pieceType != Type.WHITE_KING)
            return false;
        return true;
    }

    public List<Move> getValidMoves(int row, int col, Player.Side side)
    {
        Type type = board[row,col];
        Vector2 startPoint = new Vector2(row, col);
        if (type == Type.EMPTY)
            throw new System.ArgumentException(); //----------------------------*IllegalArgumentException();

        List<Move> moves = new List<Move>();

        //4 possible moves, 2 if not king
        if (type == Type.WHITE || type == Type.BLACK)
        {
            //2 possible moves
            int rowChange = type == Type.WHITE ? 1 : -1;

            int newRow = row + rowChange;
            if (newRow >= 0 || newRow < SIZE)
            {
                int newCol = col + 1;
                if (newCol < SIZE && getPiece(newRow, newCol) == Type.EMPTY)
                    moves.Add(new Move(startPoint, new Vector2(newRow, newCol)));
                newCol = col - 1;
                if (newCol >= 0 && getPiece(newRow, newCol) == Type.EMPTY)
                    moves.Add(new Move(startPoint, new Vector2(newRow, newCol)));
            }

        }
        //must be king
        else
        {
            //4 possible moves

            int newRow = row + 1;
            if (newRow < SIZE)
            {
                int newCol = col + 1;
                if (newCol < SIZE && getPiece(newRow, newCol) == Type.EMPTY)
                    moves.Add(new Move(startPoint, new Vector2(newRow, newCol)));
                newCol = col - 1;
                if (newCol >= 0 && getPiece(newRow, newCol) == Type.EMPTY)
                    moves.Add(new Move(startPoint, new Vector2(newRow, newCol)));
            }
            newRow = row - 1;
            if (newRow >= 0)
            {
                int newCol = col + 1;
                if (newCol < SIZE && getPiece(newRow, newCol) == Type.EMPTY)
                    moves.Add(new Move(startPoint, new Vector2(newRow, newCol)));
                newCol = col - 1;
                if (newCol >= 0 && getPiece(newRow, newCol) == Type.EMPTY)
                    moves.Add(new Move(startPoint, new Vector2(newRow, newCol)));
            }


        }

        moves.AddRange(getValidSkipMoves(row, col, side));
        return moves;
    }

    public List<Move> getValidSkipMoves(int row, int col, Player.Side side)
    {
        List<Move> move = new List<Move>();
        Vector2 start = new Vector2(row, col);

        List<Vector2> possibilities = new List<Vector2>();

        if (side == Player.Side.WHITE && getPiece(row, col) == Type.WHITE)
        {
            possibilities.Add(new Vector2(row + 2, col + 2));
            possibilities.Add(new Vector2(row + 2, col - 2));
        }
        else if (side == Player.Side.BLACK && getPiece(row, col) == Type.BLACK)
        {
            possibilities.Add(new Vector2(row - 2, col + 2));
            possibilities.Add(new Vector2(row - 2, col - 2));
        }
        else if (getPiece(row, col) == Type.BLACK_KING || getPiece(row, col) == Type.WHITE_KING)
        {
            possibilities.Add(new Vector2(row + 2, col + 2));
            possibilities.Add(new Vector2(row + 2, col - 2));
            possibilities.Add(new Vector2(row - 2, col + 2));
            possibilities.Add(new Vector2(row - 2, col - 2));
        }

        for (int i = 0; i < possibilities.Count; i++)
        {
            Vector2 temp = possibilities[i];//------------------------*.get(i);
            Move m = new Move(start, temp);
            if (temp.x < SIZE && temp.x >= 0 && temp.y < SIZE && temp.y >= 0 && getPiece((int)temp.x, (int)temp.y) == Type.EMPTY
                    && isOpponentPiece(side, getPiece(findMidSquare(m))))
            {
                move.Add(m);
            }
        }

        //System.out.println("Skip moves: " + move);
        return move;
    }

    // return true if the piece is opponents
    private bool isOpponentPiece(Player.Side current, Type opponentPiece)
    {
        if (current == Player.Side.BLACK && (opponentPiece == Type.WHITE || opponentPiece == Type.WHITE_KING))
            return true;
        if (current == Player.Side.WHITE && (opponentPiece == Type.BLACK || opponentPiece == Type.BLACK_KING))
            return true;
        return false;
    }

    public string toString()
    {
        StringBuilder b = new StringBuilder();
        b.Append("  ");
        for (int i = 0; i < board.Length; i++)
        {
            b.Append(i + " ");
        }
        b.Append("\n");
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = -1; j < SIZE ; j++) //board[i].Length
            {
                string a = "";
                if (j == -1)
                    a = i + "";
                else if (board[i,j] == Type.WHITE)
                    a = "w";
                else if (board[i,j] == Type.BLACK)
                    a = "b";
                else if (board[i,j] == Type.WHITE_KING)
                    a = "W";
                else if (board[i,j] == Type.BLACK_KING)
                    a = "B";
                else
                    a = "_";

                b.Append(a);
                b.Append(" ");
            }
            b.Append("\n");
        }
        return b.ToString();
    }

    public Board clone()
    {
        Type[,] newBoard = new Type[SIZE,SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                newBoard[i,j] = board[i,j];
            }
        }
        Board b = new Board(newBoard);
        return b;
    }
}
