using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;
using static Title.TitleMain;
using static CommonDefine;

namespace Ui
{
    public class ButtonController : MonoBehaviour
    {
        Option.OptionManager mOptionMgr;
        private void Start()
        {
            //�V�[���擾
            switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                case TITLE_SCENE_NAME:  mOptionMgr = Title.TitleMain.OptionMgr; break;
                case PUZZLE_SCENE_NAME: mOptionMgr = PuzzleMain.PuzzleMain.OptionMgr; break;
            }
        }

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

        public void IsPushOption_Option()           => mOptionMgr.IsPushOption();           //�I�v�V����
        public void IsPushBGM_Option()              => mOptionMgr.IsPushBGM();              //BGM_ON�EOFF
        public void IsPushSE_Option()               => mOptionMgr.IsPushSE();               //SE_ON�EOFF
        public void IsPushCredit_Option()           => mOptionMgr.IsPushCredit();           //�N���W�b�g
        public void IsPushQuitGame_Option()         => mOptionMgr.IsPushQuitGame();         //�Q�[������߂�
        public void IsPushYes_Option()              => mOptionMgr.IsPushYes();              //�͂�
        public void IsPushNo_Option()               => mOptionMgr.IsPushNo();               //������
        public void IsPushClose_Option()            => mOptionMgr.IsPushClose();            //����
        public void IsPushTutorial_Option()         => mOptionMgr.IsPushTutorial();         //�`���[�g���A��
        public void IsPushRedo_Option()             => mOptionMgr.IsPushRedo();             //��蒼��
        public void IsPushReturnTitle_Option()      => mOptionMgr.IsPushReturnTitle();      //�^�C�g���ɖ߂�
        public void IsPushRightArrow_Option()       => mOptionMgr.IsPushRightArrow();       //�E���
        public void IsPushLeftArrow_Option()        => mOptionMgr.IsPushLeftArrow();        //�����


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