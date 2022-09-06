using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PuzzleMain.Ui;
using Ui;
using Option;
using static Sound.SoundManager;
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

        [Header("OptionManager")]
        [SerializeField]
        OptionManager mOptionManager;


        //--------Manager--------//
        public static ScreenTap             ScreenTap       { get; private set; }
        public static SquaresManager        SquaresMgr      { get; private set; }
        public static PiecesManager         PiecesMgr       { get; private set; }
        public static GimmicksManager       GimmicksMgr     { get; private set; }
        public static SupportItemsManager   SupportItemsMgr { get; private set; }
        public static TargetManager         TargetMgr       { get; private set; }
        public static TurnManager           TurnMgr         { get; private set; }
        public static ResultManager         ResultMgr       { get; private set; }
        public static CanvasManager         CanvasMgr       { get; private set; }
        public static OptionManager         OptionMgr       { get; private set; }
        //------------------------//

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
            FlagOn(PuzzleFlag.GameOver);
            StartCoroutine(ResultMgr.GenerateGameOverObj());

            //BGM終了
            StartCoroutine(BGM_FadeStop());

            //SE再生
            SE_OneShot(SE_Type.GameOver);
        }

        /// <summary>
        /// ゲームクリア
        /// </summary>
        public void GameClear()
        {
            //セーブ
            SaveDataManager.SetClearStageNum(STAGE_NUMBER);
            SaveDataManager.DataSave();
            FlagOn(PuzzleFlag.GameClear);
            StartCoroutine(ResultMgr.GenerateGameClearObj());

            //BGM終了
            StartCoroutine(BGM_FadeStop());

            //SE再生
            SE_OneShot(SE_Type.GameClear);
        }

        /// <summary>
        /// パズルモード初期化
        /// </summary>
        void PuzzuleInitialize()
        {
            //Manager取得
            ScreenTap       = mScreenTap;
            SquaresMgr      = mSquaresManager;
            PiecesMgr       = mPiecesManager;
            GimmicksMgr     = mGimmicksManager;
            SupportItemsMgr = mSupportItemsManager;
            TargetMgr       = mTargetManager;
            TurnMgr         = mTurnManager;
            ResultMgr       = mResultManager;
            CanvasMgr       = mCanvasManager;
            OptionMgr       = mOptionManager;

            //フラグリセット
            FlagReset();

            //準備中フラグセット
            FlagOn(PuzzleFlag.GamePreparation);

            //リソースデータ読み込み
            LoadResourcesData();

            //ステージ設定
            StageSetting();

            //ScreenTapの初期化
            ScreenTap.Initialize();

            //SquaresManagerの初期化
            SquaresMgr.Initialize();

            //GimmicksManagerの初期化
            GimmicksMgr.Initialize();

            //PiecesManagerの初期化
            PiecesMgr.Initialize();

            //SupportItemsManagerの初期化
            SupportItemsMgr.Initialize();

            //TargetManagerの初期化
            TargetMgr.Initialize();

            //TurnManagerの初期化
            TurnMgr.Initialize();

            //OptionManagerの初期化
            OptionMgr.Initialize();

            //BGM開始
            int bgmInt = UnityEngine.Random.Range((int)BGM_Type.Stage1, (int)BGM_Type.Count);
            BGM_FadeStart((BGM_Type)Enum.ToObject(typeof(BGM_Type), bgmInt));

            //初期化終了処理
            StartCoroutine(InitializeEnd());
        }

        /// <summary>
        /// 初期化終了処理
        /// </summary>
        public IEnumerator InitializeEnd()
        {
            //準備完了(シーンフェード終了)まで待機
            yield return new WaitWhile(() => GetFlag(PuzzleFlag.GamePreparation));

            //強制チュートリアル開始
            OptionMgr.ForcedTutorial();
        }
    }
}