using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    public struct GridCell
    {
        public int x;
        public int y;
        public int column;
        public int row;
        public int kind;
        public int match;
        public byte alpha;
        public bool isNeed_bonus_activation;
    }
}
