using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static GameManager;
using static PuzzleDefine;
using static PuzzleMain.GimmicksManager;
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

        public static GameObject[] pieceObjArr; //駒オブジェクト配列
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
        int nextPiecesCount;                    //待機駒の個数

        public static PieceInformation[] pieceInfoArr;                          //駒の情報配列
        public static GimmickInformation[] gimmickInfoArr;                      //ギミックの情報配列
        public static List<int> destroyPiecesIndexList = new List<int>();       //削除駒の管理番号リスト
        public static List<Coroutine> gimmickCorList = new List<Coroutine>();   //動作中ギミックリスト

        //==========================================================//
        //----------------------初期設定,取得-----------------------//
        //==========================================================//

        void Start()
        {
            //マス取得
            pieceTraArr  = new Transform[SQUARES_COUNT];
            pieceObjArr  = new GameObject[SQUARES_COUNT];
            pieceInfoArr = new PieceInformation[SQUARES_COUNT];
            squareObjArr = new GameObject[SQUARES_COUNT];
            squareTraArr = new Transform[SQUARES_COUNT];
            squareSpriRenArr = new SpriteRenderer[SQUARES_COUNT];
            for (int i = 0; i < SQUARES_COUNT; i++)
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
            gimmicksMan.PlaceGimmickNotInSquare();      //駒として配置しないギミックの生成
            List<int> notPlaceIndex = new List<int>();  //駒を配置しないマス番号
            for (int i = 0; i < GIMMICKS_COUNT; i++)
            {
                //駒として管理するギミック
                if (GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[i][GIMMICK]].in_square)
                {
                    GeneraeGimmick(i);
                    notPlaceIndex.Add(GIMMICKS_INFO_ARR[i][SQUARE]);
                }
            }

            //駒のランダム配置
            for (int i = 0; i < SQUARES_COUNT; i++)
            {
                if (!squareObjArr[i].activeSelf) continue; //非表示マスは処理を飛ばす
                if (notPlaceIndex.Contains(i))   continue; //ギミックマスは処理を飛ばす

                int pieceGeneIndex = GetRandomPieceColor();
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
                int pieceGeneIndex = GetRandomPieceColor();
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
            int pieceGeneIndex = GetRandomPieceColor();
            GenerateNextPiece(pieceGeneIndex, nextPuPieceIndex);
        }

        /// <summary>
        /// ギミック作成(駒として管理する)
        /// </summary>
        /// /// <param name="gimmickIndex"> ギミック管理番号</param>
        /// /// <param name="startGenerate">初期生成？</param>
        void GeneraeGimmick(int gimmickIndex, bool startGenerate = true)
        {
            int colorId = (GIMMICKS_INFO_ARR[gimmickIndex][COLOR] < 0) ? 0 : GIMMICKS_INFO_ARR[gimmickIndex][COLOR];
            gimmickObjArr[gimmickIndex] = Instantiate(gimmickPrefabArr[GIMMICKS_INFO_ARR[gimmickIndex][GIMMICK]].prefab[colorId]);

            //Component取得
            gimmickInfoArr[gimmickIndex] = gimmickObjArr[gimmickIndex].GetComponent<GimmickInformation>();
            gimmickInfoArr[gimmickIndex].InformationSetting(gimmickIndex);

            //番号札の場合
            if (gimmickInfoArr[gimmickIndex].id == (int)Gimmicks.NumberTag)
                gimmicksMan.NumberTagOrderSetting(ref gimmickInfoArr[gimmickIndex]);

            //駒としても管理する
            int pieceIndex = GIMMICKS_INFO_ARR[gimmickIndex][SQUARE];
            pieceObjArr[pieceIndex] = gimmickObjArr[gimmickIndex];
            pieceTraArr[pieceIndex] = gimmickInfoArr[gimmickIndex].tra;
            pieceTraArr[pieceIndex].SetParent(squareTraArr[GIMMICKS_INFO_ARR[gimmickIndex][SQUARE]], false);
            pieceTraArr[pieceIndex].SetSiblingIndex(0);
            pieceTraArr[pieceIndex].localPosition = gimmickInfoArr[gimmickIndex].defaultPos;
            gimmickInfoArr[gimmickIndex].OperationFlagSetting(pieceIndex, startGenerate, gimmickInfoArr);
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
        public void DeleteGimmick(GameObject gimmickObj)
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

            int gimIndex = Array.IndexOf(gimmickObjArr, pieceObjArr[newIndex]);
            if (gimIndex >= 0) gimmickInfoArr[gimIndex].nowSquareId = newIndex;
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
            //駒
            if (pieceObjArr[squareId].tag == PIECE_TAG)
            {
                if (on) pieceInfoArr[squareId].OperationFlagON();
                else pieceInfoArr[squareId].OperationFlagOFF();
            }
            //ギミック
            else
            {
                foreach (GimmickInformation gimInfo in gimmickInfoArr)
                {
                    if (gimInfo.startSquareId == squareId && gimInfo.inSquare)
                    {
                        if (on) gimInfo.OperationFlagON();
                        else gimInfo.OperationFlagOFF();
                    }
                }
            }
        }

        /// <summary>
        /// 駒のランダム色取得
        /// </summary>
        /// <returns>駒のランダムなプレハブのインデックス</returns>
        public int GetRandomPieceColor()
        {
            return USE_COLOR_TYPE_ARR[UnityEngine.Random.Range(0, USE_COLOR_COUNT)];
        }

        /// <summary>
        /// マスにある駒の色ID取得
        /// </summary>
        /// <returns>色ID</returns>
        public int GetSquarePieceColorId(int squareIndex)
        {
            if (pieceInfoArr[squareIndex] == null) return INT_NULL;
            return pieceInfoArr[squareIndex].colorId;
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
            int SQUARES_COUNT_Up    = putPieceIndex % BOARD_LINE_COUNT;            //置いた駒の上にあるマスの数
            int SQUARES_COUNT_Left  = putPieceIndex / BOARD_LINE_COUNT;            //置いた駒の左にあるマスの数
            int SQUARES_COUNT_Down  = BOARD_LINE_COUNT - SQUARES_COUNT_Up - 1;      //置いた駒の下にあるマスの数
            int SQUARES_COUNT_Right = BOARD_COLUMN_COUNT - SQUARES_COUNT_Left - 1;  //置いた駒の右にあるマスの数

            //反転判定方向順番の設定
            int directionCounts = Enum.GetValues(typeof(Directions)).Length;
            int[] loopCounts = new int[directionCounts];
            int[][] dummyArr = new int[directionCounts][];
            foreach (Directions directions in Enum.GetValues(typeof(Directions)))
            {
                //各方向毎の処理回数設定
                loopCounts[(int)directions] = SetLoopCount(directions, ref SQUARES_COUNT_Up, ref SQUARES_COUNT_Right, ref SQUARES_COUNT_Down, ref SQUARES_COUNT_Left);

                //順番判定ギミックが存在するか？
                dummyArr[(int)directions] = new int[] { (int)directions, GetOrderIndex(ref putPieceIndex, ref loopCounts[(int)directions], (int)directions) };
            }

            //順番ギミックを昇順にソート
            const int ARRAY_INDEX = 0;    //配列番号格納index
            const int ORDER_INDEX = 1;    //順番番号格納index
            int[] dirOrder = new int[directionCounts];
            for (int i = 0; i < directionCounts; i++)
            {
                int[] minOrder = null;
                int useIndex = -1;
                int n = -1;
                foreach (int[] arr in dummyArr)
                {
                    n++;
                    if (arr == null) continue;
                    if (minOrder == null)
                    {
                        minOrder = arr;
                        useIndex = n;
                    }
                    else if (minOrder[ORDER_INDEX] > arr[ORDER_INDEX])
                    {
                        minOrder = arr;
                        useIndex = n;
                    }
                }
                dirOrder[i] = minOrder[ARRAY_INDEX];
                dummyArr[useIndex] = null;
            }

            //指定方向の反転オブジェクトの管理番号取得
            foreach (int dir in dirOrder)
            {
                if (loopCounts[dir] == 0) continue;
                reversIndexList.Add(GetReversIndex_SpecifiedDirection(ref putPieceIndex, ref putPieceColorId, ref loopCounts[dir], dir));
            }

            //nullをすべて削除
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
        int SetLoopCount(Directions directions, ref int up, ref int right, ref int down, ref int left)
        {
            switch (directions)
            {
                case Directions.Up:        return up;                              //上
                case Directions.UpRight:   return (up <= right) ? up : right;      //右上
                case Directions.Right:     return right;                           //右
                case Directions.DownRight: return (down <= right) ? down : right;  //右下
                case Directions.Down:      return down;                            //下
                case Directions.DownLeft:  return (down <= left) ? down : left;    //左下
                case Directions.Left:      return left;                            //左
                case Directions.UpLeft:    return (up <= left) ? up : left;        //左上
                default: return 0;
            }
        }

        /// <summary>
        /// 指定方向の順番ギミック番号取得
        /// </summary>
        /// <param name="putPieceIndex">  基準駒の管理番号</param>
        /// <param name="loopCount">      指定方向にあるマスの数</param>
        /// <param name="direction">      指定方向の管理番号</param>
        /// <returns>指定方向の反転オブジェクトの管理番号配列</returns>
        int GetOrderIndex(ref int putPieceIndex, ref int loopCount, int direction)
        {
            if (loopCount == 0) return NOT_NUM;
            for (int i = 1; i <= loopCount; i++)
            {
                //指定方向のインデックス番号取得
                int refIndex = GetDesignatedDirectionIndex(direction, putPieceIndex, i);

                //空マスの場合はnullを返す
                if (pieceObjArr[refIndex] == null) return NOT_NUM;

                //ギミックマスの場合
                if (pieceObjArr[refIndex].tag == GIMMICK_TAG)
                {
                    //順番指定がある場合
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceObjArr[refIndex]);
                    if (gimmickInfoArr[gimmickIndex].order != NOT_NUM)
                        return gimmickInfoArr[gimmickIndex].order;
                }
            }
            return NOT_NUM;
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
            int orderCount = 0; //順番ギミックをカウントした数(反転しないと判定した場合にカウントを戻すため)
            for (int i = 1; i <= loopCount; i++)
            {
                //指定方向のインデックス番号取得
                int refIndex = GetDesignatedDirectionIndex(direction, putPieceIndex, i);

                //空マスの場合はnullを返す
                if (pieceObjArr[refIndex] == null) break;

                //ギミックマスの場合
                if (pieceObjArr[refIndex].tag == GIMMICK_TAG)
                {
                    //ダメージを与えられるかの確認,ダメージが与えられない場合はnullを返す
                    int gimmickIndex = Array.IndexOf(gimmickObjArr, pieceObjArr[refIndex]);
                    if (!gimmickInfoArr[gimmickIndex].destructible_Piece) break;
                    if (!gimmicksMan.DamageCheck(ref putPieceColorId, ref gimmickIndex)) break;
                    if (gimmickInfoArr[gimmickIndex].order != NOT_NUM) orderCount++;    //順番判定を通過した場合はカウント
                    reversIndexList.Add(refIndex);
                }
                else
                {
                    //反転禁止駒の場合はnullを返す
                    if (!pieceInfoArr[refIndex].invertable) break;

                    //同色駒を検索
                    if (pieceInfoArr[refIndex].colorId == putPieceColorId)
                    {
                        //隣が同タグの場合はnullを返す
                        if (i == 1) break;

                        //削除対象に同タグ駒の管理番号追加
                        destroyPiecesIndexList.Add(refIndex);

                        //反転リストを返す(成功)
                        return reversIndexList.ToArray();
                    }
                    reversIndexList.Add(refIndex);
                }
            }

            //順番ギミックのカウントを戻す
            nextOrder -= orderCount;

            //nullを返す(失敗)
            return null;
        }

        /// <summary>
        /// 指定場所の管理番号取得
        /// </summary>
        /// <param name="direction">方向の管理番号</param>
        /// <param name="baseIndex">基準オブジェクトの管理番号</param>
        /// <param name="distance"> 距離</param>
        /// <returns>指定場所の管理番号</returns>
        public int GetDesignatedDirectionIndex(int direction, int baseIndex, int distance = 1)
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
            }
            return INT_NULL;
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

            //駒カウントギミックにダメージ(置いた駒)
            StartCoroutine(gimmicksMan.DamageCage(putPieceColorId));    //檻

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

                    //駒カウントギミックにダメージ(反転駒)
                    StartCoroutine(gimmicksMan.DamageCage(putPieceColorId));    //檻

                    yield return PIECE_REVERSAL_INTERVAL;
                }
                yield return PIECE_GROUP_REVERSAL_INTERVAL;
            }

            //駒カウントギミックにダメージ(最後の駒)
            StartCoroutine(gimmicksMan.DamageCage(putPieceColorId));    //檻

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
        /// <param name="generateColorId"> 生成駒の色番号</param>
        public IEnumerator ReversingPieces(int reversPieceIndex, int generateColorId)
        {
            if (pieceTraArr[reversPieceIndex] == null) yield break;

            //駒90°回転,拡大
            StartCoroutine(AllScaleChange(pieceTraArr[reversPieceIndex], REVERSE_PIECE_SCALING_SPEED, REVERSE_PIECE_CHANGE_SCALE));
            yield return StartCoroutine(RotateMovement(pieceTraArr[reversPieceIndex], REVERSE_PIECE_ROT_SPEED, REVERSE_PIECE_SWITCH_ROT));

            //元駒削除,新駒生成
            DeletePiece(reversPieceIndex);
            GeneratePiece(generateColorId, reversPieceIndex);

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
            foreach (int index in destroyPiecesIndexList)
            {
                if (pieceTraArr[index].gameObject.tag == GIMMICK_TAG) continue;
                coroutine = StartCoroutine(AllScaleChange(pieceTraArr[index], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));
            }
            yield return coroutine;

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
            if (!NOW_TURN_END_PROCESSING) StartCoroutine(TurnEnd());

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
            for (int i = SQUARES_COUNT - 1; i >= 0; i--)
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
                        int prefabIndex = GetRandomPieceColor();
                        GeneratePiece(prefabIndex, i);
                        pieceTraArr[i].localPosition = new Vector3(PIECE_DEFAULT_POS.x, i % BOARD_LINE_COUNT * SQUARE_DISTANCE, Z_PIECE);
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
                if (gimmickInfoArr[gimmickObjIndex].inSquare && !gimmickInfoArr[gimmickObjIndex].freeFall_Piece)
                {
                    return false;
                }
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
        IEnumerator GimmickDestroyCheck()
        {
            //ギミック破壊待機中フラグセット
            NOW_GIMMICK_DESTROY_WAIT = true;

            //破壊開始
            yield return StartCoroutine(gimmicksMan.DestroyGimmicks_TurnEnd());

            //ギミック破壊待機中フラグリセット
            NOW_GIMMICK_DESTROY_WAIT = false;
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
            //ターン終了処理中フラグセット
            NOW_TURN_END_PROCESSING = true;

            //特定ギミック破壊判定開始
            yield return StartCoroutine(GimmickDestroyCheck());

            //自由落下
            yield return StartCoroutine(StratFallingPieces());

            //ギミック状態変化開始
            yield return StartCoroutine(StartChangeGimmickState());

            //特定ギミック破壊判定開始
            yield return StartCoroutine(GimmickDestroyCheck());

            //ギミックのフラグリセット
            foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
            {
                if (gimmickInfo == null) continue;
                gimmickInfo.nowTurnDamage = false;
            }

            //ターン終了処理中フラグリセット
            NOW_TURN_END_PROCESSING = false;
        }
    }
}