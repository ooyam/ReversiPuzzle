using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;

public class ScreenTap : MonoBehaviour
{
    [Header("PieceManager�̎擾")]
    [SerializeField]
    PieceManager pieceMan;

    Camera mainCamera;   //���C���J����
    string[] pieceTags;  //��̃^�O
    float rayDistance = 10.0f;

    void Start()
    {
        //���C���J�����擾
        mainCamera = Camera.main;

        //��̃^�O�擾
        System.Array pieceColors = Enum.GetValues(typeof(PieceColors));
        pieceTags = new string[pieceColors.Length];
        foreach (PieceColors value in pieceColors)
        { pieceTags[(int)value] = Enum.GetName(typeof(PieceColors), value); }
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
                    //Ray�����������I�u�W�F�N�g��tag��Player��������
                    string cllidTag = hitCollider.tag;
                    if (Array.IndexOf(pieceTags, cllidTag) >= 0)
                        StartCoroutine(pieceMan.PutPieceToSquare(hitCollider.gameObject));
                }
            }
        }
    }

    //�^�b�v�\����
    bool TappableJudgment()
    {
        if (NOW_PUTTING_PIECES)    return false;  //��z�u��
        if (NOW_REVERSING_PIECES)  return false;  //��]��
        if (NOW_DESTROYING_PIECES) return false;  //��j��
        if (NOW_FALLING_PIECES)    return false;  //�����

        return true;
    }
}
