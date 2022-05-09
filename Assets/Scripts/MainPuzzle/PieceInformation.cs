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
        colorId    = (int)color;
        squareId   = _squareIndex;
        freeFall   = true;
        invertable = true;
        if (_generate)
        {
            foreach (GimmickInformation gimmickInfo in gimmickInfoArr)
            {
                foreach (int innerSquareId in gimmickInfo.innerSquaresId)
                {
                    //�M�~�b�N�̓����ɐ������ꂽ�ꍇ
                    if (innerSquareId == _squareIndex)
                    {
                        freeFall   = false;
                        invertable = false;
                    }
                }
            }
        }
    }
}
