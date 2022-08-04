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
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager
        TargetManager   mTargetMgr;     //TargetManager

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
        const int TURN_MAX = 99;    //�^�[���ő吔
        readonly WaitForSeconds RECOVERY_DELAY_TIME = new WaitForSeconds(0.3f);   //�^�[���񕜂̊Ԋu

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
            mTargetMgr   = sPuzzleMain.GetTargetManager();

            if (TURN_MAX < TURN_COUNT) TURN_COUNT = TURN_MAX;
            mNumberText.text = TURN_COUNT.ToString();
            if (TURN_COUNT <= RED_LINE) WindowChange(true);
        }

        /// <summary>
        /// �^�[������
        /// </summary>
        public void TurnDecrease()
        {
            TURN_COUNT--;
            if (0 > TURN_COUNT) TURN_COUNT = 0;
            mNumberText.text = TURN_COUNT.ToString();
            if (TURN_COUNT <= RED_LINE) WindowChange(true);
        }

        /// <summary>
        /// �^�[����(�����[�h�L������)
        /// </summary>
        public IEnumerator TurnRecovery_AdReward()
        {
            //�t�B���^�[����
            sPuzzleMain.GetCanvasManager().SetFilter(false);

            //��
            for (int i = 0; i < REWARD_RECOVERY_COUNT; i++)
            {
                TURN_COUNT++;
                if (TURN_MAX < TURN_COUNT) TURN_COUNT = TURN_MAX;
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
            yield return StartCoroutine(mGimmicksMgr.DestroyGimmicks_TurnEnd());

            //���R����
            yield return StartCoroutine(mPiecesMgr.StratFallingPieces());

            //����A�C�e�����g�p��
            if (!supportItem)
            {
                //�M�~�b�N��ԕω��J�n
                yield return StartCoroutine(mGimmicksMgr.ChangeGimmickState());
            }

            //����M�~�b�N�j�󔻒�J�n
            yield return StartCoroutine(mGimmicksMgr.DestroyGimmicks_TurnEnd());

            //�M�~�b�N�^�[���I������
            mGimmicksMgr.ResetTurnInfo();

            //�ڕW�c���m�F
            mTargetMgr.TargetCheck();

            //�^�[�����m�F
            if (!GAME_CLEAR) TurnCountCheck();

            //�^�[���I���������t���O���Z�b�g
            NOW_TURN_END_PROCESSING = false;
        }
    }
}