using Assets.Scripts.Types;
using System;
using System.Diagnostics;
using UnityEngine;
#nullable enable
public class NoteDrop : MonoBehaviour
{
    public int startPosition;
    public float time;
    public int noteSortOrder;
    public float speed = 7;
    public bool isEach;

    protected AudioTimeProvider timeProvider;

    public NoteStatus State { get; protected set; } = NoteStatus.Start;
    protected SensorType sensorPos;
    protected Sensor sensor;
    protected SensorManager manager;
    protected InputManager inputManager;
    protected NoteManager noteManager;
    protected Guid guid = Guid.NewGuid();
    protected bool isJudged = false;
    protected JudgeType judgeResult;
    protected ObjectCounter objectCounter;
    
    /// <summary>
    /// 获取当前时刻距离正解帧的时间长度
    /// </summary>
    /// <returns>
    /// 当前时刻在正解帧后方，结果为正数
    /// <para>当前时刻在正解帧前方，结果为负数</para>
    /// </returns>
    protected float GetJudgeTiming() => timeProvider.AudioTime - time;
    protected Vector3 getPositionFromDistance(float distance) => getPositionFromDistance(distance, startPosition);
    protected Vector3 getPositionFromDistance(float distance,int position)
    {
        return new Vector3(
            distance * Mathf.Cos((position * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((position * -2f + 5f) * 0.125f * Mathf.PI));
    }
}

public class NoteLongDrop : NoteDrop
{
    public float LastFor = 1f;
    public GameObject holdEffect;

    protected float playerIdleTime = 0;
    protected Stopwatch userHold = new();
    protected float judgeDiff = -1;

    protected bool isAutoTrigger = false;

    /// <summary>
    /// 返回Hold的剩余长度
    /// </summary>
    /// <returns>
    /// Hold剩余长度
    /// </returns>
    protected float GetRemainingTime() => MathF.Max(LastFor - GetJudgeTiming(),0);


    protected virtual void PlayHoldEffect()
    {
        var material = holdEffect.GetComponent<ParticleSystemRenderer>().material;
        switch (judgeResult)
        {
            case JudgeType.LatePerfect2:
            case JudgeType.FastPerfect2:
            case JudgeType.LatePerfect1:
            case JudgeType.FastPerfect1:
            case JudgeType.Perfect:
                material.SetColor("_Color", new Color(1f, 0.93f, 0.61f)); // Yellow
                break;
            case JudgeType.LateGreat:
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat2:
            case JudgeType.FastGreat2:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat:
                material.SetColor("_Color", new Color(1f, 0.70f, 0.94f)); // Pink
                break;
            case JudgeType.LateGood:
            case JudgeType.FastGood:
                material.SetColor("_Color", new Color(0.56f, 1f, 0.59f)); // Green
                break;
            case JudgeType.Miss:
                material.SetColor("_Color", new Color(1f, 1f, 1f)); // White
                break;
            default:
                break;
        }
        holdEffect.SetActive(true);        
    }
    protected virtual void StopHoldEffect()
    {
        holdEffect.SetActive(false);
    }
}