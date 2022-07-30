using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static animation.AnimationManager;

namespace PuzzleMain
{
    //�ԉ΂̉���ꏊ
    public enum FireworkSupportPlace
    {
        Center = 0,     //����
        Surroundings    //����
    }

    //���P�b�g�̉���s�^�C�v(10�̈�:�s�w�� 1�̈�:��ԍ��w��)
    public enum RocketLineType
    {
        Center = 0,     //����:0�`
        Top    = 10,    //��i:10�`
        Under  = 20     //���i:20�`
    }

    //���P�b�g�̉����^�C�v(10�̈�:��w�� 1�̈�:�s�ԍ��w��)
    public enum RocketColumnType
    {
        Center = 0,     //����:0�`
        Right  = 10,    //�E��:10�`
        Left   = 20     //����:20�`
    }

    public class SupportItemsManager : MonoBehaviour
    {
        SquaresManager  mSquaresMgr;    //SquaresManager
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager
        TurnManager     mTurnMgr;       //TurnManager

        [Header("����A�C�e���̎擾")]
        [SerializeField]
        Transform mItemBoxArr;

        [Header("�⏕�A�C�e���ҋ@�{�b�N�X�̎擾")]
        [SerializeField]
        Transform mWaitItemBoxesTra;

        GameObject[] mItemsArr;                 //����A�C�e��
        SupportItemInformation[] mItemsInfoArr; //����A�C�e�����

        Transform[]  mWaitItemBoxsTraArr;       //�ҋ@����A�C�e����Transform
        GameObject[] mWaitItemObjArr;           //�ҋ@����A�C�e��
        Animator[]   mWaitItemAniArr;           //�ҋ@����A�C�e��Animator
        bool[]       mWaitItemReadyUse;         //�ҋ@����A�C�e���g�p�\���

        int mReadyItemNumber;           //�������̃A�C�e���ԍ�
        int mDuckSupportLineNum;        //�A�q���̉���s�ԍ�
        int mFireworkSupportSquareId;   //�ԉ΂̉���}�X�ԍ�
        int mRocketSupportLineNum;      //���P�b�g(��)�̉���s�ԍ�
        int mRocketSupportColumnNum;    //���P�b�g(�c)�̉����ԍ�
        int mStarSupportSquareId;       //���̉���}�X�ԍ�

        List<int> mStarSupportedSquaresList = new List<int>();  //�������삵���}�X�̕ۊ�(�����蔻��d���h�~)

        const int DUCK_USE_DEL_PIECE_COUNT = 6;     //�A�q����������
        const int FIREWORK_USE_DIR_PIECE_COUNT = 4; //�ԉΐ�������

        /// <summary>
        /// ����A�C�e���̏�����
        /// </summary>
        public void Initialize()
        {
            mSquaresMgr  = sPuzzleMain.GetSquaresManager();
            mPiecesMgr   = sPuzzleMain.GetPiecesManager();
            mGimmicksMgr = sPuzzleMain.GetGimmicksManager();
            mTurnMgr     = sPuzzleMain.GetTurnManager();

            mReadyItemNumber    = INT_NULL;
            mItemsArr           = new GameObject[SUPPORT_ITEMS_COUNT];
            mWaitItemBoxsTraArr = new Transform[SUPPORT_ITEMS_COUNT];
            mWaitItemObjArr     = new GameObject[SUPPORT_ITEMS_COUNT];
            mWaitItemAniArr     = new Animator[SUPPORT_ITEMS_COUNT];
            mWaitItemReadyUse   = new bool[SUPPORT_ITEMS_COUNT];
            mItemsInfoArr       = new SupportItemInformation[SUPPORT_ITEMS_COUNT];
            for (int i = 0; i < SUPPORT_ITEMS_COUNT; i++)
            {
                mItemsArr[i]            = mItemBoxArr.GetChild(i).gameObject;
                mWaitItemBoxsTraArr[i]  = mWaitItemBoxesTra.GetChild(i).transform;
                mWaitItemObjArr[i]      = mWaitItemBoxsTraArr[i].GetChild(0).gameObject;
                mWaitItemAniArr[i]      = mWaitItemObjArr[i].GetComponent<Animator>();
                mWaitItemReadyUse[i]    = false;
                mItemsInfoArr[i]        = mItemsArr[i].GetComponent<SupportItemInformation>();
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
            int minLine = mSquaresMgr.GetLineNumber(criteriaSquareId);
            int maxLine = SQUARES_COUNT - BOARD_LINE_COUNT + minLine;

            //���̒[(�㉺)���擾
            int minColumn = criteriaSquareId - minLine;
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

            //�����A�C�e���̑ҋ@�I�u�W�F�N�g�\��
            int itemNum = GetGenerateItemNumber(delLine, delcolumn, dirCount, delPieceCount);
            if (itemNum != INT_NULL) StartCoroutine(SetWaitItemsActive(itemNum, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_line">      ��s�폜�H</param>
        /// <param name="_column">    ���폜�H</param>
        /// <param name="_dirCount">  �폜������</param>
        /// <param name="_pieceCount">�폜�</param>
        /// <returns></returns>
        int GetGenerateItemNumber(bool _line, bool _column, int _dirCount, int _pieceCount)
        {
            if (_line && _column)                          return (int)SupportItems.Star;           //������
            if (_column)                                   return (int)SupportItems.RocketColumn;   //���P�b�g(�c)����
            if (_line)                                     return (int)SupportItems.RocketLine;     //���P�b�g(��)����
            if (_dirCount >= FIREWORK_USE_DIR_PIECE_COUNT) return (int)SupportItems.Firework;       //�ԉ�
            if (_pieceCount >= DUCK_USE_DEL_PIECE_COUNT)   return (int)SupportItems.Duck;           //�A�q������

            //��������
            return INT_NULL;
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
                    yield return StartCoroutine(UseDuck(mSquaresMgr.GetLineNumber(_tapSquare)));
                    break;

                //�ԉ�
                case (int)SupportItems.Firework:
                    yield return StartCoroutine(UseFirework(_tapSquare));
                    allTogether = true;
                    break;

                //���P�b�g(��)
                case (int)SupportItems.RocketLine:
                    yield return StartCoroutine(UseRocketLine(mSquaresMgr.GetLineNumber(_tapSquare)));
                    allTogether = true;
                    break;

                //���P�b�g(�c)
                case (int)SupportItems.RocketColumn:
                    yield return StartCoroutine(UseRocketColumn(mSquaresMgr.GetColumnNumber(_tapSquare)));
                    allTogether = true;
                    break;

                //��
                case (int)SupportItems.Star:
                    yield return StartCoroutine(UseStar(_tapSquare));
                    allTogether = true;
                    break;
            }

            //�M�~�b�N�j��ҋ@
            foreach (Coroutine c in sGimmickCorList)
            { yield return c; }

            //��j��
            if (allTogether) yield return StartCoroutine(mPiecesMgr.StartDestroyingPieces(true));
            else StartCoroutine(mTurnMgr.TurnEnd(true));

            //���U���σ��X�g������
            mStarSupportedSquaresList = new List<int>();

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
        //---------------------------2�ԉ�--------------------------//
        //==========================================================//

        const int FIREWORK_TYPE_COUNT = 3;  //�ԉ΂̎�ސ�

        /// <summary>
        /// �ԉ΂̎g�p
        /// </summary>
        /// <param name="_squareId">�w��}�X</param>
        /// <returns></returns>
        IEnumerator UseFirework(int _squareId)
        {
            mFireworkSupportSquareId = _squareId;
            int itemNum = (int)SupportItems.Firework;
            int line    = mSquaresMgr.GetLineNumber(_squareId);
            int column  = mSquaresMgr.GetColumnNumber(_squareId);
            float posX  = mItemsInfoArr[itemNum].pos.x + SQUARE_DISTANCE * column;
            float posY  = mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * line;

            //�z�u���W�w��
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(posX, posY);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //�A�j���Đ�
            int type = USE_COLOR_TYPE_ARR[UnityEngine.Random.Range(0, FIREWORK_TYPE_COUNT)];
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT + type.ToString()));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// �ԉ΂̉���
        /// </summary>
        /// <param name="_place">����ꏊ</param>
        public void FireworkSupport(FireworkSupportPlace _place)
        {
            switch (_place)
            {
                //���S���U��
                case FireworkSupportPlace.Center:
                    mPiecesMgr.DamageSpecifiedSquare(mFireworkSupportSquareId, COLORLESS_ID, false, true);
                    break;

                //���ӂ��U��
                case FireworkSupportPlace.Surroundings:
                    foreach (Directions dir in Enum.GetValues(typeof(Directions)))
                    {
                        if (!mSquaresMgr.IsSquareSpecifiedDirection(dir, mFireworkSupportSquareId)) continue;
                        int square = mSquaresMgr.GetDesignatedDirectionIndex((int)dir, mFireworkSupportSquareId);
                        mPiecesMgr.DamageSpecifiedSquare(square, COLORLESS_ID, false, true, dir.ToString());
                    }
                    break;
            }
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

            //�X�e�[�g��
            string stateAddName = "";

            //�}�X�w��
            int squareIndex = mRocketSupportLineNum + (column * BOARD_LINE_COUNT);
            switch (_supportNum - column)
            {
                //����
                case (int)RocketLineType.Center:

                    //�X�e�[�g���w��
                    stateAddName = Directions.Left.ToString();
                    break;

                //��i
                case (int)RocketLineType.Top:

                    //�}�X�����̏ꍇ�͏����I��
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Up, squareIndex))
                        return;

                    //��i�̃}�X�ɏC��
                    squareIndex--;

                    //�X�e�[�g���w��
                    stateAddName = Directions.UpLeft.ToString();
                    break;

                //���i
                case (int)RocketLineType.Under:

                    //�}�X�����̏ꍇ�͏����I��
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Down, squareIndex))
                        return;

                    //���i�̃}�X�ɏC��
                    squareIndex++;

                    //�X�e�[�g���w��
                    stateAddName = Directions.DownLeft.ToString();
                    break;
            }

            //�}�X�փ_���[�W
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, false, true, stateAddName);
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

            //�X�e�[�g��
            string stateAddName = "";

            //�}�X�w��
            int squareIndex = line + (mRocketSupportColumnNum * BOARD_LINE_COUNT);
            switch (_supportNum - line)
            {
                //����
                case (int)RocketColumnType.Center:

                    //�X�e�[�g���w��
                    stateAddName = Directions.Up.ToString();
                    break;

                //�E��
                case (int)RocketColumnType.Right:

                    //�}�X�����̏ꍇ�͏����I��
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Right, squareIndex))
                        return;

                    //�E��̃}�X�ɏC��
                    squareIndex += BOARD_LINE_COUNT;

                    //�X�e�[�g���C��
                    stateAddName = Directions.UpRight.ToString();
                    break;

                //����
                case (int)RocketColumnType.Left:

                    //�}�X�����̏ꍇ�͏����I��
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Left, squareIndex))
                        return;

                    //����̃}�X�ɏC��
                    squareIndex -= BOARD_LINE_COUNT;

                    //�X�e�[�g���C��
                    stateAddName = Directions.UpLeft.ToString();
                    break;
            }

            //�}�X�փ_���[�W
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, false, true, stateAddName);
        }


        //==========================================================//
        //----------------------------4��---------------------------//
        //==========================================================//

        /// <summary>
        /// ���̎g�p
        /// </summary>
        /// <param name="_squareId">�w��}�X</param>
        /// <returns></returns>
        IEnumerator UseStar(int _squareId)
        {
            mStarSupportSquareId = _squareId;
            int itemNum = (int)SupportItems.Star;
            int line = mSquaresMgr.GetLineNumber(_squareId);
            int column = mSquaresMgr.GetColumnNumber(_squareId);
            float posX = mItemsInfoArr[itemNum].pos.x + SQUARE_DISTANCE * column;
            float posY = mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * line;

            //�z�u���W�w��
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(posX, posY);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //�A�j���Đ�
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// ���̉���
        /// </summary>
        /// <param name="_obj">�ڐG�I�u�W�F�N�g</param>
        public void StarSupport(GameObject _obj)
        {
            //�}�X�փ_���[�W
            int sqrId = mSquaresMgr.GetSquareId(_obj);

            //����ς������̓}�X���擾�ł��Ȃ������ꍇ�͏������X�L�b�v
            if (sqrId < 0 || mStarSupportedSquaresList.Contains(sqrId)) return;

            //�S�M�~�b�N�̏ꍇ
            string addStateName = "";
            int gimIndex = mGimmicksMgr.GetGimmickIndex_Obj(_obj);
            if (gimIndex >= 0)
            {
                if (sGimmickInfoArr[gimIndex].id == (int)Gimmicks.Steel)
                {
                    //��΂��������w��
                    int gimSqrId   = sGimmickInfoArr[gimIndex].nowSquareId;
                    int gimLine    = mSquaresMgr.GetLineNumber(gimSqrId);
                    int gimColumn  = mSquaresMgr.GetColumnNumber(gimSqrId);
                    int starLine   = mSquaresMgr.GetLineNumber(mStarSupportSquareId);
                    int starColumn = mSquaresMgr.GetColumnNumber(mStarSupportSquareId);
                    bool up    = gimLine < starLine;
                    bool down  = gimLine > starLine;
                    bool left  = gimColumn < starColumn;
                    bool right = gimColumn > starColumn;

                    if      (up && left)    addStateName = Directions.UpLeft.ToString();    //����
                    else if (up && right)   addStateName = Directions.UpRight.ToString();   //�E��
                    else if (down && left)  addStateName = Directions.DownLeft.ToString();  //����
                    else if (down && right) addStateName = Directions.DownRight.ToString(); //�E��
                    else if (up)            addStateName = Directions.Up.ToString();        //��
                    else if (down)          addStateName = Directions.Down.ToString();      //��
                    else if (left)          addStateName = Directions.Left.ToString();      //��
                    else if (right)         addStateName = Directions.Right.ToString();     //�E
                }
            }

            //����J�n
            mStarSupportedSquaresList.Add(sqrId);
            mPiecesMgr.DamageSpecifiedSquare(sqrId, COLORLESS_ID, false, true, addStateName);
        }
    }
}