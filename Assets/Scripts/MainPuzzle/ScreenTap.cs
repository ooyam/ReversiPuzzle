using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PuzzleMain;
using static PuzzleDefine;

public class ScreenTap : MonoBehaviour
{
    [Header("PieceManager�̎擾")]
    [SerializeField]
    PiecesManager piecesMan;

    Camera mainCamera;   //���C���J����
    string[] pieceTags;  //��̃^�O
    float rayDistance = 10.0f;

    void Start()
    {
        //���C���J�����擾
        mainCamera = Camera.main;

        //��̃^�O�擾
        System.Array pieceColors = Enum.GetValues(typeof(Colors));
        pieceTags = new string[pieceColors.Length];
        foreach (Colors value in pieceColors)
        { pieceTags[(int)value] = Enum.GetName(typeof(Colors), value); }
    }

    void Update()
    {
        if (TappableJudgment())
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Ray ray = new Ray(mousePos, Vector3.forward);
                RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, rayDistance);
                var hitCollider = hit.collider;
                if (hitCollider)
                {
                    //Ray�����������I�u�W�F�N�g��tag���������
                    if (Array.IndexOf(pieceTags, hitCollider.tag) >= 0)
                        piecesMan.TapObject(hitCollider.gameObject);
                }
            }
        }
    }

    //�^�b�v�\����
    bool TappableJudgment()
    {
        if (NOW_PUTTING_PIECES)       return false;  //��z�u��
        if (NOW_REVERSING_PIECES)     return false;  //��]��
        if (NOW_DESTROYING_PIECES)    return false;  //��j��
        if (NOW_FALLING_PIECES)       return false;  //�����
        if (NOW_GIMMICK_DAMAGE_WAIT)  return false;  //�M�~�b�N�_���[�W�ҋ@��
        if (NOW_GIMMICK_STATE_CHANGE) return false;  //�M�~�b�N��ԕω���

        return true;
    }
}
