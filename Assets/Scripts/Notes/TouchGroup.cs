using System;
using System.Linq;
using static NoteEffectManager;

namespace Assets.Scripts.Notes
{
    public class TouchGroup
    {
        public float Percent 
        { 
            get
            {
                if (Members.Length == 0)
                    return 0f;
                var finished = Members.Where(x => x == null);
                return finished.Count() / (float)Members.Length;
            }
        }
        public JudgeType? JudgeResult 
        { 
            get => _judgeResult; 
            set
            {
                if (Percent > 0.5f)
                    return;
                else
                    _judgeResult = value;
            }
        }
        public TouchDrop[] Members { get; set; } = Array.Empty<TouchDrop>();
        JudgeType? _judgeResult = null;
    }
}
