using System;
using System.Collections.Generic;

namespace UnityAlgorithms.Core
{
    public abstract class GameState : IComparable<GameState>
    {
          public abstract bool IsDone();
          public abstract void Progress(int action);
          public abstract List<int> LegalActions();
          public abstract override string ToString();
          public abstract long EvaluateScore();
          public abstract GameState Clone();
          public abstract int CompareTo(GameState? other);

        public static List<T> GetNextStates<T>(T state) where T : GameState
        {
            var nextStates = new List<T>();
            foreach (var action in state.LegalActions())
            {
                var nextState = (T)state.Clone();
                nextState.Progress(action);
                nextStates.Add(nextState);
            }
            return nextStates;
        }
    }
}