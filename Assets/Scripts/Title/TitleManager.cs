using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CommonDefine;
using static Title.TitleMain;
using static ObjectMove_UI.ObjectMove_UI;
using static animation.AnimationManager;
using static SaveDataManager;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        //�^�C�g���ɕ\�������̐F
        enum PieceColor
        {
            Blue,   //��
            Red,    //��
            Yellow, //��
            Green,  //��
            Violet, //��
            Orange, //��
            Brack,  //��

            Count   //����
        }

        [Header("BackGround")]
        [SerializeField]
        RectTransform mBackGroundTra;


        [Header("�^�C�g����ʃv���n�u")]
        [SerializeField]
        GameObject mTitleScreenPre;

        [Header("��X�v���C�g")]
        [SerializeField]
        Sprite[] mPiecesSpr;


        [Header("�X�e�[�W��ʃv���n�u")]
        [SerializeField]
        GameObject mStageSelScreenPre;

        [Header("�X�e�[�W�{�^���v���n�u")]
        [SerializeField]
        GameObject mStageBtnPre;

        [Header("�X�e�[�W�{�^���X�v���C�g")]
        [SerializeField]
        Sprite[] mStageBtnSpr;

        //�\�����̃I�u�W�F�N�g
        GameObject mTitleScreenObj;
        GameObject mStageSelScreenObj;

        //��]�萔
        readonly Vector3 REVERSE_ROT_SPEED       = new Vector3(0.0f, 10.0f, 0.0f);          //��]���x
        readonly Vector3 REVERSE_SWITCH_ROT      = new Vector3(0.0f, 90.0f, 0.0f);          //���]����؂�ւ��p�x
        readonly Quaternion PIECE_REVERSE_QUEST  = Quaternion.Euler(0.0f, -90.0f, 0.0f);    //��̃X�v���C�g�����ւ����̊p�x
        readonly Vector3 REVERSE_FRONT_ROT       = Vector3.zero;                            //���
        readonly WaitForSeconds WAIT_TIME        = new WaitForSeconds(0.5f);                //�ҋ@����
        readonly WaitForSeconds REVERSE_INTERVAL = new WaitForSeconds(0.1f);                //���]�Ԋu
        readonly WaitForSeconds BOTTOM_WAIT      = new WaitForSeconds(0.3f);                //���J�n���̊Ԋu
        const float REVERSE_SCALING_SPEED        = 0.02f;                                   //��̊g�k���x
        const float REVERSE_CHANGE_SCALE         = 1.2f;                                    //��̊g�厞�̃X�P�[��
        const float PIECE_DEFAULT_SCALE          = 1.0f;                                    //��̋�̃X�P�[��


        //==========================================================//
        //--------------------------������--------------------------//
        //==========================================================//

        /// <summary>
        /// ������
        /// </summary>
        public void Initialize()
        {
            //�^�C�g����ʐ���
            TitleDisplay();

            //�X�e�[�W�I����ʐ���,��\��
            StageSelectDisplay();
            mStageSelScreenObj.SetActive(false);
        }


        //==========================================================//
        //--------------------�^�C�g�����--------------------------//
        //==========================================================//

        /// <summary>
        /// �^�C�g���\��
        /// </summary>
        void TitleDisplay()
        {
            //�^�C�g����ʐ���
            mTitleScreenObj = Instantiate(mTitleScreenPre);

            //���o�J�n
            StartCoroutine(PiecesMove());
        }

        /// <summary>
        /// ��o
        /// </summary>
        IEnumerator PiecesMove()
        {
            //�^�C�g����ʔz�u,�R���|�[�l���g�擾
            Animator titleAni = mTitleScreenObj.GetComponent<Animator>();
            RectTransform tra = mTitleScreenObj.GetComponent<RectTransform>();
            tra.SetParent(mBackGroundTra, false);
            tra.localScale = Vector2.one;

            //�㕔�̋�擾
            Transform topTra = tra.GetChild(0);
            int topChildCount = topTra.childCount;
            RectTransform[] topPiecesTraArr = new RectTransform[topChildCount];
            Image[] topPiecesImgArr = new Image[topChildCount];
            for (int i = 0; i < topChildCount; i++)
            {
                GameObject obj = topTra.GetChild(i).gameObject;
                topPiecesTraArr[i] = obj.GetComponent<RectTransform>();
                topPiecesImgArr[i] = obj.GetComponent<Image>();
            }

            //�����̋�擾
            Transform bottomTra = tra.GetChild(1);
            int bottomChildCount = topTra.childCount;
            RectTransform[] bottomPiecesTraArr = new RectTransform[bottomChildCount];
            Image[] bottomPiecesImgArr = new Image[bottomChildCount];
            for (int i = 0; i < bottomChildCount; i++)
            {
                GameObject obj = bottomTra.GetChild(i).gameObject;
                bottomPiecesTraArr[i] = obj.GetComponent<RectTransform>();
                bottomPiecesImgArr[i] = obj.GetComponent<Image>();
            }

            //������F�w��
            int topStartColorId = (int)PieceColor.Blue;
            int mBottomStartColorId = (int)PieceColor.Blue;

            //���[�v�J�n
            while (sTitleState == TitleState.None)
            {
                yield return WAIT_TIME;

                //�㕔�̋�]
                topStartColorId--;
                if (topStartColorId < 0) topStartColorId = (int)PieceColor.Count - 1;
                for (int i = 0; i < topPiecesTraArr.Length; i++)
                {
                    int colorId = topStartColorId + i;
                    if (colorId >= (int)PieceColor.Count) colorId -= (int)PieceColor.Count;
                    StartCoroutine(ReversingPiece(topPiecesTraArr[i], topPiecesImgArr[i], mPiecesSpr[colorId]));
                    yield return REVERSE_INTERVAL;
                }

                yield return BOTTOM_WAIT;

                //�����̋�]
                mBottomStartColorId++;
                if (mBottomStartColorId >= (int)PieceColor.Count) mBottomStartColorId = 0;
                for (int i = bottomPiecesTraArr.Length - 1; i >= 0 ; i--)
                {
                    int colorId = mBottomStartColorId + i;
                    if (colorId >= (int)PieceColor.Count) colorId -= (int)PieceColor.Count;
                    StartCoroutine(ReversingPiece(bottomPiecesTraArr[i], bottomPiecesImgArr[i], mPiecesSpr[colorId]));
                    yield return REVERSE_INTERVAL;
                }

                yield return WAIT_TIME;

                //�^�C�g���g�k
                AnimationPlay(titleAni, STATE_NAME_EFFECT);
            }
        }

        /// <summary>
        /// ��]
        /// </summary>
        /// <param name="_tra">���]���RectTransform</param>
        /// <param name="_img">���]���Image</param>
        /// <param name="_spr">���]���Sprite</param>
        /// <returns></returns>
        IEnumerator ReversingPiece(RectTransform _tra, Image _img, Sprite _spr)
        {
            //��90����],�g��
            StartCoroutine(AllScaleChange(_tra, REVERSE_SCALING_SPEED, REVERSE_CHANGE_SCALE));
            yield return StartCoroutine(RotateMovement(_tra, REVERSE_ROT_SPEED, REVERSE_SWITCH_ROT));

            //�X�v���C�g�ؑ�
            _img.sprite = _spr;
            _tra.localRotation = PIECE_REVERSE_QUEST;

            //��90����],�k��
            StartCoroutine(AllScaleChange(_tra, REVERSE_SCALING_SPEED, PIECE_DEFAULT_SCALE));
            yield return StartCoroutine(RotateMovement(_tra, REVERSE_ROT_SPEED, REVERSE_FRONT_ROT));
        }


        //==========================================================//
        //-------------------�X�e�[�W�I�����-----------------------//
        //==========================================================//

        //�X�e�[�W�I���{�^���̐F
        enum StageBtnColor
        {
            Blue,   //��
            Red,    //��
            Yellow, //��
            Green,  //��
            Violet, //��
            Orange, //��

            Count   //����
        }

        const int STAGE_BTN_WIDTH           = 130;
        const int STAGE_BTN_HEIGHT          = 130;
        const int STAGE_BTN_OFFSET_X        = 150;
        const int STAGE_BTN_OFFSET_Y        = 150;
        const int STAGE_BTN_COLUMN_COUNT    = 5;
        const int STAGE_BTN_LINE_COUNT      = 10;
        const int STAGE_PAGE_COUNT          = 5;
        const float STAGE_PAGE_MOVE_SPEED   = 0.3f;
        readonly float STAGE_BTN_POS_X      = (STAGE_BTN_COLUMN_COUNT - 1) / 2.0f * -STAGE_BTN_OFFSET_X;
        readonly float STAGE_BTN_POS_Y      = STAGE_BTN_LINE_COUNT / 2.0f * STAGE_BTN_OFFSET_Y;
        readonly int STAGE_MAX_PAGE_INDEX   = STAGE_PAGE_COUNT - 1;
        int mDisplayPage = 0;
        Coroutine mPageMoveCor; 

        //�X�e�[�W�I���I�u�W�F�N�g�̎q�I�u�W�F�N�g
        enum StageSelChild
        { Stages, RightArrow, LeftArrow, Count }
        readonly GameObject[] mStageSelChildObj = new GameObject[(int)StageSelChild.Count];
        readonly RectTransform[] mStageSelChildTra = new RectTransform[(int)StageSelChild.Count];

        //�X�e�[�W�{�^���I�u�W�F�N�g�̎q�I�u�W�F�N�g
        enum StageBtnChild
        { Number, Filter, Clear }

        /// <summary>
        /// �X�e�[�W�I����ʕ\��
        /// </summary>
        void StageSelectDisplay()
        {
            //�^�C�g�������ς̏ꍇ
            if (mStageSelScreenObj != null)
            {
                //�A�N�e�B�u��Ԃ�
                mStageSelScreenObj.SetActive(true);

                //�����\���y�[�W�w��
                mDisplayPage = ClearStageNum / (STAGE_BTN_COLUMN_COUNT * STAGE_BTN_LINE_COUNT);
                StagePageChange();

                //�ȉ������������X�L�b�v
                return;
            }

            //�X�e�[�W�I����ʐ���
            mStageSelScreenObj = Instantiate(mStageSelScreenPre);

            //�^�C�g����ʔz�u,�R���|�[�l���g�擾
            RectTransform screenTra = mStageSelScreenObj.GetComponent<RectTransform>();
            screenTra.SetParent(mBackGroundTra, false);
            screenTra.localScale = Vector2.one;
            for (int i = 0; i < (int)StageSelChild.Count; i++)
            {
                mStageSelChildObj[i] = screenTra.GetChild(i).gameObject;
                mStageSelChildTra[i] = mStageSelChildObj[i].GetComponent<RectTransform>();
            }

            //�X�e�[�W�{�^������
            int stageNumber = 0;
            for (int i = 0; i < STAGE_PAGE_COUNT; i++)
            {
                float posX = STAGE_BTN_POS_X + PLAY_SCREEN_WIDTH * i;
                for (int a = 0; a < STAGE_BTN_LINE_COUNT; a++)
                {
                    //�s���ɐF�Ɣz�u���WY�ݒ�
                    int colorId = a;
                    if (colorId >= (int)StageBtnColor.Count) colorId -= (int)StageBtnColor.Count;
                    float posY = STAGE_BTN_POS_Y - STAGE_BTN_OFFSET_Y * a;
                    for (int b = 0; b < STAGE_BTN_COLUMN_COUNT; b++)
                    {
                        //�X�e�[�W�I�u�W�F�N�g����
                        GameObject obj = Instantiate(mStageBtnPre);
                        obj.GetComponent<Image>().sprite = mStageBtnSpr[colorId];
                        RectTransform stageBtntra = obj.GetComponent<RectTransform>();

                        //�T�C�Y,���W�w��
                        stageBtntra.SetParent(mStageSelChildTra[(int)StageSelChild.Stages], false);
                        stageBtntra.sizeDelta = new Vector2(STAGE_BTN_WIDTH, STAGE_BTN_HEIGHT);
                        stageBtntra.localPosition = new Vector2(posX + STAGE_BTN_OFFSET_X * b, posY);

                        //�X�e�[�W�ԍ��X�V,����{�^���̕\��
                        stageNumber = i * STAGE_BTN_COLUMN_COUNT * STAGE_BTN_LINE_COUNT + a * STAGE_BTN_COLUMN_COUNT + b + 1;
                        stageBtntra.GetChild((int)StageBtnChild.Number).GetComponent<Text>().text = stageNumber.ToString();
                        if (stageNumber <= ClearStageNum + 1)
                        {
                            stageBtntra.GetChild((int)StageBtnChild.Filter).gameObject.SetActive(false);
                            int n = stageNumber;
                            obj.GetComponent<Button>().onClick.AddListener(() => IsPushStageBtn(n));

                            //Clear�\��
                            if (stageNumber <= ClearStageNum)
                                stageBtntra.GetChild((int)StageBtnChild.Clear).gameObject.SetActive(true);
                        }
                    }
                }
            }

            //�����\���y�[�W�w��
            mDisplayPage = ClearStageNum / (STAGE_BTN_COLUMN_COUNT * STAGE_BTN_LINE_COUNT);
            StagePageChange();
        }

        /// <summary>
        /// �X�e�[�W�{�^��������
        /// </summary>
        /// <param name="_stageNum"></param>
        void IsPushStageBtn(int _stageNum)
        {
            SceneNavigator.Instance.Change(PUZZLE_SCENE_NAME);
        }

        /// <summary>
        /// �y�[�W�ύX
        /// </summary>
        /// <returns></returns>
        void StagePageChange()
        {
            mDisplayPage = Mathf.Clamp(mDisplayPage, 0, STAGE_MAX_PAGE_INDEX);

            //�y�[�W�ړ�
            Vector2 targetPos = new Vector2(mDisplayPage * -PLAY_SCREEN_WIDTH, 0.0f);
            if (mPageMoveCor != null) StopCoroutine(mPageMoveCor);
            mPageMoveCor = StartCoroutine(DecelerationMovement(mStageSelChildTra[(int)StageSelChild.Stages], STAGE_PAGE_MOVE_SPEED, targetPos));

            //���\����ԕύX
            mStageSelChildObj[(int)StageSelChild.RightArrow].SetActive(mDisplayPage < STAGE_MAX_PAGE_INDEX);
            mStageSelChildObj[(int)StageSelChild.LeftArrow].SetActive(mDisplayPage > 0);
        }


        //==========================================================//
        //----------------------�{�^������--------------------------//
        //==========================================================//

        /// <summary>
        /// �X�^�[�g
        /// </summary>
        public void IsPushStart()
        {
            StopAllCoroutines();
            Destroy(mTitleScreenObj);
            sTitleState = TitleState.StageSelect;
            StageSelectDisplay();
        }

        /// <summary>
        /// ���ǂ�
        /// </summary>
        public void IsPushBack()
        {
            StopAllCoroutines();
            mStageSelScreenObj.SetActive(false);
            sTitleState = TitleState.None;
            TitleDisplay();
        }

        /// <summary>
        /// �E���
        /// </summary>
        public void IsPushRightArrow()
        {
            mDisplayPage++;
            StagePageChange();
        }

        /// <summary>
        /// �����
        /// </summary>
        public void IsPushLeftArrow()
        {
            mDisplayPage--;
            StagePageChange();
        }
    }
}