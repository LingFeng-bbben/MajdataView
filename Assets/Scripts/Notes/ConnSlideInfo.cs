using UnityEngine;

namespace Assets.Scripts.Notes
{
    public class ConnSlideInfo
    {
        public float TotalLength { get; set; }
        public float TotalSlideLen { get; set; }
        public bool IsGroupPartHead { get; set; }
        public bool IsGroupPart { get; set; }
        public bool IsGroupPartEnd { get; set; }
        public GameObject Parent { get; set; } = null;
        public bool DestroyAfterJudge 
        {
            get => IsGroupPartEnd;
        }
        public bool IsConnSlide { get => IsGroupPart; }
        public bool ParentFinished 
        {
            get
            {
                if (Parent == null)
                    return true;
                else 
                    return Parent.GetComponent<SlideDrop>().isFinished;
            }
        }
        public bool ParentPendingFinish
        {
            get
            {
                if (Parent == null)
                    return false;
                else
                    return Parent.GetComponent<SlideDrop>().isPendingFinish;
            }
        }


    }
}
