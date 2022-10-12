using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Option;

public class PuzzleDefine : MonoBehaviour
{
    //----------------�p�Y���ėp�萔----------------//

    //��̐F
    public enum Colors
    {
        Blue,           //��
        Red,            //��
        Yellow,         //��
        Green,          //��
        Violet,         //��
        Orange          //��
    }
    public const int COLORLESS_ID = -1; //���F(�Ǘ��ԍ�)
    public static readonly int COLORS_COUNT = Enum.GetValues(typeof(Colors)).Length; //�F�̐�

    //�M�~�b�N
    public enum Gimmicks
    {
        Balloon,              //���D
        Balloon_Color,        //���D(�F)
        Jewelry,              //���
        Wall,                 //��
        Flower,               //�큨�Q������
        Frame,                //�g
        Frame_Color,          //�g(�F)
        Frame_Color_Change,   //�g(�F�ω�)
        Hamster,              //�n���X�^�[
        GreenWorm,            //��
        GreenWorm_Color,      //��(�F)
        Cage,                 //�B
        NumberTag,            //�ԍ��D
        Thief,                //�D�_
        Steel,                //�|
        FireworksBox,         //�ԉΔ�
        RocketBox,            //���P�b�g��
        Tornado               //����
    }
    public static readonly int GIMMICKS_COUNT = Enum.GetValues(typeof(Gimmicks)).Length;

    //����A�C�e��
    public enum SupportItems
    {
        Duck,           //�A�q��
        Firework,       //�ԉ�
        RocketLine,     //���P�b�g(��)
        RocketColumn,   //���P�b�g(�c)
        Star            //��
    }
    public static readonly int SUPPORT_ITEMS_COUNT = Enum.GetValues(typeof(SupportItems)).Length;

    //8����
    public enum Directions
    {
        Up,         //��
        Down,       //��
        Left,       //��
        Right,      //�E
        UpLeft,     //����
        UpRight,    //�E��
        DownLeft,   //����
        DownRight   //�E��
    }
    public static readonly int DIRECTIONS_COUNT = Enum.GetValues(typeof(Directions)).Length;

    //4����
    public enum FourDirections
    {
        Up,         //��
        Down,       //��
        Left,       //��
        Right       //�E
    }
    public static readonly int FOUR_DIRECTIONS_COUNT = Enum.GetValues(typeof(FourDirections)).Length;

    //�}�X
    public enum Squares
    {
        A1, A2, A3, A4, A5, A6, A7, A8,
        B1, B2, B3, B4, B5, B6, B7, B8,
        C1, C2, C3, C4, C5, C6, C7, C8,
        D1, D2, D3, D4, D5, D6, D7, D8,
        E1, E2, E3, E4, E5, E6, E7, E8,
        F1, F2, F3, F4, F5, F6, F7, F8,
        G1, G2, G3, G4, G5, G6, G7, G8,
        H1, H2, H3, H4, H5, H6, H7, H8
    }

    //Z���W
    public const float Z_ZERO           = 0f;   //0
    public const float Z_PIECE          = -1f;  //��
    public const float Z_GIMMICK        = -2f;  //�M�~�b�N(��Ƃ��ĊǗ����Ȃ�)
    public const float Z_FRONT_GIMMICK  = -3f;  //����O�̃M�~�b�N
    public const float Z_MOST_FRONT     = -4f;  //�ł���O

    //�I�u�W�F�N�g�̃^�O
    public const string GIMMICK_TAG      = "Gimmick";       //�M�~�b�N
    public const string PIECE_TAG        = "Piece";         //��
    public const string SUPPORT_ITEM_TAG = "SupportItem";   //����A�C�e��

    //��������p
    public const int TEN     = 10;
    public const int HUNDRED = 100;

    //�ėp
    public const int    BOARD_COLUMN_COUNT      = 8;                                        //�{�[�h��
    public const int    BOARD_LINE_COUNT        = 8;                                        //�{�[�h�s��
    public const int    SQUARES_COUNT           = BOARD_LINE_COUNT * BOARD_COLUMN_COUNT;    //�{�[�h����
    public const float  SQUARE_DISTANCE         = 1.46f;                                    //�}�X�̋���
    public const float  SQUARE_DISTANCE_HALF    = SQUARE_DISTANCE / 2.0f;                   //���}�X�̋���
    public const float  PIECE_DEFAULT_SCALE     = 0.65f;                                    //��̃X�P�[��

    public static readonly Vector3 PIECE_DEFAULT_POS        = new Vector3(0.0f, 0.0f, Z_PIECE);     //��̊�{���W
    public static readonly Quaternion PIECE_GENERATE_QUEST  = Quaternion.Euler(0.0f, -90.0f, 0.0f); //��̐������̊p�x
    public static readonly Quaternion DEFAULT_QUEST         = Quaternion.Euler(0.0f, 00.0f, 0.0f);  //��{�̊p�x

    public static readonly Color COLOR_PRIMARY    = new Color(1.0f, 1.0f, 1.0f, 1.0f);               //���F
    public static readonly Color COLOR_ALPHA_ZERO = new Color(1.0f, 1.0f, 1.0f, 0.0f);               //����
    public static readonly Color[] COLOR_FADE_OUT = new Color[] { COLOR_PRIMARY, COLOR_ALPHA_ZERO }; //�t�F�[�h�A�E�g
    public static readonly Color[] COLOR_FADE_IN  = new Color[] { COLOR_ALPHA_ZERO, COLOR_PRIMARY }; //�t�F�[�h�C��


    //----------------���o�萔----------------//

    //�}�X�F
    public static readonly Color SQUARE_BLUE   = new Color(0.6f, 0.6f, 1.0f, 1.0f);   //��
    public static readonly Color SQUARE_RED    = new Color(1.0f, 0.6f, 0.6f, 1.0f);   //��
    public static readonly Color SQUARE_YELLOW = new Color(1.0f, 1.0f, 0.6f, 1.0f);   //��
    public static readonly Color SQUARE_GREEN  = new Color(0.6f, 1.0f, 0.6f, 1.0f);   //��
    public static readonly Color SQUARE_VIOLET = new Color(1.0f, 0.6f, 1.0f, 1.0f);   //��
    public static readonly Color SQUARE_ORANGE = new Color(1.0f, 0.6f, 0.3f, 1.0f);   //��
    public static readonly Color SQUARE_BLACK  = new Color(0.6f, 0.6f, 0.6f, 1.0f);   //��
    public static readonly Color SQUARE_WHITE  = new Color(1.0f, 1.0f, 1.0f, 1.0f);   //��

    //�}�X�̐F�ω����x
    public const float SQUARE_CHANGE_SPEED = 0.3f;

    //��]
    public static readonly Vector3 REVERSE_PIECE_ROT_SPEED              = new Vector3(0.0f, 10.0f, 0.0f);  //��]���x
    public static readonly Vector3 REVERSE_PIECE_SWITCH_ROT             = new Vector3(0.0f, 90.0f, 0.0f);  //���]����؂�ւ��p�x
    public static readonly Vector3 REVERSE_PIECE_FRONT_ROT              = Vector3.zero;                    //���
    public static readonly WaitForSeconds PIECE_REVERSAL_INTERVAL       = new WaitForSeconds(0.05f);       //��̔��]�Ԋu
    public static readonly WaitForSeconds PIECE_GROUP_REVERSAL_INTERVAL = new WaitForSeconds(0.1f);        //��O���[�v�̔��]�Ԋu
    public const float REVERSE_PIECE_SCALING_SPEED = 0.02f;  //�g�k���x
    public const float REVERSE_PIECE_CHANGE_SCALE  = 0.9f;   //�g�厞�̃X�P�[��

    //��j��
    public const float DESTROY_PIECE_SCALING_SPEED = 0.03f;  //�g�k���x
    public const float DESTROY_PIECE_CHANGE_SCALE  = 0.0f;   //�j�󎞂̃X�P�[��

    //��z�u
    public const float PUT_PIECE_SCALING_SPEED = 0.02f;         //�g�k���x
    public const float PUT_PIECE_CHANGE_SCALE  = 0.8f;          //�g�厞�̃X�P�[��
    public const float PUT_PIECE_MOVE_SPEED    = 0.2f;          //�ړ����x
    public const float NEXT_PIECE_SLIDE_SPEED  = 0.3f;          //�ҋ@��̃X���C�h���x
    public static readonly Vector3 PUT_PIECE_MOVE_TARGET_POS = new Vector3(0.0f, 0.0f, Z_MOST_FRONT);   //�ړ��ڕW���W

    //���
    public const float FALL_PIECE_MOVE_SPEED  = 0.07f;  //�������x
    public const float FALL_PIECE_ACCELE_RATE = 0.02f;  //��������



    //----------------�X�e�[�W�萔----------------//

    //�ڕW���z��̃C���f�N�X�ԍ�
    enum TargetInfoIndex
    {
        Object, //�I�u�W�F�N�g
        Color,  //�F
        Count,  //����

        Length  //���z��T�C�Y
    }
    public static readonly int TARGET_INFO_OBJ      = (int)TargetInfoIndex.Object;  //�I�u�W�F�N�g
    public static readonly int TARGET_INFO_COLOR    = (int)TargetInfoIndex.Color;   //�F
    public static readonly int TARGET_INFO_COUNT    = (int)TargetInfoIndex.Count;   //����
    public static readonly int TARGET_INFO_LENGTH   = (int)TargetInfoIndex.Length;  //�z��̒���

    //�����M�~�b�N���z��̃C���f�N�X�ԍ�
    enum FallGimmickInfoIndex
    {
        Type,    //�M�~�b�N�̎��
        Color,      //�F
        Count,      //����

        Length      //���z��T�C�Y
    }
    public static readonly int FALL_GMCK_TYPE    = (int)FallGimmickInfoIndex.Type;       //�M�~�b�N�̎��
    public static readonly int FALL_GMCK_COLOR   = (int)FallGimmickInfoIndex.Color;      //�F
    public static readonly int FALL_GMCK_COUNT   = (int)FallGimmickInfoIndex.Count;      //����
    public static readonly int FALL_GMCK_LENGTH  = (int)FallGimmickInfoIndex.Length;     //�z��̒���

    //�z�u�M�~�b�N���z��̃C���f�b�N�X�ԍ�
    enum SetGimmickInfoIndex
    {
        Square,     //�z�u�}�X
        Type,       //�M�~�b�N�̎��
        Color,      //�w��F
        Group,      //�Ǘ��O���[�v
        Width,      //����
        Height,     //����
        Quantity,   //�w���
        Order,      //�w�菇��

        Length      //���z��T�C�Y
    }
    public static readonly int SET_GMCK_SQUARE   = (int)SetGimmickInfoIndex.Square;      //�z�u�}�X
    public static readonly int SET_GMCK_TYPE     = (int)SetGimmickInfoIndex.Type;        //�M�~�b�N�̎��
    public static readonly int SET_GMCK_COLOR    = (int)SetGimmickInfoIndex.Color;       //�w��F
    public static readonly int SET_GMCK_GROUP    = (int)SetGimmickInfoIndex.Group;       //�Ǘ��O���[�v
    public static readonly int SET_GMCK_WIDTH    = (int)SetGimmickInfoIndex.Width;       //����
    public static readonly int SET_GMCK_HEIGHT   = (int)SetGimmickInfoIndex.Height;      //����
    public static readonly int SET_GMCK_QUANTITY = (int)SetGimmickInfoIndex.Quantity;    //�w���
    public static readonly int SET_GMCK_ORDER    = (int)SetGimmickInfoIndex.Order;       //�w�菇��
    public static readonly int SET_GMCK_LENGTH   = (int)SetGimmickInfoIndex.Length;      //�z��̒���
    public const int NOT_NUM = -1;  //�e���ڔԍ��w���Ȃ��萔

    private const int TURN_MAX = 99;    //�^�[���ő吔


    //----------------���\�[�X�f�[�^----------------//

    public  static GimmicksData GIMMICKS_DATA { get; private set; }  //�M�~�b�N�f�[�^


    //----------------�X�e�[�W�ʃX�^�e�B�b�N�ϐ�----------------//

    //�p�Y���V�[���t���O
    public enum PuzzleFlag
    {
        GamePreparation,        //�Q�[���������H
        GameOver,               //�Q�[���I�[�o�[�H
        GameClear,              //�Q�[���N���A�H
        TurnRecovered,          //�^�[���񕜍ρH(��x�Q�[���[�o�[����)
        Uncontinuable,          //�Q�[���p���s�\
        NowPuttingPieces,       //��z�u���H
        NowReversingPieces,     //��]���H
        NowDestroyingPieces,    //��j�󒆁H
        NowFallingPieces,       //������H
        NowGimmickDestroyWait,  //�M�~�b�N�j��ҋ@���H
        NowGimmickStateChange,  //�M�~�b�N��ԕω����H
        NowSupportItemUse,      //����A�C�e���g�p��?
        NowSupportItemReady,    //����A�C�e��������?
        NowTurnEndProcessing,   //�^�[���I���������H
        NowOptionView,          //�I�v�V�����\�����H
        NowForcedTutorial,      //�����`���[�g���A�����H

        Length   //�t���O�z��T�C�Y
    }
    static readonly bool[] PuzzleFlags = new bool[(int)PuzzleFlag.Length];

    //�X�e�[�W�ʐݒ荀��
    public static int     STAGE_NUMBER          { get; private set; }   //�X�e�[�W�ԍ�
    public static int[]   USE_COLOR_TYPE_ARR    { get; private set; }   //�g�p�F�̎��
    public static int     USE_COLOR_COUNT       { get; private set; }   //�g�p�F�̐�
    public static int[]   HIDE_SQUARE_ARR       { get; private set; }   //��\���}�X�̊Ǘ��ԍ�
    public static int[][] GIMMICKS_INFO_ARR     { get; private set; }   //�z�u�M�~�b�N�̎�ނƃ}�X�̊Ǘ��ԍ�
    public static int     GIMMICKS_DEPLOY_COUNT { get; private set; }   //�����M�~�b�N�̔z�u��
    public static int     GIMMICKS_GROUP_COUNT  { get; private set; }   //�M�~�b�N�̃O���[�v�̐ݒ萔
    public static int[][] FALL_GMCK_INFO_ARR    { get; private set; }   //�����M�~�b�N�̎�ނƐ���
    public static int     FALL_GMCK_OBJ_COUNT   { get; private set; }   //�����M�~�b�N�̑���
    public static int[][] TARGETS_INFO_ARR      { get; private set; }   //�ڕW���
    public static int     TARGETS_COUNT         { get; private set; }   //�ڕW�̃I�u�W�F�N�g��
    public static int     TURN_COUNT            { get; private set; }   //�^�[����


    //=================================================//
    //----------------------�֐�-----------------------//
    //=================================================//

    /// <summary>
    /// �t���O�擾
    /// </summary>
    /// <param name="_flag">�t���O�^�C�v</param>
    public static bool GetFlag(PuzzleFlag _flag) => PuzzleFlags[(int)_flag];

    /// <summary>
    /// �t���OON
    /// </summary>
    /// <param name="_flag">�t���O�^�C�v</param>
    public static void FlagOn(PuzzleFlag _flag) => PuzzleFlags[(int)_flag] = true;

    /// <summary>
    /// �t���OOFF
    /// </summary>
    /// <param name="_flag">�t���O�^�C�v</param>
    public static void FlagOff(PuzzleFlag _flag) => PuzzleFlags[(int)_flag] = false;

    /// <summary>
    /// �t���O���Z�b�g
    /// </summary>
    public static void FlagReset()
    {
        for (int i = 0; i < (int)PuzzleFlag.Length; i++)
        { PuzzleFlags[i] = false; }
    }

    /// <summary>
    /// ����\�m�F
    /// </summary>
    /// <returns></returns>
    public static bool IsOperable()
    {
        for (int i = 0; i < (int)PuzzleFlag.Length; i++)
        {
            switch (i)
            {
                //����Ɋ֌W�̂Ȃ��t���O�͖���
                case (int)PuzzleFlag.TurnRecovered:         //�^�[���񕜍ρH(��x�Q�[���[�o�[����)
                case (int)PuzzleFlag.NowSupportItemReady:   //����A�C�e���������H
                    continue;
            }
            if (PuzzleFlags[i]) return false;
        }

        return true;
    }

    /// <summary>
    /// ���\�[�X�f�[�^�擾
    /// </summary>
    public static void LoadResourcesData()
    {
        GIMMICKS_DATA = Resources.Load("GimmicksData") as GimmicksData;
    }

    /// <summary>
    /// �X�e�[�W�ݒ�
    /// </summary>
    public static void StageSetting()
    {
        //�X�e�[�W�ݒ�擾
        StagesDataData stageData = GameManager.SelectStageData;

        //�X�e�[�W�ԍ��ݒ�
        STAGE_NUMBER = stageData.Id;

        //�g�p�F
        USE_COLOR_TYPE_ARR = stageData.Usecolor;
        USE_COLOR_COUNT = USE_COLOR_TYPE_ARR.Length;

        //��\���}�X
        HIDE_SQUARE_ARR = stageData.Hidesqr;

        //�^�[����
        TurnSet(stageData.Turn);

        //�����\������`���[�g���A���^�C�v
        OptionManager.ForcedTutorialType = stageData.TUTORIALTYPE;

        //�z�u�M�~�b�N, �����M�~�b�N,�ڕW�̎擾�p���X�g�E�萔�ݒ�
        const string setGimInfoName     = "Setgimmicks_";
        const string fallGimInfoName    = "Fallgimmicks_";
        const string targetInfoName     = "Tagets_";
        List<int[]> setGimList          = new List<int[]>();
        List<int[]> fallGimList         = new List<int[]>();
        List<int[]> piecesTargetsList   = new List<int[]>();

        //StagesDataData�N���X�̃����o�v���p�e�B(�ϐ�)���擾
        System.Reflection.PropertyInfo[] properties = typeof(StagesDataData).GetProperties();
        foreach (System.Reflection.PropertyInfo pro in properties)
        {
            //�z�u�M�~�b�N���擾
            if (pro.Name.Contains(setGimInfoName))
            {
                //�ϐ�����{setGimInfoName}���܂܂�Ă�����̂��擾
                int[] gimInfoArr = pro.GetValue(stageData) as int[];
                if (gimInfoArr[0] < 0) continue;

                //�Œ蒷�z��ɏC��
                int[] infoArr = new int[SET_GMCK_LENGTH];
                for (int i = 0; i < SET_GMCK_LENGTH; i++)
                {
                    if (gimInfoArr.Length > i) infoArr[i] = gimInfoArr[i];
                    else infoArr[i] = NOT_NUM;
                }
                setGimList.Add(infoArr);
            }
            //�����M�~�b�N���擾
            else if (pro.Name.Contains(fallGimInfoName))
            {
                int[] gimInfoArr = pro.GetValue(stageData) as int[];
                if (gimInfoArr[0] < 0) continue;
                fallGimList.Add(gimInfoArr);
            }
            //�ڕW���擾(��̂�)
            else if (pro.Name.Contains(targetInfoName))
            {
                int[] targetInfoArr = pro.GetValue(stageData) as int[];
                if (targetInfoArr[0] < 0) continue;

                //�Œ蒷�z��ɏC��
                int[] infoArr = new int[TARGET_INFO_LENGTH];
                for (int i = 0; i < TARGET_INFO_LENGTH; i++)
                {
                    if (i == TARGET_INFO_OBJ) infoArr[i] = -1;  //��̃I�u�W�F�N�g�ԍ���0�������w��
                    else infoArr[i] = targetInfoArr[i - 1];
                }
                piecesTargetsList.Add(infoArr);
            }
        }

        //�z�u�M�~�b�N���ݒ�
        GIMMICKS_INFO_ARR = setGimList.ToArray();
        GIMMICKS_DEPLOY_COUNT = GIMMICKS_INFO_ARR.Length;
        List<int[]> targetsList = new List<int[]>();
        List<int> usedGroupNum = new List<int>();
        GIMMICKS_GROUP_COUNT = 0;
        foreach (int[] gimmickInfo in GIMMICKS_INFO_ARR)
        {
            //�M�~�b�N�̃O���[�v���擾
            if (gimmickInfo[SET_GMCK_GROUP] > NOT_NUM && !usedGroupNum.Contains(gimmickInfo[SET_GMCK_GROUP]))
            {
                usedGroupNum.Add(GIMMICKS_GROUP_COUNT);
                GIMMICKS_GROUP_COUNT++;

                //�ڕW�ǉ��i�F�g�n�͂����Œǉ��j
                TargetAdd(gimmickInfo[SET_GMCK_TYPE], gimmickInfo[SET_GMCK_COLOR], 1);
            }

            //�F�g�n�̓X�L�b�v�i�O���[�v�P�ʂ�1�����̈ח�O�j
            switch (gimmickInfo[SET_GMCK_TYPE])
            {
                //�F�g�n�̓X�L�b�v�i�O���[�v�P�ʂ�1�����̈ח�O�j
                case (int)Gimmicks.Frame:
                case (int)Gimmicks.Frame_Color:
                case (int)Gimmicks.Frame_Color_Change:
                    break;

                //�ڕW�ǉ�
                default:
                    TargetAdd(gimmickInfo[SET_GMCK_TYPE], gimmickInfo[SET_GMCK_COLOR], 1);
                    break;
            }
        }

        //�����M�~�b�N���ݒ�
        FALL_GMCK_INFO_ARR = fallGimList.ToArray();
        foreach (int[] fallGmckInfo in FALL_GMCK_INFO_ARR)
        {
            FALL_GMCK_OBJ_COUNT += fallGmckInfo[FALL_GMCK_COUNT];
        }

        //�����M�~�b�N�̖ڕW�ǉ�
        foreach (int[] gimmickInfo in FALL_GMCK_INFO_ARR)
        {
            TargetAdd(gimmickInfo[FALL_GMCK_TYPE], gimmickInfo[FALL_GMCK_COLOR], gimmickInfo[FALL_GMCK_COUNT]);
        }

        //�ڕW����ݒ�
        targetsList.AddRange(piecesTargetsList);    //��ƃM�~�b�N�̖ڕW���X�g������
        TARGETS_INFO_ARR = targetsList.ToArray();
        TARGETS_COUNT = TARGETS_INFO_ARR.Length;

        //�ڕW�̒ǉ�
        void TargetAdd(int _gimTypeId, int _colorId, int _addCount)
        {
            //�F�ԍ��C��
            switch (_gimTypeId)
            {
                //�F�w�肪���邪�ڕW��F�ʂɐݒ肵�Ȃ�����
                case (int)Gimmicks.Jewelry:             //���
                case (int)Gimmicks.Frame_Color_Change:  //�F�g(�F�ω�)
                case (int)Gimmicks.Cage:                //�B
                    _colorId = COLORLESS_ID;
                    break;
            }

            //�ڕW�ݒ�(�z�u�M�~�b�N)
            for (int i = 0; i < targetsList.Count; i++)
            {
                //�I�u�W�F�N�g�ԍ��ƐF�ԍ��������ꍇ
                if (targetsList[i][TARGET_INFO_OBJ] == _gimTypeId &&
                    targetsList[i][TARGET_INFO_COLOR] == _colorId)
                {
                    //�ǉ����Ĉȍ~�̏������X�L�b�v
                    targetsList[i][TARGET_INFO_COUNT] += _addCount;
                    return;
                }
            }

            //�ΏۃM�~�b�N�̔z�񂪖��쐬�������ꍇ�A�V�K�쐬
            int[] newTragets = new int[TARGET_INFO_LENGTH];
            newTragets[TARGET_INFO_OBJ] = _gimTypeId;
            newTragets[TARGET_INFO_COLOR] = _colorId;
            newTragets[TARGET_INFO_COUNT] = _addCount;
            targetsList.Add(newTragets);
        }
    }

    /// <summary>
    /// �^�[���ݒ�
    /// </summary>
    /// <param name="_turn"></param>
    public static void TurnSet(int _turn)
    {
        TURN_COUNT = Mathf.Clamp(_turn, 0, TURN_MAX);
    }
}