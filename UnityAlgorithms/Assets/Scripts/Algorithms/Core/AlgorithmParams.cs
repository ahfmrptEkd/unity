using UnityAlgorithms.Core;

namespace UnityAlgorithms.Algorithms.Core
{
    public class AlgorithmParams
    {
        public int SearchWidth { get; set; } = GameConstants.AlgorithmParams.SEARCH_WIDTH;
        public int SearchDepth { get; set; } = GameConstants.AlgorithmParams.SEARCH_DEPTH;
        public int SearchNumber { get; set; } = GameConstants.AlgorithmParams.SEARCH_NUMBER;
        public long TimeThreshold { get; set; } = GameConstants.AlgorithmParams.TIME_THRESHOLD;
        
        public int PlayoutNumber { get; set; } = GameConstants.ConnectFour.PLAYOUT_NUMBER;
        public double StartTemperature { get; set; } = GameConstants.AlgorithmParams.START_TEMPERATURE;
        public double EndTemperature { get; set; } = GameConstants.AlgorithmParams.END_TEMPERATURE;
        
        public int PlayerId { get; set; } = 0;
    }
}