using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ui;
using Option;

namespace Title
{
    public class TitleMain : MonoBehaviour
    {
        //�^�C�g����ʕ\�����
        public enum TitleState
        {
            None,           //�^�C�g�����
            StageSelect,    //�X�e�[�W�I�����
        }

        [Header("TitleManager")]
        [SerializeField]
        TitleManager mTitleManager;

        [Header("OptionManager")]
        [SerializeField]
        OptionManager mOptionManager;

        [Header("CanvasManager")]
        [SerializeField]
        CanvasManager mCanvasManager;

        //----�X�^�e�B�b�N�ϐ�----//
        public static TitleManager  TitleMgr    { get; private set; }
        public static OptionManager OptionMgr   { get; private set; }
        public static CanvasManager CanvasMgr   { get; private set; }
        public static StagesData AllStagesData  { get; private set; }
        public static TitleState sTitleState;
        //-----------------------//

        /// <summary>
        /// �^�C�g���J�n
        /// </summary>
        void Awake()
        {
            //�Z�[�u�f�[�^�ǂݍ���
            SaveDataManager.DataLoad();
            sTitleState = TitleState.None;
            TitleMgr  = mTitleManager;
            OptionMgr = mOptionManager;
            CanvasMgr = mCanvasManager;

            //�^�C�g��������
            TitleMgr.Initialize();

            //�I�v�V����������
            OptionMgr.Initialize();

            //�X�e�[�W�f�[�^�擾
            AllStagesData = Resources.Load("StagesData") as StagesData;
        }
    }
}
