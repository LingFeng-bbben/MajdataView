using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static NoteEffectManager;
using static Sensor;

namespace Assets.Scripts.Notes
{
    public class TapBase: NoteDrop
    {
        public bool isBreak;
        public bool isEX;
        public bool isFakeStarRotate;

        public Sprite tapSpr;
        public Sprite eachSpr;
        public Sprite breakSpr;
        public Sprite exSpr;

        public Sprite eachLine;
        public Sprite breakLine;

        public RuntimeAnimatorController BreakShine;

        public GameObject tapLine;

        public Color exEffectTap;
        public Color exEffectEach;
        public Color exEffectBreak;

        public Material breakMaterial;

        protected SpriteRenderer exSpriteRender;
        protected SpriteRenderer lineSpriteRender;

        protected ObjectCounter ObjectCounter;

        protected SpriteRenderer spriteRenderer;
        protected InputManager inputManager;

        protected void PreLoad()
        {
            var notes = GameObject.Find("Notes").transform;
            noteManager = notes.GetComponent<NoteManager>();
            tapLine = Instantiate(tapLine, notes);
            tapLine.SetActive(false);
            lineSpriteRender = tapLine.GetComponent<SpriteRenderer>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            exSpriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
            timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
            ObjectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();

            spriteRenderer.sortingOrder += noteSortOrder;
            exSpriteRender.sortingOrder += noteSortOrder;
        }
        protected void FixedUpdate()
        {
            if (!isJudged && GetJudgeTiming() > 0.15f)
            {
                judgeResult = JudgeType.Miss;
                isJudged = true;
                Destroy(tapLine);
                Destroy(gameObject);
            }
            else if (isJudged)
            {
                Destroy(tapLine);
                Destroy(gameObject);
            }
        }
        // Update is called once per frame
        protected virtual void Update()
        {
            var timing = timeProvider.AudioTime - time;
            var distance = timing * speed + 4.8f;
            var destScale = distance * 0.4f + 0.51f;
            if (destScale < 0f)
            {
                destScale = 0f;
                return;
            }

            spriteRenderer.forceRenderingOff = false;
            if (isEX) exSpriteRender.forceRenderingOff = false;

            if (isBreak)
            {
                var extra = Math.Max(Mathf.Sin(timeProvider.GetFrame() * 0.17f) * 0.5f, 0);
                spriteRenderer.material.SetFloat("_Brightness", 0.95f + extra);
            }

            if (timing > 0 && GameObject.Find("Input").GetComponent<InputManager>().AutoPlay)
                manager.SetSensorOn(sensor.Type, guid);

            if (isFakeStarRotate)
                transform.Rotate(0f, 0f, 400f * Time.deltaTime);
            else
                transform.rotation = Quaternion.Euler(0, 0, -22.5f + -45f * (startPosition - 1));
            tapLine.transform.rotation = Quaternion.Euler(0, 0, -22.5f + -45f * (startPosition - 1));


            if (destScale > 0.3f) tapLine.SetActive(true);

            if (distance < 1.225f)
            {
                transform.localScale = new Vector3(destScale, destScale);

                distance = 1.225f;
                var pos = getPositionFromDistance(distance);
                transform.position = pos;
            }
            else
            {
                var pos = getPositionFromDistance(distance);
                transform.position = pos;
                transform.localScale = new Vector3(1f, 1f);
            }

            var lineScale = Mathf.Abs(distance / 4.8f);
            tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);


        }
        protected void Check(SensorType s, SensorStatus oStatus, SensorStatus nStatus)
        {
            if (s != sensor.Type)
                return;
            if (isJudged || !noteManager.CanJudge(gameObject, startPosition))
                return;
            if (oStatus == SensorStatus.Off && nStatus == SensorStatus.On)
            {
                if (sensor.IsJudging)
                    return;
                else
                    sensor.IsJudging = true;
                Judge();
                if (isJudged)
                {
                    Destroy(tapLine);
                    Destroy(gameObject);
                }
            }
        }
        protected void Judge()
        {

            const int JUDGE_GOOD_AREA = 150;
            const int JUDGE_GREAT_AREA = 100;
            const int JUDGE_PERFECT_AREA = 50;

            const float JUDGE_SEG_PERFECT1 = 16.66667f;
            const float JUDGE_SEG_PERFECT2 = 33.33334f;
            const float JUDGE_SEG_GREAT1 = 66.66667f;
            const float JUDGE_SEG_GREAT2 = 83.33334f;

            if (isJudged)
                return;

            var timing = timeProvider.AudioTime - time;
            var isFast = timing < 0;
            var diff = MathF.Abs(timing * 1000);
            JudgeType result;
            if (diff > JUDGE_GOOD_AREA && isFast)
                return;
            else if (diff < JUDGE_SEG_PERFECT1)
                result = JudgeType.Perfect;
            else if (diff < JUDGE_SEG_PERFECT2)
                result = JudgeType.LatePerfect1;
            else if (diff < JUDGE_PERFECT_AREA)
                result = JudgeType.LatePerfect2;
            else if (diff < JUDGE_SEG_GREAT1)
                result = JudgeType.LateGreat;
            else if (diff < JUDGE_SEG_GREAT2)
                result = JudgeType.LateGreat1;
            else if (diff < JUDGE_GREAT_AREA)
                result = JudgeType.LateGreat;
            else if (diff < JUDGE_GOOD_AREA)
                result = JudgeType.LateGood;
            else
                result = JudgeType.Miss;

            if (result != JudgeType.Miss && isFast)
                result = 14 - result;
            if (result != JudgeType.Miss && isEX)
                result = JudgeType.Perfect;

            judgeResult = result;
            isJudged = true;
        }
        protected virtual void OnDestroy()
        {
            if (GameObject.Find("Server").GetComponent<HttpHandler>().IsReloding)
                return;
            var effectManager = GameObject.Find("NoteEffects").GetComponent<NoteEffectManager>();
            effectManager.PlayEffect(startPosition, isBreak, judgeResult);
            effectManager.PlayFastLate(startPosition, judgeResult);
            if (isBreak) ObjectCounter.breakCount++;
            else ObjectCounter.tapCount++;
            GameObject.Find("Notes").GetComponent<NoteManager>().noteCount[startPosition]++;
            if (GameObject.Find("Input").GetComponent<InputManager>().AutoPlay)
                manager.SetSensorOff(sensor.Type, guid);
            sensor.OnSensorStatusChange -= Check;
            inputManager.OnSensorStatusChange -= Check;
        }
    }
}
