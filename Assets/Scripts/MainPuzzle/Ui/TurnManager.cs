using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain.Ui
{
    public class TurnManager : MonoBehaviour
    {
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

        const int RED_LINE = 5;     //�E�B���h�E��؂�ւ���^�[��
        readonly WaitForSeconds RECOVERY_DELAY_TIME = new WaitForSeconds(0.3f);   //�^�[���񕜂̊Ԋu

        //==========================================================//
        //----------------------�����ݒ�,�擾-----------------------//
        //==========================================================//

        /// <summary>
        /// TurnManager�̏�����
        /// </summary>
        public void Initialize()
        {
            mNumberText.text = TURN_COUNT.ToString();
            if (TURN_COUNT <= RED_LINE) WindowChange(true);
        }

        /// <summary>
        /// �^�[������
        /// </summary>
        public void TurnDecrease()
        {
            TurnSet(TURN_COUNT - 1);
            mNumberText.text = TURN_COUNT.ToString();
            if (TURN_COUNT <= RED_LINE) WindowChange(true);
        }

        /// <summary>
        /// �^�[����(�����[�h�L������)
        /// </summary>
        public IEnumerator TurnRecovery_AdReward()
        {
            //��
            for (int i = 0; i < REWARD_RECOVERY_COUNT; i++)
            {
                TurnSet(TURN_COUNT + 1);
                mNumberText.text = TURN_COUNT.ToString();
                if (TURN_COUNT > RED_LINE) WindowChange(false);
                yield return RECOVERY_DELAY_TIME;
            }
        }
        const int REWARD_RECOVERY_COUNT = 5;

        /// <summary>
        /// �E�B���h�ESprite�����ւ�
        /// </summary>
        /// <param name="_red"></param>
        void WindowChange(bool _red)
        {
            mTurnWindowImg.sprite = (_red) ? mTurnWindowRedSpr : mTurnWindowNormalSpr;
        }


        //==========================================================//
        //-------------------�^�[���I�����̏���---------------------//
        //==========================================================//

        /// <summary>
        /// �^�[�����m�F
        /// </summary>
        public void TurnCountCheck()
        {
            if (TURN_COUNT <= 0)
            {
                sPuzzleMain.GameOver();
            }
        }

        /// <summary>
        /// �^�[���I��
        /// </summary>
        /// <param name="supportItem">����A�C�e���H</param>
        public IEnumerator TurnEnd(bool supportItem = false)
        {
            //�^�[���I���������t���O�Z�b�g
            NOW_TURN_END_PROCESSING = true;

            //����M�~�b�N�j�󔻒�J�n
            yield return StartCoroutine(GimmicksMgr.DestroyGimmicks_TurnEnd());

            //���R����
            yield return StartCoroutine(PiecesMgr.StratFallingPieces());

            //����A�C�e�����g�p��
            if (!supportItem)
            {
                //�M�~�b�N��ԕω��J�n
                yield return StartCoroutine(GimmicksMgr.ChangeGimmickState());
            }

            //����M�~�b�N�j�󔻒�J�n
            yield return StartCoroutine(GimmicksMgr.DestroyGimmicks_TurnEnd());

            //�M�~�b�N�^�[���I������
            GimmicksMgr.ResetTurnInfo();

            //�ڕW�c���m�F
            TargetMgr.TargetCheck();

            //�^�[�����m�F
            if (!GAME_CLEAR) TurnCountCheck();

            //�^�[���I���������t���O���Z�b�g
            NOW_TURN_END_PROCESSING = false;
        }
    }
}