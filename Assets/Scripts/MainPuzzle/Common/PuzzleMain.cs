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
            StartCoroutine(mResultManager.GenerateGameOverObj());
        }

        /// <summary>
        /// �Q�[���N���A
        /// </summary>
        public void GameClear()
        {
            GAME_CLEAR = true;
            StartCoroutine(mResultManager.GenerateGameClearObj());
        }

        /// <summary>
        /// �p�Y�����[�h������
        /// </summary>
        void PuzzuleInitialize()
        {
            //�M�~�b�N�̃f�[�^�x�[�X�ǂݍ���
            GetGimmicksData();

            //�X�e�[�W�ݒ�
            StageSetting();

            //ScreenTap�̏�����
            mScreenTap.Initialize();

            //SquaresManager�̏�����
            mSquaresManager.Initialize();

            //GimmicksManager�̏�����
            mGimmicksManager.Initialize();

            //PiecesManager�̏�����
            mPiecesManager.Initialize();

            //SupportItemsManager�̏�����
            mSupportItemsManager.Initialize();

            //TargetManager�̏�����
            mTargetManager.Initialize();

            //TurnManager�̏�����
            mTurnManager.Initialize();
        }

        /// <summary>
        /// SquaresManager�̎擾
        /// </summary>
        /// <returns></returns>
        public SquaresManager GetSquaresManager()
        {  return mSquaresManager; }

        /// <summary>
        /// PiecesManager�̎擾
        /// </summary>
        /// <returns></returns>
        public PiecesManager GetPiecesManager()
        {  return mPiecesManager; }

        /// <summary>
        /// GimmicksManager�̎擾
        /// </summary>
        /// <returns></returns>
        public GimmicksManager GetGimmicksManager()
        { return mGimmicksManager; }

        /// <summary>
        /// SupportItemsManager�̎擾
        /// </summary>
        /// <returns></returns>
        public SupportItemsManager GetSupportItemsManager()
        { return mSupportItemsManager; }

        /// <summary>
        /// TargetManager�̎擾
        /// </summary>
        /// <returns></returns>
        public TargetManager GetTargetManager()
        { return mTargetManager; }

        /// <summary>
        /// TurnManager�̎擾
        /// </summary>
        /// <returns></returns>
        public TurnManager GetTurnManager()
        { return mTurnManager; }

        /// <summary>
        /// ResultManager�̎擾
        /// </summary>
        /// <returns></returns>
        public ResultManager GetResultManager()
        { return mResultManager; }

        /// <summary>
        /// CanvasManager�̎擾
        /// </summary>
        /// <returns></returns>
        public CanvasManager GetCanvasManager()
        { return mCanvasManager; }

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