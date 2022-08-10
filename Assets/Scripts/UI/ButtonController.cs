using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;
using static Title.TitleMain;

namespace Ui
{
    public class ButtonController : MonoBehaviour
    {
        //==========================================================//
        //-----------------------�^�C�g�����-----------------------//
        //==========================================================//

        /// <summary>
        /// �X�^�[�g
        /// </summary>
        public void IsPushStart_Title()
        {
            TitleMgr.IsPushStart();
        }

        /// <summary>
        /// ���ǂ�
        /// </summary>
        public void IsPushBack_Title()
        {
            TitleMgr.IsPushBack();
        }

        /// <summary>
        /// �E���
        /// </summary>
        public void IsPushRightArrow_Title()
        {
            TitleMgr.IsPushRightArrow();
        }

        /// <summary>
        /// �����
        /// </summary>
        public void IsPushLeftArrow_Title()
        {
            TitleMgr.IsPushLeftArrow();
        }

        
        //==========================================================//
        //---------------------�Q�[���I�[�o�[���-------------------//
        //==========================================================//

        /// <summary>
        /// �L��������
        /// </summary>
        public void IsPushSeeAbs_GameOver()
        {
            ResultMgr.IsPushSeeAbs();
        }

        /// <summary>
        /// ������߂�
        /// </summary>
        public void IsPushGiveUp_GameOver()
        {
            ResultMgr.IsPushGiveUp();
        }

        /// <summary>
        /// �͂�
        /// </summary>
        public void IsPushYes_GameOver()
        {
            ResultMgr.IsPushYes();
        }

        /// <summary>
        /// ������
        /// </summary>
        public void IsPushNo_GameOver()
        {
            ResultMgr.IsPushNo();
        }

        /// <summary>
        /// ����
        /// </summary>
        public void IsPushClose_GameOver()
        {
            ResultMgr.IsPushClose();
        }

        /// <summary>
        /// �Ē��킷��
        /// </summary>
        public void IsPushTryAgain_GameOver()
        {
            ResultMgr.IsPushTryAgain();
        }

        /// <summary>
        /// �^�C�g���ɖ߂�
        /// </summary>
        public void IsPushReturnTitle_GameOver()
        {
            ResultMgr.IsPushReturnTitle();
        }
    }
}