using System;
using System.Collections.Generic;
using System.Linq;
using UnityAlgorithms.Algorithms.Core;
using UnityAlgorithms.Core;
using UnityAlgorithms.Games.ConnectFour;

namespace UnityAlgorithms.Algorithms.ConnectFour
{
    public class HardAlgorithm : IAlgorithm
    {
        private AlgorithmParams parameters;
        private Random random;
        
        // MCTS constants
        private const double C = 1.0;
        private const int EXPAND_THRESHOLD = 10;
        private const int DEFAULT_ITERATIONS = 1000;
        
        public HardAlgorithm()
        {
            parameters = new AlgorithmParams();
            random = new Random();
        }
        
        public int SelectAction(GameState state)
        {
            if (state is not ConnectFourState connectFourState)
                throw new ArgumentException("State must be ConnectFourState");
                
            var legalActions = connectFourState.LegalActions();
            if (legalActions.Count == 0)
                throw new InvalidOperationException("No legal actions available");
                
            if (legalActions.Count == 1)
                return legalActions[0];
                
            var rootNode = new Node(connectFourState);
            rootNode.Expand();
            
            int iterations = parameters.SearchDepth > 0 ? parameters.SearchDepth * 100 : DEFAULT_ITERATIONS;
            
            for (int i = 0; i < iterations; i++)
            {
                rootNode.Evaluate(random);
            }
            
            // Select action with most visits
            int bestActionIndex = -1;
            int bestVisitCount = -1;
            
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                if (rootNode.ChildNodes[i].VisitCount > bestVisitCount)
                {
                    bestActionIndex = i;
                    bestVisitCount = rootNode.ChildNodes[i].VisitCount;
                }
            }
            
            return legalActions[bestActionIndex];
        }
        
        private class Node
        {
            public ConnectFourState State { get; private set; }
            public List<Node> ChildNodes { get; private set; }
            public double WinCount { get; set; }
            public int VisitCount { get; set; }
            
            public Node(ConnectFourState state)
            {
                State = (ConnectFourState)state.Clone();
                ChildNodes = new List<Node>();
                WinCount = 0.0;
                VisitCount = 0;
            }
            
            public double Evaluate(Random random)
            {
                if (State.IsDone())
                {
                    double value = 0.5; // Draw
                    var status = State.GetWinningStatus();
                    if (status == WinningStatus.Win)
                        value = 0.0; // Previous player won, current player lost
                    else if (status == WinningStatus.Lose)
                        value = 1.0; // Previous player lost, current player won
                        
                    WinCount += value;
                    VisitCount++;
                    return value;
                }
                
                if (ChildNodes.Count == 0)
                {
                    // Leaf node: run playout
                    var stateCopy = (ConnectFourState)State.Clone();
                    double value = Playout(stateCopy, random);
                    WinCount += value;
                    VisitCount++;
                    
                    if (VisitCount == EXPAND_THRESHOLD)
                        Expand();
                        
                    return value;
                }
                else
                {
                    // Internal node: select child and evaluate
                    var selectedChild = SelectChildNode();
                    double value = 1.0 - selectedChild.Evaluate(random);
                    WinCount += value;
                    VisitCount++;
                    return value;
                }
            }
            
            public void Expand()
            {
                var legalActions = State.LegalActions();
                ChildNodes.Clear();
                
                foreach (int action in legalActions)
                {
                    var childState = (ConnectFourState)State.Clone();
                    childState.Progress(action);
                    ChildNodes.Add(new Node(childState));
                }
            }
            
            private Node SelectChildNode()
            {
                // First, select unvisited child
                foreach (var child in ChildNodes)
                {
                    if (child.VisitCount == 0)
                        return child;
                }
                
                // All children visited, use UCB1
                double totalVisits = ChildNodes.Sum(child => child.VisitCount);
                double bestValue = double.NegativeInfinity;
                Node bestChild = ChildNodes[0];
                
                foreach (var child in ChildNodes)
                {
                    double exploitValue = 1.0 - (child.WinCount / child.VisitCount);
                    double exploreValue = C * Math.Sqrt(2.0 * Math.Log(totalVisits) / child.VisitCount);
                    double ucb1Value = exploitValue + exploreValue;
                    
                    if (ucb1Value > bestValue)
                    {
                        bestValue = ucb1Value;
                        bestChild = child;
                    }
                }
                
                return bestChild;
            }
        }
        
        private static double Playout(ConnectFourState state, Random random)
        {
            var status = state.GetWinningStatus();
            if (status == WinningStatus.Win)
                return 0.0; // Previous player won, current player lost
            if (status == WinningStatus.Lose)
                return 1.0; // Previous player lost, current player won  
            if (status == WinningStatus.Draw)
                return 0.5;
                
            var legalActions = state.LegalActions();
            if (legalActions.Count == 0)
                return 0.5;
                
            int randomAction = legalActions[random.Next(legalActions.Count)];
            state.Progress(randomAction);
            return 1.0 - Playout(state, random);
        }
        
        public string GetName() => "Hard (MCTS)";
        
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