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
        Option.OptionManager GetOptionMgr()
        {
            return GameManager.sSceneType switch
            {
                GameManager.SceneType.Title  => Title.TitleMain.OptionMgr,
                GameManager.SceneType.Puzzle => PuzzleMain.PuzzleMain.OptionMgr,
                _ => null,
            };
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

        public void IsPushOption_Option()      => GetOptionMgr().IsPushOption();        //�I�v�V����
        public void IsPushBGM_Option()         => GetOptionMgr().IsPushBGM();           //BGM_ON�EOFF
        public void IsPushSE_Option()          => GetOptionMgr().IsPushSE();            //SE_ON�EOFF
        public void IsPushCredit_Option()      => GetOptionMgr().IsPushCredit();        //�N���W�b�g
        public void IsPushQuitGame_Option()    => GetOptionMgr().IsPushQuitGame();      //�Q�[������߂�
        public void IsPushYes_Option()         => GetOptionMgr().IsPushYes();           //�͂�
        public void IsPushNo_Option()          => GetOptionMgr().IsPushNo();            //������
        public void IsPushClose_Option()       => GetOptionMgr().IsPushClose();         //����
        public void IsPushNext_Option()        => GetOptionMgr().IsPushNext();          //����
        public void IsPushTutorial_Option()    => GetOptionMgr().IsPushTutorial();      //�`���[�g���A��
        public void IsPushRedo_Option()        => GetOptionMgr().IsPushRedo();          //��蒼��
        public void IsPushReturnTitle_Option() => GetOptionMgr().IsPushReturnTitle();   //�^�C�g���ɖ߂�
        public void IsPushRightArrow_Option()  => GetOptionMgr().IsPushRightArrow();    //�E���
        public void IsPushLeftArrow_Option()   => GetOptionMgr().IsPushLeftArrow();     //�����


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