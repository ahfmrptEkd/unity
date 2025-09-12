using System;
using ScoreType = System.Int64;

namespace UnityAlgorithms.Core
{
    public static class GameConstants
    {
        public static readonly long INF = 1000000000;
        public static readonly int[] DX = {1, -1, 0, 0};
        public static readonly int[] DY = {0, 0, 1, -1};

        public static class Board
        {
            public const int H = 8;
            public const int W = 8;
            public const int END_TURN = 15;
        }

        public static class AlgorithmParams
        {
            public const int SEARCH_WIDTH = 3;
            public const int SEARCH_DEPTH = 7;
            public const int SEARCH_NUMBER = 4;
            public const int TIME_THRESHOLD = 10;
            public const double START_TEMPERATURE = 500.0;
            public const double END_TEMPERATURE = 10.0;
        }

        public static class ConnectFour
        {
            public const int H = 6;
            public const int W = 7;
            public const int CONNECT_NUMBER = 4;
            public const int PLAYOUT_NUMBER = 1000;
        }

    }
}