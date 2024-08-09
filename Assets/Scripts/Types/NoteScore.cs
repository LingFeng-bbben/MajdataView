using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Types
{
    public ref struct NoteScore
    {
        public long TotalScore { get; set; }
        public long TotalExtraScore { get; set; }
        public long TotalExtraScoreClassic { get; set; }
        public long LostScore { get; set; }
        public long LostExtraScore { get; set; }
        public long LostExtraScoreClassic { get; set; }
    }
}
