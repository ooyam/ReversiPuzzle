using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static animation.AnimationManager;

namespace PuzzleMain
{
    //ÔÎÌìê
    public enum FireworkSupportPlace
    {
        Center = 0,     //
        Surroundings    //üÍ
    }

    //PbgÌìs^Cv(10ÌÊ:swè 1ÌÊ:ñÔwè)
    public enum RocketLineType
    {
        Center = 0,     //:0`
        Top    = 10,    //ãi:10`
        Under  = 20     //ºi:20`
    }

    //PbgÌìñ^Cv(10ÌÊ:ñwè 1ÌÊ:sÔwè)
    public enum RocketColumnType
    {
        Center = 0,     //:0`
        Right  = 10,    //Eñ:10`
        Left   = 20     //¶ñ:20`
    }

    public class SupportItemsManager : MonoBehaviour
    {
        SquaresManager  mSquaresMgr;    //SquaresManager
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager

        [Header("ìACeÌæ¾")]
        [SerializeField]
        Transform mItemBoxArr;

        [Header("âACeÒ@{bNXÌæ¾")]
        [SerializeField]
        Transform mWaitItemBoxesTra;

        GameObject[] mItemsArr;                 //ìACe
        SupportItemInformation[] mItemsInfoArr; //ìACeîñ

        Transform[]  mWaitItemBoxsTraArr;       //Ò@ìACe Transform
        GameObject[] mWaitItemObjArr;           //Ò@ìACe
        Animator[]   mWaitItemAniArr;           //Ò@ìACeAnimator
        bool[]       mWaitItemReadyUse;         //Ò@ìACegpÂ\óÔ

        int mReadyItemNumber;           //õÌACeÔ
        int mDuckSupportLineNum;        //AqÌìsÔ
        int mFireworkSupportSquareId;   //ÔÎÌì}XÔ
        int mRocketSupportLineNum;      //Pbg(¡)ÌìsÔ
        int mRocketSupportColumnNum;    //Pbg(c)ÌìñÔ
        int mStarSupportSquareId;       //¯Ìì}XÔ

        List<int> mStarSupportedSquaresList = new List<int>();  //¯ªìµ½}XÌÛÇ(½è»èd¡h~)

        const int DUCK_USE_DEL_PIECE_COUNT = 6;     //Aq¶¬ð
        const int FIREWORK_USE_DIR_PIECE_COUNT = 4; //ÔÎ¶¬ð

        /// <summary>
        /// ìACeÌú»
        /// </summary>
        public void Initialize()
        {
            mSquaresMgr  = sPuzzleMain.GetSquaresManager();
            mPiecesMgr   = sPuzzleMain.GetPiecesManager();
            mGimmicksMgr = sPuzzleMain.GetGimmicksManager();

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
        /// Ò@ACeÌ^bv
        /// </summary>
        /// <param name="_tapObj">^bvIuWFNg</param>
        public void TapItem(GameObject _tapObj)
        {
            int waitItemIndex = Array.IndexOf(mWaitItemObjArr, _tapObj);
            if (waitItemIndex < 0) return;

            if (mWaitItemReadyUse[waitItemIndex])
                ResetWaitItemReady();             //õð
            else
                SetWaitItemReady(waitItemIndex);  //õ
        }

        /// <summary>
        /// Ò@ACeðõóÔÉ·é
        /// </summary>
        /// <param name="_itemNum">ìACeÔ</param>
        public void SetWaitItemReady(int _itemNum)
        {
            //tOZbg
            NOW_SUPPORT_ITEM_READY = true;

            //wèACeðõóÔÖ
            mWaitItemReadyUse[_itemNum] = true;
            StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_READY));

            //wèACeÈOððóÔÉ·é
            if (mReadyItemNumber >= 0)
            {
                mWaitItemReadyUse[mReadyItemNumber] = false;
                LoopAnimationStart(mWaitItemAniArr[mReadyItemNumber]);
            }

            //õÌACeÔXV
            mReadyItemNumber = _itemNum;
        }

        /// <summary>
        /// Ò@ACeðõóÔðð·é
        /// </summary>
        public void ResetWaitItemReady()
        {
            //tOZbg
            NOW_SUPPORT_ITEM_READY = false;

            //ZbgÌACeªÈ¢êÍðXLbv
            if (mReadyItemNumber < 0) return;

            //ðóÔÉ·é
            mWaitItemReadyUse[mReadyItemNumber] = false;
            LoopAnimationStart(mWaitItemAniArr[mReadyItemNumber]);

            //õÌACeÔZbg
            mReadyItemNumber = INT_NULL;
        }

        /// <summary>
        /// Ò@ìACeÌ\¦ØÖ
        /// </summary>
        /// <param name="_itemNum">ìACeÔ</param>
        /// <param name="_active"> \¦óÔ</param>
        IEnumerator SetWaitItemsActive(int _itemNum, bool _active)
        {
            //ØÖªKvÈ¢ê
            if (mWaitItemObjArr[_itemNum].activeSelf == _active) yield break;

            if (_active)
            {
                //\¦
                mWaitItemObjArr[_itemNum].SetActive(true);
                StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_ACTIVE));
            }
            else
            {
                //ñ\¦
                yield return StartCoroutine(AnimationStart(mWaitItemAniArr[_itemNum], STATE_NAME_INACTIVE));
                mWaitItemObjArr[_itemNum].SetActive(false);
            }
        }

        /// <summary>
        /// ACe¶¬
        /// </summary>
        public void GenerateItems()
        {
            //jóIuWFNgªóÌê
            if (sDestroyPiecesIndexList.Count == 0) return;

            //îîÔ
            int criteriaSquareId = sDestroyPiecesIndexList[sDestroyBasePieceIndex];

            //îsÌ[(¶E)ðæ¾
            int minLine = mSquaresMgr.GetLineNumber(criteriaSquareId);
            int maxLine = SQUARES_COUNT - BOARD_LINE_COUNT + minLine;

            //îñÌ[(ãº)ðæ¾
            int minColumn = criteriaSquareId - minLine;
            int maxColumn = minColumn + BOARD_LINE_COUNT - 1;

            //jóûüÌæ¾
            int dirCount = 0;

            //üÓ8}Xæ¾
            int[] perSquares = new int[DIRECTIONS_COUNT];
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                if (!mSquaresMgr.IsSquareSpecifiedDirection(dir, criteriaSquareId)) perSquares[(int)dir] = INT_NULL;  //[}XÌê
                else  perSquares[(int)dir] = mSquaresMgr.GetDesignatedDirectionIndex((int)dir, criteriaSquareId);     //»Ì¼
            }

            //e¶ÝtO
            bool minLineFlag   = false;
            bool maxLineFlag   = false;
            bool minColumnFlag = false;
            bool maxColumnFlag = false;

            //îÌ,¶ÝtOðæ¾
            int delPieceCount = 0;
            foreach (int i in sDestroyPiecesIndexList)
            {
                //jóûüJEg
                if (Array.IndexOf(perSquares, i) >= 0) dirCount++;

                //îÈOÍðXLbv
                if (!sPieceObjArr[i].CompareTag(PIECE_TAG)) continue;

                //s,ñ»è
                if (minLine == i)   minLineFlag   = true;
                if (maxLine == i)   maxLineFlag   = true;
                if (minColumn == i) minColumnFlag = true;
                if (maxColumn == i) maxColumnFlag = true;

                //jóîÌJEg
                delPieceCount++;
            }

            //1s,ñÌjótOÝè
            bool delLine   = minLineFlag && maxLineFlag;
            bool delcolumn = minColumnFlag && maxColumnFlag;

            //¶¬ACeÌÒ@IuWFNg\¦
            int itemNum = GetGenerateItemNumber(delLine, delcolumn, dirCount, delPieceCount);
            if (itemNum != INT_NULL) StartCoroutine(SetWaitItemsActive(itemNum, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_line">      êsíH</param>
        /// <param name="_column">    êñíH</param>
        /// <param name="_dirCount">  íûü</param>
        /// <param name="_pieceCount">íî</param>
        /// <returns></returns>
        int GetGenerateItemNumber(bool _line, bool _column, int _dirCount, int _pieceCount)
        {
            if (_line && _column)                          return (int)SupportItems.Star;           //¯¶¬
            if (_column)                                   return (int)SupportItems.RocketColumn;   //Pbg(c)¶¬
            if (_line)                                     return (int)SupportItems.RocketLine;     //Pbg(¡)¶¬
            if (_dirCount >= FIREWORK_USE_DIR_PIECE_COUNT) return (int)SupportItems.Firework;       //ÔÎ
            if (_pieceCount >= DUCK_USE_DEL_PIECE_COUNT)   return (int)SupportItems.Duck;           //Aq¶¬

            //¶¬³µ
            return INT_NULL;
        }

        /// <summary>
        /// ìACeÌ\¦ØÖ
        /// </summary>
        /// <param name="_itemNum">ìACeÔ</param>
        /// <param name="_active"> \¦óÔ</param>
        void SetItemsActive(int _itemNum, bool _active)
        {
            mItemsArr[_itemNum].SetActive(_active);
        }

        /// <summary>
        /// ìACeÌgp
        /// </summary>
        /// <param name="_tapSquare">wè}XÔ</param>
        public IEnumerator UseItems(int _tapSquare)
        {
            //ACegptOZbg
            NOW_SUPPORT_ITEM_USE = true;

            //ACeÔÛ
            int itemNum = mReadyItemNumber;

            //õóÔð
            ResetWaitItemReady();

            //Ò@ACeñ\¦
            StartCoroutine(SetWaitItemsActive(itemNum, false));

            //îð·×Ä¯¶^C~OÅjó·é
            bool allTogether = false;

            switch (itemNum)
            {
                //Aq
                case (int)SupportItems.Duck:
                    yield return StartCoroutine(UseDuck(mSquaresMgr.GetLineNumber(_tapSquare)));
                    break;

                //ÔÎ
                case (int)SupportItems.Firework:
                    yield return StartCoroutine(UseFirework(_tapSquare));
                    allTogether = true;
                    break;

                //Pbg(¡)
                case (int)SupportItems.RocketLine:
                    yield return StartCoroutine(UseRocketLine(mSquaresMgr.GetLineNumber(_tapSquare)));
                    allTogether = true;
                    break;

                //Pbg(c)
                case (int)SupportItems.RocketColumn:
                    yield return StartCoroutine(UseRocketColumn(mSquaresMgr.GetColumnNumber(_tapSquare)));
                    allTogether = true;
                    break;

                //¯
                case (int)SupportItems.Star:
                    yield return StartCoroutine(UseStar(_tapSquare));
                    allTogether = true;
                    break;
            }

            //M~bNjóÒ@
            foreach (Coroutine c in sGimmickCorList)
            { yield return c; }

            //îjó
            if (allTogether) yield return StartCoroutine(mPiecesMgr.StartDestroyingPieces(true));
            else StartCoroutine(mPiecesMgr.TurnEnd(true));

            //¯UÏXgú»
            mStarSupportedSquaresList = new List<int>();

            //ACegptOZbg
            NOW_SUPPORT_ITEM_USE = false;
        }


        //==========================================================//
        //--------------------------0Aq-------------------------//
        //==========================================================//

        /// <summary>
        /// AqÌgp
        /// </summary>
        /// <param name="_lineNum">wès</param>
        /// <returns></returns>
        IEnumerator UseDuck(int _lineNum)
        {
            mDuckSupportLineNum = _lineNum;
            int itemNum = (int)SupportItems.Duck;

            //zuÀWwè
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(0.0f, mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //AjÄ¶
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// AqÌì
        /// </summary>
        /// <param name="_attackColumn">UñÔ</param>
        public void DuckSupport(int _attackColumn)
        {
            int squareIndex = mDuckSupportLineNum + (_attackColumn * BOARD_LINE_COUNT);
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, true);
        }


        //==========================================================//
        //---------------------------2ÔÎ--------------------------//
        //==========================================================//

        const int FIREWORK_TYPE_COUNT = 3;  //ÔÎÌíÞ

        /// <summary>
        /// ÔÎÌgp
        /// </summary>
        /// <param name="_squareId">wè}X</param>
        /// <returns></returns>
        IEnumerator UseFirework(int _squareId)
        {
            mFireworkSupportSquareId = _squareId;
            int itemNum = (int)SupportItems.Firework;
            int line    = mSquaresMgr.GetLineNumber(_squareId);
            int column  = mSquaresMgr.GetColumnNumber(_squareId);
            float posX  = mItemsInfoArr[itemNum].pos.x + SQUARE_DISTANCE * column;
            float posY  = mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * line;

            //zuÀWwè
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(posX, posY);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //AjÄ¶
            int type = USE_COLOR_TYPE_ARR[UnityEngine.Random.Range(0, FIREWORK_TYPE_COUNT)];
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT + type.ToString()));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// ÔÎÌì
        /// </summary>
        /// <param name="_place">ìê</param>
        public void FireworkSupport(FireworkSupportPlace _place)
        {
            switch (_place)
            {
                //SðU
                case FireworkSupportPlace.Center:
                    mPiecesMgr.DamageSpecifiedSquare(mFireworkSupportSquareId, COLORLESS_ID, false, true);
                    break;

                //üÓðU
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
        //----------------------2Pbg(¡)-----------------------//
        //==========================================================//

        /// <summary>
        /// Pbg(¡)Ìgp
        /// </summary>
        /// <param name="_lineNum">wès</param>
        /// <returns></returns>
        IEnumerator UseRocketLine(int _lineNum)
        {
            mRocketSupportLineNum = _lineNum;
            int itemNum = (int)SupportItems.RocketLine;

            //zuÀWwè
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(0.0f, mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * _lineNum);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //AjÄ¶
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// Pbg(¡)Ìì
        /// </summary>
        /// <param name="_supportNum">ìÔ(10ÌÊ:swè 1ÌÊ:ñwè)</param>
        public void RocketLineSupport(int _supportNum)
        {
            //ñwè
            int column = _supportNum % TEN;

            //Xe[g¼
            string stateAddName = "";

            //}Xwè
            int squareIndex = mRocketSupportLineNum + (column * BOARD_LINE_COUNT);
            switch (_supportNum - column)
            {
                //
                case (int)RocketLineType.Center:

                    //Xe[g¼wè
                    stateAddName = Directions.Left.ToString();
                    break;

                //ãi
                case (int)RocketLineType.Top:

                    //}X³µÌêÍI¹
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Up, squareIndex))
                        return;

                    //ãiÌ}XÉC³
                    squareIndex--;

                    //Xe[g¼wè
                    stateAddName = Directions.UpLeft.ToString();
                    break;

                //ºi
                case (int)RocketLineType.Under:

                    //}X³µÌêÍI¹
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Down, squareIndex))
                        return;

                    //ºiÌ}XÉC³
                    squareIndex++;

                    //Xe[g¼wè
                    stateAddName = Directions.DownLeft.ToString();
                    break;
            }

            //}XÖ_[W
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, false, true, stateAddName);
        }


        //==========================================================//
        //----------------------3Pbg(c)-----------------------//
        //==========================================================//

        /// <summary>
        /// Pbg(c)Ìgp
        /// </summary>
        /// <param name="_columnNum">wèñ</param>
        /// <returns></returns>
        IEnumerator UseRocketColumn(int _columnNum)
        {
            mRocketSupportColumnNum = _columnNum;
            int itemNum = (int)SupportItems.RocketColumn;

            //zuÀWwè
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(mItemsInfoArr[itemNum].pos.x + SQUARE_DISTANCE * _columnNum, 0.0f);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //AjÄ¶
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// Pbg(c)Ìì
        /// </summary>
        /// <param name="_supportNum">ìÔ(10ÌÊ:ñwè 1ÌÊ:swè)</param>
        public void RocketColumnSupport(int _supportNum)
        {
            //swè
            int line = _supportNum % TEN;

            //Xe[g¼
            string stateAddName = "";

            //}Xwè
            int squareIndex = line + (mRocketSupportColumnNum * BOARD_LINE_COUNT);
            switch (_supportNum - line)
            {
                //
                case (int)RocketColumnType.Center:

                    //Xe[g¼wè
                    stateAddName = Directions.Up.ToString();
                    break;

                //Eñ
                case (int)RocketColumnType.Right:

                    //}X³µÌêÍI¹
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Right, squareIndex))
                        return;

                    //EñÌ}XÉC³
                    squareIndex += BOARD_LINE_COUNT;

                    //Xe[g¼C³
                    stateAddName = Directions.UpRight.ToString();
                    break;

                //¶ñ
                case (int)RocketColumnType.Left:

                    //}X³µÌêÍI¹
                    if (!mSquaresMgr.IsSquareSpecifiedDirection(Directions.Left, squareIndex))
                        return;

                    //¶ñÌ}XÉC³
                    squareIndex -= BOARD_LINE_COUNT;

                    //Xe[g¼C³
                    stateAddName = Directions.UpLeft.ToString();
                    break;
            }

            //}XÖ_[W
            mPiecesMgr.DamageSpecifiedSquare(squareIndex, COLORLESS_ID, false, true, stateAddName);
        }


        //==========================================================//
        //----------------------------4¯---------------------------//
        //==========================================================//

        /// <summary>
        /// ¯Ìgp
        /// </summary>
        /// <param name="_squareId">wè}X</param>
        /// <returns></returns>
        IEnumerator UseStar(int _squareId)
        {
            mStarSupportSquareId = _squareId;
            int itemNum = (int)SupportItems.Star;
            int line = mSquaresMgr.GetLineNumber(_squareId);
            int column = mSquaresMgr.GetColumnNumber(_squareId);
            float posX = mItemsInfoArr[itemNum].pos.x + SQUARE_DISTANCE * column;
            float posY = mItemsInfoArr[itemNum].pos.y - SQUARE_DISTANCE * line;

            //zuÀWwè
            SetItemsActive(itemNum, true);
            Vector2 setPos = new Vector2(posX, posY);
            mItemsInfoArr[itemNum].tra.localPosition = setPos;

            //AjÄ¶
            yield return StartCoroutine(AnimationStart(mItemsInfoArr[itemNum].ani, STATE_NAME_SUPPORT));
            SetItemsActive(itemNum, false);
        }

        /// <summary>
        /// ¯Ìì
        /// </summary>
        /// <param name="_obj">ÚGIuWFNg</param>
        public void StarSupport(GameObject _obj)
        {
            //}XÖ_[W
            int sqrId = mSquaresMgr.GetSquareId(_obj);

            //ìÏàµ­Í}Xªæ¾Å«È©Á½êÍðXLbv
            if (sqrId < 0 || mStarSupportedSquaresList.Contains(sqrId)) return;

            //SM~bNÌê
            string addStateName = "";
            int gimIndex = mGimmicksMgr.GetGimmickIndex_Obj(_obj);
            if (gimIndex >= 0)
            {
                if (sGimmickInfoArr[gimIndex].id == (int)Gimmicks.Steel)
                {
                    //òÎ·ûüðwè
                    int gimSqrId   = sGimmickInfoArr[gimIndex].nowSquareId;
                    int gimLine    = mSquaresMgr.GetLineNumber(gimSqrId);
                    int gimColumn  = mSquaresMgr.GetColumnNumber(gimSqrId);
                    int starLine   = mSquaresMgr.GetLineNumber(mStarSupportSquareId);
                    int starColumn = mSquaresMgr.GetColumnNumber(mStarSupportSquareId);
                    bool up    = gimLine < starLine;
                    bool down  = gimLine > starLine;
                    bool left  = gimColumn < starColumn;
                    bool right = gimColumn > starColumn;

                    if      (up && left)    addStateName = Directions.UpLeft.ToString();    //¶ã
                    else if (up && right)   addStateName = Directions.UpRight.ToString();   //Eã
                    else if (down && left)  addStateName = Directions.DownLeft.ToString();  //¶º
                    else if (down && right) addStateName = Directions.DownRight.ToString(); //Eº
                    else if (up)            addStateName = Directions.Up.ToString();        //ã
                    else if (down)          addStateName = Directions.Down.ToString();      //º
                    else if (left)          addStateName = Directions.Left.ToString();      //¶
                    else if (right)         addStateName = Directions.Right.ToString();     //E
                }
            }

            //ìJn
            mStarSupportedSquaresList.Add(sqrId);
            mPiecesMgr.DamageSpecifiedSquare(sqrId, COLORLESS_ID, false, true, addStateName);
        }
    }
}