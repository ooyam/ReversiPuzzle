using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static animation.AnimationManager;

namespace PuzzleMain
{
    //花火の援護場所
    public enum FireworkSupportPlace
    {
        Center = 0,     //中央
        Surroundings    //周囲
    }

    //ロケットの援護行タイプ(10の位:行指定 1の位:列番号指定)
    public enum RocketLineType
    {
        Center = 0,     //中央:0～
        Top    = 10,    //上段:10～
        Under  = 20     //下段:20～
    }

    //ロケットの援護列タイプ(10の位:列指定 1の位:行番号指定)
    public enum RocketColumnType
    {
        Center = 0,     //中央:0～
        Right  = 10,    //右列:10～
        Left   = 20     //左列:20～
    }

    public class SupportItemsManager : MonoBehaviour
    {
        SquaresManager  mSquaresMgr;    //SquaresManager
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager
        TurnManager     mTurnMgr;       //TurnManager

        [Header("援護アイテムの取得")]
        [SerializeField]
        Transform mItemBoxArr;

        [Header("補助アイテム待機ボックスの取得")]
        [SerializeField]
        Transform mWaitItemBoxesTra;

        GameObject[] mItemsArr;                 //援護アイテム
        SupportItemInformation[] mItemsInfoArr; //援護アイテム情報

        Transform[]  mWaitItemBoxsTraArr;       //待機援護アイテム箱Transform
        GameObject[] mWaitItemObjArr;           //待機援護アイテム
        Animator[]   mWaitItemAniArr;           //待機援護アイテムAnimator
        bool[]       mWaitItemReadyUse;         //待機援護アイテム使用可能状態

        int mReadyItemNumber;           //準備中のアイテム番号
        int mDuckSupportLineNum;        //アヒルの援護行番号
        int mFireworkSupportSquareId;   //花火の援護マス番号
        int mRocketSupportLineNum;      //ロケット(横)の援護行番号
        int mRocketSupportColumnNum;    //ロケット(縦)の援護列番号
        int mStarSupportSquareId;       //星の援護マス番号

        List<int> mStarSupportedSquaresList = new List<int>();  //星が援護したマスの保管(当たり判定重複防止)

        const int DUCK_USE_DEL_PIECE_COUNT = 6;     //アヒル生成条件
        const int FIREWORK_USE_DIR_PIECE_COUNT = 4; //花火生成条件

        /// <summary>
        /// 援護アイテムの初期化
        /// </summary>
        public void Initialize()
        {
            mSquaresMgr  = sPuzzleMain.GetSquaresManager();
            mPiecesMgr   = sPuzzleMain.GetPiecesManager();
            mGimmicksMgr = sPuzzleMain.GetGimmicksManager();
            mTurnMgr     = sPuzzleMain.GetTurnManager();

            mReadyItemNumber    = INT_NULL;
            mItemsArr           = new GameObject[SUPPORT_ITEMS_COUNT];
            mWaitItemBoxsTraArr = new Transform[SUPPORT_ITEMS_COUNT];
            mWaitItemObjArr     = new GameObject[SUPPORT_ITEMS_COUNT];
            mWaitItemAniArr     = new Animator[SUPPORT_ITEMS_COUNT];
            mWaitItemReadyUse   = new bool[SUPPORT_ITEMS_COUNT];
            mItemsInfoArr       = new SupportItemInformation[SUPPORT_ITEMS_COUNT];
            for (int i = 0; i < SUPPORT_ITEMS_COUNT; i++)
            {
                mItemsArr[i]            = mItemBoxArr.GetChild(i).gameObject;
                mWaitItemBoxsTraArr[i]  = mWaitItemBoxesTra.GetChild(i).transform;
                mWaitItemObjArr[i]      = mWaitItemBoxsTraArr[i].GetChild(0).gameObject;
                mWaitItemAniArr[i]      = mWaitItemObjArr[i].GetComponent<Animator>();
                mWaitItemReadyUse[i]    = false;
                mItemsInfoArr[i]        = mItemsArr[i].GetComponent<SupportItemInformation>();
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
            int minLine = mSquaresMgr.GetLineNumber(criteriaSquareId);
            int maxLine = SQUARES_COUNT - BOARD_LINE_COUNT + minLine;

            //基準列の端(上下)を取得
            int minColumn = criteriaSquareId - minLine;
            int maxColumn = minColumn + BOARD_LINE_COUNT - 1;

            //破壊方向数の取得
            int dirCount = 0;

            //周辺8マス取得
            int[] perSquares = new int[DIRECTIONS_COUNT];
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                if (!mSquaresMgr.IsSquareSpecifiedDirection(dir, criteriaSquareId)) perSquares[(int)dir] = INT_NULL;  //端マスの場合
                else  perSquares[(int)dir] = mSquaresMgr.GetDesignatedDirectionIndex((int)dir, criteriaSquareId);     //その他
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

            //生成アイテムの待機オブジェクト表示
            int itemNum = GetGenerateItemNumber(delLine, delcolumn, dirCount, delPieceCount);
            if (itemNum != INT_NULL) StartCoroutine(SetWaitItemsActive(itemNum, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_line">      一行削除？</param>
        /// <param name="_column">    一列削除？</param>
        /// <param name="_dirCount">  削除方向数</param>
        /// <param name="_pieceCount">削除駒数</param>
        /// <returns></returns>
        int GetGenerateItemNumber(bool _line, bool _column, int _dirCount, int _pieceCount)
        {
            if (_line && _column)                          return (int)SupportItems.Star;           //星生成
            if (_column)                                   return (int)SupportItems.RocketColumn;   //ロケット(縦)生成
            if (_line)                                     return (int)SupportItems.RocketLine;     //ロケット(横)生成
            if (_dirCount >= FIREWORK_USE_DIR_PIECE_COUNT) return (int)SupportItems.Firework;       //花火
            if (_pieceCount >= DUCK_USE_DEL_PIECE_COUNT)   return (int)SupportItems.Duck;           //アヒル生成

            //生成無し
            return INT_NULL;
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
                    yield return StartCoroutine(UseDuck(mSquaresMgr.GetLineNumber(_tapSquare)));
                    break;

                //花火
                case (int)SupportItems.Firework:
                    yield return StartCoroutine(UseFirework(_tapSquare));
                    allTogether = true;
                    break;

                //ロケット(横)
                case (int)SupportItems.RocketLine:
                    yield return StartCoroutine(UseRocketLine(mSquaresMgr.GetLineNumber(_tapSquare)));
                    allTogether = true;
                    break;

                //ロケット(縦)
                case (int)SupportItems.RocketColumn:
                    yield return StartCoroutine(UseRocketColumn(mSquaresMgr.GetColumnNumber(_tapSquare)));
                    allTogether = true;
                    break;

                //星
                case (int)SupportItems.Star:
                    yield return StartCoroutine(UseStar(_tapSquare));
                    allTogether = true;
                    break;
            }

            //ギミック破壊待機
            foreach (Coroutine c in sGimmickCorList)
            { yield return c; }

            //駒破壊
            if (allTogether) yield return StartCoroutine(mPiecesMgr.StartDestroyingPieces(true));
            else StartCoroutine(mTurnMgr.TurnEnd(true));

            //星攻撃済リスト初期化
            mStarSupportedSquaresList = new List<int>();

            //アイテム使用フラグリセット
            NOW_SUPPORT_ITEM_USE = false;
        }


        //==========================================================//
        //--------------------------0アヒル-------------------------//
        //==========================================================//

        /// <summary>
        /// アヒルの使用
        /// </summary>
        /// <param name="_lineNum">指定行</param>
        /// <returns></returns>
        IEnumerator UseDuck(int _lineNum)
        {
            mDuckSupportLineNum = _lineNum;
            int itemNum = (int)SupportItems.Duck;

            //配置座標指定
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(0.0f, mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //アニメ再生
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// アヒルの援護
        /// </summary>
        /// <param name="_attackColumn">攻撃列番号</param>
        public void DuckSupport(int _attackColumn)
        {
            int squareIndex = mDuckSupportLineNum + (_attackColumn * BOARD_LINE_COUNT);
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, true);
        }


        //==========================================================//
        //---------------------------2花火--------------------------//
        //==========================================================//

        const int FIREWORK_TYPE_COUNT = 3;  //花火の種類数

        /// <summary>
        /// 花火の使用
        /// </summary>
        /// <param name="_squareId">指定マス</param>
        /// <returns></returns>
        IEnumerator UseFirework(int _squareId)
        {
            mFireworkSupportSquareId = _squareId;
            int itemNum = (int)SupportItems.Firework;
            int line    = mSquaresMgr.GetLineNumber(_squareId);
            int column  = mSquaresMgr.GetColumnNumber(_squareId);
            float posX  = mItemsInfoArr[itemNum].pos.x + SQUARE_DISTANCE * column;
            float posY  = mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * line;

            //配置座標指定
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(posX, posY);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //アニメ再生
            int type = USE_COLOR_TYPE_ARR[UnityEngine.Random.Range(0, FIREWORK_TYPE_COUNT)];
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT + type.ToString()));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// 花火の援護
        /// </summary>
        /// <param name="_place">援護場所</param>
        public void FireworkSupport(FireworkSupportPlace _place)
        {
            switch (_place)
            {
                //中心を攻撃
                case FireworkSupportPlace.Center:
                    mPiecesMgr.DamageSpecifiedSquare(mFireworkSupportSquareId, COLORLESS_ID, false, true);
                    break;

                //周辺を攻撃
                case FireworkSupportPlace.Surroundings:
                    foreach (Directions dir in Enum.GetValues(typeof(Directions)))
                    {
                        if (!mSquaresMgr.IsSquareSpecifiedDirection(dir, mFireworkSupportSquareId)) continue;
                        int square = mSquaresMgr.GetDesignatedDirectionIndex((int)dir, mFireworkSupportSquareId);
                        mPiecesMgr.DamageSpecifiedSquare(square, COLORLESS_ID, false, true, dir.ToString());
                    }
                    break;
            }
        }


        //==========================================================//
        //----------------------2ロケット(横)-----------------------//
        //==========================================================//

        /// <summary>
        /// ロケット(横)の使用
        /// </summary>
        /// <param name="_lineNum">指定行</param>
        /// <returns></returns>
        IEnumerator UseRocketLine(int _lineNum)
        {
            mRocketSupportLineNum = _lineNum;
            int itemNum = (int)SupportItems.RocketLine;

            //配置座標指定
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(0.0f, mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //アニメ再生
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// ロケット(横)の援護
        /// </summary>
        /// <param name="_supportNum">援護番号(10の位:行指定 1の位:列指定)</param>
        public void RocketLineSupport(int _supportNum)
        {
            //列指定
            int column = _supportNum % TEN;

            //ステート名
            string stateAddName = "";

            //マス指定
            int squareIndex = mRocketSupportLineNum + (column * BOARD_LINE_COUNT);
            switch (_supportNum - column)
            {
                //中央
                case (int)RocketLineType.Center:

                    //ステート名指定
                    stateAddName = Directions.Left.ToString();
                    break;

                //上段
                case (int)RocketLineType.Top:

                    //マス無しの場合は処理終了
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Up, squareIndex))
                        return;

                    //上段のマスに修正
                    squareIndex--;

                    //ステート名指定
                    stateAddName = Directions.UpLeft.ToString();
                    break;

                //下段
                case (int)RocketLineType.Under:

                    //マス無しの場合は処理終了
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Down, squareIndex))
                        return;

                    //下段のマスに修正
                    squareIndex++;

                    //ステート名指定
                    stateAddName = Directions.DownLeft.ToString();
                    break;
            }

            //マスへダメージ
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, false, true, stateAddName);
        }


        //==========================================================//
        //----------------------3ロケット(縦)-----------------------//
        //==========================================================//

        /// <summary>
        /// ロケット(縦)の使用
        /// </summary>
        /// <param name="_columnNum">指定列</param>
        /// <returns></returns>
        IEnumerator UseRocketColumn(int _columnNum)
        {
            mRocketSupportColumnNum = _columnNum;
            int itemNum = (int)SupportItems.RocketColumn;

            //配置座標指定
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(mItemsInfoArr[itemNum].pos.x + SQUARE_DISTANCE * _columnNum, 0.0f);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //アニメ再生
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// ロケット(縦)の援護
        /// </summary>
        /// <param name="_supportNum">援護番号(10の位:列指定 1の位:行指定)</param>
        public void RocketColumnSupport(int _supportNum)
        {
            //行指定
            int line = _supportNum % TEN;

            //ステート名
            string stateAddName = "";

            //マス指定
            int squareIndex = line + (mRocketSupportColumnNum * BOARD_LINE_COUNT);
            switch (_supportNum - line)
            {
                //中央
                case (int)RocketColumnType.Center:

                    //ステート名指定
                    stateAddName = Directions.Up.ToString();
                    break;

                //右列
                case (int)RocketColumnType.Right:

                    //マス無しの場合は処理終了
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Right, squareIndex))
                        return;

                    //右列のマスに修正
                    squareIndex += BOARD_LINE_COUNT;

                    //ステート名修正
                    stateAddName = Directions.UpRight.ToString();
                    break;

                //左列
                case (int)RocketColumnType.Left:

                    //マス無しの場合は処理終了
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Left, squareIndex))
                        return;

                    //左列のマスに修正
                    squareIndex -= BOARD_LINE_COUNT;

                    //ステート名修正
                    stateAddName = Directions.UpLeft.ToString();
                    break;
            }

            //マスへダメージ
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, false, true, stateAddName);
        }


        //==========================================================//
        //----------------------------4星---------------------------//
        //==========================================================//

        /// <summary>
        /// 星の使用
        /// </summary>
        /// <param name="_squareId">指定マス</param>
        /// <returns></returns>
        IEnumerator UseStar(int _squareId)
        {
            mStarSupportSquareId = _squareId;
            int itemNum = (int)SupportItems.Star;
            int line = mSquaresMgr.GetLineNumber(_squareId);
            int column = mSquaresMgr.GetColumnNumber(_squareId);
            float posX = mItemsInfoArr[itemNum].pos.x + SQUARE_DISTANCE * column;
            float posY = mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * line;

            //配置座標指定
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(posX, posY);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //アニメ再生
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// 星の援護
        /// </summary>
        /// <param name="_obj">接触オブジェクト</param>
        public void StarSupport(GameObject _obj)
        {
            //マスへダメージ
            int sqrId = mSquaresMgr.GetSquareId(_obj);

            //援護済もしくはマスが取得できなかった場合は処理をスキップ
            if (sqrId < 0 || mStarSupportedSquaresList.Contains(sqrId)) return;

            //鉄ギミックの場合
            string addStateName = "";
            int gimIndex = mGimmicksMgr.GetGimmickIndex_Obj(_obj);
            if (gimIndex >= 0)
            {
                if (sGimmickInfoArr[gimIndex].id == (int)Gimmicks.Steel)
                {
                    //飛ばす方向を指定
                    int gimSqrId   = sGimmickInfoArr[gimIndex].nowSquareId;
                    int gimLine    = mSquaresMgr.GetLineNumber(gimSqrId);
                    int gimColumn  = mSquaresMgr.GetColumnNumber(gimSqrId);
                    int starLine   = mSquaresMgr.GetLineNumber(mStarSupportSquareId);
                    int starColumn = mSquaresMgr.GetColumnNumber(mStarSupportSquareId);
                    bool up    = gimLine < starLine;
                    bool down  = gimLine > starLine;
                    bool left  = gimColumn < starColumn;
                    bool right = gimColumn > starColumn;

                    if      (up && left)    addStateName = Directions.UpLeft.ToString();    //左上
                    else if (up && right)   addStateName = Directions.UpRight.ToString();   //右上
                    else if (down && left)  addStateName = Directions.DownLeft.ToString();  //左下
                    else if (down && right) addStateName = Directions.DownRight.ToString(); //右下
                    else if (up)            addStateName = Directions.Up.ToString();        //上
                    else if (down)          addStateName = Directions.Down.ToString();      //下
                    else if (left)          addStateName = Directions.Left.ToString();      //左
                    else if (right)         addStateName = Directions.Right.ToString();     //右
                }
            }

            //援護開始
            mStarSupportedSquaresList.Add(sqrId);
            mPiecesMgr.DamageSpecifiedSquare(sqrId, COLORLESS_ID, false, true, addStateName);
        }
    }
}