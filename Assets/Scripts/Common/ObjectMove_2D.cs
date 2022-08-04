using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CommonDefine;

namespace ObjectMove_2D
{
    public class ObjectMove_2D : MonoBehaviour
    {
        /// <summary>
        /// 揺れる動作
        /// </summary>
        /// <param name="tra">       動作オブジェクトのTransform</param>
        /// <param name="moveSpeed"> 動作速度</param>
        /// <param name="maxRot">    揺れ角度</param>
        /// <param name="moveCount"> 1サイクル動作回数(カウントしない場合は-1指定)</param>
        /// <param name="stopTime">  停止時間</param>
        /// <param name="breakCount">終了サイクル数(無限ループの場合は-1指定)</param>
        /// <param name="endTime">   揺れ終了時間(時間で止めない場合は-1指定)</param>
        public static IEnumerator ShakeMovement(Transform tra, float moveSpeed, float maxRot, int moveCount, float stopTime, int breakCount, float endTime)
        {
            bool leftMove      = true;   //左に回転？
            bool rotReturn     = false;  //角度戻し中？
            bool shakeStop     = false;  //停止？
            int loopTimes      = 0;      //動作回数
            int cycleTimes     = 0;      //サイクル回数
            float playTime     = 0.0f;   //揺れ動作再生時間
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
        /// <param name="tra">       動作オブジェクトのTransform</param>
        /// <param name="moveSpeed"> 動作速度</param>
        /// <param name="targetPos"> 目標座標</param>
        public static IEnumerator DecelerationMovement(Transform tra, float moveSpeed, Vector3 targetPos)
        {
            float offset   = 0.1f;              //停止場所のオフセット
            Vector3 nowPos = tra.localPosition; //現在の座標
            bool sideways  = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X方向に動作？
            while (true)
            {
                if (tra == null) yield break;
                tra.localPosition = Vector3.Lerp(tra.localPosition, targetPos, moveSpeed);
                nowPos = tra.localPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways  && targetPos.x - offset < nowPos.x && nowPos.x < targetPos.x + offset) ||
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
        /// <param name="tra">       動作オブジェクトのTransform</param>
        /// <param name="moveSpeed"> 動作速度</param>
        /// <param name="targetPos"> 目標座標</param>
        /// <param name="acceleRate">加速率(等速移動は0.0f指定)</param>
        const float DEFAULT_ACCELE_RATE = 0.0f;
        public static IEnumerator ConstantSpeedMovement(Transform tra, float moveSpeed, Vector3 targetPos, float acceleRate = DEFAULT_ACCELE_RATE)
        {
            float minSpeed = 0.01f;            //最低速度指定(無限ループ対策)
            float offset   = 0.05f;             //停止場所のオフセット
            Vector3 nowPos = tra.localPosition; //現在の座標
            bool sideways  = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X方向に動作？
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
                if ((sideways  && targetPos.x - offset < nowPos.x && nowPos.x < targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset < nowPos.y && nowPos.y < targetPos.y + offset))
                {
                    tra.localPosition = targetPos;
                    break;
                }
                yield return FIXED_UPDATE;
            }
        }


        /*
        /// <summary>
        /// 移動動作(MoveMovement)に要する時間計算
        /// </summary>
        /// <param name="tra">       動作オブジェクトのTransform</param>
        /// <param name="moveSpeed"> 動作速度</param>
        /// <param name="acceleRate">加速率(等速移動は1.0f指定)</param>
        /// <param name="targetPos"> 目標座標</param>
        /// <returns>float：移動時間</returns>
        public static float GetMoveTime(Transform tra, float moveSpeed, float acceleRate, Vector2 targetPos)
        {
            //クローン作製
            Transform cloneTra = CloneCreate(tra);

            //時間計算用
            float moveTime     = 0.0f;

            float minSpeed = 1.0f;                      //最低速度指定(無限ループ対策)
            float offset   = 0.5f;                      //停止場所のオフセット
            Vector2 nowPos = cloneTra.localPosition; //現在の座標
            bool sideways  = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X方向に動作？
            while (true)
            {
                moveTime  += ONE_FRAME_TIMES;
                moveSpeed *= acceleRate;
                if (0.0f <= moveSpeed && moveSpeed < minSpeed) moveSpeed = minSpeed;
                else if (-minSpeed < moveSpeed && moveSpeed <= 0.0f) moveSpeed = -minSpeed;
                cloneTra.localPosition = Vector2.MoveTowards(cloneTra.localPosition, targetPos, moveSpeed);
                nowPos = cloneTra.localPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && targetPos.x - offset <= nowPos.x && nowPos.x <= targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset <= nowPos.y && nowPos.y <= targetPos.y + offset))
                {
                    cloneTra.localPosition = targetPos;
                    break;
                }
            }

            Destroy(cloneTra.gameObject);  //クローン削除
            return moveTime;               //時間を返す
        }
        */

        /// <summary>
        /// 往復動作
        /// </summary>
        /// <param name="tra">       動作オブジェクトのTransform</param>
        /// <param name="shakeSpeed">動作速度</param>
        /// <param name="offsetX">   目標座標オフセットX</param>
        /// <param name="offsetY">   目標座標オフセットY</param>
        /// <param name="shakeTimes">移動回数</param>
        /// <param name="delayTime"> 移動間の待機時間</param>
        public static IEnumerator SlideShakeMovement(Transform tra, float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            float offset       = 0.5f;                 //停止場所のオフセット
            Vector2 defaultPos = tra.localPosition; //初期座標取得
            bool sideways      = Mathf.Abs(offsetX) >= Mathf.Abs(offsetY); //X方向に動作？

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
        /// 往復動作(SlideShakeMovement)に要する時間計算
        /// </summary>
        /// <param name="tra">       動作オブジェクトのTransform</param>
        /// <param name="shakeSpeed">動作速度</param>
        /// <param name="offsetX">   目標座標オフセットX</param>
        /// <param name="offsetY">   目標座標オフセットY</param>
        /// <param name="shakeTimes">移動回数</param>
        /// <param name="delayTime"> 移動間の待機時間</param>
        /// <returns>float：所要時間</returns>
        public static float GetSlideShakeTime(Transform tra, float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            //クローン作製
            Transform cloneTra = CloneCreate(tra);

            //時間計算用
            float moveTime     = 0.0f;

            float offset = 0.5f;                 //停止場所のオフセット
            Vector2 defaultPos = cloneTra.localPosition; //初期座標取得
            bool sideways = Mathf.Abs(offsetX) >= Mathf.Abs(offsetY); //X方向に動作？

            //往復動作
            for (int moveCount = 0; moveCount < shakeTimes; moveCount++)
            {
                int vector = (moveCount % 2 == 0) ? 1 : -1;
                Vector2 tarPos = new Vector2(defaultPos.x + offsetX * vector, defaultPos.y + offsetY * vector);
                while (true)
                {
                    moveTime += ONE_FRAME_TIMES;
                    cloneTra.localPosition = Vector2.MoveTowards(cloneTra.localPosition, tarPos, shakeSpeed);
                    Vector2 nowPos = cloneTra.localPosition;

                    //---------------------------------------------
                    //次の移動へ
                    //---------------------------------------------
                    if ((sideways && tarPos.x - offset <= nowPos.x && nowPos.x <= tarPos.x + offset) ||
                        (!sideways && tarPos.y - offset <= nowPos.y && nowPos.y <= tarPos.y + offset))
                    {
                        cloneTra.localPosition = tarPos;
                        moveTime += delayTime;
                        break;
                    }
                }
            }

            //元の座標に戻る
            while (true)
            {
                moveTime += ONE_FRAME_TIMES;
                cloneTra.localPosition = Vector2.MoveTowards(cloneTra.localPosition, defaultPos, shakeSpeed);
                Vector2 nowPos = cloneTra.localPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && defaultPos.x - offset <= nowPos.x && nowPos.x <= defaultPos.x + offset) ||
                    (!sideways && defaultPos.y - offset <= nowPos.y && nowPos.y <= defaultPos.y + offset))
                {
                    cloneTra.localPosition = defaultPos;
                    break;
                }
            }

            Destroy(cloneTra.gameObject);  //クローン削除
            return moveTime;               //時間を返す
        }


        /// <summary>
        /// 回転動作
        /// </summary>
        /// <param name="tra">動作オブジェクトのTransform</param>
        /// <param name="rotSpeed">拡縮速度</param>
        /// <param name="stopRot"> 回転後の角度(絶対角)</param>
        public static IEnumerator RotateMovement(Transform tra, Vector3 rotSpeed, Vector3 stopRot)
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
                Vector3 nowRot   = tra.localEulerAngles;
                float refRot     = nowRot.x;
                float refStopRot = stopRot.x;
                switch (axis)
                {
                    case 1:
                        refRot     = nowRot.y;
                        refStopRot = stopRot.y;
                        break;
                    case 2:
                        refRot     = nowRot.z;
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
        /// 回転動作(RotateMovement)に要する時間計算
        /// </summary>
        /// <param name="tra">     動作オブジェクトのTransform</param>
        /// <param name="rotSpeed">拡縮速度</param>
        /// <param name="stopRot"> 回転後の角度(絶対角)</param>
        /// <returns></returns>
        public static float GetRotateMoveTime(Transform tra, Vector3 rotSpeed, Vector3 stopRot)
        {
            //クローン作製
            Transform cloneTra = CloneCreate(tra);

            //時間計算用
            float moveTime     = 0.0f;

            //最も多く動作する軸判定
            int axis = 0;
            if (rotSpeed.x < rotSpeed.y)
                axis = (rotSpeed.y > rotSpeed.z) ? 1 : 2;
            else if (rotSpeed.x < rotSpeed.z)
                axis = 2;

            //回転
            float tolerance    = 5.0f;
            while (true)
            {
                moveTime += ONE_FRAME_TIMES;
                cloneTra.Rotate(rotSpeed.x, rotSpeed.y, rotSpeed.z);
                Vector3 nowRot = cloneTra.localEulerAngles;
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
            }

            Destroy(cloneTra.gameObject);  //クローン削除
            return moveTime;               //時間を返す
        }


        /// <summary>
        /// 全軸の拡大縮小動作
        /// </summary>
        /// <param name="tra">         動作オブジェクトのTransform</param>
        /// <param name="scalingSpeed">拡縮速度(すべて正の数で指定)</param>
        /// <param name="changedScale"> 変更後の拡大率</param>
        /// <returns></returns>
        public static IEnumerator AllScaleChange(Transform tra, float scalingSpeed, float changedScale)
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
                if ((scaleUp  && (x > changedScale || y > changedScale)) || 
                    (!scaleUp && (x < changedScale || y < changedScale)))
                    break;

                //現在のスケール更新
                nowScale = tra.localScale;
                yield return FIXED_UPDATE;
            }
        }

        /// <summary>
        /// 色変更動作(SpriteRenderer.color)
        /// </summary>
        /// <param name="spri">       変更対象SpriteRenderer</param>
        /// <param name="changeSpeed">変更速度</param>
        /// <param name="colArray">   変更色の配列(0:現在の色)</param>
        /// <param name="chengeCount">ループ回数(配列1周で1カウント、-1指定で無限再生)</param>
        /// <returns></returns>
        public static IEnumerator SpriteRendererPaletteChange(SpriteRenderer spri, float changeSpeed, Color[] colArray, int chengeCount = 1)
        {
            int loopTimes = 0;                  //繰り返し回数
            int colCount  = colArray.Length;    //変更色の数
            bool infinite = chengeCount < 0;    //無限ループ？

            int nowIndex  = 0;                  //現在の色
            int nextIndex = 1;                  //次の色
            float judgeRange  = 5.0f / 255.0f;  //判定範囲

            spri.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                if (spri == null) yield break;

                //色変更開始
                spri.color = Color.Lerp(spri.color, colArray[nextIndex], changeSpeed);

                //変更終了
                Color nowColor = spri.color;
                if (nowColor[0] + judgeRange >= colArray[nextIndex][0] && colArray[nextIndex][0] >= nowColor[0] - judgeRange && //R
                    nowColor[1] + judgeRange >= colArray[nextIndex][1] && colArray[nextIndex][1] >= nowColor[1] - judgeRange && //G
                    nowColor[2] + judgeRange >= colArray[nextIndex][2] && colArray[nextIndex][2] >= nowColor[2] - judgeRange && //B
                    nowColor[3] + judgeRange >= colArray[nextIndex][3] && colArray[nextIndex][3] >= nowColor[3] - judgeRange)   //A
                {
                    loopTimes++;
                    nowIndex = nextIndex;
                    nextIndex = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                }
                yield return new WaitForSecondsRealtime(ONE_FRAME_TIMES);
            }
            spri.color = colArray[nowIndex];
        }

        /// <summary>
        /// 色変更動作(SpriteRendererPaletteChange) に要する時間計算
        /// </summary>
        /// <param name="spri">       変更対象SpriteRenderer</param>
        /// <param name="changeSpeed">変更速度</param>
        /// <param name="colArray">   変更色の配列(0:現在の色)</param>
        /// <param name="compArray">  比較番号指定配列(0:R 1:G 2:B 3:A)</param>
        /// <param name="chengeCount">ループ回数(配列1周で1カウント、-1指定で無限再生)</param>
        /// <returns></returns>
        public static float GetSpriteRendererPaletteChangeTime(SpriteRenderer spri, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            //クローン作製
            SpriteRenderer clonespri = CloneCreate_SpriteRenderer(spri);
            float moveTime = 0.0f;

            int loopTimes      = 0;                //繰り返し回数
            int colCount       = colArray.Length;  //変更色の数
            bool infinite      = chengeCount < 0;  //無限ループ？

            int nowIndex       = 0;    //現在の色
            int nextIndex      = 1;    //次の色
            float nextCompCol  = colArray[nextIndex][compArray[nowIndex]];   //比較色指定
            float judgeRange   = 5.0f / 255.0f;                              //判定範囲

            clonespri.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                clonespri.color = Color.Lerp(clonespri.color, colArray[nextIndex], changeSpeed);
                float nowCompCol = clonespri.color[compArray[nowIndex]];
                if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                {
                    nowIndex    = nextIndex;
                    nextIndex   = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                    nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                    loopTimes++;
                }

                //時間計算
                moveTime += ONE_FRAME_TIMES;
            }
            clonespri.color = colArray[nowIndex];

            Destroy(clonespri.gameObject); //クローン削除
            return moveTime;              //時間を返す
        }


        //========================================================================
        //クローンをオリジナルと同座標にセット
        //========================================================================
        //tra;  　動作オブジェクトのTransform
        //========================================================================
        public static Transform CloneCreate(Transform tra)
        {
            GameObject clone       = GameObject.Instantiate(tra.gameObject) as GameObject;
            Transform cloneTra = clone.GetComponent<Transform>();
            Transform parentTra    = tra.parent.gameObject.transform;
            cloneTra.SetParent(parentTra, false);
            cloneTra.localRotation = tra.localRotation;
            return cloneTra;
        }
        //========================================================================
        //spri;  　動作オブジェクトのSpriteRenderer
        //========================================================================
        public static SpriteRenderer CloneCreate_SpriteRenderer(SpriteRenderer spri)
        {
            GameObject clone = GameObject.Instantiate(spri.gameObject) as GameObject;
            SpriteRenderer clonespri   = clone.GetComponent<SpriteRenderer>();
            clonespri.color   = spri.color;
            return clonespri;
        }
    }
}