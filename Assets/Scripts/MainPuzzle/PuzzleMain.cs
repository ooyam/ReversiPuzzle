using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }
}