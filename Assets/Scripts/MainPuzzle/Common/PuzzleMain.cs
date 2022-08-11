#define DO_COMPLIE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuzzleMain.Ui;
using Ui;
using static SaveDataManager;
using static PuzzleDefine;

namespace PuzzleMain
{
    public class PuzzleMain : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField]
        GameObject mManagers;

#if !DO_COMPLIE
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
#endif

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
        //------------------------//

        //----�X�^�e�B�b�N�ϐ�----//
        public static PuzzleMain            sPuzzleMain;                                //���g�̃C���X�^���X

        public static GameObject[]          sSquareObjArr;                              //�}�X�I�u�W�F�N�g�z��
        public static Transform[]           sSquareTraArr;                              //�}�XTransform�z��
        public static int                   sNextPiecesCount;                           //�ҋ@��̌�
        public static Transform[]           sNextPieceBoxTraArr;                        //�ҋ@�Transform�z��

        public static GameObject[]          sPieceObjArr;                               //��I�u�W�F�N�g�z��
        public static PieceInformation[]    sPieceInfoArr;                              //��̏��z��

        public static GameObject[]          sGimmickObjArr;                             //�M�~�b�N�I�u�W�F�N�g�z��
        public static GimmickInformation[]  sGimmickInfoArr;                            //�M�~�b�N�̏��z��
        public static List<Coroutine>       sGimmickCorList = new List<Coroutine>();    //���쒆�M�~�b�N���X�g
        public static List<int>             sDestroyPiecesIndexList = new List<int>();  //�폜��̊Ǘ��ԍ����X�g
        public static int                   sDestroyBasePieceIndex = 0;                 //�폜����̊��(�u������)�̊i�[�C���f�b�N�X

        public static int                   sNumberTagNextOrder = 0;                    //���ɔj�󂷂�ԍ�(�ԍ��D�M�~�b�N�p)
        //------------------------//

        /// <summary>
        /// �p�Y�����[�h�J�n
        /// </summary>
        void Awake()
        {
            //�C���X�^���X����
            if (sPuzzleMain == null)
            {
                sPuzzleMain = this;
            }

            //�������J�n
            PuzzuleInitialize();
        }

        /// <summary>
        /// �Q�[���I�[�o�[
        /// </summary>
        public void GameOver()
        {
            GAME_OVER = true;
            StartCoroutine(ResultMgr.GenerateGameOverObj());
        }

        /// <summary>
        /// �Q�[���N���A
        /// </summary>
        public void GameClear()
        {
            //�Z�[�u
            DataSave(STAGE_NUMBER);
            GAME_CLEAR = true;
            StartCoroutine(ResultMgr.GenerateGameClearObj());
        }

        /// <summary>
        /// �p�Y�����[�h������
        /// </summary>
        void PuzzuleInitialize()
        {
            //Manager�擾
#if !DO_COMPLIE
            ScreenTap = mScreenTap;
            SquaresMgr = mSquaresManager;
            PiecesMgr = mPiecesManager;
            GimmicksMgr = mGimmicksManager;
            SupportItemsMgr = mSupportItemsManager;
            TargetMgr = mTargetManager;
            TurnMgr = mTurnManager;
            ResultMgr = mResultManager;
            CanvasMgr = mCanvasManager;
#endif
            ScreenTap       = mManagers.GetComponent<ScreenTap>();
            SquaresMgr      = mManagers.GetComponent<SquaresManager>();
            PiecesMgr       = mManagers.GetComponent<PiecesManager>();
            GimmicksMgr     = mManagers.GetComponent<GimmicksManager>();
            SupportItemsMgr = mManagers.GetComponent<SupportItemsManager>();
            TargetMgr       = mManagers.GetComponent<TargetManager>();
            TurnMgr         = mManagers.GetComponent<TurnManager>();
            ResultMgr       = mManagers.GetComponent<ResultManager>();
            CanvasMgr       = mManagers.GetComponent<CanvasManager>();

            //�t���O���Z�b�g
            FlagReset();

            //�M�~�b�N�̃f�[�^�x�[�X�ǂݍ���
            GetGimmicksData();

            //�X�e�[�W�ݒ�
            StageSetting();

            //ScreenTap�̏�����
            ScreenTap.Initialize();

            //SquaresManager�̏�����
            SquaresMgr.Initialize();

            //GimmicksManager�̏�����
            GimmicksMgr.Initialize();

            //PiecesManager�̏�����
            PiecesMgr.Initialize();

            //SupportItemsManager�̏�����
            SupportItemsMgr.Initialize();

            //TargetManager�̏�����
            TargetMgr.Initialize();

            //TurnManager�̏�����
            TurnMgr.Initialize();
        }

#if !DO_COMPLIE
        /// <summary>
        /// �Q�[���X�e�[�g�t���O�Z�b�g
        /// </summary>
        /// <param name="_gameState">�X�e�[�g</param>
        public static void FlagSet(GameState _gameState)
        {
            GAME_STATE = _gameState;
        }

        /// <summary>
        /// �Q�[���X�e�[�g�t���O���Z�b�g
        /// </summary>
        public static void FlagReset()
        {
            FlagSet(GameState.NONE);
        }
#endif
    }
}