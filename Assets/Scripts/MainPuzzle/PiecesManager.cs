using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static GameManager;
using static PuzzleDefine;
using static ObjectMove_2D.ObjectMove_2D;

namespace PuzzleMain
{
    public class PiecesManager : MonoBehaviour
    {
        [Header("GimmicksManagerの取得")]
        [SerializeField]
        GimmicksManager gimmicksMan;

        [Header("駒プレハブの取得")]
        [SerializeField]
        GameObject[] piecePrefabArr;

        [Header("ギミックプレハブの取得")]
        public gimmickArr[] gimmickPrefabArr;
        [System.Serializable]
        public class gimmickArr
        { public GameObject[] prefab; }

        [Header("リバーシ盤の取得")]
        [SerializeField]
        Transform reversiBoardTra;

        [Header("待機駒ボックスの取得")]
        [SerializeField]
        Transform nextPieceBoxesTra;

        GameObject[] pieceObjArr;               //駒オブジェクト配列
        Transform[]  pieceTraArr;               //駒Transform配列
        GameObject[] squareObjArr;              //マスオブジェクト配列
        public static Transform[] squareTraArr; //マスTransform配列
        SpriteRenderer[] squareSpriRenArr;      //マスSpriteRenderer配列
        GameObject[] nextPieceObjArr;           //待機駒オブジェクト配列
        Transform[]  nextPieceTraArr;           //待機駒Transform配列
        GameObject[] nextPieceBoxObjArr;        //待機駒箱オブジェクト配列
        Transform[]  nextPieceBoxTraArr;        //待機駒箱Transform配列
        Transform    nextPieceFrameTra;         //次に置くコマの指定フレーム
        GameObject[] gimmickObjArr;             //ギミックオブジェクト配列
        int nextPuPieceIndex = 0;               //次に置く駒の管理番号
        int squaresCount;                       //マスの個数
        int nextPiecesCount;                    //待機駒の個数
        int[] directionsIntArr;                 //8方向の管理番号配列

        public static PieceInformation[] pieceInfoArr;                          //駒の情報配列
        public static GimmickInformation[] gimmickInfoArr;                      //ギミックの情報配列
        public static List<int> destroyPiecesIndexList = new List<int>();       //削除駒の管理番号リスト
        public static List<Coroutine> gimmickCorList = new List<Coroutine>();   //動作中ギミックリスト

        //==========================================================//
        //----------------------初期設定,取得-----------------------//
        //==========================================================//

        void Start()
        {
            //8方向の管理番号取得
            System.Array directions = Enum.GetValues(typeof(Directions));
            directionsIntArr = new int[directions.Length];
            foreach (Directions direction in directions)
            { directionsIntArr[(int)direction] = (int)direction; }

            //マス取得
            squaresCount = BOARD_COLUMN_COUNT * BOARD_LINE_COUNT;
            pieceTraArr  = new Transform[squaresCount];
            pieceObjArr  = new GameObject[squaresCount];
            pieceInfoArr = new PieceInformation[squaresCount];
            squareObjArr = new GameObject[squaresCount];
            squareTraArr = new Transform[squaresCount];
            squareSpriRenArr = new SpriteRenderer[squaresCount];
            for (int i = 0; i < squaresCount; i++)
            {
                squareObjArr[i] = reversiBoardTra.GetChild(i).gameObject;
                squareTraArr[i] = squareObjArr[i].transform;
                squareSpriRenArr[i] = squareObjArr[i].GetComponent<SpriteRenderer>();
            }

            //使用しないマスを非表示
            foreach (int i in HIDE_SQUARE_ARR)
            { squareObjArr[i].SetActive(false); }

            //ギミックを配置
            gimmickObjArr  = new GameObject[GIMMICKS_COUNT];
            gimmickInfoArr = new GimmickInformation[GIMMICKS_COUNT];
            List<int> notPlaceIndex = new List<int>();  //駒を配置しないマス番号
            bool notSquare = false;                     //駒として管理しないギミックがある
            for (int i = 0; i < GIMMICKS_COUNT; i++)
            {
                //駒として管理するギミック
                if (GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[i][GIMMICK]].in_square)
                {
                    GeneraeGimmick(i);
                    notPlaceIndex.Add(GIMMICKS_INFO_ARR[i][SQUARE]);
                }
                else notSquare = true;
            }
            //駒として配置しないギミック
            if (notSquare) gimmicksMan.PlaceGimmickNotInSquare();

            //駒のランダム配置
            for (int i = 0; i < squaresCount; i++)
            {
                if (!squareObjArr[i].activeSelf) continue; //非表示マスは処理を飛ばす
                if (notPlaceIndex.Contains(i))   continue; //ギミックマスは処理を飛ばす

                int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
                GeneratePiece(pieceGeneIndex, i, true);
            }

            //待機駒の箱取得
            nextPiecesCount    = nextPieceBoxesTra.childCount;
            nextPieceBoxObjArr = new GameObject[nextPiecesCount];
            nextPieceBoxTraArr = new Transform[nextPiecesCount];
            for (int i = 0; i < nextPiecesCount; i++)
            {
                nextPieceBoxObjArr[i] = nextPieceBoxesTra.GetChild(i).gameObject;
                nextPieceBoxTraArr[i] = nextPieceBoxObjArr[i].transform;

            }

            //次に置くコマの指定フレーム取得
            nextPieceFrameTra = nextPieceBoxTraArr[0].GetChild(0).gameObject.transform;

            //待機駒生成
            nextPieceObjArr = new GameObject[nextPiecesCount];
            nextPieceTraArr = new Transform[nextPiecesCount];
            for (int i = 0; i < nextPiecesCount; i++)
            {
                int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
                GenerateNextPiece(pieceGeneIndex, i);
            }
        }

        //==========================================================//


        //==========================================================//
        //-----------------------汎用動作---------------------------//
        //==========================================================//

        /// <summary>
        /// 駒作成
        /// </summary>
        /// /// <param name="prefabIndex">   生成駒プレハブ番号</param>
        /// /// <param name="pieceIndex">    駒管理番号</param>
        /// /// <param name="startGenerate"> 初期生成</param>
        void GeneratePiece(int prefabIndex, int pieceIndex, bool startGenerate = false)
        {
            pieceObjArr[pieceIndex]  = Instantiate(piecePrefabArr[prefabIndex]);
            pieceInfoArr[pieceIndex] = pieceObjArr[pieceIndex].GetComponent<PieceInformation>();
            pieceInfoArr[pieceIndex].InformationSetting(pieceIndex, startGenerate, gimmickInfoArr);
            pieceTraArr[pieceIndex]  = pieceInfoArr[pieceIndex].tra;
            pieceTraArr[pieceIndex].SetParent(squareTraArr[pieceIndex], false);
            pieceTraArr[pieceIndex].SetSiblingIndex(0);
        }

        /// <summary>
        /// 待機駒作成
        /// </summary>
        /// /// <param name="prefabIndex">生成駒プレハブ番号</param>
        /// /// <param name="pieceIndex"> 待機駒管理番号</param>
        void GenerateNextPiece(int prefabIndex, int pieceIndex)
        {
            nextPieceObjArr[pieceIndex] = Instantiate(piecePrefabArr[prefabIndex]);
            nextPieceTraArr[pieceIndex] = nextPieceObjArr[pieceIndex].transform;
            nextPieceTraArr[pieceIndex].SetParent(nextPieceBoxTraArr[pieceIndex], false);
        }

        /// <summary>
        /// 駒削除
        /// </summary>
        /// /// <param name="pieceIndex">削除駒の管理番号</param>
        void DeletePiece(int pieceIndex)
        {
            Destroy(pieceObjArr[pieceIndex]);
            pieceObjArr[pieceIndex]  = null;
            pieceTraArr[pieceIndex]  = null;
            pieceInfoArr[pieceIndex] = null;
        }

        /// <summary>
        /// 待機駒を盤面に置く
        /// </summary>
        /// <param name="squareId">配置マス管理番号</param>
        void PutPiece(int squareId)
        {
            //盤面の駒削除,管理配列差し替え
            DeletePiece(squareId);
            pieceObjArr[squareId]  = nextPieceObjArr[nextPuPieceIndex];
            pieceTraArr[squareId]  = nextPieceTraArr[nextPuPieceIndex];
            pieceInfoArr[squareId] = pieceObjArr[squareId].GetComponent<PieceInformation>();
            pieceInfoArr[squareId].InformationSetting(squareId, false);

            //待機駒生成
            int pieceGeneIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
            GenerateNextPiece(pieceGeneIndex, nextPuPieceIndex);
        }

        /// <summary>
        /// ギミック作成
        /// </summary>
        /// /// <param name="gimmickIndex">ギミック管理番号</param>
        void GeneraeGimmick(int gimmickIndex)
        {
            int colorId = (GIMMICKS_INFO_ARR[gimmickIndex][COLOR] < 0) ? 0 : GIMMICKS_INFO_ARR[gimmickIndex][COLOR];
            gimmickObjArr[gimmickIndex] = Instantiate(gimmickPrefabArr[GIMMICKS_INFO_ARR[gimmickIndex][GIMMICK]].prefab[colorId]);

            //Component取得
            gimmickInfoArr[gimmickIndex] = gimmickObjArr[gimmickIndex].GetComponent<GimmickInformation>();
            gimmickInfoArr[gimmickIndex].InformationSetting(gimmickIndex);

            //駒としても管理する
            int pieceIndex = GIMMICKS_INFO_ARR[gimmickIndex][SQUARE];
            pieceObjArr[pieceIndex] = gimmickObjArr[gimmickIndex];
            pieceTraArr[pieceIndex] = gimmickInfoArr[gimmickIndex].tra;
            pieceTraArr[pieceIndex].SetParent(squareTraArr[GIMMICKS_INFO_ARR[gimmickIndex][SQUARE]], false);
            pieceTraArr[pieceIndex].SetSiblingIndex(0);
            pieceTraArr[pieceIndex].localPosition = gimmickInfoArr[gimmickIndex].defaultPos;
        }

        /// <summary>
        /// マスとして管理しないギミック配置
        /// </summary>
        /// <param name="gimmickObj">配置オブジェクト</param>
        /// <param name="squareId">  配置マス管理番号</param>
        public void PlaceGimmick(GameObject gimmickObj, int squareId)
        {
            gimmickObj.transform.SetParent(squareTraArr[squareId], false);
        }

        /// <summary>
        /// ギミック削除
        /// </summary>
        /// /// <param name="pieceIndex">削除駒の管理番号</param>
        void DeleteGimmick(GameObject gimmickObj)
        {
            int gimmickIndex = Array.IndexOf(gimmickObjArr, gimmickObj);
            gimmickObjArr[gimmickIndex] = null;

            DeletePiece(Array.IndexOf(pieceObjArr, gimmickObj));
        }


        /// <summary>
        /// 管理番号の更新
        /// </summary>
        /// <param name="oldIndex">更新前の管理番号</param>
        /// <param name="newIndex">更新後の管理番号</param>
        void UpdateManagementIndex(int oldIndex, int newIndex)
        {
            pieceObjArr[newIndex]  = pieceObjArr[oldIndex];
            pieceTraArr[newIndex]  = pieceTraArr[oldIndex];
            pieceInfoArr[newIndex] = pieceInfoArr[oldIndex];
            pieceTraArr[newIndex].SetParent(squareTraArr[newIndex], true);
            pieceTraArr[newIndex].SetSiblingIndex(0);
            pieceObjArr[oldIndex]  = null;
            pieceTraArr[oldIndex]  = null;
            pieceInfoArr[oldIndex] = null;
            if (pieceInfoArr[newIndex] != null) pieceInfoArr[newIndex].squareId = newIndex;
        }

        /// <summary>
        /// マスの色変更
        /// </summary>
        /// <param name="afterColor"> 変化後の色</param>
        /// <param name="squareId">   マス管理番号</param>
        /// <param name="fade">       フェードの有無</param>
        public IEnumerator SquareColorChange(Color afterColor, int squareId, bool fade)
        {
            if (!fade) squareSpriRenArr[squareId].color = afterColor;
            else yield return StartCoroutine(SpriteRendererPaletteChange(squareSpriRenArr[squareId], SQUARE_CHANGE_SPEED, new Color[] { squareSpriRenArr[squareId].color, afterColor }));
        }

        /// <summary>
        /// 駒の操作可能フラグ切替
        /// </summary>
        /// <param name="squareId">マス管理番号</param>
        /// <param name="on">      true:操作可能にする</param>
        public void PieceOperationFlagChange(int squareId, bool on)
        {
            if (on) pieceInfoArr[squareId].OperationFlagON();
            else    pieceInfoArr[squareId].OperationFlagOFF();
        }

        //==========================================================//



        //==========================================================//
        //-----------------------駒配置動作-------------------------//
        //==========================================================//

        /// <summary>
        /// オブジェクトがタップされた
        /// </summary>
        /// <param name="tapObj"></param>
        public void TapObject(GameObject tapObj)
        {
            //ギミックには直置きできない
            if (tapObj.tag == GIMMICK_TAG) return;

            //盤上の駒の場合
            int pieceObjIndex = Array.IndexOf(pieceObjArr, tapObj);
            if (pieceObjIndex >= 0)
            {
                //反転禁止フラグ確認
                if (!pieceInfoArr[pieceObjIndex].invertable) return;
                StartCoroutine(PutPieceToSquare(tapObj));
            }

            //待機駒の場合
            if (Array.IndexOf(nextPieceObjArr, tapObj) >= 0) MoveNextPieceFrame(tapObj);
        }

        /// <summary>
        /// マスに駒を置く
        /// </summary>
        /// <param name="deletePiece">削除駒</param>
        IEnumerator PutPieceToSquare(GameObject deletePiece)
        {
            //配置中フラグセット
            NOW_PUTTING_PIECES = true;

            //削除する駒のマスに指定中の待機駒をセットする
            int putIndex = Array.IndexOf(pieceObjArr, deletePiece);
            nextPieceTraArr[nextPuPieceIndex].SetParent(squareTraArr[putIndex], true);
            nextPieceTraArr[nextPuPieceIndex].SetSiblingIndex(0);

            //駒拡大
            Coroutine scaleUpCoroutine = StartCoroutine(AllScaleChange(nextPieceTraArr[nextPuPieceIndex], PUT_PIECE_SCALING_SPEED, PUT_PIECE_CHANGE_SCALE));

            //待機駒の移動
            Vector3 nowPos = nextPieceTraArr[nextPuPieceIndex].localPosition;
            nextPieceTraArr[nextPuPieceIndex].localPosition = new Vector3(nowPos.x, nowPos.y, PUT_PIECE_MOVE_START_Z);
            yield return StartCoroutine(DecelerationMovement(nextPieceTraArr[nextPuPieceIndex], PUT_PIECE_MOVE_SPEED, PIECE_DEFAULT_POS));

            //待機駒を置く
            PutPiece(putIndex);

            //90°回転
            nextPieceTraArr[nextPuPieceIndex].localRotation = PIECE_GENERATE_QUA;
            StartCoroutine(RotateMovement(nextPieceTraArr[nextPuPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_FRONT_ROT));

            //駒縮小
            StopCoroutine(scaleUpCoroutine);
            yield return StartCoroutine(AllScaleChange(pieceTraArr[putIndex], PUT_PIECE_SCALING_SPEED, PIECE_DEFAULT_SCALE));
            pieceTraArr[putIndex].localScale = new Vector2(PIECE_DEFAULT_SCALE, PIECE_DEFAULT_SCALE);

            //反転駒リスト取得
            int colorId = pieceInfoArr[putIndex].colorId;       //置いた駒の色番号取得
            List<int[]> reversIndexList = new List<int[]>();    //反転駒の管理番号格納リスト
            if (GetReversIndex(putIndex, ref colorId, ref reversIndexList))
            {
                //削除対象に置いた駒の管理番号追加
                destroyPiecesIndexList.Add(putIndex);

                //反転開始
                StartCoroutine(StratReversingPieces(colorId, reversIndexList));
            }
            else
            {
                //ターン終了処理開始
                StartCoroutine(TurnEnd());
            }

            //配置中フラグリセット
            NOW_PUTTING_PIECES = false;
        }

        /// <summary>
        /// 駒の反転判定
        /// </summary>
        /// <param name="putPieceIndex">  置いた駒の管理番号</param>
        /// <param name="putPieceColorId">置いた駒の色番号</param>
        /// <param name="reversIndexList">反転駒格納リスト(参照)</param>
        /// <returns>反転駒の有無</returns>
        bool GetReversIndex(int putPieceIndex, ref int putPieceColorId, ref List<int[]> reversIndexList)
        {
            //同タグ番号取得
            int squaresCount_Up    = putPieceIndex % BOARD_LINE_COUNT;            //置いた駒の上にあるマスの数
            int squaresCount_Left  = putPieceIndex / BOARD_LINE_COUNT;            //置いた駒の左にあるマスの数
            int squaresCount_Down  = BOARD_LINE_COUNT - squaresCount_Up - 1;      //置いた駒の下にあるマスの数
            int squaresCount_Right = BOARD_COLUMN_COUNT - squaresCount_Left - 1;  //置いた駒の右にあるマスの数

            //方向ごとに反転駒の番号取得
            foreach (int directionsInt in directionsIntArr)
            {
                int loopCount = SetLoopCount(directionsInt, ref squaresCount_Up, ref squaresCount_Right, ref squaresCount_Down, ref squaresCount_Left);
                if (loopCount == 0) continue;
                reversIndexList.Add(GetReversIndex_SpecifiedDirection(ref putPieceIndex, ref putPieceColorId, ref loopCount, directionsInt));
            }

            //反転駒の有無を返す
            reversIndexList.RemoveAll(item => item == null);
            return reversIndexList.Count > 0;
        }

        /// <summary>
        /// 処置回数の設定
        /// </summary>
        /// <param name="directions">方向の管理番号</param>
        /// <param name="up">        上にあるマスの数</param>
        /// <param name="right">     右にあるマスの数</param>
        /// <param name="down">      下にあるマスの数</param>
        /// <param name="left">      左にあるマスの数</param>
        /// <returns>処理回数</returns>
        int SetLoopCount(int directions, ref int up, ref int right, ref int down, ref int left)
        {
            switch (directions)
            {
                case (int)Directions.Up:        return up;                              //上
                case (int)Directions.UpRight:   return (up <= right) ? up : right;      //右上
                case (int)Directions.Right:     return right;                           //右
                case (int)Directions.DownRight: return (down <= right) ? down : right;  //右下
                case (int)Directions.Down:      return down;                            //下
                case (int)Directions.DownLeft:  return (down <= left) ? down : left;    //左下
                case (int)Directions.Left:      return left;                            //左
                case (int)Directions.UpLeft:    return (up <= left) ? up : left;        //左上
                default: return 0;
            }
        }

        /// <summary>
        /// 指定方向の反転オブジェクトの管理番号取得
        /// </summary>
        /// <param name="putPieceIndex">  基準駒の管理番号</param>
        /// <param name="putPieceColorId">基準駒の色番号</param>
        /// <param name="loopCount">      指定方向にあるマスの数</param>
        /// <param name="direction">      指定方向の管理番号</param>
        /// <returns>指定方向の反転オブジェクトの管理番号配列</returns>
        int[] GetReversIndex_SpecifiedDirection(ref int putPieceIndex, ref int putPieceColorId, ref int loopCount, int direction)
        {
            List<int> reversIndexList = new List<int>();
            for (int i = 1; i <= loopCount; i++)
            {
                //指定方向のインデックス番号取得
                int refIndex = GetPlaceIndex(ref direction, ref putPieceIndex, ref i);

                //空マスの場合はnullを返す
                if (pieceObjArr[refIndex] == null) return null;

                //ギミックマスの場合
                if (pieceObjArr[refIndex].tag == GIMMICK_TAG)
                {
                    //ダメージを与えられるかの確認,ダメージが与えられない場合はnullを返す
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceObjArr[refIndex]);
                    if (!gimmicksMan.DamageCheck(ref putPieceColorId, ref gimmickIndex))
                        return null;
                    reversIndexList.Add(refIndex);
                }
                else
                {
                    //反転禁止駒の場合はnullを返す
                    if (!pieceInfoArr[refIndex].invertable) return null;

                    //同色駒を検索
                    if (pieceInfoArr[refIndex].colorId == putPieceColorId)
                    {
                        //隣が同タグの場合はnullを返す
                        if (i == 1) return null;

                        //削除対象に同タグ駒の管理番号追加
                        destroyPiecesIndexList.Add(refIndex);
                        break;
                    }
                    reversIndexList.Add(refIndex);
                }

                //最後まで同タグ駒がない場合はnullを返す
                if (i == loopCount) return null;
            }
            return reversIndexList.ToArray();
        }

        /// <summary>
        /// 指定場所の管理番号取得
        /// </summary>
        /// <param name="direction">方向の管理番号</param>
        /// <param name="baseIndex">基準オブジェクトの管理番号</param>
        /// <param name="distance"> 距離</param>
        /// <returns>指定場所の管理番号</returns>
        int GetPlaceIndex(ref int direction, ref int baseIndex, ref int distance)
        {
            switch (direction)
            {
                case (int)Directions.Up:        return baseIndex - distance;                                //上
                case (int)Directions.UpRight:   return baseIndex + BOARD_LINE_COUNT * distance - distance;  //右上
                case (int)Directions.Right:     return baseIndex + BOARD_LINE_COUNT * distance;             //右
                case (int)Directions.DownRight: return baseIndex + BOARD_LINE_COUNT * distance + distance;  //右下
                case (int)Directions.Down:      return baseIndex + distance;                                //下
                case (int)Directions.DownLeft:  return baseIndex - BOARD_LINE_COUNT * distance + distance;  //左下
                case (int)Directions.Left:      return baseIndex - BOARD_LINE_COUNT * distance;             //左
                case (int)Directions.UpLeft:    return baseIndex - BOARD_LINE_COUNT * distance - distance;  //左上
                default: return 0;
            }
        }

        //==========================================================//


        //==========================================================//
        //---------------次に置く駒指定フレームの動作---------------//
        //==========================================================//

        /// <summary>
        /// 次投擲駒の指定フレーム移動
        /// </summary>
        /// <param name="tapPieceObj"></param>
        /// <returns></returns>
        void MoveNextPieceFrame(GameObject tapPieceObj)
        {
            //次に置くコマの管理番号更新
            nextPuPieceIndex = Array.IndexOf(nextPieceObjArr, tapPieceObj);

            //移動
            nextPieceFrameTra.SetParent(nextPieceBoxTraArr[nextPuPieceIndex], false);
        }

        //==========================================================//


        //==========================================================//
        //-----------------------駒反転動作-------------------------//
        //==========================================================//

        /// <summary>
        /// 駒反転開始
        /// </summary>
        /// <param name="putPieceColorId">置いた駒の色番号</param>
        /// <param name="reversIndexList">反転駒格納リスト</param>
        /// <returns></returns>
        IEnumerator StratReversingPieces(int putPieceColorId, List<int[]> reversIndexList)
        {
            //反転中フラグセット
            NOW_REVERSING_PIECES = true;

            //反転開始
            Coroutine coroutine = null;
            foreach (int[] reversIndexArr in reversIndexList)
            {
                if (reversIndexArr == null) continue;
                foreach (int reversIndex in reversIndexArr)
                {
                    //ギミックであればギミック破壊動作に移る
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceObjArr[reversIndex]);
                    if (gimmickIndex >= 0)
                    {
                        gimmicksMan.DamageGimmick(ref gimmickIndex, reversIndex);
                        yield return PIECE_REVERSAL_INTERVAL;
                        continue;
                    }

                    //削除対象に反転駒の管理番号追加
                    destroyPiecesIndexList.Add(reversIndex);

                    //反転
                    coroutine = StartCoroutine(ReversingPieces(reversIndex, putPieceColorId));
                    yield return PIECE_REVERSAL_INTERVAL;
                }
                yield return PIECE_GROUP_REVERSAL_INTERVAL;
            }

            //ギミック終了待機
            foreach (Coroutine gimmickCor in gimmickCorList)
            { yield return gimmickCor; }
            gimmickCorList = new List<Coroutine>();

            //反転終了待機
            yield return coroutine;

            //反転駒の破壊
            StartCoroutine(StartDestroyingPieces());

            //反転中フラグリセット
            NOW_REVERSING_PIECES = false;
        }

        /// <summary>
        /// 駒の反転
        /// </summary>
        /// <param name="reversPieceIndex">裏返す駒の管理番号</param>
        /// <param name="prefabIndex">     生成駒プレハブ番号</param>
        IEnumerator ReversingPieces(int reversPieceIndex, int prefabIndex)
        {
            //駒90°回転,拡大
            StartCoroutine(AllScaleChange(pieceTraArr[reversPieceIndex], REVERSE_PIECE_SCALING_SPEED, REVERSE_PIECE_CHANGE_SCALE));
            yield return StartCoroutine(RotateMovement(pieceTraArr[reversPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_SWITCH_ROT));

            //元駒削除,新駒生成
            DeletePiece(reversPieceIndex);
            GeneratePiece(prefabIndex, reversPieceIndex);

            //駒90°回転,縮小
            pieceTraArr[reversPieceIndex].localScale    = new Vector3(REVERSE_PIECE_CHANGE_SCALE, REVERSE_PIECE_CHANGE_SCALE, 0.0f);
            pieceTraArr[reversPieceIndex].localRotation = PIECE_GENERATE_QUA;
            StartCoroutine(AllScaleChange(pieceTraArr[reversPieceIndex], REVERSE_PIECE_SCALING_SPEED, PIECE_DEFAULT_SCALE));
            yield return StartCoroutine(RotateMovement(pieceTraArr[reversPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_FRONT_ROT));
        }

        //==========================================================//



        //==========================================================//
        //-----------------------駒破壊動作-------------------------//
        //==========================================================//

        /// <summary>
        /// 駒破壊開始
        /// </summary>
        IEnumerator StartDestroyingPieces()
        {
            //破壊中フラグセット
            NOW_DESTROYING_PIECES = true;

            //駒縮小
            Coroutine coroutine = null;
            List<Coroutine> gimCorList = new List<Coroutine>();
            foreach (int index in destroyPiecesIndexList)
            {
                if (pieceTraArr[index].gameObject.tag == GIMMICK_TAG) continue;
                coroutine = StartCoroutine(AllScaleChange(pieceTraArr[index], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));

                //駒カウントギミックにダメージ
                gimCorList.Add(StartCoroutine(gimmicksMan.DamageCage(pieceInfoArr[index].colorId)));    //檻
            }
            yield return coroutine;
            foreach (Coroutine cor in gimCorList)
            { yield return cor; }

            //駒削除
            foreach (int pieceIndex in destroyPiecesIndexList)
            {
                GameObject obj = pieceObjArr[pieceIndex];
                if (obj.tag == GIMMICK_TAG) DeleteGimmick(obj);
                else DeletePiece(pieceIndex);
            }

            //削除駒リストの初期化
            destroyPiecesIndexList = new List<int>();

            //駒の落下開始
            StartCoroutine(StratFallingPieces());

            //破壊中フラグリセット
            NOW_DESTROYING_PIECES = false;
        }

        /// <summary>
        /// 駒の落下開始
        /// </summary>
        IEnumerator StratFallingPieces()
        {
            //駒落下中フラグセット
            NOW_FALLING_PIECES = true;

            //全駒管理番号の更新,落下駒リストの取得
            List<int> fallPiecesIndexList = SettingOfFallingPieces();

            //落下開始
            List<Coroutine> coroutineList = new List<Coroutine>();
            foreach (int fallPieceIndex in fallPiecesIndexList)
            {
                if (pieceTraArr[fallPieceIndex] != null)
                {
                    Vector3 targetPos = PIECE_DEFAULT_POS;
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceTraArr[fallPieceIndex].gameObject);
                    if (gimmickIndex >= 0) targetPos = gimmickInfoArr[gimmickIndex].defaultPos;
                    coroutineList.Add(StartCoroutine(ConstantSpeedMovement(pieceTraArr[fallPieceIndex], FALL_PIECE_MOVE_SPEED, targetPos, FALL_PIECE_ACCELE_RATE)));
                }
            }
            foreach (Coroutine coroutine in coroutineList)
            { yield return coroutine; }

            //ターン終了処理開始
            StartCoroutine(TurnEnd());

            //駒落下中フラグリセット
            NOW_FALLING_PIECES = false;
        }

        /// <summary>
        /// 落下駒の設定
        /// </summary>
        /// <returns>落下駒リスト</returns>
        List<int> SettingOfFallingPieces()
        {
            List<int> fallPiecesIndexList = new List<int>();
            for (int i = squaresCount - 1; i >= 0; i--)
            {
                //空マスでなえれば処理をスキップ
                if (pieceObjArr[i] != null) continue;

                //落下する駒リストに追加
                fallPiecesIndexList.Add(i);

                //駒の上にある管理番号をすべて調査
                int loopCount = i % BOARD_LINE_COUNT;
                for (int n = 0; n <= loopCount; n++)
                {
                    //自身よりn個上の番号が空でない場合
                    int refIndex = i - n;
                    if (pieceObjArr[refIndex] != null)
                    {
                        //自由らか可能オブジェクトの場合,管理番号更新
                        if (FreeFallJudgement(ref pieceObjArr[refIndex]))
                            UpdateManagementIndex(i - n, i);
                        break;
                    }

                    //落下できる駒が盤上に存在しない場合
                    if (n == loopCount)
                    {
                        //新規ランダム生成
                        int prefabIndex = UnityEngine.Random.Range(0, USE_PIECE_COUNT);
                        GeneratePiece(prefabIndex, i);
                        pieceTraArr[i].localPosition = FALL_PIECE_GENERATE_POS;
                    }
                }

            }

            return fallPiecesIndexList;
        }

        /// <summary>
        /// 自由落下判定
        /// </summary>
        /// <param name="pieceObj"></param>
        /// <returns>true:自由落下</returns>
        bool FreeFallJudgement(ref GameObject pieceObj)
        {
            if (pieceObj.tag == PIECE_TAG)
            {
                //駒の場合
                int pieceObjIndex = Array.IndexOf(pieceObjArr, pieceObj);
                return pieceInfoArr[pieceObjIndex].freeFall;
            }
            else if (pieceObj.tag == GIMMICK_TAG)
            {
                //ギミックの場合
                int gimmickObjIndex = Array.IndexOf(gimmickObjArr, pieceObj);
                return gimmickInfoArr[gimmickObjIndex].freeFall;
            }

            return false;
        }


        //==========================================================//
        //-------------------ターン終了時の処理---------------------//
        //==========================================================//

        /// <summary>
        /// ギミック破壊確認
        /// </summary>
        IEnumerator GimmickDeleteCheck()
        {
            //ギミックダメージ待機中フラグセット
            NOW_GIMMICK_DAMAGE_WAIT = true;

            //枠のダメージ確認
            yield return StartCoroutine(gimmicksMan.DamageFrame());

            //ギミックダメージ待機中フラグリセット
            NOW_GIMMICK_DAMAGE_WAIT = false;
        }

        /// <summary>
        /// ギミックの状態変化開始
        /// </summary>
        /// <returns></returns>
        IEnumerator StartChangeGimmickState()
        {
            //ギミック状態変化中フラグセット
            NOW_GIMMICK_STATE_CHANGE = true;

            //状態変化待機
            yield return StartCoroutine(gimmicksMan.ChangeGimmickState());

            //ギミック状態変化中フラグリセット
            NOW_GIMMICK_STATE_CHANGE = false;
        }

        /// <summary>
        /// ターン終了
        /// </summary>
        IEnumerator TurnEnd()
        {
            //特定ギミック破壊判定開始
            yield return StartCoroutine(GimmickDeleteCheck());

            //ギミック状態変化開始
            yield return StartCoroutine(StartChangeGimmickState());

            //特定ギミック破壊判定開始
            yield return StartCoroutine(GimmickDeleteCheck());

            //ギミックのフラグリセット
            foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
            {
                if (gimmickInfo == null) continue;
                gimmickInfo.nowTurnDamage = false;
            }
        }
    }
}