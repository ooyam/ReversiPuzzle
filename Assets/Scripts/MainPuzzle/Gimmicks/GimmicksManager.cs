using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static animation.AnimationManager;

namespace PuzzleMain
{
    public class GimmicksManager : MonoBehaviour
    {
        PiecesManager piecesMgr;    //PiecesManager

        [Header("���sprite")]
        [SerializeField]
        Sprite[] jewelrySprArr;
    
        [Header("�gsprite(�c)")]
        [SerializeField]
        Sprite[] frameHeightSprArr;
    
        [Header("�gsprite(��)")]
        [SerializeField]
        Sprite[] frameWidthSprArr;
    
        [Header("�gsprite(�p)")]
        [SerializeField]
        Sprite[] frameCornerSprArr;

        [Header("�Bsprite(���e)")]
        [SerializeField]
        Sprite[] CageBobmSprArr;

        [Header("�Bsprite(����)")]
        [SerializeField]
        Sprite[] CageNumberSprArr;

        [Header("�ԍ��Dsprite(����)")]
        [SerializeField]
        Sprite[] NumberTagSprArr;

        //�O���[�v�M�~�b�N�̃��X�g
        List<GameObject>[] frameObjListArr;             //�g�I�u�W�F�N�g���X�g(�O���[�v��)
        List<GimmickInformation>[] frameInfoListArr;    //�g�I�u�W�F�N�g��񃊃X�g(�O���[�v��)
        List<int>[] frameSquareIdListArr;               //�g�z�u�}�X���X�g(�O���[�v��)
        int[] groupColorNumArr;                         //�O���[�v���̎w��F�ԍ�

        //�T�C�Y�σM�~�b�N�̔z��
        GameObject[] cageObjArr;            //�B�I�u�W�F�N�g���X�g
        GimmickInformation[] cageInfoArr;   //�B�I�u�W�F�N�g��񃊃X�g
        int[] cageSquareIdArr;              //�B�z�u�}�X���X�g


        //==========================================================//
        //----------------------�����ݒ�,�擾-----------------------//
        //==========================================================//

        /// <summary>
        /// GimmicksManager�̏�����
        /// </summary>
        public void Initialize()
        {
            piecesMgr = sPuzzleMain.GetPiecesManager();
        }


        //===============================================//
        //=========�M�~�b�N�_���[�W�E��ԕω�============//
        //===============================================//

        /// <summary>
        /// �M�~�b�N�Ƀ_���[�W�����邩�̊m�F
        /// </summary>
        /// /// <param name="putPieceColorId">  �u������̃^�O</param>
        /// /// <param name="gimmickIndex">     �M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
        public bool DamageCheck(ref int putPieceColorId, ref int gimmickIndex)
        {
            //�_���[�W�̗L���t���O
            bool damage = false;

            switch (sGimmickInfoArr[gimmickIndex].id)
            {
                //������
                case (int)Gimmicks.Balloon:     //���D
                case (int)Gimmicks.Wall:        //��
                case (int)Gimmicks.Flower:      //��
                case (int)Gimmicks.Hamster:     //�n���X�^�[
                case (int)Gimmicks.Tornado:     //����
                    damage = true;
                    break;

                //�F����
                case (int)Gimmicks.Balloon_Color: //���D(�F)
                case (int)Gimmicks.Jewelry:       //���
                    if (putPieceColorId == sGimmickInfoArr[gimmickIndex].colorId)
                        damage = true;
                    break;

                //����
                case (int)Gimmicks.NumberTag:
                    if (sNumberTagNextOrder == sGimmickInfoArr[gimmickIndex].order)
                    {
                        damage = true;
                        sNumberTagNextOrder++;
                    }
                    break;
            }

            //���ʂ�Ԃ�
            return damage;
        }

        /// <summary>
        /// �M�~�b�N�Ƀ_���[�W
        /// </summary>
        /// /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
        /// /// <param name="squareIndex"> �M�~�b�N�z�u�ԍ�</param>
        public void DamageGimmick(ref int gimmickIndex, int squareIndex)
        {
            string stateName = "";     //�X�e�[�g��
            GimmickInformation gimmInfo = sGimmickInfoArr[gimmickIndex]; //�M�~�b�N�̏��擾

            switch (gimmInfo.id)
            {
                //�������j��
                case (int)Gimmicks.Balloon:         //���D
                case (int)Gimmicks.Balloon_Color:   //���D(�F)
                case (int)Gimmicks.Jewelry:         //���
                case (int)Gimmicks.NumberTag:       //�ԍ��D
                case (int)Gimmicks.Tornado:         //����
                    stateName = STATE_NAME_BURST;
                    break;

                //������_���[�W
                case (int)Gimmicks.Wall:    //��
                case (int)Gimmicks.Flower:  //��
                case (int)Gimmicks.Hamster: //�n���X�^�[
                    stateName = STATE_NAME_DAMAGE + gimmInfo.remainingTimes.ToString();
                    if (!gimmInfo.destructible) gimmInfo.destructible = true;
                    break;
            }

            //�_���[�W�A�j���[�V�����J�n
            sGimmickCorList.Add(StartCoroutine(AnimationStart(gimmInfo.ani, stateName)));

            //�_���[�W�񐔌v�Z
            gimmInfo.remainingTimes--;

            //���̃^�[���Ƀ_���[�W���󂯂����̃t���OON
            gimmInfo.nowTurnDamage = true;

            //�_���[�W�c�񐔂�0�Ŕj��
            if (gimmInfo.remainingTimes <= 0)
                sDestroyPiecesIndexList.Add(squareIndex);
        }

        /// <summary>
        /// �M�~�b�N�̏�ԕω�
        /// </summary>
        public IEnumerator ChangeGimmickState()
        {
            //�R���[�`�����X�g
            List<Coroutine> coroutineList = new List<Coroutine>();
            Coroutine coroutine = null;

            //�����I�u�W�F�N�g��񃊃X�g
            tornadoInfoList = new List<GimmickInformation>();

            //�ʏ�M�~�b�N
            if (sGimmickInfoArr != null)
            {
                foreach (GimmickInformation gimmInfo in sGimmickInfoArr)
                {
                    if (gimmInfo == null) continue;

                    switch (gimmInfo.id)
                    {
                        //���(sprite�ؑ�)
                        case (int)Gimmicks.Jewelry:

                            //�q�I�u�W�F�N�g��sprit�X�V
                            int oldColorType = Array.IndexOf(USE_COLOR_TYPE_ARR, gimmInfo.colorId);
                            int newColorType = (oldColorType == USE_COLOR_COUNT - 1) ? 0 : oldColorType + 1;
                            gimmInfo.colorId = USE_COLOR_TYPE_ARR[newColorType];
                            Sprite newSprite = jewelrySprArr[gimmInfo.colorId];
                            gimmInfo.spriRenChild[0].sprite = newSprite;

                            //sprit�ύX
                            coroutine = StartCoroutine(SpriteChange(gimmInfo.ani, gimmInfo.spriRen, newSprite));
                            coroutineList.Add(coroutine);
                            break;

                        //�n���X�^�[(�A���t���O�m�F)
                        case (int)Gimmicks.Hamster:

                            //�_���[�W1���
                            if (gimmInfo.destructible)
                            {
                                //���̃^�[���Ƀ_���[�W���󂯂Ă��Ȃ��ꍇ
                                if (!gimmInfo.nowTurnDamage)
                                {
                                    //������Ԃɖ߂�
                                    gimmInfo.destructible = false;
                                    gimmInfo.remainingTimes++;
                                    coroutine = StartCoroutine(AnimationStart(gimmInfo.ani, STATE_NAME_RETURN));
                                    coroutineList.Add(coroutine);
                                }
                            }
                            break;

                        //����
                        case (int)Gimmicks.Tornado:
                            tornadoInfoList.Add(gimmInfo);
                            break;
                    }
                }
            }

            //�����M�~�b�N����J�n
            if (tornadoInfoList.Count > 0)
            {
                coroutine = StartCoroutine(StartTornadoAttack());
                coroutineList.Add(coroutine);
            }

            //�g�M�~�b�N
            if (frameInfoListArr != null)
            {
                foreach (List<GimmickInformation> frameInfoList in frameInfoListArr)
                {
                    if (frameInfoList == null) continue;
                    List<int> changedSquare = new List<int>();
                    foreach (GimmickInformation gimmInfo in frameInfoList)
                    {
                        switch (gimmInfo.id)
                        {
                            //�g(�F�ω�)
                            case (int)Gimmicks.Frame_Color_Change:

                                //�q�I�u�W�F�N�g��sprit�X�V
                                int oldColorType = Array.IndexOf(USE_COLOR_TYPE_ARR, gimmInfo.colorId);
                                int newColorType = (oldColorType == USE_COLOR_COUNT - 1) ? 0 : oldColorType + 1;
                                gimmInfo.colorId = USE_COLOR_TYPE_ARR[newColorType];
                                Sprite newSprite = (gimmInfo.tra.localPosition.x == 0.0f) ? frameWidthSprArr[gimmInfo.colorId] : frameHeightSprArr[gimmInfo.colorId];
                                gimmInfo.spriRenChild[0].sprite = newSprite;

                                //sprit�ύX
                                coroutine = StartCoroutine(SpriteChange(gimmInfo.ani, gimmInfo.spriRen, newSprite));
                                coroutineList.Add(coroutine);

                                //�}�X�̐F�ύX
                                if (!changedSquare.Contains(gimmInfo.startSquareId))
                                {
                                    coroutine = StartCoroutine(piecesMgr.SquareColorChange(GetSquareColor(gimmInfo.colorId), gimmInfo.startSquareId, true));
                                    coroutineList.Add(coroutine);
                                    changedSquare.Add(gimmInfo.startSquareId);
                                }

                                //�p��Sprite�ݒ�
                                gimmInfo.spriRenChild[1].sprite = frameCornerSprArr[gimmInfo.colorId];
                                gimmInfo.spriRenChild[2].sprite = frameCornerSprArr[gimmInfo.colorId];
                                break;
                        }
                    }
                }
            }

            //�M�~�b�N�ύX�ҋ@
            foreach (Coroutine c in coroutineList)
            { yield return c; }
        }

        /// <summary>
        /// �X�v���C�g�����ւ�
        /// </summary>
        /// <param name="ani"></param>
        /// <param name="newSprite"></param>
        IEnumerator SpriteChange(Animator ani, SpriteRenderer spriRen, Sprite newSprite)
        {
            yield return StartCoroutine(AnimationStart(ani, STATE_NAME_COLOR_CHANGE));
            spriRen.sprite = newSprite;
        }

        /// <summary>
        /// �^�[���I�����̃M�~�b�N�j��
        /// </summary>
        /// <returns></returns>
        public IEnumerator DestroyGimmicks_TurnEnd()
        {
            //�M�~�b�N�j��
            Coroutine[] coroutines = new Coroutine[]
            {
                StartCoroutine(DestroyFrame()),     //�g
                StartCoroutine(DestroyCage())       //�B
            };
            foreach (Coroutine c in coroutines)
            { yield return c; }
        }


        //===============================================//
        //==================�M�~�b�N�z�u=================//
        //===============================================//

        /// <summary>
        /// �}�X�Ƃ��ĊǗ����Ȃ��M�~�b�N�̔z�u
        /// </summary>
        public void PlaceGimmickNotInSquare()
        {
            //�O���[�v�ԍ��ɉ������F�ԍ��z��
            groupColorNumArr     = new int[GIMMICKS_GROUP_COUNT];
            frameSquareIdListArr = new List<int>[GIMMICKS_GROUP_COUNT];

            //�B��񃊃X�g
            List<int[]> cageInfoArrList = new List<int[]>();

            //�I�u�W�F�N�g�Ǘ����X�g�쐬
            foreach (int[] gimInfo in GIMMICKS_INFO_ARR)
            {
                switch (gimInfo[GIMMICK])
                {
                    //�g
                    case (int)Gimmicks.Frame:               //�g
                    case (int)Gimmicks.Frame_Color:         //�g(�F)
                    case (int)Gimmicks.Frame_Color_Change:  //�g(�F�ύX)
                        //�O���[�v���ƂɃ��X�g�쐬
                        if (frameSquareIdListArr[gimInfo[GROUP]] == null)
                            frameSquareIdListArr[gimInfo[GROUP]] = new List<int>();
                        frameSquareIdListArr[gimInfo[GROUP]].Add(gimInfo[SQUARE]);
                        break;

                    //�T�C�Y�σM�~�b�N
                    case (int)Gimmicks.Cage:    //�B
                        cageInfoArrList.Add(gimInfo);
                        break;
                }

                //�O���[�v�̎w��F�ԍ�
                if (gimInfo[GROUP] != NOT_NUM) groupColorNumArr[gimInfo[GROUP]] = gimInfo[COLOR];
            }

            //�g�z�u
            if (frameSquareIdListArr != null) PlacementLocation_Frame();

            //�B�z�u
            if (cageInfoArrList != null) GenerateCage(ref cageInfoArrList);
        }



        //===============================================//
        //===========�g�iFrame�j�̌ŗL�֐�===============//
        //===============================================//

        //�v���n�u�̎q�I�u�W�F�N�g�ԍ�
        const int FRAME_CORNER_1 = 1; //�p1
        const int FRAME_CORNER_2 = 2; //�p2

        /// <summary>
        /// �}�X�w��F�擾
        /// </summary>
        /// <param name="colorId">�F�ԍ�</param>
        /// <returns>�}�X�F</returns>
        Color GetSquareColor(int colorId)
        {
            Color color = SQUARE_WHITE;
            switch (colorId)
            {
                case (int)Colors.Blue:   color = SQUARE_BLUE;   break;
                case (int)Colors.Red:    color = SQUARE_RED;    break;
                case (int)Colors.Yellow: color = SQUARE_YELLOW; break;
                case (int)Colors.Green:  color = SQUARE_GREEN;  break;
                case (int)Colors.Violet: color = SQUARE_VIOLET; break;
                case (int)Colors.Orange: color = SQUARE_ORANGE; break;
                case COLORLESS_ID:       color = SQUARE_BLACK;  break;
            }
            return color;
        }

        /// <summary>
        /// �z�u�ꏊ����(�g)
        /// Frame
        /// FrameColor
        /// FrameColorChange
        /// </summary>
        void PlacementLocation_Frame()
        {
            //�I�u�W�F�N�g,��񃊃X�g�̐���
            frameObjListArr  = new List<GameObject>[GIMMICKS_GROUP_COUNT];
            frameInfoListArr = new List<GimmickInformation>[GIMMICKS_GROUP_COUNT];

            int groupId = 0;
            foreach (List<int> squareList in frameSquareIdListArr)
            {
                frameObjListArr[groupId]  = new List<GameObject>();
                frameInfoListArr[groupId] = new List<GimmickInformation>();
                foreach (int squareIndex in squareList)
                {
                    //�g����
                    if (!squareList.Contains(squareIndex - 1))
                        GenerateFrame(groupColorNumArr[groupId], squareIndex, frameWidthSprArr, Directions.Up, groupId);        //��        
                    if (!squareList.Contains(squareIndex + 1))
                        GenerateFrame(groupColorNumArr[groupId], squareIndex, frameWidthSprArr, Directions.Down, groupId);      //��        
                    if (!squareList.Contains(squareIndex - BOARD_LINE_COUNT))
                        GenerateFrame(groupColorNumArr[groupId], squareIndex, frameHeightSprArr, Directions.Left, groupId);     //��        
                    if (!squareList.Contains(squareIndex + BOARD_LINE_COUNT))
                        GenerateFrame(groupColorNumArr[groupId], squareIndex, frameHeightSprArr, Directions.Right, groupId);    //�E

                    //�}�X�̐F�w��
                    Color color = GetSquareColor(groupColorNumArr[groupId]);
                    StartCoroutine(piecesMgr.SquareColorChange(color, squareIndex, false));
                }
                groupId++;
            }
        }

        /// <summary>
        /// �g����
        /// </summary>
        /// <param name="colorId">    �F�ԍ�</param>
        /// <param name="squareIndex">�}�X�Ǘ��ԍ�</param>
        /// <param name="spriArr">    sprite�z��</param>
        /// <param name="direction">  �z�u����</param>
        /// <param name="groupId">    �O���[�v�ԍ�</param>
        void GenerateFrame(int colorId, int squareIndex, Sprite[] spriArr, Directions direction, int groupId)
        {
            //�t���[������,�z�u
            GameObject frameObj = Instantiate(piecesMgr.gimmickPrefabArr[(int)Gimmicks.Frame].prefab[(int)direction]);
            frameObjListArr[groupId].Add(frameObj);
            piecesMgr.PlaceGimmick(frameObj, squareIndex);

            //�t���[���M�~�b�N�̏��擾
            GimmickInformation gimInfo = frameObj.GetComponent<GimmickInformation>();
            frameInfoListArr[groupId].Add(gimInfo);
            gimInfo.InformationSetting_SquareIndex(squareIndex, INT_NULL, groupId);
            sGimmickInfoArr[gimInfo.settingIndex] = gimInfo;

            //sprite�ݒ�
            if (colorId == COLORLESS_ID) colorId = COLORS_COUNT;
            gimInfo.spriRen.sprite = spriArr[colorId];
            gimInfo.spriRenChild[FRAME_CORNER_1].sprite = frameCornerSprArr[colorId]; //�p1
            gimInfo.spriRenChild[FRAME_CORNER_2].sprite = frameCornerSprArr[colorId]; //�p2
        }

        /// <summary>
        /// �g�j��m�F�E���s
        /// </summary>
        IEnumerator DestroyFrame()
        {
            if (frameSquareIdListArr == null) yield break;

            bool frameListNull = true; //�M�~�b�N�̑��݂̗L��
            int groupId = 0;           //�O���[�v�ԍ�

            foreach (List<int> squareList in frameSquareIdListArr)
            {
                if (squareList == null)
                {
                    groupId++;
                    continue;
                }

                frameListNull = false;

                //�w��F�̎擾
                int colorId = frameInfoListArr[groupId][0].colorId;
                bool burst = true; //�M�~�b�N�j��t���O

                foreach (int i in squareList)
                {
                    //��łȂ��ꍇ�͏����I��
                    if (sPieceInfoArr[i] == null)
                    {
                        burst = false;
                        break;
                    }

                    //�ŏ��̋�̃^�O���w��F�ɐݒ�
                    if (colorId == COLORLESS_ID && squareList[0] == i)
                    {
                        colorId = sPieceInfoArr[i].colorId;
                    }

                    //�w��F�łȂ��ꍇ
                    if (colorId != sPieceInfoArr[i].colorId)
                    {
                        burst = false;
                        break;
                    }
                }

                //�M�~�b�N�j��J�n
                if (burst)
                {
                    //�M�~�b�N�j�󉉏o
                    Coroutine coroutine = null;
                    foreach (GimmickInformation gimmickInfo in frameInfoListArr[groupId])
                    { coroutine = StartCoroutine(AnimationStart(gimmickInfo.ani, STATE_NAME_BURST)); }
                    yield return coroutine;

                    //�M�~�b�N�j��
                    foreach (GameObject obj in frameObjListArr[groupId])
                    { Destroy(obj);}
                    frameObjListArr[groupId] = null;

                    //�M�~�b�N�����폜
                    frameInfoListArr[groupId] = null;

                    //�}�X�̐F��߂�
                    foreach (int i in squareList)
                    { StartCoroutine(piecesMgr.SquareColorChange(SQUARE_WHITE, i, true)); }
                    frameSquareIdListArr[groupId] = null;
                }

                //���̃O���[�v��
                groupId++;
            }

            //��������x������Ȃ�����(���X�g����)�ꍇ
            if (frameListNull)
            {
                frameInfoListArr     = null;
                frameObjListArr      = null;
                frameSquareIdListArr = null;
            }
        }



        //===============================================//
        //============�B�iCage�j�̌ŗL�֐�===============//
        //===============================================//

        //�v���n�u�̎q�I�u�W�F�N�g�ԍ�
        const int CAGE_BOBM          = 0; //���e
        const int CAGE_NUMBER_RIGHT  = 1; //�����E�z�u
        const int CAGE_NUMBER_LEFT   = 2; //�������z�u
        const int CAGE_NUMBER_CENTER = 3; //���������z�u(1���p)

        /// <summary>
        /// �B����
        /// </summary>
        /// <param name="cageInfoArrList">�B�̏��z��</param>
        void GenerateCage(ref List<int[]> cageInfoArrList)
        {
            int cageCount    = cageInfoArrList.Count;
            cageObjArr       = new GameObject[cageCount];               //�B�I�u�W�F�N�g���X�g
            cageInfoArr      = new GimmickInformation[cageCount];       //�B�I�u�W�F�N�g��񃊃X�g
            cageSquareIdArr = new int[cageCount];                       //�B�z�u�}�X���X�g
            for (int i = 0; i < cageCount; i++)
            {
                int[] cageInfo = cageInfoArrList[i];

                //�B����,�z�u
                cageObjArr[i]  = Instantiate(piecesMgr.gimmickPrefabArr[cageInfo[GIMMICK]].prefab[0]);
                cageInfoArr[i] = cageObjArr[i].GetComponent<GimmickInformation>();
                cageSquareIdArr[i] = cageInfo[SQUARE];
                piecesMgr.PlaceGimmick(cageObjArr[i], cageInfo[SQUARE]);
                cageInfoArr[i].InformationSetting_SquareIndex(cageInfo[SQUARE], cageInfo[GIMMICK], NOT_NUM);
                sGimmickInfoArr[cageInfoArr[i].settingIndex] = cageInfoArr[i];
                cageInfoArr[i].spriRenChild[CAGE_BOBM].sprite = CageBobmSprArr[cageInfo[COLOR]];

                //���W�w��
                Vector3 _pos = new Vector3(
                    cageInfoArr[i].defaultPos.x * (cageInfo[WIDTH] - 1),
                    cageInfoArr[i].defaultPos.y * (cageInfo[HEIGHT] - 1),
                    cageInfoArr[i].defaultPos.z);
                cageInfoArr[i].tra.localPosition = _pos;

                //�X�P�[���w��
                Vector3 _scale = new Vector3(
                    cageInfoArr[i].defaultScale.x * cageInfo[WIDTH],
                    cageInfoArr[i].defaultScale.y * cageInfo[HEIGHT],
                    cageInfoArr[i].defaultScale.z);
                cageInfoArr[i].spriRen.size = _scale;

                //sprite�X�V(����)
                BobmCountSpriteUpdate(cageInfoArr[i]);
            }
        }

        /// <summary>
        /// ���e�̔ԍ��X�V
        /// </summary>
        /// <param name="_cageInfo"></param>
        void BobmCountSpriteUpdate(GimmickInformation _cageInfo)
        {
            if (_cageInfo.remainingQuantity < TEN)
            {
                //1��(�����\��)
                _cageInfo.objChild[CAGE_NUMBER_RIGHT].SetActive(false);     //�E
                _cageInfo.objChild[CAGE_NUMBER_LEFT].SetActive(false);      //��
                _cageInfo.objChild[CAGE_NUMBER_CENTER].SetActive(true);     //����
                _cageInfo.spriRenChild[CAGE_NUMBER_CENTER].sprite = CageNumberSprArr[_cageInfo.remainingQuantity];
            }
            else
            {
                //2��
                _cageInfo.objChild[CAGE_NUMBER_RIGHT].SetActive(true);      //�E
                _cageInfo.objChild[CAGE_NUMBER_LEFT].SetActive(true);       //��
                _cageInfo.objChild[CAGE_NUMBER_CENTER].SetActive(false);    //����
                _cageInfo.spriRenChild[CAGE_NUMBER_RIGHT].sprite = CageNumberSprArr[_cageInfo.remainingQuantity % TEN];   //1�̈�
                _cageInfo.spriRenChild[CAGE_NUMBER_LEFT].sprite  = CageNumberSprArr[_cageInfo.remainingQuantity / TEN];   //10�̈�
            }
        }

        /// <summary>
        /// �B�_���[�W
        /// </summary>
        /// <param name="_colorId">�F�ԍ�</param>
        public IEnumerator DamageCage(int _colorId)
        {
            foreach (GimmickInformation cageInfo in cageInfoArr)
            {
                if (cageInfo == null) continue;
                if (cageInfo.remainingQuantity == 0) continue;

                if (cageInfo.colorId == _colorId && cageInfo.remainingQuantity > 0)
                {
                    //���F�̔��e�J�E���g�����炷
                    cageInfo.remainingQuantity--;

                    //sprite�X�V(����)
                    BobmCountSpriteUpdate(cageInfo);
                    yield return StartCoroutine(AnimationStart(cageInfo.ani, STATE_NAME_DAMAGE));
                    break;
                }
            }
        }

        /// <summary>
        /// �B�j��
        /// </summary>
        IEnumerator DestroyCage()
        {
            List<int> desIndexList = new List<int>();
            Coroutine coroutine = null;
            int index = -1;

            foreach (GimmickInformation cageInfo in cageInfoArr)
            {
                index++;
                if (cageInfo == null) continue;

                //�M�~�b�N�j��
                if (cageInfo.remainingQuantity == 0)
                {
                    coroutine = StartCoroutine(AnimationStart(cageInfo.ani, STATE_NAME_BURST));
                    desIndexList.Add(index);
                }
            }
            yield return coroutine;

            if (desIndexList.Count == 0) yield break;

            //�M�~�b�N�j��
            foreach (int desIndex in desIndexList)
            {
                //��̑���t���O�ؑ�
                foreach (int squareId in cageInfoArr[desIndex].innerSquaresId)
                { piecesMgr.PieceOperationFlagChange(squareId, true); }

                //�I�u�W�F�N�g�j��
                Destroy(cageObjArr[desIndex]);

                //�Ǘ��z�񃊃Z�b�g
                cageObjArr[desIndex] = null;           //�B�I�u�W�F�N�g���X�g
                cageInfoArr[desIndex] = null;          //�B�I�u�W�F�N�g��񃊃X�g
                cageSquareIdArr[desIndex] = INT_NULL;  //�B�z�u�}�X���X�g
            }
        }



        //===============================================//
        //=========�ԍ��D�iNumberTag�j�̌ŗL�֐�=========//
        //===============================================//

        //�v���n�u�̎q�I�u�W�F�N�g�ԍ�
        const int NUMBERTAG_FRONT = 0; //�O��(�ԍ��L�ږ�)

        /// <summary>
        /// ���Ԑݒ�(sprite�ݒ�)
        /// </summary>
        /// <param name="gimInfo">�M�~�b�N���</param>
        public void NumberTagOrderSetting(ref GimmickInformation gimInfo)
        {
            gimInfo.spriRenChild[NUMBERTAG_FRONT].sprite = NumberTagSprArr[gimInfo.order];
        }


        //===============================================//
        //============�����iTornado�j�̌ŗL�֐�==========//
        //===============================================//

        //�������U������}�X�ԍ��z��̃C���f�b�N�X�ԍ�
        enum TornadoAttackInfoIndex
        {
            AttackFirst = 0,    //�ŏ��ɍU������}�X�ԍ�
            AttackSecond,       //2�߂ɍU������}�X�ԍ�
            AttackThird,        //3�߂ɍU������}�X�ԍ�
            AttackDirection,    //�U������

            ArrayCount          //�z��T�C�Y
        }
        const int ATK_FIRST  = (int)TornadoAttackInfoIndex.AttackFirst;
        const int ATK_SECOND = (int)TornadoAttackInfoIndex.AttackSecond;
        const int ATK_THIRD  = (int)TornadoAttackInfoIndex.AttackThird;
        const int ATK_COUNT  = ATK_THIRD + 1; //�U���ӏ��̐�(3)
        const int ATK_DIR    = (int)TornadoAttackInfoIndex.AttackDirection;

        //�������U��������z��
        List<GimmickInformation> tornadoInfoList;
        int[][] tornadoAttackInfoArr;

        /// <summary>
        /// �������U��������̐ݒ�
        /// </summary>
        void SetTornadoAttackInfo()
        {
            //�U���}�X�ԍ��̐ݒ�
            int tornadoCount = tornadoInfoList.Count;
            tornadoAttackInfoArr = new int[tornadoCount][];
            for (int i = 0; i < tornadoCount; i++)
            {
                tornadoAttackInfoArr[i] = new int[(int)TornadoAttackInfoIndex.ArrayCount];

                //����8�}�X���U���\���m�F
                int[] piecesIndexArr      = new int[DIRECTIONS_COUNT];      //���Ӌ�̊Ǘ��ԍ�
                bool[] atkPossibleSquares = new bool[DIRECTIONS_COUNT];     //�U���\
                bool[] squareNull         = new bool[DIRECTIONS_COUNT];     //��}�X����
                int nowSquareId           = tornadoInfoList[i].nowSquareId; //���݂̃}�XID
                foreach (Directions dir in Enum.GetValues(typeof(Directions)))
                {
                    squareNull[(int)dir] = false;
                    atkPossibleSquares[(int)dir] = false;

                    //�������Ղ̒[�ɂ���ꍇ�͏����X�L�b�v
                    if (!piecesMgr.IsSquareSpecifiedDirection(dir, nowSquareId)) continue;

                    //�e�����̃}�X�Ǘ��ԍ��擾
                    piecesIndexArr[(int)dir] = piecesMgr.GetDesignatedDirectionIndex((int)dir, nowSquareId);
                    if (0 <= piecesIndexArr[(int)dir] && piecesIndexArr[(int)dir] < SQUARES_COUNT)
                    {
                        if (sPieceObjArr[piecesIndexArr[(int)dir]] == null)
                        {
                            //��}�X
                            squareNull[(int)dir] = true;
                            atkPossibleSquares[(int)dir] = true;
                        }
                        else if (sPieceInfoArr[piecesIndexArr[(int)dir]] != null && sPieceInfoArr[piecesIndexArr[(int)dir]].invertable)
                        {
                            //���]�\��
                            atkPossibleSquares[(int)dir] = true;
                        }
                    }
                }

                int[]  atkDirNumArr          = new int[ATK_COUNT];               //�U���ӏ��̔ԍ�
                bool[] atkPossibleDirections = new bool[FOUR_DIRECTIONS_COUNT];  //�U���\�����̎擾
                int[]  nullSquareCount       = new int[FOUR_DIRECTIONS_COUNT];   //��}�X�̐�
                foreach (FourDirections fourDir in Enum.GetValues(typeof(FourDirections)))
                {
                    switch (fourDir)
                    {
                        //�����
                        case FourDirections.Up:
                            atkDirNumArr[ATK_FIRST]  = (int)Directions.Up;         //��
                            atkDirNumArr[ATK_SECOND] = (int)Directions.UpRight;    //�E��
                            atkDirNumArr[ATK_THIRD]  = (int)Directions.Right;      //�E
                            break;
                        //������
                        case FourDirections.Down:
                            atkDirNumArr[ATK_FIRST]  = (int)Directions.Down;       //��
                            atkDirNumArr[ATK_SECOND] = (int)Directions.DownLeft;   //����
                            atkDirNumArr[ATK_THIRD]  = (int)Directions.Left;       //��
                            break;
                        //������
                        case FourDirections.Left:
                            atkDirNumArr[ATK_FIRST]  = (int)Directions.Left;       //��
                            atkDirNumArr[ATK_SECOND] = (int)Directions.UpLeft;     //����
                            atkDirNumArr[ATK_THIRD]  = (int)Directions.Up;         //��
                            break;
                        //�E����
                        case FourDirections.Right:
                            atkDirNumArr[ATK_FIRST]  = (int)Directions.Right;      //�E
                            atkDirNumArr[ATK_SECOND] = (int)Directions.DownRight;  //�E��
                            atkDirNumArr[ATK_THIRD]  = (int)Directions.Down;       //��
                            break;
                    }

                    int fourDirNum = (int)fourDir;
                    nullSquareCount[fourDirNum] = 0;
                    bool atkPoss = true;�@//�U���\��������t���O

                    foreach (int atkDirNum in atkDirNumArr)
                    {
                        //��}�X�̃J�E���g
                        if (squareNull[atkDirNum]) nullSquareCount[fourDirNum]++;

                        //�U���\��������(�S�����U���\��true)
                        if (!atkPossibleSquares[atkDirNum]) atkPoss = false;
                    }

                    //�U���\�����̐ݒ�
                    atkPossibleDirections[fourDirNum] = atkPoss;
                }

                //��}�X�ŏ��l�擾
                int nullMinCount = ATK_COUNT;
                for (int a = 0; a < FOUR_DIRECTIONS_COUNT; a++)
                {
                    if (!atkPossibleDirections[a]) continue;
                    if (nullSquareCount[a] >= nullMinCount) continue;
                    nullMinCount = nullSquareCount[a];
                }

                //�g�p����p�^�[���ԍ��擾
                List<int> usePatternList = new List<int>();
                for (int a = 0; a < FOUR_DIRECTIONS_COUNT; a++)
                {
                    if (!atkPossibleDirections[a]) continue;
                    if (nullMinCount != nullSquareCount[a]) continue;
                    usePatternList.Add(a);
                }
                int usePossibleCount = usePatternList.Count;
                if (usePossibleCount == 0)
                {
                    tornadoAttackInfoArr[i] = null;
                    continue;
                }
                int usePatternIndex = usePatternList[UnityEngine.Random.Range(0, usePossibleCount)];

                //�U�������̐ݒ�
                tornadoAttackInfoArr[i][ATK_DIR] = usePatternIndex;
                switch (usePatternIndex)
                {
                    //�����
                    case (int)FourDirections.Up:
                        tornadoAttackInfoArr[i][ATK_FIRST]  = piecesIndexArr[(int)Directions.Up];
                        tornadoAttackInfoArr[i][ATK_SECOND] = piecesIndexArr[(int)Directions.UpRight];
                        tornadoAttackInfoArr[i][ATK_THIRD]  = piecesIndexArr[(int)Directions.Right];
                        break;
                    //������
                    case (int)FourDirections.Down:
                        tornadoAttackInfoArr[i][ATK_FIRST]  = piecesIndexArr[(int)Directions.Down];
                        tornadoAttackInfoArr[i][ATK_SECOND] = piecesIndexArr[(int)Directions.DownLeft];
                        tornadoAttackInfoArr[i][ATK_THIRD]  = piecesIndexArr[(int)Directions.Left];
                        break;
                    //������
                    case (int)FourDirections.Left:
                        tornadoAttackInfoArr[i][ATK_FIRST]  = piecesIndexArr[(int)Directions.Left];
                        tornadoAttackInfoArr[i][ATK_SECOND] = piecesIndexArr[(int)Directions.UpLeft];
                        tornadoAttackInfoArr[i][ATK_THIRD]  = piecesIndexArr[(int)Directions.Up];
                        break;
                    //�E����
                    case (int)FourDirections.Right:
                        tornadoAttackInfoArr[i][ATK_FIRST]  = piecesIndexArr[(int)Directions.Right];
                        tornadoAttackInfoArr[i][ATK_SECOND] = piecesIndexArr[(int)Directions.DownRight];
                        tornadoAttackInfoArr[i][ATK_THIRD]  = piecesIndexArr[(int)Directions.Down];
                        break;
                }
            }
        }

        /// <summary>
        /// �����̍U���J�n
        /// </summary>
        IEnumerator StartTornadoAttack()
        {
            //�U���ݒ�
            SetTornadoAttackInfo();

            //�R���[�`�����X�g
            List<Coroutine> coroutineList = new List<Coroutine>();
            Coroutine coroutine = null;

            //�U���J�n
            int tornadoCount = tornadoInfoList.Count;
            for (int i = 0; i < tornadoCount; i++)
            {
                if (tornadoAttackInfoArr[i] == null) continue;
                string directionName = Enum.GetNames(typeof(FourDirections))[tornadoAttackInfoArr[i][ATK_DIR]];
                coroutine = StartCoroutine(AnimationStart(tornadoInfoList[i].ani, STATE_NAME_ATTACK + directionName));
                coroutineList.Add(coroutine);
            }

            //�I���ҋ@
            if (coroutineList.Count == 0) yield break;
            foreach (Coroutine c in coroutineList)
            { yield return c; }

            //�������J��
            tornadoInfoList = null;
            tornadoAttackInfoArr = null;
        }

        /// <summary>
        /// �����̍U��
        /// </summary>
        /// <param name="atkNum">�U���ԍ�</param>
        public void TornadoAttackPieceChange(GimmickInformation gimInfo, int atkNum)
        {
            int gimIndex = tornadoInfoList.IndexOf(gimInfo);
            int nowPieceColor = piecesMgr.GetSquarePieceColorId(tornadoAttackInfoArr[gimIndex][atkNum]);

            //���̋�Ɠ��F�������ꍇ��10��܂ōĎ��s����
            int generateColorId = 0;
            for (int i = 0; i < 10; i++)
            {
                generateColorId = piecesMgr.GetRandomPieceColor();
                if (nowPieceColor != generateColorId) break;
            }
            StartCoroutine(piecesMgr.ReversingPieces(tornadoAttackInfoArr[gimIndex][atkNum], generateColorId));
        }
    }
}