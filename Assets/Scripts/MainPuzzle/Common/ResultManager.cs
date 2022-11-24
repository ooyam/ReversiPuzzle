using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CommonDefine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static animation.AnimationManager;
using static Sound.SoundManager;

namespace PuzzleMain
{
    public class ResultManager : MonoBehaviour
    {
        [Header("�Q�[���N���A�v���n�u")]
        [SerializeField]
        GameObject mGameClearPre;

        [Header("�Q�[���I�[�o�[�v���n�u")]
        [SerializeField]
        GameObject[] mGameOverPreArr;
        enum GameOverObjType
        { WithRecovery, NoRecovery, Count }   //0:�^�[���񕜗L, 1:�{�^������

        [Header("�m�F�E�B���h�E�v���n�u")]
        [SerializeField]
        GameObject mConfirmWinPre;

        [Header("�����[�h�L���v���n�u")]
        [SerializeField]
        GameObject mAdRewardPre;

        [Header("�C���^�[�X�e�B�V�����L��")]
        [SerializeField]
        GoogleAdmobInterstitial mAdInterstitial;

        [Header("���b�Z�[�W�E�B���h�E(�{�^������)�v���n�u")]
        [SerializeField]
        GameObject mMessageWinNoBtnPre;

        [Header("���b�Z�[�W�E�B���h�E�v���n�u")]
        [SerializeField]
        GameObject mMessageWinPre;

        [Header("Result�I�u�W�F�N�g�i�[�ꏊ")]
        [SerializeField]
        RectTransform mResultTra;

        GameObject mDisplayObj;         //�\�����I�u�W�F�N�g
        GoogleAdmobReward mAdReward;    //�����[�h�L���N���X
        Coroutine mAppearanceCor;       //�o���A�j���[�V�����R���[�`��

        //==========================================================//
        //------------------------���U���g���----------------------//
        //==========================================================//

        /// <summary>
        /// �m�F�E�B���h�E�\�����
        /// </summary>
        public enum ConfirmWinStatus
        {
            TryAgain,       //�Ē��킷��H
            ReturnTitle     //�^�C�g���ɖ߂�H
        }
        [HideInInspector]
        public ConfirmWinStatus mConfirmWinState = ConfirmWinStatus.TryAgain;

        /// <summary>
        /// �Q�[���N���A
        /// </summary>
        public IEnumerator GenerateGameClearObj()
        {
            ObjectDestroy();
            mDisplayObj = Instantiate(mGameClearPre);
            yield return StartCoroutine(ObjectAppearance());
            yield return new WaitForSeconds(3.0f);

            //�C���^�[�X�e�B�V�����L���\��
            StartCoroutine(CanvasMgr.SetFilter(true));
            yield return StartCoroutine(ShowAdInterstitial());

            //�^�C�g���ֈڊ�
            ReturnTitle();
        }

        /// <summary>
        /// �Q�[���I�[�o�[
        /// </summary>
        public IEnumerator GenerateGameOverObj()
        {
            StartCoroutine(CanvasMgr.SetFilter(true));
            ObjectDestroy();

            //�p���s�\�̏ꍇ
#if UNITY_ANDROID
            mDisplayObj = Instantiate(mGameOverPreArr[
                !GetFlag(PuzzleFlag.Uncontinuable) && !GetFlag(PuzzleFlag.TurnRecovered) ?
                (int)GameOverObjType.WithRecovery : (int)GameOverObjType.NoRecovery]);
#else
            mDisplayObj = Instantiate(mGameOverPreArr[(int)GameOverObjType.NoRecovery]);
#endif
            yield return StartCoroutine(ObjectAppearance());
        }

        /// <summary>
        /// �m�F�E�B���h�E
        /// </summary>
        public void GenerateConfirmObj()
        {
            //�m�F���e�ݒ�
            string msg = "";
            switch (mConfirmWinState)
            {
                case ConfirmWinStatus.TryAgain:
                    msg = "���������ǒ��킵�܂����H";
                    break;
                case ConfirmWinStatus.ReturnTitle:
                    msg = "�^�C�g����ʂɖ߂�܂�\n��낵���ł����H";
                    break;
            }

            //�����o�ϐ��̎Q�Ƃ����݂���(�I�u�W�F�N�g������)�ꍇ�͉��ߐ������Ȃ�
            if (!mConfirmMsgText)
            {
                //�E�B���h�E����
                ObjectDestroy();
                mDisplayObj = Instantiate(mConfirmWinPre);
                mConfirmMsgText = mDisplayObj.transform.GetChild(0).GetChild(0).GetComponent<Text>();
                StartCoroutine(ObjectAppearance(false));
            }

            //�e�L�X�g���f
            mConfirmMsgText.text = msg;
        }
        Text mConfirmMsgText;   //�m�F�E�B���h�E���b�Z�[�W�e�L�X�g

        /// <summary>
        /// �I�u�W�F�N�g�o��
        /// </summary>
        /// <param name="_anime">�o���A�j���̗L��</param>
        IEnumerator ObjectAppearance(bool _anime = true)
        {
            RectTransform tra = mDisplayObj.GetComponent<RectTransform>();
            tra.SetParent(mResultTra);
            tra.localPosition = Vector3.zero;
            tra.localScale = Vector3.one;
            if (_anime)
            {
                Animator ani = mDisplayObj.GetComponent<Animator>();
                mAppearanceCor = StartCoroutine(AnimationStart(ani, STATE_NAME_APPEARANCE));
                yield return mAppearanceCor;
            }
        }

        /// <summary>
        /// �I�u�W�F�N�g�폜
        /// </summary>
        void ObjectDestroy()
        {
            if (mDisplayObj != null)
            {
                if (mAppearanceCor != null) StopCoroutine(mAppearanceCor);  //�o���A�j���[�V�������~
                Destroy(mDisplayObj);   //�I�u�W�F�N�g�j��
            }
        }


        //==========================================================//
        //------------------------�L���\��--------------------------//
        //==========================================================//

        //---����---//

        const float LOADING_MAX_TIME = 30.0f;   //�L���ǂݍ��ݍő厞��
        GameObject mMessageWaitObj;             //�ҋ@���I�u�W�F�N�g
        Text mMessageMsgText;                   //���b�Z�[�W�e�L�X�g

        /// <summary>
        /// �ǂݍ��ݒ����b�Z�[�W�\��
        /// </summary>
        void AdLoadingMessageDisplay()
        {
            //�ǂݍ��ݒ����b�Z�[�W�\��
            ObjectDestroy();
            mDisplayObj = Instantiate(mMessageWinNoBtnPre);
            StartCoroutine(ObjectAppearance(false));
            Transform winTra = mDisplayObj.transform.GetChild(0);
            mMessageMsgText = winTra.GetChild(0).GetComponent<Text>();
            mMessageMsgText.text = "�L����ǂݍ���ł��܂�\n���΂炭���҂���������\n\n";

            //�ҋ@���I�u�W�F�N�g�\��
            mMessageWaitObj = winTra.GetChild(1).gameObject;
            mMessageWaitObj.SetActive(true);
        }


        //---�C���^�[�X�e�B�V����---//

        const int SHOW_STAGE_NUM = 5;                                               //5�X�e�[�W���Ƃɕ\�����s��
        readonly WaitForSeconds MESSAGE_DISPLAY_TIME = new WaitForSeconds(2.0f);    //���b�Z�[�W�\������

        /// <summary>
        /// �C���^�[�X�e�B�V�����L���\��
        /// </summary>
        IEnumerator ShowAdInterstitial()
        {
#if !UNITY_ANDROID
            yield break;
#endif
            if (STAGE_NUMBER % SHOW_STAGE_NUM != 0) yield break;

            //�ǂݍ��ݒ����b�Z�[�W�\��
            AdLoadingMessageDisplay();

            //�ǂݍ��݊J�n
            mAdInterstitial.AdInterstitialStart();

            bool end = false;
            float waitTime = 0.0f;
            while (!end)
            {
                yield return FIXED_UPDATE;
                switch (mAdInterstitial.AdState)
                {
                    //�L���\��
                    case GoogleAdmobInterstitial.State.Showing:
                        ObjectDestroy();
                        end = true;
                        break;

                    //�ǂݍ��ݎ��s
                    case GoogleAdmobInterstitial.State.loadFailed:

                        //�L���j��
                        mAdInterstitial.OnDestroy();

                        //���b�Z�[�W�ؑ�
                        mMessageMsgText.text = "�L���̎擾�Ɏ��s���܂���\n�^�C�g���ɖ߂�܂�";
                        mMessageWaitObj.SetActive(false);
                        yield return MESSAGE_DISPLAY_TIME;

                        //�����I��
                        yield break;

                    //����
                    case GoogleAdmobInterstitial.State.Closed:

                        //���b�Z�[�W�j��
                        ObjectDestroy();

                        //�L���j��
                        mAdInterstitial.OnDestroy();

                        //�����I��
                        yield break;
                }

                //�ҋ@���Ԃ̏���𒴂����ꍇ
                if (waitTime > LOADING_MAX_TIME)
                {
                    //�L���j��
                    mAdInterstitial.OnDestroy();

                    //���b�Z�[�W�\��
                    mMessageMsgText.text = "�L���̎擾�Ɏ��s���܂���\n�^�C�g���ɖ߂�܂�";
                    mMessageWaitObj.SetActive(false);
                    yield return MESSAGE_DISPLAY_TIME;

                    //�����I��
                    yield break;
                }

                if (end) break;
                waitTime += ONE_FRAME_TIMES;
            }

            //�L���\�����I������܂őҋ@
            yield return new WaitUntil(() => mAdInterstitial.AdState == GoogleAdmobInterstitial.State.Showing);
        }


        //---------�����[�h---------//

        /// <summary>
        /// �����[�h�L���\�����
        /// </summary>
        public enum AdRewardState
        {
            None,
            Loading,        //�ǂݍ��ݒ�
            Loaded,         //�ǂݍ��݊���
            FailedToLoad,   //�ǂݍ��ݎ��s
            AdOpen,         //�L���\��
            FailedToOpen,   //�L���\�����s
            AdClosed,       //�L���I��
            EarnedReward    //��V�l��
        }
        [HideInInspector]
        public AdRewardState mRewardState = AdRewardState.None;

        /// <summary>
        /// �����[�h�L���\��
        /// </summary>
        void GenerateAdRewardObj()
        {
            //�\�����̏ꍇ�͏��������Ȃ�
            if (mRewardState != AdRewardState.None) return;

            //SE
            SE_OneShot(SE_Type.BtnYes);

            //���U���g�I�u�W�F�N�g�̍폜
            ObjectDestroy();

            //�ǂݍ��݊J�n�\��
            AdLoadingMessageDisplay();

            //�I�u�W�F�N�g����
            GameObject obj = Instantiate(mAdRewardPre);
            mAdReward = obj.GetComponent<GoogleAdmobReward>();

            //��ԊĎ��J�n
            mRewardState = AdRewardState.Loading;
            StartCoroutine(StatusMonitoringRewardAds());
        }

        /// <summary>
        /// �����[�h�L���̏�ԊĎ�
        /// </summary>
        /// <returns></returns>
        IEnumerator StatusMonitoringRewardAds()
        {
            //��ԊĎ�
            float waitTime = 0.0f;
            bool end = false;
            while (true)
            {
                switch (mRewardState)
                {
                    //�ǂݍ��݊���
                    case AdRewardState.Loaded:
                        //�L���J�n
                        mAdReward.ShowRewardAd();
                        break;

                    //��V�l�����s
                    case AdRewardState.FailedToLoad:    //�ǂݍ��ݎ��s
                    case AdRewardState.FailedToOpen:    //�L���\�����s
                        end = true;
                        break;

                    //��V�l������
                    case AdRewardState.EarnedReward:
                        StartCoroutine(TurnMgr.TurnRecovery_AdReward());
                        end = true;
                        break;
                }

                //�ҋ@���Ԃ̏���𒴂����ꍇ
                if (mRewardState != AdRewardState.AdOpen && waitTime > LOADING_MAX_TIME)
                {
                    //�ǂݍ��ݎ��s
                    mRewardState = AdRewardState.FailedToLoad;
                    end = true;
                    waitTime += ONE_FRAME_TIMES;
                }

                //��ԊĎ��I��
                if (end) break;
                yield return FIXED_UPDATE;
            }

            //���b�Z�[�W�\��
            GenerateMessageWindow();

            //�L���I�u�W�F�N�g�j��
            mAdReward.RewardOnDestroy();
        }

        /// <summary>
        /// ���b�Z�[�W�\��
        /// </summary>
        public void GenerateMessageWindow()
        {
            //�\��������w��
            string msg = "";

            //�u���ꏊ���Ȃ�
            if (GetFlag(PuzzleFlag.Uncontinuable))
            {
                msg = "���u����ꏊ������܂���\n�Q�[���I�[�o�[�ƂȂ�܂�";
            }
            //���̑�
            else
            {
                //�����[�h�L��
                switch (mRewardState)
                {
                    //��V�l�������ɍL�������
                    case AdRewardState.AdClosed:
                        msg = "�L�����L�����Z�����܂���";
                        break;

                    //�ǂݍ��݊���
                    case AdRewardState.Loaded:
                        msg = "�L����\�����܂�";
                        break;

                    //�\�����s
                    case AdRewardState.FailedToLoad:    //�ǂݍ��ݎ��s
                    case AdRewardState.FailedToOpen:    //�L���\�����s
                        msg = "�L���̕\���Ɏ��s���܂���\n������x��������������";
                        break;

                    //��V�l��
                    case AdRewardState.EarnedReward:
                        msg = "5�^�[���񕜂��܂���\n�Q�[�����ĊJ���܂�";
                        break;

                }
            }

            //�ҋ@���I�u�W�F�N�g������ or ���b�Z�[�W�E�B���h�E���o�Ă��Ȃ�
            if (mMessageWaitObj || !mMessageMsgText)
            {
                //�I�u�W�F�N�g�\��
                ObjectDestroy();
                mDisplayObj = Instantiate(mMessageWinPre);
                StartCoroutine(ObjectAppearance(false));

                //�e�L�X�g�擾
                Transform winTra = mDisplayObj.transform.GetChild(0);
                mMessageMsgText = winTra.GetChild(0).GetComponent<Text>();
            }

            //�e�L�X�g���f
            mMessageMsgText.text = msg;
        }

        /// <summary>
        /// �Q�[���ĊJ
        /// </summary>
        /// <returns></returns>
        IEnumerator GameRestart()
        {
            //�t�B���^�[�̉���
            Coroutine cor = StartCoroutine(CanvasMgr.SetFilter(false));

            //�t�B���^�[�̉����ҋ@(����{�^���̃^�b�v�ɋ�𔽉������Ȃ����߂ł�����)
            yield return null;  //1�t���[���͕K���ҋ@
            yield return cor;

            //�t���O�̃��Z�b�g
            FlagReset();
            FlagOn(PuzzleFlag.TurnRecovered);

            //BGM�ĊJ
            BGM_FadeRestart();
        }

        /// <summary>
        /// �Ē���
        /// </summary>
        /// <returns></returns>
        void TryAgain()
        {
            SceneFader.SceneChangeFade(PUZZLE_SCENE_NAME);
        }

        /// <summary>
        /// �^�C�g����
        /// </summary>
        /// <returns></returns>
        void ReturnTitle()
        {
            SceneFader.SceneChangeFade(TITLE_SCENE_NAME);
        }


        //==========================================================//
        //----------------------�{�^������--------------------------//
        //==========================================================//

        /// <summary>
        /// �͂�
        /// </summary>
        public void IsPushYes()
        {
            //SE
            SE_OneShot(SE_Type.BtnYes);

            switch (mConfirmWinState)
            {
                //�Ē��킷��H
                case ConfirmWinStatus.TryAgain:
                    TryAgain();
                    break;

                //�^�C�g���ɖ߂�H
                case ConfirmWinStatus.ReturnTitle:
                    ReturnTitle();
                    break;
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        public void IsPushNo()
        {
            //SE
            SE_OneShot(SE_Type.BtnNo);

            switch (mConfirmWinState)
            {
                //�Ē��킷��H
                case ConfirmWinStatus.TryAgain:
                    mConfirmWinState = ConfirmWinStatus.ReturnTitle;
                    GenerateConfirmObj();
                    break;

                //�^�C�g���ɖ߂�H
                case ConfirmWinStatus.ReturnTitle:
                    StartCoroutine(GenerateGameOverObj());
                    break;
            }
        }

        /// <summary>
        /// �L��������
        /// </summary>
        public void IsPushSeeAbs()
        {
            GenerateAdRewardObj();
        }

        /// <summary>
        /// ������߂�
        /// </summary>
        public void IsPushGiveUp()
        {
            //SE
            SE_OneShot(SE_Type.BtnYes);

            mConfirmWinState = ConfirmWinStatus.TryAgain;
            GenerateConfirmObj();
        }

        /// <summary>
        /// ����E�L�����Z��
        /// </summary>
        public void IsPushClose()
        {
            //SE
            SE_OneShot(SE_Type.BtnNo);

            //�u���ꏊ���Ȃ�
            if (GetFlag(PuzzleFlag.Uncontinuable))
            {
                //�Q�[���I�[�o�[
                sPuzzleMain.GameOver();
            }
            //���̑�
            else
            {
                //�����[�h�L��
                switch (mRewardState)
                {
                    case AdRewardState.FailedToLoad:    //�ǂݍ��ݎ��s
                    case AdRewardState.FailedToOpen:    //�L���\�����s
                    case AdRewardState.AdClosed:        //��V�l�������ɍL�������

                        //�Q�[���[�o�[�I�u�W�F�N�g�Đ���
                        StartCoroutine(GenerateGameOverObj());
                        mRewardState = AdRewardState.None;
                        break;

                    //��V�l��
                    case AdRewardState.EarnedReward:

                        //�Q�[���ĊJ
                        ObjectDestroy();
                        StartCoroutine(GameRestart());
                        mRewardState = AdRewardState.None;
                        break;

                    //���̑�(���邱�Ƃ͂Ȃ��͂�)
                    default:

                        //�Q�[���I�o�[�\������蒼��
                        StartCoroutine(GenerateGameOverObj());
                        mRewardState = AdRewardState.None;
                        break;
                }
            }
        }

        /// <summary>
        /// �Ē��킷��
        /// </summary>
        public void IsPushTryAgain()
        {
            //SE
            SE_OneShot(SE_Type.BtnYes);

            TryAgain();
        }

        /// <summary>
        /// �^�C�g���ɖ߂�
        /// </summary>
        public void IsPushReturnTitle()
        {
            //SE
            SE_OneShot(SE_Type.BtnYes);

            ReturnTitle();
        }
    }
}