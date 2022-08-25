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

        public void IsPushStart_Title()      => TitleMgr.IsPushStart();         //�X�^�[�g
        public void IsPushBack_Title()       => TitleMgr.IsPushBack();          //���ǂ�
        public void IsPushRightArrow_Title() => TitleMgr.IsPushRightArrow();    //�E���
        public void IsPushLeftArrow_Title()  => TitleMgr.IsPushLeftArrow();     //�����


        //==========================================================//
        //---------------------�I�v�V�������-----------------------//
        //==========================================================//

        public void IsPushOption_Option()   => OptionMgr.IsPushOption();    //�I�v�V����
        public void IsPushBGM_Option()      => OptionMgr.IsPushBGM();       //BGM_ON�EOFF
        public void IsPushSE_Option()       => OptionMgr.IsPushSE();        //SE_ON�EOFF
        public void IsPushCredit_Option()   => OptionMgr.IsPushCredit();    //�N���W�b�g
        public void IsPushQuitGame_Option() => OptionMgr.IsPushQuitGame();  //�Q�[������߂�
        public void IsPushYes_Option()      => OptionMgr.IsPushYes();       //�͂�
        public void IsPushNo_Option()       => OptionMgr.IsPushNo();        //������
        public void IsPushClose_Option()    => OptionMgr.IsPushClose();     //����


        //==========================================================//
        //---------------------�Q�[���I�[�o�[���-------------------//
        //==========================================================//

        public void IsPushSeeAbs_GameOver()      => ResultMgr.IsPushSeeAbs();       //�L��������
        public void IsPushGiveUp_GameOver()      => ResultMgr.IsPushGiveUp();       //������߂�
        public void IsPushYes_GameOver()         => ResultMgr.IsPushYes();          //�͂�
        public void IsPushNo_GameOver()          => ResultMgr.IsPushNo();           //������
        public void IsPushClose_GameOver()       => ResultMgr.IsPushClose();        //����
        public void IsPushTryAgain_GameOver()    => ResultMgr.IsPushTryAgain();     //�Ē��킷��
        public void IsPushReturnTitle_GameOver() => ResultMgr.IsPushReturnTitle();  //�^�C�g���ɖ߂�
    }
}