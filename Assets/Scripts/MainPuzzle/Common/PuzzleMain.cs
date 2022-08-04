#define DO_COMPLIE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuzzleMain.Ui;
using Ui;
using static PuzzleDefine;

namespace PuzzleMain
{
    public class PuzzleMain : MonoBehaviour
    {
        [Header("ScreenTap")]
        [SerializeField]
        ScreenTap mScreenTap;

        [Header("SquaresManager")]
        [SerializeField]
        SquaresManager mSquaresManager;

        [Header("PiecesManager")]
        [SerializeField]
        PiecesManager mPiecesManager;

        [Header("GimmicksManager")]
        [SerializeField]
        GimmicksManager mGimmicksManager;

        [Header("SupportItemsManager")]
        [SerializeField]
        SupportItemsManager mSupportItemsManager;

        [Header("TargetManager")]
        [SerializeField]
        TargetManager mTargetManager;

        [Header("TurnManager")]
        [SerializeField]
        TurnManager mTurnManager;

        [Header("ResultManager")]
        [SerializeField]
        ResultManager mResultManager;

        [Header("CanvasManager")]
        [SerializeField]
        CanvasManager mCanvasManager;

        //----スタティック変数----//
        public static PuzzleMain            sPuzzleMain;                                //自身のインスタンス

        public static GameObject[]          sSquareObjArr;                              //マスオブジェクト配列
        public static Transform[]           sSquareTraArr;                              //マスTransform配列
        public static int                   sNextPiecesCount;                           //待機駒の個数
        public static Transform[]           sNextPieceBoxTraArr;                        //待機駒箱Transform配列

        public static GameObject[]          sPieceObjArr;                               //駒オブジェクト配列
        public static PieceInformation[]    sPieceInfoArr;                              //駒の情報配列

        public static GameObject[]          sGimmickObjArr;                             //ギミックオブジェクト配列
        public static GimmickInformation[]  sGimmickInfoArr;                            //ギミックの情報配列
        public static List<Coroutine>       sGimmickCorList = new List<Coroutine>();    //動作中ギミックリスト
        public static List<int>             sDestroyPiecesIndexList = new List<int>();  //削除駒の管理番号リスト
        public static int                   sDestroyBasePieceIndex = 0;                 //削除駒内の基準駒(置いた駒)の格納インデックス

        public static int                   sNumberTagNextOrder = 0;                    //次に破壊する番号(番号札ギミック用)
        //------------------------//

        /// <summary>
        /// パズルモード開始
        /// </summary>
        void Awake()
        {
            //インスタンス生成
            if (sPuzzleMain == null)
            {
                sPuzzleMain = this;
            }

            //初期化開始
            PuzzuleInitialize();
        }

        /// <summary>
        /// ゲームオーバー
        /// </summary>
        public void GameOver()
        {
            GAME_OVER = true;
            StartCoroutine(mResultManager.GenerateGameOverObj());
        }

        /// <summary>
        /// ゲームクリア
        /// </summary>
        public void GameClear()
        {
            GAME_CLEAR = true;
            StartCoroutine(mResultManager.GenerateGameClearObj());
        }

        /// <summary>
        /// パズルモード初期化
        /// </summary>
        void PuzzuleInitialize()
        {
            //ギミックのデータベース読み込み
            GetGimmicksData();

            //ステージ設定
            StageSetting();

            //ScreenTapの初期化
            mScreenTap.Initialize();

            //SquaresManagerの初期化
            mSquaresManager.Initialize();

            //GimmicksManagerの初期化
            mGimmicksManager.Initialize();

            //PiecesManagerの初期化
            mPiecesManager.Initialize();

            //SupportItemsManagerの初期化
            mSupportItemsManager.Initialize();

            //TargetManagerの初期化
            mTargetManager.Initialize();

            //TurnManagerの初期化
            mTurnManager.Initialize();
        }

        /// <summary>
        /// SquaresManagerの取得
        /// </summary>
        /// <returns></returns>
        public SquaresManager GetSquaresManager()
        {  return mSquaresManager; }

        /// <summary>
        /// PiecesManagerの取得
        /// </summary>
        /// <returns></returns>
        public PiecesManager GetPiecesManager()
        {  return mPiecesManager; }

        /// <summary>
        /// GimmicksManagerの取得
        /// </summary>
        /// <returns></returns>
        public GimmicksManager GetGimmicksManager()
        { return mGimmicksManager; }

        /// <summary>
        /// SupportItemsManagerの取得
        /// </summary>
        /// <returns></returns>
        public SupportItemsManager GetSupportItemsManager()
        { return mSupportItemsManager; }

        /// <summary>
        /// TargetManagerの取得
        /// </summary>
        /// <returns></returns>
        public TargetManager GetTargetManager()
        { return mTargetManager; }

        /// <summary>
        /// TurnManagerの取得
        /// </summary>
        /// <returns></returns>
        public TurnManager GetTurnManager()
        { return mTurnManager; }

        /// <summary>
        /// ResultManagerの取得
        /// </summary>
        /// <returns></returns>
        public ResultManager GetResultManager()
        { return mResultManager; }

        /// <summary>
        /// CanvasManagerの取得
        /// </summary>
        /// <returns></returns>
        public CanvasManager GetCanvasManager()
        { return mCanvasManager; }

#if !DO_COMPLIE
        /// <summary>
        /// ゲームステートフラグセット
        /// </summary>
        /// <param name="_gameState">ステート</param>
        public static void FlagSet(GameState _gameState)
        {
            GAME_STATE = _gameState;
        }

        /// <summary>
        /// ゲームステートフラグリセット
        /// </summary>
        public static void FlagReset()
        {
            FlagSet(GameState.NONE);
        }
#endif
    }
}