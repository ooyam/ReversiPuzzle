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
            FlagOn(PuzzleFlag.GameOver);
            StartCoroutine(ResultMgr.GenerateGameOverObj());

            //BGM�I��
            StartCoroutine(BGM_FadeStop());

            //SE�Đ�
            SE_OneShot(SE_Type.GameOver);
        }

        /// <summary>
        /// �Q�[���N���A
        /// </summary>
        public void GameClear()
        {
            //�Z�[�u
            SaveDataManager.SetClearStageNum(STAGE_NUMBER);
            SaveDataManager.DataSave();
            FlagOn(PuzzleFlag.GameClear);
            StartCoroutine(ResultMgr.GenerateGameClearObj());

            //BGM�I��
            StartCoroutine(BGM_FadeStop());

            //SE�Đ�
            SE_OneShot(SE_Type.GameClear);
        }

        /// <summary>
        /// �p�Y�����[�h������
        /// </summary>
        void PuzzuleInitialize()
        {
            //Manager�擾
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

            //�t���O���Z�b�g
            FlagReset();

            //�������t���O�Z�b�g
            FlagOn(PuzzleFlag.GamePreparation);

            //���\�[�X�f�[�^�ǂݍ���
            LoadResourcesData();

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

            //OptionManager�̏�����
            OptionMgr.Initialize();

            //BGM�J�n
            int bgmInt = UnityEngine.Random.Range((int)BGM_Type.Stage1, (int)BGM_Type.Count);
            BGM_FadeStart((BGM_Type)Enum.ToObject(typeof(BGM_Type), bgmInt));

            //�������I������
            StartCoroutine(InitializeEnd());
        }

        /// <summary>
        /// �������I������
        /// </summary>
        public IEnumerator InitializeEnd()
        {
            //��������(�V�[���t�F�[�h�I��)�܂őҋ@
            yield return new WaitWhile(() => GetFlag(PuzzleFlag.GamePreparation));

            //�����`���[�g���A���J�n
            OptionMgr.ForcedTutorial();
        }
    }
}