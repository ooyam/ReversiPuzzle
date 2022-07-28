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
        [Header("�ڕW�\���{�b�N�X�̎擾")]
        [SerializeField]
        RectTransform mTexturesBoxTra;

        [Header("�ڕW�\���p�M�~�b�N�v���n�u�̎擾")]
        public TargetGimmicksArr[] mTargetGimmicksPrefabArr;
        [Serializable]
        public class TargetGimmicksArr
        { public GameObject[] prefab; }

        [Header("�ڕW�\���p��v���n�u�̎擾")]
        public GameObject[] mTargetPiecesPrefabArr;

        RectTransform[] mTargetTraArr;
        Text[] mCountTextsArr;  //�c�萔�ʂ̕\��Text

        const float TARGET_COUNT_MAX = 5; //�ڕW�ő吔

        //==========================================================//
        //----------------------�����ݒ�,�擾-----------------------//
        //==========================================================//

        /// <summary>
        /// TargetManager�̏�����
        /// </summary>
        public void Initialize()
        {
            //�\���Ԋu�̐ݒ�
            float winWidth = mTexturesBoxTra.rect.width;
            float span = winWidth / TARGET_COUNT_MAX;
            float posX = (TARGETS_COUNT + 1) / 2.0f * -span;

            //�ڕW�̕\��
            mTargetTraArr = new RectTransform[TARGETS_COUNT];
            mCountTextsArr = new Text[TARGETS_COUNT];
            for (int i = 0; i < TARGETS_COUNT; i++)
            {
                //�ڕW�e�N�X�`���[�̐���
                GameObject obj;
                if (TARGETS_INFO_ARR[i][TARGET_INFO_OBJ] < 0)
                {
                    //��
                    obj = Instantiate(mTargetPiecesPrefabArr[TARGETS_INFO_ARR[i][TARGET_INFO_COLOR]]);
                }
                else
                {
                    //�M�~�b�N
                    int colorId = (TARGETS_INFO_ARR[i][TARGET_INFO_COLOR] < 0) ? 0 : TARGETS_INFO_ARR[i][TARGET_INFO_COLOR];
                    obj = Instantiate(mTargetGimmicksPrefabArr[TARGETS_INFO_ARR[i][TARGET_INFO_OBJ]].prefab[colorId]);
                }

                //���擾
                mTargetTraArr[i] = obj.GetComponent<RectTransform>();
                mCountTextsArr[i] = mTargetTraArr[i].GetChild(0).GetComponent<Text>();

                //�\��
                posX += span;
                mTargetTraArr[i].SetParent(mTexturesBoxTra);
                mTargetTraArr[i].localPosition = new Vector2(posX, 0.0f);
                mCountTextsArr[i].text = TARGETS_INFO_ARR[i][TARGET_INFO_COUNT].ToString();
            }
        }


    }
}
