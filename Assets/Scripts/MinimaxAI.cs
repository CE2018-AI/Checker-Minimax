using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MinimaxAI : Player {
    
    private Vector2 skippingPoint;
    private int depth;
    private long totalTimeElapsed;
    private double numMovesCalled;
    private int pruned = 0;
    private bool checkVec = false;
    public MinimaxAI(string name, Side s)
    : base(name, s)   //----------------------------------****** 
    {

    }
    public MinimaxAI(Side s, int depth)
    :base("MinimaxAI", s) //---------------------------------******
    {

        this.depth = depth;
        this.totalTimeElapsed = 0;
    }
    private static long nanoTime()
    {
        long nano = 10000L * Stopwatch.GetTimestamp();
        nano /= TimeSpan.TicksPerMillisecond;
        nano *= 100L;
        return nano;
    }
    public Board.Decision makeMove(Board board)
    {
        
        numMovesCalled++;
        long startTime = nanoTime();
        Move m = minimaxStart(board, depth, getSide(), true);
        totalTimeElapsed += nanoTime() - startTime;
        //System.out.println("m is: " + m);
        //Move move = board.getAllValidMoves(getSide()).get(m);
        Main.println("::::::::" + m.getEnd() + "  " + m.getStart());
        Board.Decision decision = board.makeMove(m, getSide());
        if (decision == Board.Decision.ADDITIONAL_MOVE)
        {
            skippingPoint = m.getEnd();
            checkVec = true;
        }

        //System.out.println("Pruned tree: " + pruned + " times");
        return decision;
    }
    public string getAverageTimePerMove()
    {
        return totalTimeElapsed / numMovesCalled * System.Math.Pow(10, -6) + " milliseconds";
    }
    private Move minimaxStart(Board board, int depth, Side side, bool maximizingPlayer)
    {
        double alpha = double.NegativeInfinity;
        double beta = double.PositiveInfinity;

        List<Move> possibleMoves;
        if (!checkVec) //--------------------------***
            possibleMoves = board.getAllValidMoves(side);
        else
        {
            possibleMoves = board.getValidSkipMoves((int)skippingPoint.x, (int)skippingPoint.y, side);
            checkVec = false;//-------------------------null;
        }
        //System.out.println("side: " + side + " " + possibleMoves.size());

        List<double> heuristics = new List<double>();
        if (possibleMoves == null)//----------------.isEmpty()
            return null;
        Board tempBoard = null;
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            tempBoard = board.clone();
            tempBoard.makeMove(possibleMoves[i], side);
            heuristics.Add(minimax(tempBoard, depth - 1, flipSide(side), !maximizingPlayer, alpha, beta));
        }
        //System.out.println("\nMinimax at depth: " + depth + "\n" + heuristics);

        double maxHeuristics = double.NegativeInfinity;

        System.Random rand = new System.Random();
        for (int i = heuristics.Count - 1; i >= 0; i--)
        {
            if (heuristics[i] >= maxHeuristics)
            {
                maxHeuristics = heuristics[i];
            }
        }
        //Main.println("Unfiltered heuristics: " + heuristics);
        for (int i = 0; i < heuristics.Count; i++)
        {
            if (heuristics[i] < maxHeuristics)
            {
                heuristics.RemoveAt(i);
                possibleMoves.RemoveAt(i);
                i--;
            }
        }
        //Main.println("Filtered/max heuristics: " + heuristics);
        // return possibleMoves.get(rand.nextInt(possibleMoves.Count));
        return possibleMoves[0];//rand.Next(possibleMoves.Count)
    }

    private double minimax(Board board, int depth, Side side, bool maximizingPlayer, double alpha, double beta)
    {
        if (depth == 0)
        {
            return getHeuristic(board);
        }
        List<Move> possibleMoves = board.getAllValidMoves(side);

        double initial = 0;
        Board tempBoard = null;
        if (maximizingPlayer)
        {
            initial = double.NegativeInfinity;
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                tempBoard = board.clone();
                tempBoard.makeMove(possibleMoves[i], side);

                double result = minimax(tempBoard, depth - 1, flipSide(side), !maximizingPlayer, alpha, beta);

                initial = Math.Max(result, initial);
                alpha = Math.Max(alpha, initial);

                if (alpha >= beta)
                    break;
            }
        }
        //minimizing
        else
        {
            initial = double.PositiveInfinity;
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                tempBoard = board.clone();
                tempBoard.makeMove(possibleMoves[i], side);

                double result = minimax(tempBoard, depth - 1, flipSide(side), !maximizingPlayer, alpha, beta);

                initial = Math.Min(result, initial);
                alpha = Math.Min(alpha, initial);

                if (alpha >= beta)
                    break;
            }
        }

        return initial;
    }

    private double getHeuristic(Board b)
    {
        //naive implementation
        //        if(getSide() == Side.WHITE)
        //            return b.getNumWhitePieces() - b.getNumBlackPieces();
        //        return b.getNumBlackPieces() - b.getNumWhitePieces();

        double kingWeight = 1.2;
        double result = 0;
        if (getSide() == Side.WHITE)
            result = b.getNumWhiteKingPieces() * kingWeight + b.getNumWhiteNormalPieces() - b.getNumBlackKingPieces() *
                    kingWeight -
                    b.getNumBlackNormalPieces();
        else
            result = b.getNumBlackKingPieces() * kingWeight + b.getNumBlackNormalPieces() - b.getNumWhiteKingPieces() *
                    kingWeight -
                    b.getNumWhiteNormalPieces();
        return result;

    }

    private Side flipSide(Side side)
    {
        if (side == Side.BLACK)
            return Side.WHITE;
        return Side.BLACK;
    }
    
}
