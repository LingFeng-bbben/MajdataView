using UnityEngine;
#nullable enable
namespace Assets.Scripts.Types
{
    public class ConnSlideInfo
    {
        /// <summary>
        /// 表示Slide Group的总时值
        /// </summary>
        public float TotalLength { get; set; }
        /// <summary>
        /// 表示Slide Group的总长度
        /// </summary>
        public float TotalSlideLen { get; set; }
        /// <summary>
        /// 指示该Slide是否位于Group的头部
        /// </summary>
        public bool IsGroupPartHead { get; set; }
        /// <summary>
        /// 指示该Slide是否位于Group内
        /// </summary>
        public bool IsGroupPart { get; set; }
        /// <summary>
        /// 指示该Slide是否位于Group的尾部
        /// </summary>
        public bool IsGroupPartEnd { get; set; }
        /// <summary>
        /// 获取位于该Slide前方的Slide的GameObject对象
        /// </summary>
        public GameObject? Parent { get; set; } = null;
        /// <summary>
        /// null
        /// </summary>
        public bool DestroyAfterJudge
        {
            get => IsGroupPartEnd;
        }
        /// <summary>
        /// 指示当前Slide是否为Connect Slide
        /// </summary>
        public bool IsConnSlide { get => IsGroupPart; }
        /// <summary>
        /// 获取前方Slide是否完成
        /// </summary>
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
        /// <summary>
        /// 获取前方Slide是否准备完成
        /// </summary>
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
