using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PiecesManager;
using static ObjectMove_2D.ObjectMove_2D;

namespace PuzzleMain
{
    public class GimmicksManager : MonoBehaviour
    {
        [Header("PiecesManager�̎擾")]
        [SerializeField]
        PiecesManager PiecesMan;

        [Header("��΂�sprite")]
        [SerializeField]
        Sprite[] jewelrySprArr;
    
        [Header("�g��Prefab")]
        [SerializeField]
        GameObject[] framePrefab; //0:��,1:�c
    
        [Header("�g��sprite(�c)")]
        [SerializeField]
        Sprite[] frameHeightSprArr;
    
        [Header("�g��sprite(��)")]
        [SerializeField]
        Sprite[] frameWidthSprArr;
    
        [Header("�g��sprite(�p)")]
        [SerializeField]
        Sprite[] frameCornerSprArr;

        //�A�j���[�V�����X�e�[�g��
        const string STATE_NAME_EMPTY        = "Empty";       //�������
        const string STATE_NAME_WAIT         = "Wait";        //�ҋ@
        const string STATE_NAME_BURST        = "Burst";       //�j��
        const string STATE_NAME_DAMAGE       = "Damage";      //������_���[�W
        const string STATE_NAME_COLOR_CHANGE = "ColorChange"; //�F�̍X�V
        const string STATE_NAME_RETURN_STATE = "ReturnState"; //��Ԃ�߂�

        //�͈͎w��M�~�b�N�̃��X�g
        List<GameObject>[] frameObjListArr;             //�g�I�u�W�F�N�g���X�g(�O���[�v��)
        List<GimmickInformation>[] frameInfoListArr;    //�g�I�u�W�F�N�g��񃊃X�g(�O���[�v��)
        List<int>[] frameSquareNumListArr;              //�g�z�u�}�X���X�g(�O���[�v��)
        int[] groupColorNumArr;                         //�O���[�v���̎w��F�ԍ�


        void Awake()
        {
            //�M�~�b�N�̐ݒ�ǂݍ���
            GimmickSetting();
            StageSetting();
        }

        IEnumerator Start()
        {
            //��Ƃ��ĊǗ����Ȃ��M�~�b�N�̔z�u
            yield return null;
            PlaceGimmickNotInSquare();
        }


        //===============================================//
        //===========�A�j���[�V��������֐�==============//
        //===============================================//

        /// <summary>
        /// �A�j���[�V�����Đ�
        /// </summary>
        /// <param name="ani">      �j�󂷂�I�u�W�F�N�g��Animator</param>
        /// <param name="stateName">�Đ��A�j���[�V�����X�e�[�g��</param>
        IEnumerator AnimationStart(Animator ani, string stateName)
        {
            //�A�j���[�V�����J�n
            ani.Play(stateName, 0, 0.0f);

            //�A�j���[�V�����I���ҋ@
            yield return null;
            yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f);
        }

        /// <summary>
        /// ���[�v�A�j���[�V�����Đ�
        /// </summary>
        /// <param name="ani">      �j�󂷂�I�u�W�F�N�g��Animator</param>
        /// <param name="stateName">�Đ��A�j���[�V�����X�e�[�g��</param>
        void LoopAnimationStart(Animator ani, string stateName = STATE_NAME_EMPTY)
        {
            //�A�j���[�V�����J�n
            ani.Play(stateName, 0, 0.0f);
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

            switch (gimmickInfoArr[gimmickIndex].id)
            {
                case (int)Gimmicks.Balloon:  //���D
                case (int)Gimmicks.Wall:     //��
                case (int)Gimmicks.Flower:   //��
                case (int)Gimmicks.Hamster:  //�n���X�^�[
                    damage = true;
                    break;

                case (int)Gimmicks.Balloon_Color: //���D(�F)
                case (int)Gimmicks.Jewelry:       //���
                    //�F����
                    if (putPieceColorId == gimmickInfoArr[gimmickIndex].colorId)
                        damage = true;
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
            int damageTimesfixNum = 0; //�X�e�[�g���Z�o�p
            string stateName = "";     //�X�e�[�g��
            GimmickInformation gimmInfo = gimmickInfoArr[gimmickIndex]; //�M�~�b�N�̏��擾

            switch (gimmInfo.id)
            {
                //������
                case (int)Gimmicks.Balloon:         //���D
                case (int)Gimmicks.Balloon_Color:   //���D(�F)
                case (int)Gimmicks.Jewelry:         //���
                    stateName = STATE_NAME_BURST;
                    break;

                //������_���[�W
                case (int)Gimmicks.Wall:    //��
                case (int)Gimmicks.Flower:  //��
                case (int)Gimmicks.Hamster: //�n���X�^�[
                    damageTimesfixNum = gimmInfo.remainingTimes + 1;
                    stateName = STATE_NAME_DAMAGE + (-gimmInfo.remainingTimes + damageTimesfixNum).ToString();
                    if (!gimmInfo.destructible) gimmInfo.destructible = true;
                    break;
            }

            //�_���[�W�A�j���[�V�����J�n
            gimmickCorList.Add(StartCoroutine(AnimationStart(gimmInfo.ani, stateName)));

            //�_���[�W�񐔌v�Z
            gimmInfo.remainingTimes--;

            //���̃^�[���Ƀ_���[�W���󂯂����̃t���OON
            gimmInfo.nowTurnDamage = true;

            //�_���[�W�c�񐔂�0�Ŕj��
            if (gimmInfo.remainingTimes <= 0)
                destroyPiecesIndexList.Add(squareIndex);
        }

        /// <summary>
        /// �M�~�b�N�̏�ԕω�
        /// </summary>
        public IEnumerator ChangeGimmickState()
        {
            List<Coroutine> coroutineList = new List<Coroutine>();

            //�ʏ�M�~�b�N
            if (gimmickInfoArr != null)
            {
                foreach (GimmickInformation gimmInfo in gimmickInfoArr)
                {
                    if (gimmInfo == null) continue;

                    switch (gimmInfo.id)
                    {
                        //���(sprite�ؑ�)
                        case (int)Gimmicks.Jewelry:

                            //�q�I�u�W�F�N�g��sprit�X�V
                            gimmInfo.colorId++;
                            if (gimmInfo.colorId >= USE_PIECE_COUNT) gimmInfo.colorId = 0;
                            Sprite newSprite = jewelrySprArr[gimmInfo.colorId];
                            gimmInfo.spriRenChild[0].sprite = newSprite;

                            //sprit�ύX
                            coroutineList.Add(StartCoroutine(SpriteChange(gimmInfo.ani, gimmInfo.spriRen, newSprite)));
                            break;

                        //�n���X�^�[(�A���t���O�m�F)
                        case (int)Gimmicks.Hamster:

                            //�_���[�W1���
                            if (gimmInfo.destructible)
                            {
                                //���̃^�[���Ƀ_���[�W���󂯂�
                                if (gimmInfo.nowTurnDamage)
                                {
                                    //�_���[�W1�ҋ@���
                                    LoopAnimationStart(gimmInfo.ani, STATE_NAME_WAIT);
                                }
                                else
                                {
                                    //������Ԃɖ߂�
                                    gimmInfo.destructible = false;
                                    gimmInfo.remainingTimes++;
                                    coroutineList.Add(StartCoroutine(AnimationStart(gimmInfo.ani, STATE_NAME_RETURN_STATE)));
                                    LoopAnimationStart(gimmInfo.ani);
                                }
                            }
                            break;
                    }
                }
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
                                gimmInfo.colorId++;
                                if (gimmInfo.colorId >= USE_PIECE_COUNT) gimmInfo.colorId = 0;
                                Sprite newSprite = (gimmInfo.tra.localPosition.x == 0.0f) ? frameWidthSprArr[gimmInfo.colorId] : frameHeightSprArr[gimmInfo.colorId];
                                gimmInfo.spriRenChild[0].sprite = newSprite;

                                //sprit�ύX
                                coroutineList.Add(StartCoroutine(SpriteChange(gimmInfo.ani, gimmInfo.spriRen, newSprite)));

                                //�}�X�̐F�ύX
                                if (!changedSquare.Contains(gimmInfo.startSquareId))
                                {
                                    coroutineList.Add(StartCoroutine(PiecesMan.SquareColorChange(GetSquareColor(gimmInfo.colorId), gimmInfo.startSquareId, true)));
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
            foreach (Coroutine coroutine in coroutineList)
            { yield return coroutine; }
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


        //===============================================//
        //==================�M�~�b�N�z�u=================//
        //===============================================//

        /// <summary>
        /// �}�X�Ƃ��ĊǗ����Ȃ��M�~�b�N�̔z�u
        /// </summary>
        void PlaceGimmickNotInSquare()
        {
            //�O���[�v�ԍ��ɉ������F�ԍ��z��
            groupColorNumArr      = new int[GIMMICKS_GROUP_COUNT];
            frameSquareNumListArr = new List<int>[GIMMICKS_GROUP_COUNT];

            //�O���[�v���ƂɃ��X�g�쐬
            foreach (int[] gimInfo in GIMMICKS_INFO_ARR)
            {
                switch (gimInfo[GIMMICK])
                {
                    //�O���[�v�M�~�b�N
                    case (int)Gimmicks.Frame:               //�g
                    case (int)Gimmicks.Frame_Color:         //�g(�F)
                    case (int)Gimmicks.Frame_Color_Change:  //�g(�F�ύX)
                        if (frameSquareNumListArr[gimInfo[GROUP]] == null)
                            frameSquareNumListArr[gimInfo[GROUP]] = new List<int>();
                        frameSquareNumListArr[gimInfo[GROUP]].Add(gimInfo[SQUARE]);
                        break;
                }

                //�O���[�v�̎w��F�ԍ�
                if (gimInfo[GROUP] != NOT_GROUP_ID) groupColorNumArr[gimInfo[GROUP]] = gimInfo[COLOR];
            }

            //�g�z�u
            if (frameSquareNumListArr != null) PlacementLocation_Frame();
        }



        //===============================================//
        //===========�g�iFrame�j�̌ŗL�֐�===============//
        //===============================================//

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
            foreach (List<int> squareList in frameSquareNumListArr)
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
                    StartCoroutine(PiecesMan.SquareColorChange(color, squareIndex, false));
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
            //�t���[�������E�z�u
            GameObject frameObj = Instantiate(framePrefab[(int)direction]);
            frameObjListArr[groupId].Add(frameObj);
            PiecesMan.PlaceGimmick(frameObj, squareIndex);

            //�t���[���M�~�b�N�̏��擾
            GimmickInformation gimInfo = frameObj.GetComponent<GimmickInformation>();
            frameInfoListArr[groupId].Add(gimInfo);
            gimInfo.InformationSetting_SquareIndex(squareIndex);

            //sprite�ݒ�
            if (colorId == COLORLESS_ID) colorId = COLORS_COUNT;
            gimInfo.spriRen.sprite = spriArr[colorId];
            gimInfo.spriRenChild[1].sprite = frameCornerSprArr[colorId]; //�p1
            gimInfo.spriRenChild[2].sprite = frameCornerSprArr[colorId]; //�p2
        }

        /// <summary>
        /// �g�j��m�F�E���s
        /// </summary>
        public IEnumerator DamageFrame()
        {
            if (frameSquareNumListArr == null) yield break;

            bool frameListNull = true; //�M�~�b�N�̑��݂̗L��
            int groupId = 0;           //�O���[�v�ԍ�

            foreach (List<int> squareList in frameSquareNumListArr)
            {
                if (squareList == null)
                {
                    groupId++;
                    continue;
                }

                frameListNull = false;

                //�w��F�̎擾
                string specifiedColor = "";
                int colorId = frameInfoListArr[groupId][0].colorId;
                if (colorId != COLORLESS_ID)
                {
                    Colors color = (Colors)Enum.ToObject(typeof(Colors), colorId);
                    specifiedColor = color.ToString();
                }

                bool first = true; //���񃋁[�v�t���O
                bool burst = true; //�M�~�b�N�j��t���O

                foreach (int i in squareList)
                {
                    //�w��F�����ŏ��񃋁[�v�̏ꍇ
                    if (first && colorId == COLORLESS_ID)
                    {
                        //�ŏ��̋�̃^�O���w��F�ɐݒ�
                        specifiedColor = squareTraArr[i].GetChild(0).tag;
                        if (specifiedColor == GIMMICK_TAG)
                        {
                            //�M�~�b�N�̏ꍇ�͏����I��
                            burst = false;
                            break;
                        }
                        first = false;
                    }

                    //�^�O�����w��F�łȂ��ꍇ
                    if (specifiedColor != squareTraArr[i].GetChild(0).tag)
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
                    { StartCoroutine(PiecesMan.SquareColorChange(SQUARE_WHITE, i, true)); }
                    frameSquareNumListArr[groupId] = null;
                }

                //���̃O���[�v��
                groupId++;
            }

            //��������x������Ȃ�����(���X�g����)�ꍇ
            if (frameListNull)
            {
                frameInfoListArr      = null;
                frameObjListArr       = null;
                frameSquareNumListArr = null;
            }
        }
    }
}