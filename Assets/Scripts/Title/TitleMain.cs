using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    public class TitleMain : MonoBehaviour
    {
        public enum TitleState
        {
            None,           //�^�C�g�����
            StageSelect,    //�X�e�[�W�I�����
            Option,         //�I�v�V�������
        }
        public static TitleState sTitleState = TitleState.None;

        [Header("TitleManager")]
        [SerializeField]
        TitleManager mTitleManager;
        public static TitleManager TitleMgr { get; private set; }

        void Start()
        {
            TitleMgr = mTitleManager;
            TitleMgr.Initialize();
        }
    }
}
