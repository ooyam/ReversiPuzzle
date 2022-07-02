using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static ObjectMove_2D.ObjectMove_2D;
using static animation.AnimationManager;

namespace PuzzleMain
{
    //���P�b�g�̉���ԍ�(10�̈�:�s�w�� 1�̈�:��ԍ��w��)
    public enum RocketLineSupportNumber
    {
        Center = 0,     //����:0�`
        Top    = 10,    //��i:10�`
        Under  = 20     //���i:20�`
    }

    //���P�b�g�̉����^�C�v
    public enum RocketColumnType
    {
        Center = 0,     //����:0�`
        Right  = 10,    //�E��:10�`
        Left   = 20     //����:20�`
    }

    public class SupportItemsManager : MonoBehaviour
    {
        SquaresManager mSquaresMgr;  //SquaresManager
        PiecesManager  mPiecesMgr;   //PiecesManager

        [Header("����A�C�e���̎擾")]
        [SerializeField]
        GameObject[] mItemsArr;

        [Header("�⏕�A�C�e���ҋ@�{�b�N�X�̎擾")]
        [SerializeField]
        Transform mWaitItemBoxesTra;

        SupportItemInformation[] mItemsInfoArr; //����A�C�e�����

        Transform[]  mWaitItemBoxsTraArr;       //�ҋ@����A�C�e����Transform
        GameObject[] mWaitItemObjArr;           //�ҋ@����A�C�e��
        Animator[]   mWaitItemAniArr;           //�ҋ@����A�C�e��Animator
        bool[]       mWaitItemReadyUse;         //�ҋ@����A�C�e���g�p�\���

        int mReadyItemNumber;           //�������̃A�C�e���ԍ�
        int mDuckSupportLineNum;        //�A�q���̉���s�ԍ�
        int mRocketSupportLineNum;      //���P�b�g(��)�̉���s�ԍ�
        int mRocketSupportColumnNum;    //���P�b�g(�c)�̉����ԍ�

        const int DUCK_USE_DEL_PIECE_COUNT = 6;     //�A�q����������
        const int FIREWORK_USE_DIR_PIECE_COUNT = 4; //�ԉΐ�������

        /// <summary>
        /// ����A�C�e���̏�����
        /// </summary>
        public void Initialize()
        {
            mSquaresMgr = sPuzzleMain.GetSquaresManager();
            mPiecesMgr  = sPuzzleMain.GetPiecesManager();

            mReadyItemNumber    = INT_NULL;
            mWaitItemBoxsTraArr = new Transform[SUPPORT_ITEMS_COUNT];
            mWaitItemObjArr     = new GameObject[SUPPORT_ITEMS_COUNT];
            mWaitItemAniArr     = new Animator[SUPPORT_ITEMS_COUNT];
            mWaitItemReadyUse   = new bool[SUPPORT_ITEMS_COUNT];
            mItemsInfoArr       = new SupportItemInformation[SUPPORT_ITEMS_COUNT];
            for (int i = 0; i < SUPPORT_ITEMS_COUNT; i++)
            {
                mWaitItemBoxsTraArr[i] = mWaitItemBoxesTra.GetChild(i).transform;
                mWaitItemObjArr[i]     = mWaitItemBoxsTraArr[i].GetChild(0).gameObject;
                mWaitItemAniArr[i]     = mWaitItemObjArr[i].GetComponent<Animator>();
                mWaitItemReadyUse[i]   = false;
                mItemsInfoArr[i]       = mItemsArr[i].GetComponent<SupportItemInformation>();
                mItemsInfoArr[i].InformationSetting();
            }
        }

        /// <summary>
        /// �ҋ@�A�C�e���̃^�b�v
        /// </summary>
        /// <param name="_tapObj">�^�b�v�I�u�W�F�N�g</param>
        public void TapItem(GameObject _tapObj)
        {
            int waitItemIndex = Array.IndexOf(mWaitItemObjArr, _tapObj);
            if (waitItemIndex < 0) return;

            if (mWaitItemReadyUse[waitItemIndex])
                ResetWaitItemReady();             //��������
            else
                SetWaitItemReady(waitItemIndex);  //����
        }

        /// <summary>
        /// �ҋ@�A�C�e����������Ԃɂ���
        /// </summary>
        /// <param name="_itemNum">����A�C�e���ԍ�</param>
        public void SetWaitItemReady(int _itemNum)
        {
            //�t���O�Z�b�g
            NOW_SUPPORT_ITEM_READY = true;

            //�w��A�C�e����������Ԃ�
            mWaitItemReadyUse[_itemNum] = true;
            StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_READY));

            //�w��A�C�e���ȊO��������Ԃɂ���
            if (mReadyItemNumber >= 0)
            {
                mWaitItemReadyUse[mReadyItemNumber] = false;
                LoopAnimationStart(mWaitItemAniArr[mReadyItemNumber]);
            }

            //�������̃A�C�e���ԍ��X�V
            mReadyItemNumber = _itemNum;
        }

        /// <summary>
        /// �ҋ@�A�C�e����������Ԃ���������
        /// </summary>
        public void ResetWaitItemReady()
        {
            //�t���O���Z�b�g
            NOW_SUPPORT_ITEM_READY = false;

            //�Z�b�g���̃A�C�e�����Ȃ��ꍇ�͏������X�L�b�v
            if (mReadyItemNumber < 0) return;

            //������Ԃɂ���
            mWaitItemReadyUse[mReadyItemNumber] = false;
            LoopAnimationStart(mWaitItemAniArr[mReadyItemNumber]);

            //�������̃A�C�e���ԍ����Z�b�g
            mReadyItemNumber = INT_NULL;
        }

        /// <summary>
        /// �ҋ@����A�C�e���̕\���ؑ�
        /// </summary>
        /// <param name="_itemNum">����A�C�e���ԍ�</param>
        /// <param name="_active"> �\�����</param>
        IEnumerator SetWaitItemsActive(int _itemNum, bool _active)
        {
            //�ؑւ��K�v�Ȃ��ꍇ
            if (mWaitItemObjArr[_itemNum].activeSelf == _active) yield break;

            if (_active)
            {
                //�\��
                mWaitItemObjArr[_itemNum].SetActive(true);
                StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_ACTIVE));
            }
            else
            {
                //��\��
                yield return StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_INACTIVE));
                mWaitItemObjArr[_itemNum].SetActive(false);
            }
        }

        /// <summary>
        /// �A�C�e������
        /// </summary>
        public void GenerateItems()
        {
            //�j��I�u�W�F�N�g����̏ꍇ
            if (sDestroyPiecesIndexList.Count == 0) return;

            //���ԍ�
            int criteriaSquareId = sDestroyPiecesIndexList[sDestroyBasePieceIndex];

            //��s�̒[(���E)���擾
            int minLine = criteriaSquareId % BOARD_LINE_COUNT;
            int maxLine = SQUARES_COUNT - BOARD_LINE_COUNT + minLine;

            //���̒[(�㉺)���擾
            int minColumn = criteriaSquareId - criteriaSquareId % BOARD_LINE_COUNT;
            int maxColumn = minColumn + BOARD_LINE_COUNT - 1;

            //�j��������̎擾
            int dirCount = 0;

            //����8�}�X�擾
            int[] perSquares = new int[DIRECTIONS_COUNT];
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                if (!mSquaresMgr.IsSquareSpecifiedDirection(dir, criteriaSquareId)) perSquares[(int)dir] = INT_NULL;  //�[�}�X�̏ꍇ
                else  perSquares[(int)dir] = mSquaresMgr.GetDesignatedDirectionIndex((int)dir, criteriaSquareId);     //���̑�
            }

            //�e���݃t���O
            bool minLineFlag   = false;
            bool maxLineFlag   = false;
            bool minColumnFlag = false;
            bool maxColumnFlag = false;

            //��̐�,���݃t���O���擾
            int delPieceCount = 0;
            foreach (int i in sDestroyPiecesIndexList)
            {
                //�j��������J�E���g
                if (Array.IndexOf(perSquares, i) >= 0) dirCount++;

                //��ȊO�͏������X�L�b�v
                if (!sPieceObjArr[i].CompareTag(PIECE_TAG)) continue;

                //�s,�񔻒�
                if (minLine == i)   minLineFlag   = true;
                if (maxLine == i)   maxLineFlag   = true;
                if (minColumn == i) minColumnFlag = true;
                if (maxColumn == i) maxColumnFlag = true;

                //�j���̐��J�E���g
                delPieceCount++;
            }

            //1�s,��̔j��t���O�ݒ�
            bool delLine   = minLineFlag && maxLineFlag;
            bool delcolumn = minColumnFlag && maxColumnFlag;

            //�����A�C�e������
            if (delLine && delcolumn)
            {
                //�S����
            }
            else if (delcolumn)
            {
                //���P�b�g(�c)
            }
            else if (delLine)
            {
                //���P�b�g(��)
            }
            else if (dirCount >= FIREWORK_USE_DIR_PIECE_COUNT)
            {
                //�ԉ�
            }
            else if (delPieceCount >= DUCK_USE_DEL_PIECE_COUNT)
            {
                //�A�q��
            }

            //���ق�Ƃ�else if
            if (delcolumn)
            {
                //���P�b�g(�c)����
                StartCoroutine(SetWaitItemsActive((int)SupportItems.RocketColumn, true));
            }
            else if (delLine)
            {
                //���P�b�g(��)����
                StartCoroutine(SetWaitItemsActive((int)SupportItems.RocketLine, true));
            }
            else if (delPieceCount >= DUCK_USE_DEL_PIECE_COUNT)
            {
                //�A�q������
                StartCoroutine(SetWaitItemsActive((int)SupportItems.Duck, true));
            }
        }

        /// <summary>
        /// ����A�C�e���̕\���ؑ�
        /// </summary>
        /// <param name="_itemNum">����A�C�e���ԍ�</param>
        /// <param name="_active"> �\�����</param>
        void SetItemsActive(int _itemNum, bool _active)
        {
            mItemsArr[_itemNum].SetActive(_active);
        }

        /// <summary>
        /// ����A�C�e���̎g�p
        /// </summary>
        /// <param name="_tapSquare">�w��}�X�ԍ�</param>
        public IEnumerator UseItems(int _tapSquare)
        {
            //�A�C�e���g�p�t���O�Z�b�g
            NOW_SUPPORT_ITEM_USE = true;

            //�A�C�e���ԍ��ێ�
            int itemNum = mReadyItemNumber;

            //������ԉ���
            ResetWaitItemReady();

            //�ҋ@�A�C�e����\��
            StartCoroutine(SetWaitItemsActive(itemNum, false));

            //������ׂē����^�C�~���O�Ŕj�󂷂�
            bool allTogether = false;

            switch (itemNum)
            {
                //�A�q��
                case (int)SupportItems.Duck:
                    yield return StartCoroutine(UseDuck(_tapSquare % BOARD_LINE_COUNT));
                    break;

                //���P�b�g(��)
                case (int)SupportItems.RocketLine:
                    yield return StartCoroutine(UseRocketLine(_tapSquare % BOARD_LINE_COUNT));
                    allTogether = true;
                    break;

                //���P�b�g(�c)
                case (int)SupportItems.RocketColumn:
                    yield return StartCoroutine(UseRocketColumn(_tapSquare / BOARD_LINE_COUNT));
                    allTogether = true;
                    break;
            }

            //�M�~�b�N�j��ҋ@
            foreach (Coroutine c in sGimmickCorList)
            { yield return c; }

            //��j��
            if (allTogether) yield return StartCoroutine(mPiecesMgr.StartDestroyingPieces(true));
            else StartCoroutine(mPiecesMgr.TurnEnd(true));

            //�A�C�e���g�p�t���O���Z�b�g
            NOW_SUPPORT_ITEM_USE = false;
        }


        //==========================================================//
        //--------------------------0�A�q��-------------------------//
        //==========================================================//

        /// <summary>
        /// �A�q���̎g�p
        /// </summary>
        /// <param name="_lineNum">�w��s</param>
        /// <returns></returns>
        IEnumerator UseDuck(int _lineNum)
        {
            mDuckSupportLineNum = _lineNum;
            int itemNum = (int)SupportItems.Duck;

            //�z�u���W�w��
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(0.0f, mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //�A�j���Đ�
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// �A�q���̉���
        /// </summary>
        /// <param name="_attackColumn">�U����ԍ�</param>
        public void DuckSupport(int _attackColumn)
        {
            int squareIndex = mDuckSupportLineNum + (_attackColumn * BOARD_LINE_COUNT);
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, true);
        }


        //==========================================================//
        //----------------------2���P�b�g(��)-----------------------//
        //==========================================================//

        /// <summary>
        /// ���P�b�g(��)�̎g�p
        /// </summary>
        /// <param name="_lineNum">�w��s</param>
        /// <returns></returns>
        IEnumerator UseRocketLine(int _lineNum)
        {
            mRocketSupportLineNum = _lineNum;
            int itemNum = (int)SupportItems.RocketLine;

            //�z�u���W�w��
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(0.0f, mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //�A�j���Đ�
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// ���P�b�g(��)�̉���
        /// </summary>
        /// <param name="_supportNum">����ԍ�(10�̈�:�s�w�� 1�̈�:��w��)</param>
        public void RocketLineSupport(int _supportNum)
        {
            //��w��
            int column = _supportNum % TEN;

            //�}�X�w��
            int squareIndex = mRocketSupportLineNum + (column * BOARD_LINE_COUNT);
            switch (_supportNum - column)
            {
                //��i
                case (int)RocketLineSupportNumber.Top:

                    //�}�X�����̏ꍇ�͏����I��
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Up, squareIndex))
                        return;

                    //��i�̃}�X�ɏC��
                    squareIndex--;
                    break;

                //���i
                case (int)RocketLineSupportNumber.Under:

                    //�}�X�����̏ꍇ�͏����I��
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Down, squareIndex))
                        return;

                    //���i�̃}�X�ɏC��
                    squareIndex++;
                    break;
            }

            //�}�X�փ_���[�W
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID);
        }


        //==========================================================//
        //----------------------3���P�b�g(�c)-----------------------//
        //==========================================================//

        /// <summary>
        /// ���P�b�g(�c)�̎g�p
        /// </summary>
        /// <param name="_columnNum">�w���</param>
        /// <returns></returns>
        IEnumerator UseRocketColumn(int _columnNum)
        {
            mRocketSupportColumnNum = _columnNum;
            int itemNum = (int)SupportItems.RocketColumn;

            //�z�u���W�w��
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(mItemsInfoArr[itemNum].pos.x + SQUARE_DISTANCE * _columnNum, 0.0f);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //�A�j���Đ�
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// ���P�b�g(�c)�̉���
        /// </summary>
        /// <param name="_supportNum">����ԍ�(10�̈�:��w�� 1�̈�:�s�w��)</param>
        public void RocketColumnSupport(int _supportNum)
        {
            //�s�w��
            int line = _supportNum % TEN;

            //�}�X�w��
            int squareIndex = line + (mRocketSupportColumnNum * BOARD_LINE_COUNT);
            switch (_supportNum - line)
            {
                //�E��
                case (int)RocketColumnType.Right:

                    //�}�X�����̏ꍇ�͏����I��
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Right, squareIndex))
                        return;

                    //�E��̃}�X�ɏC��
                    squareIndex += BOARD_LINE_COUNT;
                    break;

                //����
                case (int)RocketColumnType.Left:

                    //�}�X�����̏ꍇ�͏����I��
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Left, squareIndex))
                        return;

                    //����̃}�X�ɏC��
                    squareIndex -= BOARD_LINE_COUNT;
                    break;
            }

            //�}�X�փ_���[�W
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID);
        }
    }
}