using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static ObjectMove_2D.ObjectMove_2D;
using static animation.AnimationManager;

namespace PuzzleMain
{
    //ロケットの援護番号(10の位:行指定 1の位:列番号指定)
    public enum RocketLineSupportNumber
    {
        Center = 0,     //中央:0～
        Top    = 10,    //上段:10～
        Under  = 20     //下段:20～
    }

    //ロケットの援護列タイプ
    public enum RocketColumnType
    {
        Center = 0,
        Right,
        Left
    }

    public class SupportItemsManager : MonoBehaviour
    {
        PiecesManager piecesMgr;    //PiecesManager

        [Header("援護アイテムの取得")]
        [SerializeField]
        GameObject[] mItemsArr;

        [Header("補助アイテム待機ボックスの取得")]
        [SerializeField]
        Transform mWaitItemBoxesTra;

        SupportItemInformation[] mItemsInfoArr; //援護アイテム情報

        Transform[]  mWaitItemBoxsTraArr;       //待機援護アイテム箱Transform
        GameObject[] mWaitItemObjArr;           //待機援護アイテム
        Animator[]   mWaitItemAniArr;           //待機援護アイテムAnimator
        bool[]       mWaitItemReadyUse;         //待機援護アイテム使用可能状態

        int mReadyItemNumber;           //準備中のアイテム番号
        int mDuckSupportLineNum;        //アヒルの援護行番号
        int mRocketSupportLineNum;  //ロケットの援護行番号

        const int DUCK_USE_DEL_PIECE_COUNT = 6;     //アヒル生成条件
        const int FIREWORK_USE_DIR_PIECE_COUNT = 4; //花火生成条件

        /// <summary>
        /// 援護アイテムの初期化
        /// </summary>
        public void Initialize()
        {
            piecesMgr = sPuzzleMain.GetPiecesManager();

            mReadyItemNumber    = INT_NULL;
            mWaitItemBoxsTraArr = new Transform[SUPPORT_ITEMS_COUNT];
            mWaitItemObjArr     = new GameObject[SUPPORT_ITEMS_COUNT];
            mWaitItemAniArr     = new Animator[SUPPORT_ITEMS_COUNT];
            mWaitItemReadyUse   = new bool[SUPPORT_ITEMS_COUNT];
            mItemsInfoArr       = new SupportItemInformation[SUPPORT_ITEMS_COUNT];
            for (int i = 0; i < SUPPORT_ITEMS_COUNT; i++)
            {
                mWaitItemBoxsTraArr[i] = mWaitItemBoxesTra.GetChild(i).transform;
                mWaitItemObjArr[i]     = mWaitItemBoxsTraArr[i].GetChild(0).gameObject;
                mWaitItemAniArr[i]     = mWaitItemObjArr[i].GetComponent<Animator>();
                mWaitItemReadyUse[i]   = false;
                mItemsInfoArr[i]       = mItemsArr[i].GetComponent<SupportItemInformation>();
                mItemsInfoArr[i].InformationSetting();
            }
        }

        /// <summary>
        /// 待機アイテムのタップ
        /// </summary>
        /// <param name="_tapObj">タップオブジェクト</param>
        public void TapItem(GameObject _tapObj)
        {
            int waitItemIndex = Array.IndexOf(mWaitItemObjArr, _tapObj);
            if (waitItemIndex < 0) return;

            if (mWaitItemReadyUse[waitItemIndex])
                ResetWaitItemReady();             //準備解除
            else
                SetWaitItemReady(waitItemIndex);  //準備
        }

        /// <summary>
        /// 待機アイテムを準備状態にする
        /// </summary>
        /// <param name="_itemNum">援護アイテム番号</param>
        public void SetWaitItemReady(int _itemNum)
        {
            //フラグセット
            NOW_SUPPORT_ITEM_READY = true;

            //指定アイテムを準備状態へ
            mWaitItemReadyUse[_itemNum] = true;
            StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_READY));

            //指定アイテム以外を解除状態にする
            if (mReadyItemNumber >= 0)
            {
                mWaitItemReadyUse[mReadyItemNumber] = false;
                LoopAnimationStart(mWaitItemAniArr[mReadyItemNumber]);
            }

            //準備中のアイテム番号更新
            mReadyItemNumber = _itemNum;
        }

        /// <summary>
        /// 待機アイテムを準備状態を解除する
        /// </summary>
        public void ResetWaitItemReady()
        {
            //フラグリセット
            NOW_SUPPORT_ITEM_READY = false;

            //セット中のアイテムがない場合は処理をスキップ
            if (mReadyItemNumber < 0) return;

            //解除状態にする
            mWaitItemReadyUse[mReadyItemNumber] = false;
            LoopAnimationStart(mWaitItemAniArr[mReadyItemNumber]);

            //準備中のアイテム番号リセット
            mReadyItemNumber = INT_NULL;
        }

        /// <summary>
        /// 待機援護アイテムの表示切替
        /// </summary>
        /// <param name="_itemNum">援護アイテム番号</param>
        /// <param name="_active"> 表示状態</param>
        IEnumerator SetWaitItemsActive(int _itemNum, bool _active)
        {
            //切替が必要ない場合
            if (mWaitItemObjArr[_itemNum].activeSelf == _active) yield break;

            if (_active)
            {
                //表示
                mWaitItemObjArr[_itemNum].SetActive(true);
                StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_ACTIVE));
            }
            else
            {
                //非表示
                yield return StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_INACTIVE));
                mWaitItemObjArr[_itemNum].SetActive(false);
            }
        }

        /// <summary>
        /// アイテム生成
        /// </summary>
        public void GenerateItems()
        {
            //破壊オブジェクトが空の場合
            if (sDestroyPiecesIndexList.Count == 0) return;

            //基準駒番号
            int criteriaSquareId = sDestroyPiecesIndexList[sDestroyBasePieceIndex];

            //基準行の端(左右)を取得
            int minLine = criteriaSquareId % BOARD_LINE_COUNT;
            int maxLine = SQUARES_COUNT - BOARD_LINE_COUNT + minLine;

            //基準列の端(上下)を取得
            int minColumn = criteriaSquareId - criteriaSquareId % BOARD_LINE_COUNT;
            int maxColumn = minColumn + BOARD_LINE_COUNT - 1;

            //破壊方向数の取得
            int dirCount = 0;

            //周辺8マス取得
            int[] perSquares = new int[DIRECTIONS_COUNT];
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                if (!piecesMgr.IsSquareSpecifiedDirection(dir, criteriaSquareId)) perSquares[(int)dir] = INT_NULL;  //端マスの場合
                else  perSquares[(int)dir] = piecesMgr.GetDesignatedDirectionIndex((int)dir, criteriaSquareId);     //その他
            }

            //各存在フラグ
            bool minLineFlag   = false;
            bool maxLineFlag   = false;
            bool minColumnFlag = false;
            bool maxColumnFlag = false;

            //駒の数,存在フラグを取得
            int delPieceCount = 0;
            foreach (int i in sDestroyPiecesIndexList)
            {
                //破壊方向数カウント
                if (Array.IndexOf(perSquares, i) >= 0) dirCount++;

                //駒以外は処理をスキップ
                if (!sPieceObjArr[i].CompareTag(PIECE_TAG)) continue;

                //行,列判定
                if (minLine == i)   minLineFlag   = true;
                if (maxLine == i)   maxLineFlag   = true;
                if (minColumn == i) minColumnFlag = true;
                if (maxColumn == i) maxColumnFlag = true;

                //破壊駒の数カウント
                delPieceCount++;
            }

            //1行,列の破壊フラグ設定
            bool delLine   = minLineFlag && maxLineFlag;
            bool delcolumn = minColumnFlag && maxColumnFlag;

            //生成アイテム決定
            if (delLine && delcolumn)
            {
                //全消し
            }
            else if (delcolumn)
            {
                //ロケット(縦)
            }
            else if (delLine)
            {
                //ロケット(横)
            }
            else if (dirCount >= FIREWORK_USE_DIR_PIECE_COUNT)
            {
                //花火
            }
            else if (delPieceCount >= DUCK_USE_DEL_PIECE_COUNT)
            {
                //アヒル
            }

            //※ほんとはelse if
            if (delLine)
            {
                //ロケット(横)生成
                StartCoroutine(SetWaitItemsActive((int)SupportItems.RocketLine, true));
            }
            else if (delPieceCount >= DUCK_USE_DEL_PIECE_COUNT)
            {
                //アヒル生成
                StartCoroutine(SetWaitItemsActive((int)SupportItems.Duck, true));
            }
        }

        /// <summary>
        /// 援護アイテムの表示切替
        /// </summary>
        /// <param name="_itemNum">援護アイテム番号</param>
        /// <param name="_active"> 表示状態</param>
        void SetItemsActive(int _itemNum, bool _active)
        {
            mItemsArr[_itemNum].SetActive(_active);
        }

        /// <summary>
        /// 援護アイテムの使用
        /// </summary>
        /// <param name="_tapSquare">指定マス番号</param>
        public IEnumerator UseItems(int _tapSquare)
        {
            //アイテム使用フラグセット
            NOW_SUPPORT_ITEM_USE = true;

            //アイテム番号保持
            int itemNum = mReadyItemNumber;

            //準備状態解除
            ResetWaitItemReady();

            //待機アイテム非表示
            StartCoroutine(SetWaitItemsActive(itemNum, false));

            //駒をすべて同じタイミングで破壊する
            bool allTogether = false;

            switch (itemNum)
            {
                //アヒル
                case (int)SupportItems.Duck:
                    yield return StartCoroutine(UseDuck(_tapSquare % BOARD_LINE_COUNT));
                    break;

                //ロケット
                case (int)SupportItems.RocketLine:
                    yield return StartCoroutine(UseRocketLine(_tapSquare % BOARD_LINE_COUNT));
                    allTogether = true;
                    break;
            }

            //ギミック破壊待機
            foreach (Coroutine c in sGimmickCorList)
            { yield return c; }

            //駒破壊
            if (allTogether) yield return StartCoroutine(piecesMgr.StartDestroyingPieces(true));
            else StartCoroutine(piecesMgr.TurnEnd(true));

            //アイテム使用フラグリセット
            NOW_SUPPORT_ITEM_USE = false;
        }

        /// <summary>
        /// アヒルの使用
        /// </summary>
        /// <param name="_lineNum">指定行</param>
        /// <returns></returns>
        IEnumerator UseDuck(int _lineNum)
        {
            mDuckSupportLineNum = _lineNum;
            int duckNum = (int)SupportItems.Duck;

            //配置座標指定
            SetItemsActive(duckNum, true);
            Vector2 setPos = new Vector2(0.0f, -SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[duckNum].tra.localPosition = setPos;

            //アニメ再生
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[duckNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(duckNum, false);
        }

        /// <summary>
        /// アヒルの援護
        /// </summary>
        /// <param name="_attackColumn">攻撃列番号</param>
        public void DuckSupport(int _attackColumn)
        {
            int squareIndex = mDuckSupportLineNum + (_attackColumn * BOARD_LINE_COUNT);
            piecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, true);
        }

        /// <summary>
        /// ロケット(横)の使用
        /// </summary>
        /// <param name="_lineNum">指定行</param>
        /// <returns></returns>
        IEnumerator UseRocketLine(int _lineNum)
        {
            mRocketSupportLineNum = _lineNum;
            int RocketNum = (int)SupportItems.RocketLine;

            //配置座標指定
            SetItemsActive(RocketNum, true);
            Vector2 setPos = new Vector2(0.0f, -SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[RocketNum].tra.localPosition = setPos;

            //アニメ再生
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[RocketNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(RocketNum, false);
        }

        /// <summary>
        /// ロケット(横)の援護
        /// </summary>
        /// <param name="_supportNum">援護番号(10の位:行指定 1の位:列番号指定)</param>
        public void RocketLineSupport(int _supportNum)
        {
            //列指定
            int column = _supportNum % TEN;

            //マス指定
            int squareIndex = mRocketSupportLineNum + (column * BOARD_LINE_COUNT);
            switch (_supportNum - column)
            {
                //上段
                case (int)RocketLineSupportNumber.Top:

                    //マス無しの場合は処理終了
                    if (!piecesMgr.IsSquareSpecifiedDirection(Directions.Up, squareIndex))
                        return;

                    //上段のマスに修正
                    squareIndex--;
                    break;

                //下段
                case (int)RocketLineSupportNumber.Under:

                    //マス無しの場合は処理終了
                    if (!piecesMgr.IsSquareSpecifiedDirection(Directions.Down, squareIndex))
                        return;

                    //下段のマスに修正
                    squareIndex++;
                    break;
            }

            //マスへダメージ
            piecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID);
        }
    }
}