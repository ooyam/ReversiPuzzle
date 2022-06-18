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

        //1ターンの破壊情報
        int  mDelPieceCount = 0;        //破壊した駒の数
        int  mDelDirCount   = 0;        //破壊した方向の数
        bool mDelLine       = false;    //1行破壊？
        bool mDelColumn     = false;    //1列破壊？

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
                mWaitItemBoxsTraArr[i]  = mWaitItemBoxesTra.GetChild(i).transform;
                mWaitItemObjArr[i]      = mWaitItemBoxsTraArr[i].GetChild(0).gameObject;
                mWaitItemAniArr[i]      = mWaitItemObjArr[i].GetComponent<Animator>();
                mWaitItemReadyUse[i]    = false;
                mItemsInfoArr[i]        = mItemsArr[i].GetComponent<SupportItemInformation>();
                mItemsInfoArr[i].InformationSetting();
            }
        }

        /// <summary>
        /// 待機アイテムのタップ判定
        /// </summary>
        /// <param name="_tapObj">タップオブジェクト</param>
        public void TapJudgmentWaitItem(GameObject _tapObj)
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
        void SetWaitItemsActive(int _itemNum, bool _active)
        {
            mWaitItemObjArr[_itemNum].SetActive(_active);

            if (_active) StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_ACTIVE));
            else         StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_INACTIVE));
        }

        /// <summary>
        /// 駒破壊情報設定
        /// </summary>
        /// <param name="_delPieceCount">駒反転数</param>
        /// <param name="_dirCount">     反転方向数</param>
        /// <param name="_line">         1行反転</param>
        /// <param name="_column">       1列反転</param>
        public void SetPieceDeleteInfomation(int _delPieceCount, int _dirCount, bool _line, bool _column)
        {
            mDelPieceCount = _delPieceCount;    
            mDelDirCount   = _dirCount;    
            mDelLine       = _line;
            mDelColumn     = _column;
        }

        /// <summary>
        /// 駒破壊情報リセット
        /// </summary>
        public void ResetPieceDeleteInfomation()
        {
            SetPieceDeleteInfomation(0, 0, false, false);
        }

        /// <summary>
        /// アイテム使用可能判定
        /// </summary>
        public void SetItems()
        {
            if (mDelLine && mDelColumn)
            {
                //全消し
            }
            else if (mDelColumn)
            {
                //ロケット(縦)
            }
            else if (mDelLine)
            {
                //ロケット(横)
            }
            else if (mDelDirCount >= FIREWORK_USE_DIR_PIECE_COUNT)
            {
                //花火
            }

            //※ほんとはelse if
            if (mDelPieceCount >= DUCK_USE_DEL_PIECE_COUNT)
            {
                //アヒル生成
                SetWaitItemsActive((int)SupportItems.Duck, true);
            }

            //駒破壊情報リセット
            ResetPieceDeleteInfomation();
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

            switch (itemNum)
            {
                //アヒル
                case (int)SupportItems.Duck:
                    yield return StartCoroutine(UseDuck(_tapSquare % BOARD_LINE_COUNT));
                    break;
            }

            //ギミック破壊待機
            foreach (Coroutine gimmickCor in sGimmickCorList)
            { yield return gimmickCor; }
            sGimmickCorList = new List<Coroutine>();

            //待機アイテム非表示
            SetWaitItemsActive(itemNum, false);

            //駒破壊開始
            StartCoroutine(piecesMgr.StartDestroyingPieces());

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
            Vector2 setPos = new Vector2(0.0f, -SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[duckNum].tra.position = setPos;

            //アニメ再生
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[duckNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(duckNum, false);

            //駒破壊開始
            yield return StartCoroutine(piecesMgr.StartDestroyingPieces());
        }

        /// <summary>
        /// アヒルの援護
        /// </summary>
        /// <param name="_attackColumn">攻撃列番号</param>
        public void DuckSupport(int _attackColumn)
        {
            int squareIndex = mDuckSupportLineNum + (_attackColumn * BOARD_LINE_COUNT);
            piecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID);
        }
    }
}