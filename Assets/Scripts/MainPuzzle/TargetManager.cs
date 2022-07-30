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

        Text[] mCountTextsArr;  //�c�萔�ʂ̕\��Text

        const int TARGET_MAX = 5;           //�ڕW�ő吔
        const int TARGET_COUNT_MAX = 999;   //�ڕWk���ő吔

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
            float span = winWidth / TARGET_MAX;
            float posX = (TARGETS_COUNT + 1) / 2.0f * -span;

            //�ڕW�̕\��
            RectTransform[] targetTraArr = new RectTransform[TARGETS_COUNT];
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
                targetTraArr[i] = obj.GetComponent<RectTransform>();
                mCountTextsArr[i] = targetTraArr[i].GetChild(0).GetComponent<Text>();

                //�\��
                posX += span;
                targetTraArr[i].SetParent(mTexturesBoxTra);
                targetTraArr[i].localPosition = new Vector2(posX, 0.0f);
                int count = TARGETS_INFO_ARR[i][TARGET_INFO_COUNT];
                if (count > TARGET_COUNT_MAX) count = TARGET_COUNT_MAX;
                mCountTextsArr[i].text = count.ToString();
            }
        }

        /// <summary>
        /// �ڕW�̌����m�F
        /// </summary>
        /// <param name="_colorId">��FID</param>
        /// <param name="_gimmickId">�M�~�b�NID</param>
        public void TargetDecreaseCheck(int _colorId, int _gimmickId = INT_NULL)
        {
            for (int i = 0; i < TARGETS_COUNT; i++)
            {
                if (_gimmickId == INT_NULL)
                {
                    //��
                    if (TARGETS_INFO_ARR[i][TARGET_INFO_OBJ] < 0 &&
                        TARGETS_INFO_ARR[i][TARGET_INFO_COLOR] == _colorId)
                    {
                        TargetDecrease(i);
                    }
                }
                else
                {
                    //�M�~�b�N
                    if (TARGETS_INFO_ARR[i][TARGET_INFO_OBJ] == _gimmickId)
                    {
                        TargetDecrease(i);
                    }
                }
            }
        }

        /// <summary>
        /// �ڕW����
        /// </summary>
        /// <param name="_targetIndex">�ڕW�Ǘ��ԍ�</param>
        void TargetDecrease(int _targetIndex)
        {
            int count = int.Parse(mCountTextsArr[_targetIndex].text) - 1;
            if (count < 0) count = 0;
            mCountTextsArr[_targetIndex].text = count.ToString();
        }

        /// <summary>
        /// �ڕW�m�F
        /// </summary>
        public void TargetCheck()
        {
            //���ׂĂ̖ڕW�c���m�F
            foreach (Text countText in mCountTextsArr)
            {
                if (int.Parse(countText.text) > 0)
                {
                    //�܂��c���Ă���
                    return;
                }
            }

            //���ׂ�0�ɂȂ���
            sPuzzleMain.GameClear();
        }
    }
}
