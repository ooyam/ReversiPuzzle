using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static ObjectMove_2D.ObjectMove_2D;

namespace PuzzleMain
{
    public class PiecesManager : MonoBehaviour
    {
        SquaresManager      squaresMgr;     //SquaresManager
        GimmicksManager     gimmicksMgr;    //GimmicksManager
        SupportItemsManager stItemsMgr;     //SupportItemsManager

        [Header("駒プレハブの取得")]
        [SerializeField]
        GameObject[] piecePrefabArr;

        [Header("黒駒プレハブの取得")]
        [SerializeField]
        GameObject blackPiecePrefab;

        Transform[]  pieceTraArr;       //駒Transform配列
        GameObject[] nextPieceObjArr;   //待機駒オブジェクト配列
        Transform[]  nextPieceTraArr;   //待機駒Transform配列

        int nextPuPieceIndex = 0;       //次に置く駒の管理番号

        //==========================================================//
        //----------------------初期設定,取得-----------------------//
        //==========================================================//

        /// <summary>
        /// PiecesManagerの初期化
        /// </summary>
        public void Initialize()
        {
            //他クラス取得
            squaresMgr  = sPuzzleMain.GetSquaresManager();
            gimmicksMgr = sPuzzleMain.GetGimmicksManager();
            stItemsMgr  = sPuzzleMain.GetSupportItemsManager();

            pieceTraArr         = new Transform[SQUARES_COUNT];
            sPieceObjArr        = new GameObject[SQUARES_COUNT];
            sPieceInfoArr       = new PieceInformation[SQUARES_COUNT];

            //ギミック情報設定（駒として管理する）
            List<int> notPlaceIndex = new List<int>();  //駒を配置しないマス番号
            for (int i = 0; i < GIMMICKS_COUNT; i++)
            {
                //駒として管理するギミック
                if (GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[i][GIMMICK]].in_square)
                {
                    SetGimmicksInfomation(i);
                    notPlaceIndex.Add(GIMMICKS_INFO_ARR[i][SQUARE]);
                }
            }

            //駒のランダム配置
            for (int i = 0; i < SQUARES_COUNT; i++)
            {
                if (!sSquareObjArr[i].activeSelf) continue; //非表示マスは処理を飛ばす
                if (notPlaceIndex.Contains(i))   continue; //ギミックマスは処理を飛ばす

                int pieceGeneIndex = GetRandomPieceColor();
                GeneratePiece(pieceGeneIndex, i, true);
            }

            //待機駒生成
            nextPieceObjArr = new GameObject[sNextPiecesCount];
            nextPieceTraArr = new Transform[sNextPiecesCount];
            for (int i = 0; i < sNextPiecesCount; i++)
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
            if(prefabIndex == COLORLESS_ID) sPieceObjArr[pieceIndex] = Instantiate(blackPiecePrefab);
            else sPieceObjArr[pieceIndex] = Instantiate(piecePrefabArr[prefabIndex]);
            sPieceInfoArr[pieceIndex] = sPieceObjArr[pieceIndex].GetComponent<PieceInformation>();
            sPieceInfoArr[pieceIndex].InformationSetting(pieceIndex, startGenerate, sGimmickInfoArr);
            pieceTraArr[pieceIndex] = sPieceInfoArr[pieceIndex].tra;
            pieceTraArr[pieceIndex].SetParent(sSquareTraArr[pieceIndex], false);
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
            nextPieceTraArr[pieceIndex].SetParent(sNextPieceBoxTraArr[pieceIndex], false);
        }

        /// <summary>
        /// 駒削除
        /// </summary>
        /// /// <param name="pieceIndex">削除駒の管理番号</param>
        void DeletePiece(int pieceIndex)
        {
            Destroy(sPieceObjArr[pieceIndex]);
            sPieceObjArr[pieceIndex]  = null;
            pieceTraArr[pieceIndex]   = null;
            sPieceInfoArr[pieceIndex] = null;
        }

        /// <summary>
        /// 待機駒を盤面に置く
        /// </summary>
        /// <param name="squareId">配置マス管理番号</param>
        void PutPiece(int squareId)
        {
            //盤面の駒削除,管理配列差し替え
            DeletePiece(squareId);
            sPieceObjArr[squareId]  = nextPieceObjArr[nextPuPieceIndex];
            pieceTraArr[squareId]  = nextPieceTraArr[nextPuPieceIndex];
            sPieceInfoArr[squareId] = sPieceObjArr[squareId].GetComponent<PieceInformation>();
            sPieceInfoArr[squareId].InformationSetting(squareId, false);

            //待機駒生成
            int pieceGeneIndex = GetRandomPieceColor();
            GenerateNextPiece(pieceGeneIndex, nextPuPieceIndex);
        }

        /// <summary>
        /// ギミック情報設定（駒として管理する）
        /// </summary>
        /// /// <param name="gimmickIndex"> ギミック管理番号</param>
        /// /// <param name="startGenerate">初期生成？</param>
        void SetGimmicksInfomation(int gimmickIndex, bool startGenerate = true)
        {
            //駒としても管理する
            int pieceIndex = GIMMICKS_INFO_ARR[gimmickIndex][SQUARE];
            sPieceObjArr[pieceIndex] = sGimmickObjArr[gimmickIndex];
            pieceTraArr[pieceIndex] = sGimmickInfoArr[gimmickIndex].tra;
            pieceTraArr[pieceIndex].SetParent(sSquareTraArr[GIMMICKS_INFO_ARR[gimmickIndex][SQUARE]], false);
            pieceTraArr[pieceIndex].SetSiblingIndex(0);
            pieceTraArr[pieceIndex].localPosition = sGimmickInfoArr[gimmickIndex].defaultPos;
            sGimmickInfoArr[gimmickIndex].OperationFlagSetting(pieceIndex, startGenerate, sGimmickInfoArr);
        }

        /// <summary>
        /// マスとして管理しないギミック配置
        /// </summary>
        /// <param name="gimmickObj">配置オブジェクト</param>
        /// <param name="squareId">  配置マス管理番号</param>
        public void PlaceGimmick(GameObject gimmickObj, int squareId)
        {
            gimmickObj.transform.SetParent(sSquareTraArr[squareId], false);
        }

        /// <summary>
        /// ギミック削除
        /// </summary>
        /// /// <param name="gimmickObj">削除駒</param>
        public void DeleteGimmick(GameObject gimmickObj)
        {
            sGimmickObjArr[gimmicksMgr.GetGimmickIndex_Obj(gimmickObj)] = null;

            DeletePiece(Array.IndexOf(sPieceObjArr, gimmickObj));
        }

        /// <summary>
        /// 管理番号の更新
        /// </summary>
        /// <param name="oldIndex">更新前の管理番号</param>
        /// <param name="newIndex">更新後の管理番号</param>
        void UpdateManagementIndex(int oldIndex, int newIndex)
        {
            sPieceObjArr[newIndex]  = sPieceObjArr[oldIndex];
            pieceTraArr[newIndex]   = pieceTraArr[oldIndex];
            sPieceInfoArr[newIndex] = sPieceInfoArr[oldIndex];
            pieceTraArr[newIndex].SetParent(sSquareTraArr[newIndex], true);
            pieceTraArr[newIndex].SetSiblingIndex(0);
            sPieceObjArr[oldIndex]  = null;
            pieceTraArr[oldIndex]   = null;
            sPieceInfoArr[oldIndex] = null;
            if (sPieceInfoArr[newIndex] != null) sPieceInfoArr[newIndex].squareId = newIndex;

            int gimIndex = gimmicksMgr.GetGimmickIndex_Square(newIndex);
            if (gimIndex >= 0) sGimmickInfoArr[gimIndex].nowSquareId = newIndex;
        }

        /// <summary>
        /// 駒の操作可能フラグ切替
        /// </summary>
        /// <param name="squareId">マス管理番号</param>
        /// <param name="on">      true:操作可能にする</param>
        public void PieceOperationFlagChange(int squareId, bool on)
        {
            //駒
            if (sPieceObjArr[squareId].CompareTag(PIECE_TAG))
            {
                if (on) sPieceInfoArr[squareId].OperationFlagON();
                else sPieceInfoArr[squareId].OperationFlagOFF();
            }
            //ギミック
            else
            {
                foreach (GimmickInformation gimInfo in sGimmickInfoArr)
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
        /// <param name="squareId">マス管理番号</param>
        /// <returns>色ID</returns>
        public int GetSquarePieceColorId(int squareId)
        {
            if (sPieceInfoArr[squareId] == null) return INT_NULL;
            return sPieceInfoArr[squareId].colorId;
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
            //援護アイテム準備中以外はギミックをタップできない
            if (tapObj.CompareTag(GIMMICK_TAG) && !NOW_SUPPORT_ITEM_READY) return;

            //オブジェクト番号取得
            int pieceObjIndex = Array.IndexOf(sPieceObjArr, tapObj);
            int nextPieceObjIndex = Array.IndexOf(nextPieceObjArr, tapObj);

            //盤上の駒の場合
            if (pieceObjIndex >= 0)
            {
                //援護アイテム準備中の場合
                if (NOW_SUPPORT_ITEM_READY)
                {
                    StartCoroutine(stItemsMgr.UseItems(pieceObjIndex));
                }
                //駒反転(反転フラグ確認)
                else if (sPieceInfoArr[pieceObjIndex].invertable)
                {
                    StartCoroutine(PutPieceToSquare(tapObj));
                }
            }
            //待機駒の場合
            else if (nextPieceObjIndex >= 0)
            {
                //援護アイテム準備中の場合は解除
                if (NOW_SUPPORT_ITEM_READY)
                {
                    stItemsMgr.ResetWaitItemReady();
                }
                nextPuPieceIndex = nextPieceObjIndex;
                squaresMgr.MoveNextPieceFrame(nextPuPieceIndex);
            }
        }

        /// <summary>
        /// 指定マスのダメージ
        /// </summary>
        /// <param name="squareIndex">   マス番号</param>
        /// <param name="reversiColorId">反転後の色ID</param>
        /// <param name="instantly">     単体・即破壊</param>
        /// <param name="assault">       強撃</param>
        /// <param name="stateAddName">  ステート名追加文字列</param>
        public void DamageSpecifiedSquare(int squareIndex, int reversiColorId, bool instantly, bool assault = false, string stateAddName = "")
        {
            if (sPieceObjArr[squareIndex] == null) return;   //空マス

            //ギミック
            if (sPieceObjArr[squareIndex].CompareTag(GIMMICK_TAG))
            {
                //ダメージ判定
                int gimmickIndex = gimmicksMgr.GetGimmickIndex_Square(squareIndex);
                bool damage = gimmicksMgr.DamageCheck(ref sGimmickInfoArr[gimmickIndex].colorId, ref gimmickIndex, assault);

                //ダメージ有の場合
                if (damage) gimmicksMgr.DamageGimmick(ref gimmickIndex, squareIndex, stateAddName);
            }
            //駒
            else
            {
                //反転可能判定
                if (sPieceInfoArr[squareIndex].invertable)
                {
                    //駒カウントギミックにダメージ(反転駒)
                    StartCoroutine(gimmicksMgr.DamageCage(sPieceInfoArr[squareIndex].colorId));    //檻

                    //反転開始
                    if (!instantly) StartCoroutine(ReversingPieces(squareIndex, reversiColorId));
                    sDestroyPiecesIndexList.Add(squareIndex);
                }
            }

            //即破壊
            if (instantly)
            {
                foreach (int i in sDestroyPiecesIndexList)
                { StartCoroutine(StratReversingPiece(reversiColorId, i)); }

                //破壊リストリセット
                sDestroyPiecesIndexList = new List<int>();
            }
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
            int putIndex = Array.IndexOf(sPieceObjArr, deletePiece);
            nextPieceTraArr[nextPuPieceIndex].SetParent(sSquareTraArr[putIndex], true);
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

            //駒カウントギミックにダメージ(置いた駒)
            StartCoroutine(gimmicksMgr.DamageCage(sPieceInfoArr[putIndex].colorId));    //檻

            //反転駒リスト取得
            int colorId = sPieceInfoArr[putIndex].colorId;      //置いた駒の色番号取得
            List<int[]> reversIndexList = new List<int[]>();    //反転駒の管理番号格納リスト
            if (GetReversIndex(putIndex, ref colorId, ref reversIndexList))
            {
                //削除対象に置いた駒の管理番号追加
                sDestroyPiecesIndexList.Insert(sDestroyBasePieceIndex, putIndex);

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
            int sqrCountUp    = putPieceIndex % BOARD_LINE_COUNT;       //置いた駒の上にあるマスの数
            int sqrCountLeft  = putPieceIndex / BOARD_LINE_COUNT;       //置いた駒の左にあるマスの数
            int sqrCountDown  = BOARD_LINE_COUNT - sqrCountUp - 1;      //置いた駒の下にあるマスの数
            int sqrCountRight = BOARD_COLUMN_COUNT - sqrCountLeft - 1;  //置いた駒の右にあるマスの数

            //反転判定方向順番の設定
            int[] loopCounts = new int[DIRECTIONS_COUNT];
            int[][] dummyArr = new int[DIRECTIONS_COUNT][];
            foreach (Directions directions in Enum.GetValues(typeof(Directions)))
            {
                //各方向毎の処理回数設定
                loopCounts[(int)directions] = squaresMgr.SetLoopCount(directions, ref sqrCountUp, ref sqrCountRight, ref sqrCountDown, ref sqrCountLeft);

                //順番判定ギミックが存在するか？
                dummyArr[(int)directions] = new int[] { (int)directions, GetOrderIndex(ref putPieceIndex, ref loopCounts[(int)directions], (int)directions) };
            }

            //順番ギミックを昇順にソート
            const int ARRAY_INDEX = 0;    //配列番号格納index
            const int ORDER_INDEX = 1;    //順番番号格納index
            int[] dirOrder = new int[DIRECTIONS_COUNT];
            for (int i = 0; i < DIRECTIONS_COUNT; i++)
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
                int refIndex = squaresMgr.GetDesignatedDirectionIndex(direction, putPieceIndex, i);

                //空マスの場合はnullを返す
                if (sPieceObjArr[refIndex] == null) return NOT_NUM;

                //ギミックマスの場合
                if (sPieceObjArr[refIndex].CompareTag(GIMMICK_TAG))
                {
                    //順番指定がある場合
                    int gimmickIndex = gimmicksMgr.GetGimmickIndex_Square(refIndex);
                    if (sGimmickInfoArr[gimmickIndex].order != NOT_NUM)
                        return sGimmickInfoArr[gimmickIndex].order;
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
                int refIndex = squaresMgr.GetDesignatedDirectionIndex(direction, putPieceIndex, i);

                //空マスの場合はnullを返す
                if (sPieceObjArr[refIndex] == null) break;

                //ギミックマスの場合
                if (sPieceObjArr[refIndex].CompareTag(GIMMICK_TAG))
                {
                    //ダメージを与えられるかの確認,ダメージが与えられない場合はnullを返す
                    int gimmickIndex = gimmicksMgr.GetGimmickIndex_Square(refIndex);
                    if (!sGimmickInfoArr[gimmickIndex].destructible_Piece) break;
                    if (!gimmicksMgr.DamageCheck(ref putPieceColorId, ref gimmickIndex)) break;
                    if (sGimmickInfoArr[gimmickIndex].order != NOT_NUM) orderCount++;    //順番判定を通過した場合はカウント
                    reversIndexList.Add(refIndex);
                }
                else
                {
                    //反転禁止駒の場合はnullを返す
                    if (!sPieceInfoArr[refIndex].invertable) break;

                    //同色駒を検索
                    if (sPieceInfoArr[refIndex].colorId == putPieceColorId)
                    {
                        //隣が同タグの場合はnullを返す
                        if (i == 1) break;

                        //削除対象に同色駒の管理番号追加
                        sDestroyPiecesIndexList.Add(refIndex);

                        //反転リストを返す(成功)
                        return reversIndexList.ToArray();
                    }
                    reversIndexList.Add(refIndex);
                }
            }

            //順番ギミックのカウントを戻す
            sNumberTagNextOrder -= orderCount;

            //nullを返す(失敗)
            return null;
        }
        //==========================================================//


        //==========================================================//
        //-----------------------駒反転動作-------------------------//
        //==========================================================//

        /// <summary>
        /// 駒反転開始(単体)
        /// </summary>
        /// <param name="putPieceColorId">置いた駒の色番号</param>
        /// <param name="reversIndex">    反転駒管理番号</param>
        /// <returns></returns>
        IEnumerator StratReversingPiece(int putPieceColorId, int reversIndex)
        {
            //反転中フラグセット
            NOW_REVERSING_PIECES = true;

            //ギミック
            if (sPieceObjArr[reversIndex].CompareTag(GIMMICK_TAG))
            {
                //ギミック終了待機
                yield return sGimmickCorList[sGimmickCorList.Count - 1];
            }
            //駒
            else
            {
                //反転
                Coroutine coroutine = StartCoroutine(ReversingPieces(reversIndex, putPieceColorId));

                //駒カウントギミックにダメージ(反転駒)
                StartCoroutine(gimmicksMgr.DamageCage(putPieceColorId));    //檻

                //反転終了待機
                yield return coroutine;
            }

            //反転駒の破壊
            StartCoroutine(StartDestroyingPiece(reversIndex));

            //反転中フラグリセット
            NOW_REVERSING_PIECES = false;
        }

        /// <summary>
        /// 駒反転開始(複数)
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
                    int gimmickIndex = gimmicksMgr.GetGimmickIndex_Square(reversIndex);
                    if (gimmickIndex >= 0)
                    {
                        gimmicksMgr.DamageGimmick(ref gimmickIndex, reversIndex);
                        yield return PIECE_REVERSAL_INTERVAL;
                        continue;
                    }

                    //削除対象に反転駒の管理番号追加
                    sDestroyPiecesIndexList.Add(reversIndex);

                    //駒カウントギミックにダメージ(反転駒)
                    StartCoroutine(gimmicksMgr.DamageCage(sPieceInfoArr[reversIndex].colorId));    //檻

                    //反転
                    coroutine = StartCoroutine(ReversingPieces(reversIndex, putPieceColorId));

                    yield return PIECE_REVERSAL_INTERVAL;
                }
                yield return PIECE_GROUP_REVERSAL_INTERVAL;
            }

            //駒カウントギミックにダメージ(反転駒)
            StartCoroutine(gimmicksMgr.DamageCage(putPieceColorId));    //檻

            //ギミック終了待機
            foreach (Coroutine c in sGimmickCorList)
            { yield return c; }

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
        /// 駒破壊開始(単体)
        /// </summary>
        /// <param name="destroyIndex">駒管理番号</param>
        public IEnumerator StartDestroyingPiece(int destroyIndex)
        {
            //破壊中フラグセット
            NOW_DESTROYING_PIECES = true;

            //駒縮小
            if (sPieceObjArr[destroyIndex].CompareTag(PIECE_TAG))
            yield return StartCoroutine(AllScaleChange(pieceTraArr[destroyIndex], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));

            //駒削除
            GameObject obj = sPieceObjArr[destroyIndex];
            if (obj.CompareTag(GIMMICK_TAG)) DeleteGimmick(obj);
            else DeletePiece(destroyIndex);

            //駒の落下開始
            yield return StartCoroutine(StratFallingPieces());

            //破壊中フラグリセット
            NOW_DESTROYING_PIECES = false;
        }

        /// <summary>
        /// 駒破壊開始(複数)
        /// </summary>
        /// <param name="useSupport">援護アイテム使用？</param>
        public IEnumerator StartDestroyingPieces(bool useSupport = false)
        {
            //破壊中フラグセット
            NOW_DESTROYING_PIECES = true;

            //駒縮小
            Coroutine coroutine = null;
            foreach (int index in sDestroyPiecesIndexList)
            {
                if (sPieceObjArr[index].CompareTag(GIMMICK_TAG)) continue;
                coroutine = StartCoroutine(AllScaleChange(pieceTraArr[index], DESTROY_PIECE_SCALING_SPEED, DESTROY_PIECE_CHANGE_SCALE));
            }
            yield return coroutine;
            
            //援護アイテム生成
           　if (!useSupport) stItemsMgr.GenerateItems();

            //駒削除
            foreach (int pieceIndex in sDestroyPiecesIndexList)
            {
                GameObject obj = sPieceObjArr[pieceIndex];
                if (obj.CompareTag(GIMMICK_TAG)) DeleteGimmick(obj);
                else DeletePiece(pieceIndex);
            }

            //削除駒リストの初期化
            sDestroyPiecesIndexList = new List<int>();

            //駒の落下開始
            yield return StartCoroutine(StratFallingPieces());

            //ターン終了
            StartCoroutine(TurnEnd(useSupport));

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
                    int gimmickIndex = gimmicksMgr.GetGimmickIndex_Square(fallPieceIndex);
                    if (gimmickIndex >= 0) targetPos = sGimmickInfoArr[gimmickIndex].defaultPos;
                    coroutineList.Add(StartCoroutine(ConstantSpeedMovement(pieceTraArr[fallPieceIndex], FALL_PIECE_MOVE_SPEED, targetPos, FALL_PIECE_ACCELE_RATE)));
                }
            }
            foreach (Coroutine coroutine in coroutineList)
            { yield return coroutine; }

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
                //非表示マスの場合は処理をスキップ
                if (!sSquareObjArr[i].activeSelf) continue;

                //空マスでなければ処理をスキップ
                if (sPieceObjArr[i] != null) continue;

                //落下する駒リストに追加
                fallPiecesIndexList.Add(i);

                //駒の上にある管理番号をすべて調査
                int loopCount = i % BOARD_LINE_COUNT;
                for (int n = 0; n <= loopCount; n++)
                {
                    //自身よりn個上の番号が空でない場合
                    int refIndex = i - n;
                    if (sPieceObjArr[refIndex] != null)
                    {
                        //自由らか可能オブジェクトの場合,管理番号更新
                        if (FreeFallJudgement(ref sPieceObjArr[refIndex]))
                            UpdateManagementIndex(i - n, i);
                        break;
                    }

                    //落下できる駒が盤上に存在しない場合
                    if (n == loopCount)
                    {
                        //新規ランダム生成
                        int prefabIndex = GetRandomPieceColor();
                        GeneratePiece(prefabIndex, i);
                        pieceTraArr[i].localPosition = new Vector3(PIECE_DEFAULT_POS.x, BOARD_LINE_COUNT * SQUARE_DISTANCE, Z_PIECE);
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
            if (pieceObj.CompareTag(PIECE_TAG))
            {
                //駒の場合
                int pieceObjIndex = Array.IndexOf(sPieceObjArr, pieceObj);
                return sPieceInfoArr[pieceObjIndex].freeFall;
            }
            else if (pieceObj.CompareTag(GIMMICK_TAG))
            {
                //ギミックの場合
                int gimmickObjIndex = gimmicksMgr.GetGimmickIndex_Obj(pieceObj);
                if (sGimmickInfoArr[gimmickObjIndex].inSquare && !sGimmickInfoArr[gimmickObjIndex].freeFall_Piece)
                {
                    return false;
                }
                return sGimmickInfoArr[gimmickObjIndex].freeFall;
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
            yield return StartCoroutine(gimmicksMgr.DestroyGimmicks_TurnEnd());

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
            yield return StartCoroutine(gimmicksMgr.ChangeGimmickState());

            //ギミック状態変化中フラグリセット
            NOW_GIMMICK_STATE_CHANGE = false;
        }

        /// <summary>
        /// ターン終了
        /// </summary>
        /// <param name="supportItem">援護アイテム？</param>
        public IEnumerator TurnEnd(bool supportItem = false)
        {
            //ターン終了処理中フラグセット
            NOW_TURN_END_PROCESSING = true;

            //援護アイテム使用時
            if (supportItem)
            {
                //特定ギミック破壊判定開始
                yield return StartCoroutine(GimmickDestroyCheck());

                //自由落下
                yield return StartCoroutine(StratFallingPieces());

                //特定ギミック破壊判定開始
                yield return StartCoroutine(GimmickDestroyCheck());

                //ギミックのフラグリセット
                foreach (GimmickInformation gimmickInfo in sGimmickInfoArr)
                {
                    if (gimmickInfo == null) continue;
                    gimmickInfo.nowTurnDamage = false;
                }
            }
            //援護アイテム未使用時
            else
            {
                //特定ギミック破壊判定開始
                yield return StartCoroutine(GimmickDestroyCheck());

                //自由落下
                yield return StartCoroutine(StratFallingPieces());

                //ギミック状態変化開始
                yield return StartCoroutine(StartChangeGimmickState());

                //特定ギミック破壊判定開始
                yield return StartCoroutine(GimmickDestroyCheck());

                //ギミックのフラグリセット
                foreach (GimmickInformation gimmickInfo in sGimmickInfoArr)
                {
                    if (gimmickInfo == null) continue;
                    gimmickInfo.nowTurnDamage = false;
                }
            }

            //ギミック待機リストの初期化
            sGimmickCorList = new List<Coroutine>();

            //ターン終了処理中フラグリセット
            NOW_TURN_END_PROCESSING = false;
        }
    }
}