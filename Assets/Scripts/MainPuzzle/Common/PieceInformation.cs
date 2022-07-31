using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class PieceInformation : MonoBehaviour
{
    //�R���|�[�l���g
    [System.NonSerialized] public Transform tra;

    //����
    [Header("��̐F�I��")]
    public Colors color;                            //�F
    public bool colorRless;                         //�F�Ȃ��t���O
    [System.NonSerialized] public int  colorId;     //�F�ԍ�
    [System.NonSerialized] public int  squareId;    //�}�X�Ǘ��ԍ�
    [System.NonSerialized] public bool freeFall;    //���R�����t���O
    [System.NonSerialized] public bool invertable;  //���]�\�t���O

    /// <summary>
    /// ����̐ݒ�
    /// </summary>
    /// <param name="_squareIndex">     �z�u�}�X�Ǘ��ԍ�</param>
    /// <param name="_generate">        �����H</param>
    /// <param name="gimmickInfoArr">   �M�~�b�N���z��</param>
    public void InformationSetting(int _squareIndex, bool _generate, GimmickInformation[] gimmickInfoArr = null)
    {
        tra        = this.transform;
        colorId    = (colorRless) ? COLORLESS_ID : (int)color;
        squareId   = _squareIndex;
        OperationFlagON();
        if (_generate)
        {
            foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
            {
                if (gimmickInfo == null) continue;
                if (gimmickInfo.innerSquaresId == null) continue;
                foreach (int innerSquareId in gimmickInfo.innerSquaresId)
                {
                    //�M�~�b�N�̓����ɐ������ꂽ�ꍇ
                    if (innerSquareId == _squareIndex)
                    {
                        OperationFlagOFF();
                    }
                }
            }
        }
    }

    /// <summary>
    /// ����t���O���I���ɂ���
    /// </summary>
    public void OperationFlagON()
    {
        freeFall   = true;
        invertable = true;
    }

    /// <summary>
    /// ����t���O���I���ɂ���
    /// </summary>
    public void OperationFlagOFF()
    {
        freeFall   = false;
        invertable = false;
    }
}
