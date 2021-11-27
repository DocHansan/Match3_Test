using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    public struct GridCell
    {
        public int x,
            y,
            column,
            row,
            kind,
            match;
        public byte alpha;
    }
}
