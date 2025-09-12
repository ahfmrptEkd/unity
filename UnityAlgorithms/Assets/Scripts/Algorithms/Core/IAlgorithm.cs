using UnityAlgorithms.Core;

namespace UnityAlgorithms.Algorithms.Core
{
    public interface IAlgorithm
    {
        int SelectAction(GameState state);
        string GetName();
        void SetParams(AlgorithmParams parameters);
        GameState RunAndEvaluate(GameState state, int action);
    }
}