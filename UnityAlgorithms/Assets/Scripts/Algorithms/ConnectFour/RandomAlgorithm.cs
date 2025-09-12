using System;
using UnityAlgorithms.Algorithms.Core;
using UnityAlgorithms.Core;
using UnityAlgorithms.Games.ConnectFour;

namespace UnityAlgorithms.Algorithms.ConnectFour
{
    public class RandomAlgorithm : IAlgorithm
    {
        private AlgorithmParams parameters;
        private readonly Random random;
        
        public RandomAlgorithm()
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
                
            return legalActions[random.Next(legalActions.Count)];
        }
        
        public string GetName() => "Random (Connect Four)";
        
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