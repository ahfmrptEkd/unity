using System;
using System.Linq;
using System.Collections.Generic;
using UnityAlgorithms.Algorithms.Core;
using UnityAlgorithms.Core;
using UnityAlgorithms.Games.ConnectFour;

namespace UnityAlgorithms.Algorithms.ConnectFour
{
    public class EasyAlgorithm : IAlgorithm
    {
        private AlgorithmParams parameters;
        private readonly Random random;
        
        public EasyAlgorithm()
        {
            random = new Random();
            parameters = new AlgorithmParams();
        }
        
        public int SelectAction(GameState state)
        {
            if (state is not ConnectFourState connectFourState)
                throw new ArgumentException("State must be ConnectFourState");
                
            var legalActions = connectFourState.LegalActions();
            if (legalActions.Count == 0)
                throw new InvalidOperationException("No legal actions available");

            // 1. Check for immediate win
            foreach (int action in legalActions)
            {
                var testState = (ConnectFourState)connectFourState.Clone();
                testState.Progress(action);
                if (testState.GetWinningStatus() == WinningStatus.Lose) // We win after our move
                {
                    return action;
                }
            }

            // 2. Check for immediate block (prevent opponent's win)
            foreach (int action in legalActions)
            {
                var testState = (ConnectFourState)connectFourState.Clone();
                testState.Progress(action); // Our move
                
                // Check if opponent can win immediately after our move
                var opponentActions = testState.LegalActions();
                bool needToBlock = false;
                
                foreach (int opponentAction in opponentActions)
                {
                    var opponentState = (ConnectFourState)testState.Clone();
                    opponentState.Progress(opponentAction);
                    if (opponentState.GetWinningStatus() == WinningStatus.Lose) // Opponent wins
                    {
                        needToBlock = true;
                        break;
                    }
                }
                
                if (!needToBlock)
                {
                    return action; // This move prevents opponent's immediate win
                }
            }

            // 3. Prefer center columns (weighted random)
            var weights = new List<(int action, int weight)>();
            foreach (int action in legalActions)
            {
                int weight = GameConstants.ConnectFour.W - Math.Abs(action - GameConstants.ConnectFour.W / 2);
                weights.Add((action, weight));
            }
            
            int totalWeight = weights.Sum(w => w.weight);
            int randomValue = random.Next(totalWeight);
            int currentWeight = 0;
            
            foreach (var (action, weight) in weights)
            {
                currentWeight += weight;
                if (randomValue < currentWeight)
                {
                    return action;
                }
            }
            
            return legalActions[random.Next(legalActions.Count)];
        }
        
        public string GetName() => "Easy (Connect Four)";
        
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