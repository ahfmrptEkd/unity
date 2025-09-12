using System;
using System.Collections.Generic;
using System.Text;
using UnityAlgorithms.Core;

namespace UnityAlgorithms.Games.ConnectFour
{
    public enum WinningStatus
    {
        None,
        Win,
        Lose,
        Draw,
    }

    public class ConnectFourState : GameState
    {

        private bool isFirst = true;
        private int[,] myBoard;
        private int[,] enemyBoard;
        private WinningStatus winningStatus = WinningStatus.None;

        private void InitializeBoard()
        {
            myBoard = new int[GameConstants.ConnectFour.H, GameConstants.ConnectFour.W];
            enemyBoard = new int[GameConstants.ConnectFour.H, GameConstants.ConnectFour.W];
        }

        public long EvaluatedScore { get; private set; } = 0;


        public ConnectFourState()
        {
            InitializeBoard();
        }

        public ConnectFourState(int seed)
        {
            isFirst = true;
            winningStatus = WinningStatus.None;
            InitializeBoard();
        }

        public override bool IsDone()
        {
            return winningStatus != WinningStatus.None;
        }

        public override void Progress(int action)
        {
            // 1. take a turn
            (int y, int x) coordinate = PlacePiece(action);

            // 2. check if the game is done
            CheckWinCondition(coordinate);
            
            // 3. end the turn
            SwapBoards();
            
            // 4. check if game is draw
            if (winningStatus == WinningStatus.None && LegalActions().Count == 0)
            {
                winningStatus = WinningStatus.Draw;
            }
        }

        public override List<int> LegalActions()
        {
            var actions = new List<int>();
            
            for (int x = 0; x < GameConstants.ConnectFour.W; x++)
            {
                // Check from bottom to top for first empty space (Connect4 rule)
                for (int y = 0; y < GameConstants.ConnectFour.H; y++)
                {
                    if (myBoard[y, x] == 0 && enemyBoard[y, x] == 0)
                    {
                        actions.Add(x);
                        break; // Column is available, move to next column
                    }
                }
            }
            return actions;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Current turn: {(isFirst ? "X" : "O")}");
            sb.AppendLine();
            sb.AppendLine("    1   2   3   4   5   6   7");
            sb.AppendLine("  +---+---+---+---+---+---+---+");

            for (int y = GameConstants.ConnectFour.H - 1; y >= 0; y--)
            {
                sb.Append("  |");
                for (int x = 0; x < GameConstants.ConnectFour.W; x++)
                {
                    char c = ' ';
                    if (myBoard[y, x] == 1)
                    {
                        c = (isFirst ? 'X' : 'O');
                    }
                    else if (enemyBoard[y, x] == 1)
                    {
                        c = (isFirst ? 'O' : 'X');
                    }

                    sb.Append($" {c} |");
                }
                sb.AppendLine();
                sb.AppendLine("  +---+---+---+---+---+---+---+");
            }

            return sb.ToString();
        }

        public override long EvaluateScore()
        {
            if (IsDone())
            {
                return winningStatus switch
                {
                    WinningStatus.Win => GameConstants.INF,
                    WinningStatus.Lose => -GameConstants.INF,
                    WinningStatus.Draw => 0,
                    _ => 0,
                };
            }

            // Adding reward weight to the center column
            int score = 0;
            for (int y = 0; y < GameConstants.ConnectFour.H; y++)
            {
                for (int x = 0; x < GameConstants.ConnectFour.W; x++)
                {
                    int weight = GameConstants.ConnectFour.W - Math.Abs(x - GameConstants.ConnectFour.W / 2);
                    if (myBoard[y, x] == 1)
                    {
                        score += weight;
                    }
                    else if (enemyBoard[y, x] == 1)
                    {
                        score -= weight;
                    }
                }
            }

            EvaluatedScore = score;
            return EvaluatedScore;
        }

    public override GameState Clone()
    {
        var clone = new ConnectFourState();
        clone.isFirst = this.isFirst;
        clone.winningStatus = this.winningStatus;
        clone.EvaluatedScore = this.EvaluatedScore;
        clone.myBoard = (int[,])this.myBoard.Clone();
        clone.enemyBoard = (int[,])this.enemyBoard.Clone();
        return clone;
    }

    public override int CompareTo(GameState? other)
    {
        if (other == null) return 1;
        if (other is ConnectFourState connectFourOther)
        {
            return EvaluatedScore.CompareTo(connectFourOther.EvaluatedScore);
        }
        return 0;
    }

    public WinningStatus GetWinningStatus() => winningStatus;
    public bool IsFirst() => isFirst;

    private (int y, int x) PlacePiece(int x)
    {
        for (int y = 0; y < GameConstants.ConnectFour.H; y++)
        {
            if (myBoard[y, x] == 0 && enemyBoard[y, x] == 0)
            {
                myBoard[y, x] = 1;
                return (y, x);
            }
        }
        throw new InvalidOperationException($"Cannot place piece in column {x}");
    }

    private void CheckWinCondition((int y, int x) coordinate)
    {
        // 가로, 세로, 대각선 체크 구현
        // (구현 복잡하므로 간략화 버전 제시)
        if (CheckDirection(coordinate, (0, 1)) || // 가로
            CheckDirection(coordinate, (1, 0)) || // 세로
            CheckDirection(coordinate, (1, 1)) || // 대각선 \
            CheckDirection(coordinate, (1, -1)))  // 대각선 /
        {
            winningStatus = WinningStatus.Lose; // 내가 이겼으므로 상대방 패배
        }
    }

    private bool CheckDirection((int y, int x) start, (int dy, int dx) direction)
    {
        int count = 1; // 시작점 포함

        // 한 방향으로 카운트
        var (y, x) = (start.y + direction.dy, start.x + direction.dx);
        while (IsValidPosition(y, x) && myBoard[y, x] == 1)
        {
            count++;
            y += direction.dy;
            x += direction.dx;
        }

        // 반대 방향으로 카운트
        (y, x) = (start.y - direction.dy, start.x - direction.dx);
        while (IsValidPosition(y, x) && myBoard[y, x] == 1)
        {
            count++;
            y -= direction.dy;
            x -= direction.dx;
        }

        return count >= GameConstants.ConnectFour.CONNECT_NUMBER;
    }

    private bool IsValidPosition(int y, int x)
    {
        return y >= 0 && y < GameConstants.ConnectFour.H &&
                x >= 0 && x < GameConstants.ConnectFour.W;
    }

    private void SwapBoards()
    {
        (myBoard, enemyBoard) = (enemyBoard, myBoard);
        isFirst = !isFirst;
    }
    }
}