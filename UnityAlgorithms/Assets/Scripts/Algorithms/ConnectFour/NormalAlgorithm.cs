using System;
using System.Collections.Generic;
using UnityAlgorithms.Algorithms.Core;
using UnityAlgorithms.Core;
using UnityAlgorithms.Games.ConnectFour;

namespace UnityAlgorithms.Algorithms.ConnectFour
{
    public class NormalAlgorithm : IAlgorithm
    {
        private AlgorithmParams parameters;
        
        public NormalAlgorithm()
        {
            parameters = new AlgorithmParams();
        }
        
        public int SelectAction(GameState state)
        {
            if (state is not ConnectFourState connectFourState)
                throw new ArgumentException("State must be ConnectFourState");
                
            var legalActions = connectFourState.LegalActions();
            if (legalActions.Count == 0)
                throw new InvalidOperationException("No legal actions available");

            int depth = parameters.SearchDepth > 0 ? parameters.SearchDepth : 6;
            var result = AlphaBeta(connectFourState, depth, double.NegativeInfinity, double.PositiveInfinity, true);
            
            return result.action != -1 ? result.action : legalActions[0];
        }
        
        private (int action, double value) AlphaBeta(ConnectFourState state, int depth, double alpha, double beta, bool maximizingPlayer)
        {
            if (state.IsDone() || depth == 0)
            {
                var status = state.GetWinningStatus();
                if (status == WinningStatus.Win)
                {
                    return (-1, maximizingPlayer ? 1000.0 : -1000.0);
                }
                else if (status == WinningStatus.Lose)
                {
                    return (-1, maximizingPlayer ? -1000.0 : 1000.0);
                }
                else if (status == WinningStatus.Draw)
                {
                    return (-1, 0.0);
                }
                
                double evaluation = EvaluatePosition(state);
                return (-1, maximizingPlayer ? evaluation : -evaluation);
            }
            
            var legalActions = state.LegalActions();
            if (legalActions.Count == 0)
            {
                return (-1, 0.0);
            }
            
            int bestAction = legalActions[0];
            double bestValue = maximizingPlayer ? double.NegativeInfinity : double.PositiveInfinity;
            
            foreach (int action in legalActions)
            {
                var nextState = (ConnectFourState)state.Clone();
                nextState.Progress(action);
                
                double value = AlphaBeta(nextState, depth - 1, alpha, beta, !maximizingPlayer).value;
                
                if (maximizingPlayer)
                {
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestAction = action;
                    }
                    alpha = Math.Max(alpha, bestValue);
                }
                else
                {
                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestAction = action;
                    }
                    beta = Math.Min(beta, bestValue);
                }
                
                if (beta <= alpha)
                {
                    break;
                }
            }
            
            return (bestAction, bestValue);
        }
        
        private double EvaluatePosition(ConnectFourState state)
        {
            const int W = GameConstants.ConnectFour.W;
            const int H = GameConstants.ConnectFour.H;
            double score = 0.0;
            
            for (int row = 0; row < H; row++)
            {
                for (int col = 0; col < W; col++)
                {
                    bool isMyPiece = state.GetMyBoard()[row, col] == 1;
                    bool isEnemyPiece = state.GetEnemyBoard()[row, col] == 1;
                    
                    if (isMyPiece)
                    {
                        score += EvaluatePositionAt(state, row, col, true, W, H);
                    }
                    else if (isEnemyPiece)
                    {
                        score -= EvaluatePositionAt(state, row, col, false, W, H);
                    }
                }
            }
            
            return score;
        }
        
        private double EvaluatePositionAt(ConnectFourState state, int row, int col, bool isMyPiece, int W, int H)
        {
            double score = 0.0;
            
            int[] dx = { 0, 1, 1, 1 };
            int[] dy = { 1, 0, 1, -1 };
            
            var myBoard = state.GetMyBoard();
            var enemyBoard = state.GetEnemyBoard();
            
            for (int dir = 0; dir < 4; dir++)
            {
                int count = 1;
                int openEnds = 0;
                
                for (int direction = -1; direction <= 1; direction += 2)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        int newRow = row + dx[dir] * i * direction;
                        int newCol = col + dy[dir] * i * direction;
                        
                        if (newRow < 0 || newRow >= H || newCol < 0 || newCol >= W)
                        {
                            break;
                        }
                        
                        bool newIsMyPiece = myBoard[newRow, newCol] == 1;
                        bool newIsEnemyPiece = enemyBoard[newRow, newCol] == 1;
                        bool isEmpty = !newIsMyPiece && !newIsEnemyPiece;
                        
                        if ((isMyPiece && newIsMyPiece) || (!isMyPiece && newIsEnemyPiece))
                        {
                            count++;
                        }
                        else if (isEmpty)
                        {
                            openEnds++;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                
                if (count >= 4)
                {
                    score += 1000;
                }
                else if (count == 3 && openEnds >= 1)
                {
                    score += 50;
                }
                else if (count == 2 && openEnds >= 1)
                {
                    score += 10;
                }
                else if (count == 1 && openEnds >= 1)
                {
                    score += 1;
                }
            }
            
            if (col >= W / 2 - 1 && col <= W / 2 + 1)
            {
                score += 3;
            }
            
            return score;
        }
        
        public string GetName() => "Normal (Alpha-Beta Pruning)";
        
        public void SetParams(AlgorithmParams parameters)
        {
            this.parameters = parameters;
        }
        
        public GameState RunAndEvaluate(GameState state, int action)
        {
            if (state is not ConnectFourState connectFourState)
                throw new ArgumentException("State must be ConnectFourState");
                
            var nextState = (ConnectFourState)connectFourState.Clone();
            nextState.Progress(action);
            return nextState;
        }
    }
}