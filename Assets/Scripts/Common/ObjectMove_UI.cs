using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CommonDefine;

namespace ObjectMove_UI
{
    public class ObjectMove_UI : MonoBehaviour
    {
        /// <summary>
        /// 揺れる動作
        /// </summary>
        /// <param name="tra">       動作オブジェクトのRectTransform</param>
        /// <param name="moveSpeed"> 動作速度</param>
        /// <param name="maxRot">    揺れ角度</param>
        /// <param name="moveCount"> 1サイクル動作回数(カウントしない場合は-1指定)</param>
        /// <param name="stopTime">  停止時間</param>
        /// <param name="breakCount">終了サイクル数(無限ループの場合は-1指定)</param>
        /// <param name="endTime">   揺れ終了時間(時間で止めない場合は-1指定)</param>
        public static IEnumerator ShakeMovement(RectTransform tra, float moveSpeed, float maxRot, int moveCount, float stopTime, int breakCount, float endTime)
        {
            bool leftMove = true;   //左に回転？
            bool rotReturn = false;  //角度戻し中？
            bool shakeStop = false;  //停止？
            int loopTimes = 0;      //動作回数
            int cycleTimes = 0;      //サイクル回数
            float playTime = 0.0f;   //揺れ動作再生時間
            while (true)
            {
                if (tra == null) yield break;
                float rotZ = tra.localRotation.eulerAngles.z;
                rotZ = (rotZ >= 180.0f) ? rotZ - 360.0f : rotZ;
                if (!rotReturn)
                {
                    if (leftMove)
                    {
                        //---------------------------------------------
                        //左に回転
                        //---------------------------------------------
                        tra.Rotate(0.0f, 0.0f, moveSpeed);
                        if (rotZ > maxRot) leftMove = false;
                    }
                    else
                    {
                        //---------------------------------------------
                        //右に回転
                        //---------------------------------------------
                        tra.Rotate(0.0f, 0.0f, -moveSpeed);
                        if (rotZ < -maxRot)
                        {
                            loopTimes++;
                            if (0 < moveCount && moveCount <= loopTimes) rotReturn = true;  //終了サイクル数を超えた時に揺れを止める
                            else leftMove = true;
                        }
                    }
                }
                else
                {
                    //---------------------------------------------
                    //角度を0に戻す
                    //---------------------------------------------
                    tra.Rotate(0.0f, 0.0f, moveSpeed);
                    if (-0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        shakeStop = true;
                    }

                    //---------------------------------------------
                    //揺れを止める
                    //---------------------------------------------
                    if (shakeStop)
                    {
                        cycleTimes++;
                        if (0 < breakCount && breakCount <= cycleTimes) break;  //揺れ終了
                        else yield return new WaitForSeconds(stopTime);         //一時停止
                        loopTimes = 0;
                        rotReturn = false;
                        shakeStop = false;
                    }
                }

                //---------------------------------------------
                //時間で停止する場合の処理
                //---------------------------------------------
                if (0 < endTime)
                {
                    playTime += ONE_FRAME_TIMES;
                    if (playTime >= endTime && -0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        break;  //揺れ終了
                    }
                }
                yield return FIXED_UPDATE;
            }
        }


        /// <summary>
        /// 減速移動
        /// </summary>
        /// <param name="tra">       動作オブジェクトのRectTransform</param>
        /// <param name="moveSpeed"> 動作速度</param>
        /// <param name="targetPos"> 目標座標</param>
        public static IEnumerator DecelerationMovement(RectTransform tra, float moveSpeed, Vector3 targetPos)
        {
            float offset = 0.1f;              //停止場所のオフセット
            Vector3 nowPos = tra.localPosition; //現在の座標
            bool sideways = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X方向に動作？
            while (true)
            {
                if (tra == null) yield break;
                tra.localPosition = Vector3.Lerp(tra.localPosition, targetPos, moveSpeed);
                nowPos = tra.localPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && targetPos.x - offset < nowPos.x && nowPos.x < targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset < nowPos.y && nowPos.y < targetPos.y + offset))
                {
                    tra.localPosition = targetPos;
                    break;
                }
                yield return FIXED_UPDATE;
            }
        }

        /// <summary>
        /// 等速移動
        /// </summary>
        /// <param name="tra">       動作オブジェクトのRectTransform</param>
        /// <param name="moveSpeed"> 動作速度</param>
        /// <param name="targetPos"> 目標座標</param>
        /// <param name="acceleRate">加速率(等速移動は0.0f指定)</param>
        const float DEFAULT_ACCELE_RATE = 0.0f;
        public static IEnumerator ConstantSpeedMovement(RectTransform tra, float moveSpeed, Vector3 targetPos, float acceleRate = DEFAULT_ACCELE_RATE)
        {
            float minSpeed = 0.01f;            //最低速度指定(無限ループ対策)
            float offset = 0.05f;             //停止場所のオフセット
            Vector3 nowPos = tra.localPosition; //現在の座標
            bool sideways = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X方向に動作？
            while (true)
            {
                if (tra == null) yield break;
                moveSpeed += acceleRate;
                if (acceleRate != DEFAULT_ACCELE_RATE)
                {
                    if (0.0f <= moveSpeed && moveSpeed < minSpeed) moveSpeed = minSpeed;
                    else if (-minSpeed < moveSpeed && moveSpeed <= 0.0f) moveSpeed = -minSpeed;
                }
                tra.localPosition = Vector3.MoveTowards(tra.localPosition, targetPos, moveSpeed);
                nowPos = tra.localPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && targetPos.x - offset < nowPos.x && nowPos.x < targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset < nowPos.y && nowPos.y < targetPos.y + offset))
                {
                    tra.localPosition = targetPos;
                    break;
                }
                yield return FIXED_UPDATE;
            }
        }

        /// <summary>
        /// 往復動作
        /// </summary>
        /// <param name="tra">       動作オブジェクトのRectTransform</param>
        /// <param name="shakeSpeed">動作速度</param>
        /// <param name="offsetX">   目標座標オフセットX</param>
        /// <param name="offsetY">   目標座標オフセットY</param>
        /// <param name="shakeTimes">移動回数</param>
        /// <param name="delayTime"> 移動間の待機時間</param>
        public static IEnumerator SlideShakeMovement(RectTransform tra, float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            float offset = 0.5f;                 //停止場所のオフセット
            Vector2 defaultPos = tra.localPosition; //初期座標取得
            bool sideways = Mathf.Abs(offsetX) >= Mathf.Abs(offsetY); //X方向に動作？

            //往復動作
            for (int moveCount = 0; moveCount < shakeTimes; moveCount++)
            {
                int vector = (moveCount % 2 == 0) ? 1 : -1;
                Vector2 tarPos = new Vector2(defaultPos.x + offsetX * vector, defaultPos.y + offsetY * vector);
                while (true)
                {
                    if (tra == null) yield break;
                    tra.localPosition = Vector2.MoveTowards(tra.localPosition, tarPos, shakeSpeed);
                    Vector2 nowPos = tra.localPosition;

                    //---------------------------------------------
                    //次の移動へ
                    //---------------------------------------------
                    if ((sideways && tarPos.x - offset <= nowPos.x && nowPos.x <= tarPos.x + offset) ||
                        (!sideways && tarPos.y - offset <= nowPos.y && nowPos.y <= tarPos.y + offset))
                    {
                        tra.localPosition = tarPos;
                        yield return new WaitForSeconds(delayTime);
                        break;
                    }
                    yield return FIXED_UPDATE;
                }
            }

            //元の座標に戻る
            while (true)
            {
                if (tra == null) yield break;
                tra.localPosition = Vector2.MoveTowards(tra.localPosition, defaultPos, shakeSpeed);
                Vector2 nowPos = tra.localPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && defaultPos.x - offset <= nowPos.x && nowPos.x <= defaultPos.x + offset) ||
                    (!sideways && defaultPos.y - offset <= nowPos.y && nowPos.y <= defaultPos.y + offset))
                {
                    tra.localPosition = defaultPos;
                    break;
                }
                yield return FIXED_UPDATE;
            }
        }

        /// <summary>
        /// 回転動作
        /// </summary>
        /// <param name="tra">動作オブジェクトのRectTransform</param>
        /// <param name="rotSpeed">拡縮速度</param>
        /// <param name="stopRot"> 回転後の角度(絶対角)</param>
        public static IEnumerator RotateMovement(RectTransform tra, Vector3 rotSpeed, Vector3 stopRot)
        {
            //最も多く動作する軸判定
            int axis = 0;
            if (rotSpeed.x < rotSpeed.y)
                axis = (rotSpeed.y > rotSpeed.z) ? 1 : 2;
            else if (rotSpeed.x < rotSpeed.z)
                axis = 2;

            //回転
            float tolerance = 10.0f;
            while (true)
            {
                if (tra == null) yield break;
                tra.Rotate(rotSpeed.x, rotSpeed.y, rotSpeed.z);
                Vector3 nowRot = tra.localEulerAngles;
                float refRot = nowRot.x;
                float refStopRot = stopRot.x;
                switch (axis)
                {
                    case 1:
                        refRot = nowRot.y;
                        refStopRot = stopRot.y;
                        break;
                    case 2:
                        refRot = nowRot.z;
                        refStopRot = stopRot.z;
                        break;
                }
                if (refStopRot - tolerance <= refRot && refRot <= refStopRot + tolerance) break;
                yield return FIXED_UPDATE;
            }

            //最終角度に合わせる
            tra.localRotation = Quaternion.Euler(stopRot.x, stopRot.y, stopRot.z);
        }

        /// <summary>
        /// 全軸の拡大縮小動作
        /// </summary>
        /// <param name="tra">         動作オブジェクトのRectTransform</param>
        /// <param name="scalingSpeed">拡縮速度(すべて正の数で指定)</param>
        /// <param name="changedScale"> 変更後の拡大率</param>
        /// <returns></returns>
        public static IEnumerator AllScaleChange(RectTransform tra, float scalingSpeed, float changedScale)
        {
            Vector3 nowScale = tra.localScale;         //現在のスケール
            bool scaleUp = nowScale.x < changedScale;  //拡大？

            //縮小の場合は速度を反転
            if (!scaleUp) scalingSpeed *= -1;

            while (true)
            {
                if (tra == null) yield break;

                //拡縮率更新
                float x = nowScale.x + scalingSpeed;
                float y = nowScale.y + scalingSpeed;
                tra.localScale = new Vector3(x, y, nowScale.z);

                //終了判定
                if ((scaleUp && (x > changedScale || y > changedScale)) ||
                    (!scaleUp && (x < changedScale || y < changedScale)))
                    break;

                //現在のスケール更新
                nowScale = tra.localScale;
                yield return FIXED_UPDATE;
            }
        }

        /// <summary>
        /// 色変更動作(Image.color)
        /// </summary>
        /// <param name="img">        変更対象Image</param>
        /// <param name="changeSpeed">変更速度</param>
        /// <param name="colArray">   変更色の配列(0:現在の色)</param>
        /// <param name="chengeCount">ループ回数(配列1周で1カウント、-1指定で無限再生)</param>
        /// <returns></returns>
        public static IEnumerator ImagePaletteChange(Image img, float changeSpeed, Color32[] colArray, int chengeCount = 1)
        {
            int loopTimes = 0;                  //繰り返し回数
            int colCount = colArray.Length;     //変更色の数
            bool infinite = chengeCount < 0;    //無限ループ？

            int nowIndex = 0;                   //現在の色
            int nextIndex = 1;                  //次の色
            int judgeRange = 5;                 //判定範囲

            img.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                if (img == null) yield break;

                //色変更開始
                img.color = Color.Lerp(img.color, colArray[nextIndex], changeSpeed);

                //変更終了
                Color32 nowColor = img.color;
                if (nowColor.r + judgeRange >= colArray[nextIndex].r && colArray[nextIndex].r >= nowColor.r - judgeRange && //R
                    nowColor.g + judgeRange >= colArray[nextIndex].g && colArray[nextIndex].g >= nowColor.g - judgeRange && //G
                    nowColor.b + judgeRange >= colArray[nextIndex].b && colArray[nextIndex].b >= nowColor.b - judgeRange && //B
                    nowColor.a + judgeRange >= colArray[nextIndex].a && colArray[nextIndex].a >= nowColor.a - judgeRange)   //A
                {
                    loopTimes++;
                    nowIndex = nextIndex;
                    nextIndex = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                }
                yield return new WaitForSecondsRealtime(ONE_FRAME_TIMES);
            }
            img.color = colArray[nowIndex];
        }
    }
}