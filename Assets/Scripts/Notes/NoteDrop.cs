using UnityEngine;

public class NoteDrop : MonoBehaviour
{
    public float time;
    public int noteSortOrder;
}

public class NoteLongDrop : NoteDrop
{
    public float LastFor = 1f;
}