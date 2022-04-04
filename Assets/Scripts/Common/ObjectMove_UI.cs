using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ObjectMove_UI
{
    public class ObjectMove_UI : MonoBehaviour
    {
        //========================================================================
        //定数
        //========================================================================
        public static bool SCALING_INFINITE_END       = false;  //拡縮無限再生終了フラグ
        public static bool COLOR_CHANGE_INFINITE_END  = false;  //2色点滅無限再生終了フラグ

        //========================================================================
        //揺れる動作
        //========================================================================
        //tra;        動作オブジェクトのRectTransform
        //moveSpeed;  動作速度
        //maxRot;     揺れ角度
        //moveCount;  1サイクル動作回数(カウントしない場合は-1指定)
        //stopTime;   停止時間
        //breakCount; 終了サイクル数(無限ループの場合は-1指定)
        //endTime;    揺れ終了時間(時間で止めない場合は-1指定)
        //========================================================================
        public static IEnumerator ShakeMovement(RectTransform tra, float moveSpeed, float maxRot, int moveCount, float stopTime, int breakCount, float endTime)
        {
            bool leftMove      = true;   //左に回転？
            bool rotReturn     = false;  //角度戻し中？
            bool shakeStop     = false;  //停止？
            int loopTimes      = 0;      //動作回数
            int cycleTimes     = 0;      //サイクル回数
            float playTime     = 0.0f;   //揺れ動作再生時間
            float oneFrameTime = 0.02f;  //1フレームの時間
            while (true)
            {
                yield return new WaitForFixedUpdate();
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
                    playTime += oneFrameTime;
                    if (playTime >= endTime && -0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        break;  //揺れ終了
                    }
                }
            }
        }


        //========================================================================
        //移動動作
        //========================================================================
        //tra;        動作オブジェクトのRectTransform
        //moveSpeed;  動作速度
        //acceleRate; 加速率(等速移動は1.0f指定)
        //targetPos;  目標座標
        //========================================================================
        public static IEnumerator MoveMovement(RectTransform tra, float moveSpeed, float acceleRate, Vector2 targetPos)
        {
            float minSpeed = 1.0f;                 //最低速度指定(無限ループ対策)
            float offset   = 0.5f;                 //停止場所のオフセット
            Vector2 nowPos = tra.anchoredPosition; //現在の座標
            bool sideways  = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X方向に動作？
            while (true)
            {
                yield return new WaitForFixedUpdate();
                moveSpeed *= acceleRate;
                if (0.0f <= moveSpeed && moveSpeed < minSpeed) moveSpeed = minSpeed;
                else if (-minSpeed < moveSpeed && moveSpeed <= 0.0f) moveSpeed = -minSpeed;
                tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, targetPos, moveSpeed);
                nowPos = tra.anchoredPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && targetPos.x - offset <= nowPos.x && nowPos.x <= targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset <= nowPos.y && nowPos.y <= targetPos.y + offset))
                {
                    tra.anchoredPosition = targetPos;
                    break;
                }
            }
        }


        //========================================================================
        //移動動作(MoveMovement)に要する時間計算
        //========================================================================
        //tra;        動作オブジェクトのRectTransform
        //moveSpeed;  動作速度
        //acceleRate; 加速率(等速移動は1.0f指定)
        //targetPos;  目標座標
        //========================================================================
        public static float GetMoveTime(RectTransform tra, float moveSpeed, float acceleRate, Vector2 targetPos)
        {
            //クローン作製
            RectTransform cloneTra = CloneCreate(tra);

            //時間計算用
            float oneFrameTime = 0.02f;
            float moveTime     = 0.0f;

            float minSpeed = 1.0f;                      //最低速度指定(無限ループ対策)
            float offset   = 0.5f;                      //停止場所のオフセット
            Vector2 nowPos = cloneTra.anchoredPosition; //現在の座標
            bool sideways  = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X方向に動作？
            while (true)
            {
                moveTime  += oneFrameTime;
                moveSpeed *= acceleRate;
                if (0.0f <= moveSpeed && moveSpeed < minSpeed) moveSpeed = minSpeed;
                else if (-minSpeed < moveSpeed && moveSpeed <= 0.0f) moveSpeed = -minSpeed;
                cloneTra.anchoredPosition = Vector2.MoveTowards(cloneTra.anchoredPosition, targetPos, moveSpeed);
                nowPos = cloneTra.anchoredPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && targetPos.x - offset <= nowPos.x && nowPos.x <= targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset <= nowPos.y && nowPos.y <= targetPos.y + offset))
                {
                    cloneTra.anchoredPosition = targetPos;
                    break;
                }
            }

            Destroy(cloneTra.gameObject);  //クローン削除
            return moveTime;               //時間を返す
        }


        //========================================================================
        //往復動作
        //========================================================================
        //tra;        動作オブジェクトのRectTransform
        //shakeSpeed; 動作速度
        //offsetX;    目標座標オフセットX
        //offsetY;    目標座標オフセットY
        //shakeTimes; 移動回数
        //delayTime;  移動間の待機時間
        //========================================================================
        public static IEnumerator SlideShakeMovement(RectTransform tra, float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            float offset       = 0.5f;                 //停止場所のオフセット
            Vector2 defaultPos = tra.anchoredPosition; //初期座標取得
            bool sideways      = Mathf.Abs(offsetX) >= Mathf.Abs(offsetY); //X方向に動作？

            //往復動作
            for (int moveCount = 0; moveCount < shakeTimes; moveCount++)
            {
                int vector = (moveCount % 2 == 0) ? 1 : -1;
                Vector2 tarPos = new Vector2(defaultPos.x + offsetX * vector, defaultPos.y + offsetY * vector);
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, tarPos, shakeSpeed);
                    Vector2 nowPos = tra.anchoredPosition;

                    //---------------------------------------------
                    //次の移動へ
                    //---------------------------------------------
                    if ((sideways && tarPos.x - offset <= nowPos.x && nowPos.x <= tarPos.x + offset) ||
                        (!sideways && tarPos.y - offset <= nowPos.y && nowPos.y <= tarPos.y + offset))
                    {
                        tra.anchoredPosition = tarPos;
                        yield return new WaitForSeconds(delayTime);
                        break;
                    }
                }
            }

            //元の座標に戻る
            while (true)
            {
                yield return new WaitForFixedUpdate();
                tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, defaultPos, shakeSpeed);
                Vector2 nowPos = tra.anchoredPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && defaultPos.x - offset <= nowPos.x && nowPos.x <= defaultPos.x + offset) ||
                    (!sideways && defaultPos.y - offset <= nowPos.y && nowPos.y <= defaultPos.y + offset))
                {
                    tra.anchoredPosition = defaultPos;
                    break;
                }
            }
        }


        //========================================================================
        //往復動作(SlideShakeMovement)に要する時間計算
        //========================================================================
        //tra;        動作オブジェクトのRectTransform
        //shakeSpeed; 動作速度
        //offsetX;    目標座標オフセットX
        //offsetY;    目標座標オフセットY
        //shakeTimes; 移動回数
        //delayTime;  移動間の待機時間
        //========================================================================
        public static float GetSlideShakeTime(RectTransform tra, float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            //クローン作製
            RectTransform cloneTra = CloneCreate(tra);

            //時間計算用
            float oneFrameTime = 0.02f;
            float moveTime     = 0.0f;

            float offset = 0.5f;                 //停止場所のオフセット
            Vector2 defaultPos = cloneTra.anchoredPosition; //初期座標取得
            bool sideways = Mathf.Abs(offsetX) >= Mathf.Abs(offsetY); //X方向に動作？

            //往復動作
            for (int moveCount = 0; moveCount < shakeTimes; moveCount++)
            {
                int vector = (moveCount % 2 == 0) ? 1 : -1;
                Vector2 tarPos = new Vector2(defaultPos.x + offsetX * vector, defaultPos.y + offsetY * vector);
                while (true)
                {
                    moveTime += oneFrameTime;
                    cloneTra.anchoredPosition = Vector2.MoveTowards(cloneTra.anchoredPosition, tarPos, shakeSpeed);
                    Vector2 nowPos = cloneTra.anchoredPosition;

                    //---------------------------------------------
                    //次の移動へ
                    //---------------------------------------------
                    if ((sideways && tarPos.x - offset <= nowPos.x && nowPos.x <= tarPos.x + offset) ||
                        (!sideways && tarPos.y - offset <= nowPos.y && nowPos.y <= tarPos.y + offset))
                    {
                        cloneTra.anchoredPosition = tarPos;
                        moveTime += delayTime;
                        break;
                    }
                }
            }

            //元の座標に戻る
            while (true)
            {
                moveTime += oneFrameTime;
                cloneTra.anchoredPosition = Vector2.MoveTowards(cloneTra.anchoredPosition, defaultPos, shakeSpeed);
                Vector2 nowPos = cloneTra.anchoredPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && defaultPos.x - offset <= nowPos.x && nowPos.x <= defaultPos.x + offset) ||
                    (!sideways && defaultPos.y - offset <= nowPos.y && nowPos.y <= defaultPos.y + offset))
                {
                    cloneTra.anchoredPosition = defaultPos;
                    break;
                }
            }

            Destroy(cloneTra.gameObject);  //クローン削除
            return moveTime;               //時間を返す
        }


        //========================================================================
        //回転動作
        //========================================================================
        //traArray;    動作オブジェクトのRectTransform[]
        //rotSpeed;    拡縮速度
        //stopRot;     回転後の角度(絶対角)
        //========================================================================
        public static IEnumerator RotateMovement(RectTransform[] traArray, Vector3 rotSpeed, Vector3 stopRot)
        {
            //最も多く動作する軸判定
            int axis = 0;
            if (rotSpeed.x < rotSpeed.y)
                axis = (rotSpeed.y > rotSpeed.z) ? 1 : 2;
            else if (rotSpeed.x < rotSpeed.z)
                axis = 2;

            //回転
            float tolerance = 5.0f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                foreach (RectTransform tra in traArray)
                { tra.Rotate(rotSpeed.x, rotSpeed.y, rotSpeed.z); }
                Vector3 nowRot   = traArray[0].localEulerAngles;
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
            }

            //最終角度に合わせる
            foreach (RectTransform tra in traArray)
            { tra.localRotation = Quaternion.Euler(stopRot.x, stopRot.y, stopRot.z); }
        }


        //========================================================================
        //回転動作(RotateMovement)に要する時間計算
        //========================================================================
        //tra;         動作オブジェクトのRectTransform
        //rotSpeed;    拡縮速度
        //stopRot;     回転後の角度(絶対角)
        //========================================================================
        public static float GetRotateMoveTime(RectTransform tra, Vector3 rotSpeed, Vector3 stopRot)
        {
            //クローン作製
            RectTransform cloneTra = CloneCreate(tra);

            //時間計算用
            float oneFrameTime = 0.02f;
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
                moveTime += oneFrameTime;
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


        //========================================================================
        //拡大縮小動作
        //========================================================================
        //tra;          動作オブジェクトのRectTransform
        //scalingSpeed; 拡縮速度
        //changeScale;  変更時の拡大率
        //endScale;     終了時の拡大率
        //scalingTimes; 拡縮回数(配列1周で1カウント、-1指定で無限再生)
        //========================================================================
        public static IEnumerator ScaleChange(RectTransform tra, Vector3 scalingSpeed, float changeScale, float endScale, int scalingTimes)
        {
            Vector3 nowScale = tra.localScale;          //現在の拡大率
            float judgeAxis  = 0.0f;                    //判定軸
            bool scaleChange = true;                    //変更動作中？

            //最も多く拡縮する軸判定
            int axis    = 0;
            float mathX = Mathf.Abs(scalingSpeed.x);
            float mathY = Mathf.Abs(scalingSpeed.y);
            float mathZ = Mathf.Abs(scalingSpeed.z);
            if (mathX < mathY)
                axis = (mathY > mathZ) ? 1 : 2;
            else if (mathX < mathZ)
                axis = 2;

            //拡大判定
            switch (axis)
            {
                case 0: judgeAxis = nowScale.x; break;
                case 1: judgeAxis = nowScale.y; break;
                case 2: judgeAxis = nowScale.z; break;
            }
            bool scaleUp = changeScale > judgeAxis;

            bool infinite = scalingTimes < 0;
            int loopTimes = 0;
            while (infinite || loopTimes < scalingTimes)
            {
                while (true)
                {
                    yield return new WaitForFixedUpdate();

                    //拡縮率更新
                    tra.localScale = nowScale;

                    //判定基準軸指定
                    switch (axis)
                    {
                        case 0: judgeAxis = nowScale.x; break;
                        case 1: judgeAxis = nowScale.y; break;
                        case 2: judgeAxis = nowScale.z; break;
                    }

                    if (scaleChange)
                    {
                        //変更動作
                        if ((scaleUp && judgeAxis >= changeScale) || (!scaleUp && judgeAxis <= changeScale)) scaleChange = false;
                        nowScale = new Vector3(nowScale.x + scalingSpeed.x, nowScale.y + scalingSpeed.y, nowScale.z + scalingSpeed.z);
                    }
                    else
                    {
                        //終了動作
                        if ((scaleUp && judgeAxis <= endScale) || (!scaleUp && judgeAxis >= endScale)) break;
                        nowScale = new Vector3(nowScale.x - scalingSpeed.x, nowScale.y - scalingSpeed.y, nowScale.z - scalingSpeed.z);
                    }
                }

                //変数リセット
                tra.localScale = Vector3.one * endScale;
                scaleChange = true;

                //無限再生終了判定
                if (infinite && SCALING_INFINITE_END)
                {
                    SCALING_INFINITE_END = false;
                    break;
                }
                loopTimes++;
            }
        }


        //========================================================================
        //拡大縮小動作(ScaleChange)に要する時間計算
        //========================================================================
        //tra;          動作オブジェクトのRectTransform
        //scalingSpeed; 拡縮速度
        //changeScale;  変更時の拡大率
        //endScale;     終了時の拡大率
        //scalingTimes; 拡縮回数(※0未満の場合は戻り値0.0f)
        //========================================================================
        public static float GetScaleChangeTime(RectTransform tra, Vector3 scalingSpeed, float changeScale, float endScale, int scalingTimes)
        {
            //クローン作製
            RectTransform cloneTra = CloneCreate(tra);

            //時間計算用
            float oneFrameTime = 0.02f;
            float moveTime     = 0.0f;

            Vector3 nowScale = cloneTra.localScale;     //現在の拡大率
            float judgeAxis  = 0.0f;                    //判定軸
            bool scaleChange = true;                    //変更動作中？
            bool scaleUp     = changeScale > endScale;  //拡大？

            //最も多く拡縮する軸判定
            int axis = 0;
            if (scaleUp && (scalingSpeed.x < scalingSpeed.y) || !scaleUp && (scalingSpeed.x > scalingSpeed.y))
                axis = (scaleUp && (scalingSpeed.y > scalingSpeed.z) || !scaleUp && (scalingSpeed.y < scalingSpeed.z)) ? 1 : 2;
            else if (scaleUp && (scalingSpeed.x < scalingSpeed.z) || !scaleUp && (scalingSpeed.x > scalingSpeed.z))
                axis = 2;

            for (int loopTimes = 0; loopTimes < scalingTimes; loopTimes++)
            {
                while (true)
                {
                    moveTime += oneFrameTime;

                    //拡縮率更新
                    cloneTra.localScale = nowScale;

                    //判定基準軸指定
                    switch (axis)
                    {
                        case 0: judgeAxis = nowScale.x; break;
                        case 1: judgeAxis = nowScale.y; break;
                        case 2: judgeAxis = nowScale.z; break;
                    }

                    if (scaleChange)
                    {
                        //変更動作
                        if ((scaleUp && judgeAxis >= changeScale) || (!scaleUp && judgeAxis <= changeScale)) scaleChange = false;
                        nowScale = new Vector3(nowScale.x + scalingSpeed.x, nowScale.y + scalingSpeed.y, nowScale.z + scalingSpeed.z);
                    }
                    else
                    {
                        //終了動作
                        if ((scaleUp && judgeAxis <= endScale) || (!scaleUp && judgeAxis >= endScale)) break;
                        nowScale = new Vector3(nowScale.x - scalingSpeed.x, nowScale.y - scalingSpeed.y, nowScale.z - scalingSpeed.z);
                    }
                }

                //変数リセット
                cloneTra.localScale = Vector3.one * endScale;
                scaleChange = true;
            }

            Destroy(cloneTra.gameObject);  //クローン削除
            return moveTime;               //時間を返す
        }


        //========================================================================
        //色変更動作(Image.color)
        //========================================================================
        //ima;          変更対象Image
        //changeSpeed;  変更速度
        //colArray;     変更色の配列(0:現在の色)
        //compArray;    比較番号指定配列(0:R 1:G 2:B 3:A)
        //chengeCount;  ループ回数(配列1周で1カウント、-1指定で無限再生)
        //========================================================================
        public static IEnumerator ImagePaletteChange(Image ima, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            float oneFrameTime = 0.02f;            //1フレーム時間
            int loopTimes      = 0;                //繰り返し回数
            int colCount       = colArray.Length;  //変更色の数
            bool infinite      = chengeCount < 0;  //無限ループ？

            int nowIndex  = 0;    //現在の色
            int nextIndex = 1;    //次の色
            float nextCompCol = colArray[nextIndex][compArray[nowIndex]];   //比較色指定
            float judgeRange  = 5.0f / 255.0f;                              //判定範囲

            ima.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                ima.color = Color.Lerp(ima.color, colArray[nextIndex], changeSpeed);
                float nowCompCol = ima.color[compArray[nowIndex]];
                if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                {
                    nowIndex    = nextIndex;
                    nextIndex   = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                    nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                    loopTimes++;

                    //無限ループ終了判定
                    if (infinite && COLOR_CHANGE_INFINITE_END && nowIndex == 0)
                    {
                        COLOR_CHANGE_INFINITE_END = false;
                        break;
                    }
                }
                yield return new WaitForSecondsRealtime(oneFrameTime);
            }
            ima.color = colArray[nowIndex];
        }

        //========================================================================
        //色変更動作(ImagePaletteChange) に要する時間計算
        //========================================================================
        //ima;          変更対象Image
        //changeSpeed;  変更速度
        //colArray;     変更色の配列(0:現在の色)
        //compArray;    比較番号指定配列(0:R 1:G 2:B 3:A)
        //chengeCount;  ループ回数(配列1周で1カウント、-1指定で無限再生)
        //========================================================================
        public static float GetImagePaletteChangeTime(Image ima, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            //クローン作製
            Image cloneIma = CloneCreate_Image(ima);
            float moveTime = 0.0f;

            float oneFrameTime = 0.02f;            //1フレーム時間
            int loopTimes      = 0;                //繰り返し回数
            int colCount       = colArray.Length;  //変更色の数
            bool infinite      = chengeCount < 0;  //無限ループ？

            int nowIndex       = 0;    //現在の色
            int nextIndex      = 1;    //次の色
            float nextCompCol  = colArray[nextIndex][compArray[nowIndex]];   //比較色指定
            float judgeRange   = 5.0f / 255.0f;                              //判定範囲

            cloneIma.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                cloneIma.color = Color.Lerp(cloneIma.color, colArray[nextIndex], changeSpeed);
                float nowCompCol = cloneIma.color[compArray[nowIndex]];
                if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                {
                    nowIndex    = nextIndex;
                    nextIndex   = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                    nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                    loopTimes++;

                    //無限ループ終了判定
                    if (infinite && COLOR_CHANGE_INFINITE_END && nowIndex == 0)
                    {
                        COLOR_CHANGE_INFINITE_END = false;
                        break;
                    }
                }

                //時間計算
                moveTime += oneFrameTime;
            }
            cloneIma.color = colArray[nowIndex];

            Destroy(cloneIma.gameObject); //クローン削除
            return moveTime;              //時間を返す
        }


        //========================================================================
        //色変更動作(Text.color)
        //========================================================================
        //tex;          変更対象Text
        //changeSpeed;  変更速度
        //colArray;     変更色の配列(0:現在の色)
        //compArray;    比較番号指定配列(0:R 1:G 2:B 3:A)
        //chengeCount;  ループ回数(一色変更で1カウント、-1指定で無限再生)
        //========================================================================
        public static IEnumerator TextPaletteChange(Text tex, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            float oneFrameTime = 0.02f;            //1フレーム時間
            int loopTimes      = 0;                //繰り返し回数
            int colCount       = colArray.Length;  //変更色の数
            bool infinite      = chengeCount < 0;  //無限ループ？

            int nowIndex       = 0;    //現在の色
            int nextIndex      = 1;    //次の色
            float nextCompCol  = colArray[nextIndex][compArray[nowIndex]];   //比較色指定
            float judgeRange   = 5.0f / 255.0f;                              //判定範囲

            tex.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                tex.color = Color.Lerp(tex.color, colArray[nextIndex], changeSpeed);
                float nowCompCol = tex.color[compArray[nowIndex]];
                if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                {
                    nowIndex    = nextIndex;
                    nextIndex   = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                    nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                    loopTimes++;

                    //無限ループ終了判定
                    if (infinite && COLOR_CHANGE_INFINITE_END && nowIndex == 0)
                    {
                        COLOR_CHANGE_INFINITE_END = false;
                        break;
                    }
                }
                yield return new WaitForSecondsRealtime(oneFrameTime);
            }
            tex.color = colArray[nowIndex];
        }

        //========================================================================
        //色変更動作(TextPaletteChange) に要する時間計算
        //========================================================================
        //tex;          変更対象Text
        //changeSpeed;  変更速度
        //colArray;     変更色の配列(0:現在の色)
        //compArray;    比較番号指定配列(0:R 1:G 2:B 3:A)
        //chengeCount;  ループ回数(一色変更で1カウント、-1指定で無限再生)
        //========================================================================
        public static float GetTextPaletteChangeTime(Text tex, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            //クローン作成
            Text cloneTex = CloneCreate_Text(tex);
            float moveTime = 0.0f;

            float oneFrameTime = 0.02f;            //1フレーム時間
            int loopTimes      = 0;                //繰り返し回数
            int colCount       = colArray.Length;  //変更色の数
            bool infinite      = chengeCount < 0;  //無限ループ？

            int nowIndex       = 0;    //現在の色
            int nextIndex      = 1;    //次の色
            float nextCompCol  = colArray[nextIndex][compArray[nowIndex]];   //比較色指定
            float judgeRange   = 5.0f / 255.0f;                              //判定範囲

            cloneTex.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                cloneTex.color   = Color.Lerp(cloneTex.color, colArray[nextIndex], changeSpeed);
                float nowCompCol = cloneTex.color[compArray[nowIndex]];
                if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                {
                    nowIndex    = nextIndex;
                    nextIndex   = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                    nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                    loopTimes++;

                    //無限ループ終了判定
                    if (infinite && COLOR_CHANGE_INFINITE_END && nowIndex == 0)
                    {
                        COLOR_CHANGE_INFINITE_END = false;
                        break;
                    }
                }

                //時間計算
                moveTime += oneFrameTime;
            }
            cloneTex.color = colArray[nowIndex];

            Destroy(cloneTex.gameObject);  //クローン削除
            return moveTime;               //時間を返す
        }


        //========================================================================
        //クローンをオリジナルと同座標にセット
        //========================================================================
        //tra;  　動作オブジェクトのRectTransform
        //========================================================================
        public static RectTransform CloneCreate(RectTransform tra)
        {
            GameObject clone       = GameObject.Instantiate(tra.gameObject) as GameObject;
            RectTransform cloneTra = clone.GetComponent<RectTransform>();
            Transform parentTra    = tra.parent.gameObject.transform;
            cloneTra.SetParent(parentTra, false);
            cloneTra.localRotation = tra.localRotation;
            return cloneTra;
        }
        //========================================================================
        //ima;  　動作オブジェクトのImage
        //========================================================================
        public static Image CloneCreate_Image(Image ima)
        {
            GameObject clone = GameObject.Instantiate(ima.gameObject) as GameObject;
            Image cloneIma   = clone.GetComponent<Image>();
            cloneIma.color   = ima.color;
            return cloneIma;
        }
        //========================================================================
        //tex;  　動作オブジェクトのText
        //========================================================================
        public static Text CloneCreate_Text(Text tex)
        {
            GameObject clone = GameObject.Instantiate(tex.gameObject) as GameObject;
            Text cloneTex    = clone.GetComponent<Text>();
            cloneTex.color   = tex.color;
            return cloneTex;
        }
    }
}