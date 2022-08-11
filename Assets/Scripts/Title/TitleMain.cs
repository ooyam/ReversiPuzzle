using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SaveDataManager;

namespace Title
{
    public class TitleMain : MonoBehaviour
    {
        //�^�C�g����ʕ\�����
        public enum TitleState
        {
            None,           //�^�C�g�����
            StageSelect,    //�X�e�[�W�I�����
            Option,         //�I�v�V�������
        }

        [Header("TitleManager")]
        [SerializeField]
        TitleManager mTitleManager;

        //----�X�^�e�B�b�N�ϐ�----//
        public static TitleManager TitleMgr { get; private set; }
        public static TitleState sTitleState;
        //-----------------------//

        /// <summary>
        /// �^�C�g���J�n
        /// </summary>
        void Awake()
        {
            //�Z�[�u�f�[�^�ǂݍ���
            DataLoad();
            sTitleState = TitleState.None;
            TitleMgr = mTitleManager;

            //�^�C�g��������
            TitleMgr.Initialize();
        }
    }
}
