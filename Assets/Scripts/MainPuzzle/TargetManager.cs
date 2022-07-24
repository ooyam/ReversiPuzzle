using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class TargetManager : MonoBehaviour
    {
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager

        [Header("�ڕW�\���{�b�N�X�̎擾")]
        [SerializeField]
        RectTransform mTexturesBoxTra;

        [Header("�ڕW�\���p�M�~�b�N�v���n�u�̎擾")]
        public TargetGimmicksArr[] mTargetGimmicksPrefabArr;
        [Serializable]
        public class TargetGimmicksArr
        { public GameObject[] prefab; }

        [Header("�ڕW�\���p��v���n�u�̎擾")]
        public TargetPiecesArr[] mTargetPiecesPrefabArr;
        [Serializable]
        public class TargetPiecesArr
        { public GameObject[] prefab; }

        RectTransform[] mTargetTraArr;
        Text[] mCountTextsArr;  //�c�萔�ʂ̕\��Text

        //==========================================================//
        //----------------------�����ݒ�,�擾-----------------------//
        //==========================================================//

        /// <summary>
        /// TargetManager�̏�����
        /// </summary>
        public void Initialize()
        {
            mPiecesMgr   = sPuzzleMain.GetPiecesManager();
            mGimmicksMgr = sPuzzleMain.GetGimmicksManager();

            //�ڕW�̕\��
            mTargetTraArr = new RectTransform[TARGETS_COUNT];
            mCountTextsArr = new Text[TARGETS_COUNT];
            for (int i = 0; i < TARGETS_COUNT; i++)
            {
                int colorId = (TARGETS_INFO_ARR[i][TARGET_INFO_COLOR] < 0) ? 0 : TARGETS_INFO_ARR[i][TARGET_INFO_COLOR];
                GameObject obj = Instantiate(mTargetGimmicksPrefabArr[TARGETS_INFO_ARR[i][TARGET_INFO_OBJ]].prefab[colorId]);
                mTargetTraArr[i] = obj.GetComponent<RectTransform>();
                mCountTextsArr[i] = mTargetTraArr[i].GetChild(0).GetComponent<Text>();
            }

        }
    }
}
