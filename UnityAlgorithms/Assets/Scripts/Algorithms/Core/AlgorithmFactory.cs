using System;
using UnityAlgorithms.Algorithms.ConnectFour;

namespace UnityAlgorithms.Algorithms.Core
{
    public static class AlgorithmFactory
    {
        public static IAlgorithm CreateAlgorithm(string algorithmName, AlgorithmParams? parameters = null)
        {
            parameters ??= new AlgorithmParams();
            
            IAlgorithm algorithm = algorithmName.ToLower() switch
            {
                "random" => new RandomAlgorithm(),
                _ => throw new ArgumentException($"Unknown algorithm: {algorithmName}")
            };
            
            algorithm.SetParams(parameters);
            return algorithm;
        }
    }
}