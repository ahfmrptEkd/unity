using UnityAlgorithms.Games.ConnectFour;
using UnityAlgorithms.Algorithms.Core;
using UnityAlgorithms.Algorithms.ConnectFour;

Console.WriteLine("=== Connect Four: Player vs AI ===\n");

// AI difficulty selection
Console.WriteLine("Select AI difficulty:");
Console.WriteLine("1. Easy (Random moves)");
Console.WriteLine("2. Normal (Alpha-Beta Pruning)");
Console.Write("Choose difficulty (1-2): ");

string algorithmName = "easy"; // default
if (int.TryParse(Console.ReadLine(), out int difficulty))
{
    algorithmName = difficulty switch
    {
        1 => "easy",
        2 => "normal",
        _ => "easy"
    };
}

var ai = AlgorithmFactory.CreateAlgorithm(algorithmName);
Console.WriteLine($"Selected: {ai.GetName()}\n");

var state = new ConnectFourState();

Console.WriteLine("You are X (first player)");
Console.WriteLine("AI is O (second player)");
Console.WriteLine("Enter column number (1-7) to place your piece\n");

while (!state.IsDone())
{
    Console.WriteLine(state.ToString());
    
    if (state.IsFirst())
    {
        // Player turn
        Console.Write("Your turn (1-7): ");
        if (int.TryParse(Console.ReadLine(), out int column) && column >= 1 && column <= 7)
        {
            int action = column - 1; // Convert to 0-based
            var legalActions = state.LegalActions();
            
            if (legalActions.Contains(action))
            {
                state.Progress(action);
                Console.WriteLine($"You placed piece in column {column}\n");
            }
            else
            {
                Console.WriteLine("Invalid move! Column is full. Try again.\n");
            }
        }
        else
        {
            Console.WriteLine("Invalid input! Enter a number between 1-7.\n");
        }
    }
    else
    {
        // AI turn
        Console.WriteLine("AI is thinking...");
        int aiAction = ai.SelectAction(state);
        state.Progress(aiAction);
        Console.WriteLine($"AI placed piece in column {aiAction + 1}\n");
    }
}

// Game over
Console.WriteLine(state.ToString());
switch (state.GetWinningStatus())
{
    case WinningStatus.Win:
        // Win is the previous player's victory (after SwapBoards, the current IsFirst is opposite)
        Console.WriteLine(!state.IsFirst() ? "🎉 You won!" : "🤖 AI won!");
        break;
    case WinningStatus.Lose:
        Console.WriteLine(!state.IsFirst() ? "🤖 AI won!" : "🎉 You won!");
        break;
    case WinningStatus.Draw:
        Console.WriteLine("🤝 It's a draw!");
        break;
}
