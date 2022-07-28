using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class TurnManager : MonoBehaviour
    {
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager

        [Header("�^�[�����E�B���h�E(�ʏ�)��Sprite�擾")]
        [SerializeField]
        Sprite mTurnWindowNormalSpr;

        [Header("�^�[�����E�B���h�E(��)��Sprite�擾")]
        [SerializeField]
        Sprite mTurnWindowRedSpr;

        [Header("�^�[�����E�B���h�EImage�̎擾")]
        [SerializeField]
        Image mTurnWindowImg;

        [Header("�^�[����Text�̎擾")]
        [SerializeField]
        Text mNumberText;

        const int RED_LINE = 5; //�E�B���h�E��؂�ւ���^�[��
        readonly WaitForSeconds RECOVERY_DELAY_TIME = new WaitForSeconds(0.2f);   //�^�[���񕜂̊Ԋu

        //==========================================================//
        //----------------------�����ݒ�,�擾-----------------------//
        //==========================================================//

        /// <summary>
        /// TurnManager�̏�����
        /// </summary>
        public void Initialize()
        {
            mPiecesMgr   = sPuzzleMain.GetPiecesManager();
            mGimmicksMgr = sPuzzleMain.GetGimmicksManager();
            mNumberText.text = TURN_COUNT.ToString();
        }

        /// <summary>
        /// �^�[������
        /// </summary>
        public void TurnDecrease()
        {
            TURN_COUNT--;
            mNumberText.text = TURN_COUNT.ToString();
            if (TURN_COUNT <= RED_LINE) WindowChange(true);
        }

        /// <summary>
        /// �^�[����
        /// </summary>
        /// <param name="_count">�񕜐�</param>
        public IEnumerator TurnRecovery(int _count = 1)
        {
            for (int i = 0; i < _count; i++)
            {
                TURN_COUNT++;
                mNumberText.text = TURN_COUNT.ToString();
                if (TURN_COUNT > RED_LINE) WindowChange(false);
                yield return RECOVERY_DELAY_TIME;
            }
        }

        /// <summary>
        /// �E�B���h�ESprite�����ւ�
        /// </summary>
        /// <param name="_red"></param>
        void WindowChange(bool _red)
        {
            mTurnWindowImg.sprite = (_red) ? mTurnWindowRedSpr : mTurnWindowNormalSpr;
        }
    }
}