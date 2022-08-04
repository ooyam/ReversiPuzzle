using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;

namespace Ui
{
    public class ButtonController : MonoBehaviour
    {
        //==========================================================//
        //---------------------�Q�[���I�[�o�[���-------------------//
        //==========================================================//

        /// <summary>
        /// �L��������
        /// </summary>
        public void IsPushSeeAbs_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushSeeAbs();
        }

        /// <summary>
        /// ������߂�
        /// </summary>
        public void IsPushGiveUp_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushGiveUp();
        }

        /// <summary>
        /// �͂�
        /// </summary>
        public void IsPushYes_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushYes();
        }

        /// <summary>
        /// ������
        /// </summary>
        public void IsPushNo_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushNo();
        }

        /// <summary>
        /// ����
        /// </summary>
        public void IsPushClose_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushClose();
        }
    }
}