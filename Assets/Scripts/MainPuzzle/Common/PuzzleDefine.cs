using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static CommonDefine;

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
    public const float Z_ZERO    = 0.0f;    //0
    public const float Z_PIECE   = -1.0f;   //��
    public const float Z_GIMMICK = -2.0f;   //�M�~�b�N(��Ƃ��ĊǗ����Ȃ�)

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
    public const float PUT_PIECE_SCALING_SPEED = 0.02f;             //�g�k���x
    public const float PUT_PIECE_CHANGE_SCALE  = 0.8f;              //�g�厞�̃X�P�[��
    public const float PUT_PIECE_MOVE_START_Z  = Z_GIMMICK - 0.1f;  //�ړ��J�nz���W(localPosition)
    public const float PUT_PIECE_MOVE_SPEED    = 0.2f;              //�ړ����x
    public const float NEXT_PIECE_SLIDE_SPEED  = 0.3f;              //�ҋ@��̃X���C�h���x

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
    public static readonly int TARGET_INFO_OBJ   = (int)TargetInfoIndex.Object; //�I�u�W�F�N�g
    public static readonly int TARGET_INFO_COLOR = (int)TargetInfoIndex.Color;  //�F
    public static readonly int TARGET_INFO_COUNT = (int)TargetInfoIndex.Count;  //����

    //�I�u�W�F�N�g�̃^�O
    public const string GIMMICK_TAG      = "Gimmick";       //�M�~�b�N
    public const string PIECE_TAG        = "Piece";         //��
    public const string SUPPORT_ITEM_TAG = "SupportItem";   //����A�C�e��

    //�M�~�b�N���z��̃C���f�b�N�X�ԍ�
    enum GimmickInfoIndex
    {
        Square,     //�z�u�}�X
        Gimmick,    //�M�~�b�N�̎��
        Color,      //�w��F
        Group,      //�Ǘ��O���[�v
        Width,      //����
        Height,     //����
        Quantity,   //�w���
        Order,      //�w�菇��

        Length      //���z��T�C�Y
    }
    public static readonly int SQUARE   = (int)GimmickInfoIndex.Square;     //�z�u�}�X
    public static readonly int GIMMICK  = (int)GimmickInfoIndex.Gimmick;    //�M�~�b�N�̎��
    public static readonly int COLOR    = (int)GimmickInfoIndex.Color;      //�w��F
    public static readonly int GROUP    = (int)GimmickInfoIndex.Group;      //�Ǘ��O���[�v
    public static readonly int WIDTH    = (int)GimmickInfoIndex.Width;      //����
    public static readonly int HEIGHT   = (int)GimmickInfoIndex.Height;     //����
    public static readonly int QUANTITY = (int)GimmickInfoIndex.Quantity;   //�w���
    public static readonly int ORDER    = (int)GimmickInfoIndex.Order;      //�w�菇��
    public const int NOT_NUM = -1;  //�e���ڔԍ��w���Ȃ��萔

    private const int TURN_MAX = 99;    //�^�[���ő吔



    //----------------���\�[�X�f�[�^----------------//

    public  static GimmicksData GIMMICKS_DATA { get; private set; }  //�M�~�b�N�f�[�^
    private static StagesData   STAGES_DATA   { get; set; }          //�X�e�[�W�f�[�^


    //----------------�X�e�[�W�ʃX�^�e�B�b�N�ϐ�----------------//

    //�t���O
    public static bool GAME_START                   = false;  //�Q�[���J�n�H
    public static bool GAME_OVER                    = false;  //�Q�[���I�[�o�[�H
    public static bool GAME_CLEAR                   = false;  //�Q�[���N���A�H
    public static bool TURN_RECOVERED               = false;  //�^�[���񕜍ρH(��x�Q�[���[�o�[����)
    public static bool NOW_PUTTING_PIECES           = false;  //��z�u���H
    public static bool NOW_REVERSING_PIECES         = false;  //��]���H
    public static bool NOW_DESTROYING_PIECES        = false;  //��j�󒆁H
    public static bool NOW_FALLING_PIECES           = false;  //������H
    public static bool NOW_GIMMICK_DESTROY_WAIT     = false;  //�M�~�b�N�j��ҋ@���H
    public static bool NOW_GIMMICK_STATE_CHANGE     = false;  //�M�~�b�N��ԕω����H
    public static bool NOW_SUPPORT_ITEM_USE         = false;  //����A�C�e���g�p��?
    public static bool NOW_SUPPORT_ITEM_READY       = false;  //����A�C�e��������?
    public static bool NOW_TURN_END_PROCESSING      = false;  //�^�[���I���������H
    public static bool NOW_OPTION_VIEW              = false;  //�I�v�V�����\�����H

    //�X�e�[�W�ʐݒ荀��
    public static int     STAGE_NUMBER          { get; private set; }   //�X�e�[�W�ԍ�
    public static int[]   USE_COLOR_TYPE_ARR    { get; private set; }   //�g�p�F�̎��
    public static int     USE_COLOR_COUNT       { get; private set; }   //�g�p�F�̐�
    public static int[]   HIDE_SQUARE_ARR       { get; private set; }   //��\���}�X�̊Ǘ��ԍ�
    public static int[][] GIMMICKS_INFO_ARR     { get; private set; }   //�M�~�b�N�̎�ނƃ}�X�̊Ǘ��ԍ�
    public static int     GIMMICKS_DEPLOY_COUNT { get; private set; }   //�����M�~�b�N�̔z�u��
    public static int     GIMMICKS_GROUP_COUNT  { get; private set; }   //�M�~�b�N�̃O���[�v�̐ݒ萔
    public static int[][] TARGETS_INFO_ARR      { get; private set; }   //�ڕW���
    public static int     TARGETS_COUNT         { get; private set; }   //�ڕW�̃I�u�W�F�N�g��
    public static int     TURN_COUNT            { get; private set; }   //�^�[����


    //=================================================//
    //----------------------�֐�-----------------------//
    //=================================================//

    /// <summary>
    /// �t���O���Z�b�g
    /// </summary>
    public static void FlagReset()
    {
        GAME_START                  = false;
        GAME_OVER                   = false;
        GAME_CLEAR                  = false;
        TURN_RECOVERED              = false;
        NOW_PUTTING_PIECES          = false;
        NOW_REVERSING_PIECES        = false;
        NOW_DESTROYING_PIECES       = false;
        NOW_FALLING_PIECES          = false;
        NOW_GIMMICK_DESTROY_WAIT    = false;
        NOW_GIMMICK_STATE_CHANGE    = false;
        NOW_SUPPORT_ITEM_USE        = false;
        NOW_SUPPORT_ITEM_READY      = false;
        NOW_TURN_END_PROCESSING     = false;
        NOW_OPTION_VIEW             = false;
    }

    /// <summary>
    /// ����\�m�F
    /// </summary>
    /// <returns></returns>
    public static bool IsOperable()
    {
        if (GAME_CLEAR)                 return false;  //�Q�[���N���A
        if (GAME_OVER)                  return false;  //�Q�[���I�[�o�[
        if (NOW_PUTTING_PIECES)         return false;  //��z�u��
        if (NOW_REVERSING_PIECES)       return false;  //��]��
        if (NOW_DESTROYING_PIECES)      return false;  //��j��
        if (NOW_FALLING_PIECES)         return false;  //�����
        if (NOW_GIMMICK_DESTROY_WAIT)   return false;  //�M�~�b�N�j��ҋ@��
        if (NOW_GIMMICK_STATE_CHANGE)   return false;  //�M�~�b�N��ԕω���
        if (NOW_SUPPORT_ITEM_USE)       return false;  //����A�C�e���g�p��
        if (NOW_TURN_END_PROCESSING)    return false;  //�^�[���I��������
        if (NOW_OPTION_VIEW)            return false;  //�I�v�V�����\����

        return true;
    }

    /// <summary>
    /// ���\�[�X�f�[�^�擾
    /// </summary>
    public static void LoadResourcesData()
    {
        GIMMICKS_DATA = Resources.Load("GimmicksData") as GimmicksData;
        STAGES_DATA = Resources.Load("StagesData") as StagesData;
    }

    /// <summary>
    /// �X�e�[�W�ݒ�
    /// </summary>
    public static void StageSetting()
    {
        //�X�e�[�W�ԍ��ݒ�
        STAGE_NUMBER = GameManager.SelectStage;

        //�X�e�[�W�f�[�^�ǂݍ���
        var stageData = STAGES_DATA.dataArray[STAGE_NUMBER - 1];

        //�g�p�F
        USE_COLOR_TYPE_ARR = stageData.Use_Color;
        USE_COLOR_COUNT = USE_COLOR_TYPE_ARR.Length;

        //��\���}�X
        HIDE_SQUARE_ARR = stageData.Hide_Sqr;

        //�^�[����
        TurnSet(stageData.Turn);

        //�z�u�M�~�b�N�擾�p���X�g,�萔�ݒ�
        const int gimInfoLength = (int)GimmickInfoIndex.Length;
        const string gimInfoName = "Gimmicks_";
        List<int[]> gimmicksList = new List<int[]>();

        //�ڕW�擾�p���X�g,�萔�ݒ�
        const int targetInfoLength = (int)TargetInfoIndex.Length;
        const string targetInfoName = "Tagets_";
        List<int[]> targetsList = new List<int[]>();

        //StagesDataData�N���X�̃����o�v���p�e�B(�ϐ�)���擾
        System.Reflection.PropertyInfo[] properties = typeof(StagesDataData).GetProperties();
        foreach (System.Reflection.PropertyInfo pro in properties)
        {
            //�z�u�M�~�b�N���擾
            if (pro.Name.Contains(gimInfoName))
            {
                int[] gimInfoArr = pro.GetValue(stageData) as int[];
                if (gimInfoArr.Length == gimInfoLength)
                    gimmicksList.Add(gimInfoArr);
            }

            //�ڕW���擾
            if (pro.Name.Contains(targetInfoName))
            {
                int[] targetInfoArr = pro.GetValue(stageData) as int[];
                if (targetInfoArr.Length == targetInfoLength)
                    targetsList.Add(targetInfoArr);
            }
        }

        //�z�u�M�~�b�N���ݒ�
        GIMMICKS_INFO_ARR = gimmicksList.ToArray();
        GIMMICKS_DEPLOY_COUNT = GIMMICKS_INFO_ARR.Length;
        List<int> usedGroupNum = new List<int>();
        GIMMICKS_GROUP_COUNT = 0;
        foreach (int[] gimmickInfo in GIMMICKS_INFO_ARR)
        {
            if (gimmickInfo[GROUP] > NOT_NUM && !usedGroupNum.Contains(gimmickInfo[GROUP]))
            {
                usedGroupNum.Add(GIMMICKS_GROUP_COUNT);
                GIMMICKS_GROUP_COUNT++;
            }
        }

        //�ڕW���ݒ�
        TARGETS_INFO_ARR = targetsList.ToArray();
        TARGETS_COUNT = TARGETS_INFO_ARR.Length;
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