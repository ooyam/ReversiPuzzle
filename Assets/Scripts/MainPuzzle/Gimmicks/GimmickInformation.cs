using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class GimmickInformation : MonoBehaviour
{
    //�R���|�[�l���g
    [System.NonSerialized] public Animator ani;
    [System.NonSerialized] public Transform tra;
    [System.NonSerialized] public SpriteRenderer   spriRen;
    [System.NonSerialized] public SpriteRenderer[] spriRenChild;
    [System.NonSerialized] public GameObject       obj;
    [System.NonSerialized] public GameObject[]     objChild;

    //�M�~�b�N���
    [System.NonSerialized] public int     settingIndex;         //�X�e�[�W���̐ݒ�ԍ�
    [System.NonSerialized] public int     startSquareId;        //�����}�X�ԍ�
    [System.NonSerialized] public int     nowSquareId;          //���݂̃}�X�ԍ�
    [System.NonSerialized] public int     groupId;              //�O���[�v�ԍ�
    [System.NonSerialized] public int     id;                   //�M�~�b�N�ԍ�
    [System.NonSerialized] public int     colorId;              //�F�ԍ�
    [System.NonSerialized] public int     remainingTimes;       //�c��_���[�W��(�M�~�b�N�̌Œ萔)
    [System.NonSerialized] public int     remainingQuantity;    //�c��_���[�W��(�X�e�[�W���̎w�萔)
    [System.NonSerialized] public int     order;                //�w��ԍ�
    [System.NonSerialized] public bool    freeFall;             //���R�����t���O
    [System.NonSerialized] public bool    destructible;         //�j��\�t���O(true�F�j��\)
    [System.NonSerialized] public bool    nowTurnDamage;        //���̃^�[���Ƀ_���[�W���󂯂����̃t���O
    [System.NonSerialized] public bool    assaultOnly;          //�����ł̂ݔj��\
    [System.NonSerialized] public bool    inSquare;             //��Ƃ��Ĕz�u���邩�̃t���O(true�F��Ƃ��Ĕz�u)
    [System.NonSerialized] public Vector3 defaultPos;           //����W
    [System.NonSerialized] public Vector3 defaultScale;         //��X�P�[��
    [System.NonSerialized] public int[]   innerSquaresId;       //�����̃}�X�ԍ�

    //��Ƃ��ĊǗ�����ꍇ�̏��
    [System.NonSerialized] public bool freeFall_Piece;          //���R�����t���O(��Ƃ���)
    [System.NonSerialized] public bool destructible_Piece;      //�j��\�t���O(true�F�j��\)


    /// <summary>
    /// �R���|�[�l���g�̐ݒ�
    /// </summary>
    void ComponentSetting()
    {
        ani     = GetComponent<Animator>();
        tra     = transform;
        spriRen = GetComponent<SpriteRenderer>();
        obj     = gameObject;

        int childCount = tra.childCount;
        objChild     = new GameObject[childCount];
        spriRenChild = new SpriteRenderer[childCount];
        for (int i = 0; i < childCount; i++)
        {
            objChild[i]     = tra.GetChild(i).gameObject;
            spriRenChild[i] = objChild[i].GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// ���ʐݒ�
    /// </summary>
    /// <param name="_index">  �X�e�[�W���̐ݒ�ԍ� </param>
    /// <param name="_typeId">  �M�~�b�N�ԍ�</param>
    /// <param name="_colorId"> �F�ԍ�</param>
    /// <param name="_squareId">���݂̃}�X�ԍ�</param>
    /// <param name="_groupId"> �O���[�v�ԍ�</param>
    /// <param name="_quantity">�c��_���[�W��</param>
    /// <param name="_order">   �w��ԍ�</param>
    void CommonInfoSetting(int _index, int _typeId, int _colorId, int _squareId = NOT_NUM, int _groupId = NOT_NUM, int _quantity = NOT_NUM, int _order = NOT_NUM)
    {
        var data            = GIMMICKS_DATA.dataArray[_typeId];
        settingIndex        = _index;
        startSquareId       = _squareId;
        nowSquareId         = startSquareId;
        groupId             = _groupId;
        id                  = data.Id;
        colorId             = _colorId;
        remainingTimes      = data.Damage_Times;
        remainingQuantity   = _quantity;
        order               = _order;
        freeFall            = data.Free_Fall;
        destructible        = !data.Continuous;
        assaultOnly         = data.Assault_Only;
        inSquare            = data.In_Square;
        defaultPos          = new Vector3(data.Position_X, data.Position_Y, (inSquare) ? Z_PIECE : Z_GIMMICK);
        defaultScale        = new Vector3(data.Scale_X, data.Scale_Y, 1.0f);
    }

    /// <summary>
    /// �M�~�b�N���̐ݒ�
    /// </summary>
    /// <param name="_index">�X�e�[�W���̃M�~�b�N�Ǘ��ԍ�</param>
    public void InformationSetting(int _index)
    {
        ComponentSetting();
        CommonInfoSetting(
            _index,                                         //�X�e�[�W���̐ݒ�ԍ�
            GIMMICKS_INFO_ARR[_index][SET_GMCK_TYPE],       //�M�~�b�N�ԍ�
            GIMMICKS_INFO_ARR[_index][SET_GMCK_COLOR],      //�F�ԍ�
            GIMMICKS_INFO_ARR[_index][SET_GMCK_SQUARE],     //���݂̃}�X�ԍ�
            GIMMICKS_INFO_ARR[_index][SET_GMCK_GROUP],      //�O���[�v�ԍ�
            GIMMICKS_INFO_ARR[_index][SET_GMCK_QUANTITY],   //�c��_���[�W��
            GIMMICKS_INFO_ARR[_index][SET_GMCK_ORDER]);     //�w��ԍ�

        switch (id)
        {
            //�M�~�b�N�����̋�𑀍�֎~�ɂ���M�~�b�N
            case (int)Gimmicks.Cage:    //�B
                innerSquaresId = new int[GIMMICKS_INFO_ARR[_index][SET_GMCK_WIDTH] * GIMMICKS_INFO_ARR[_index][SET_GMCK_HEIGHT]];
                int i = 0;
                for (int w = 0; w < GIMMICKS_INFO_ARR[_index][SET_GMCK_WIDTH]; w++)      //�������[�v
                {
                    for (int h = 0; h < GIMMICKS_INFO_ARR[_index][SET_GMCK_HEIGHT]; h++) //���������[�v
                    {
                        innerSquaresId[i] = startSquareId + w * BOARD_LINE_COUNT + h;
                        i++;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// �M�~�b�N���̐ݒ�(�}�X�ԍ�����)
    /// </summary>
    /// <param name="_squareIndex">�}�X�Ǘ��ԍ�</param>
    /// <param name="_gimmickId">  �M�~�b�N�ԍ�</param>
    /// <param name="_groupId">    �O���[�v�ԍ�</param>
    public void InformationSetting_SquareIndex(int _squareIndex, int _gimmickId, int _groupId)
    {
        for (int i = 0; i < GIMMICKS_DEPLOY_COUNT; i++)
        {
            if (GIMMICKS_INFO_ARR[i][SET_GMCK_SQUARE] == _squareIndex &&
                (GIMMICKS_INFO_ARR[i][SET_GMCK_TYPE] == _gimmickId || (_groupId >= 0 && GIMMICKS_INFO_ARR[i][SET_GMCK_GROUP] == _groupId)))
            {
                InformationSetting(i);
                break;
            }
        }
    }

    /// <summary>
    /// �M�~�b�N���̐ݒ�(�����M�~�b�N)
    /// </summary>
    /// <param name="_index">�Ǘ��ԍ�</param>
    /// <param name="_typeId">���</param>
    /// <param name="_colorId">�F�ԍ�</param>
    public void InformationSetting_FallGimmicks(int _index, int _typeId, int _colorId)
    {
        ComponentSetting();
        CommonInfoSetting(_index, _typeId, _colorId);
    }

    /// <summary>
    /// ��Ƃ��ăt���O�ݒ�
    /// </summary>
    /// <param name="_squareIndex">     �z�u�}�X�Ǘ��ԍ�</param>
    /// <param name="_generate">        �����H</param>
    /// <param name="gimmickInfoArr">   �M�~�b�N���z��</param>
    public void OperationFlagSetting(int _squareIndex, bool _generate, GimmickInformation[] gimmickInfoArr = null)
    {
        OperationFlagON();
        if (!_generate || !inSquare) return;

        foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
        {
            if (gimmickInfo == null) continue;
            if (gimmickInfo.innerSquaresId == null) continue;
            foreach (int squareId in gimmickInfo.innerSquaresId)
            {
                //�M�~�b�N�̓����ɐ������ꂽ�ꍇ
                if (squareId == _squareIndex)
                {
                    OperationFlagOFF();
                }
            }
        }
    }

    /// <summary>
    /// ����t���O���I���ɂ���
    /// </summary>
    public void OperationFlagON()
    {
        freeFall_Piece = true;
        destructible_Piece = true;
    }

    /// <summary>
    /// ����t���O���I���ɂ���
    /// </summary>
    public void OperationFlagOFF()
    {
        freeFall_Piece = false;
        destructible_Piece = false;
    }
}