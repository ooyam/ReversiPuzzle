using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PuzzleMain;
using static PuzzleDefine;

public class ScreenTap : MonoBehaviour
{
    [Header("PieceManagerの取得")]
    [SerializeField]
    PiecesManager piecesMan;

    Camera mainCamera;   //メインカメラ
    string[] pieceTags;  //駒のタグ
    float rayDistance = 10.0f;

    void Start()
    {
        //メインカメラ取得
        mainCamera = Camera.main;

        //駒のタグ取得
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
                    //Rayが当たったオブジェクトのtagが駒だったら
                    if (Array.IndexOf(pieceTags, hitCollider.tag) >= 0)
                        piecesMan.TapObject(hitCollider.gameObject);
                }
            }
        }
    }

    //タップ可能判定
    bool TappableJudgment()
    {
        if (NOW_PUTTING_PIECES)       return false;  //駒配置中
        if (NOW_REVERSING_PIECES)     return false;  //駒反転中
        if (NOW_DESTROYING_PIECES)    return false;  //駒破壊中
        if (NOW_FALLING_PIECES)       return false;  //駒落下中
        if (NOW_GIMMICK_DAMAGE_WAIT)  return false;  //ギミックダメージ待機中
        if (NOW_GIMMICK_STATE_CHANGE) return false;  //ギミック状態変化中

        return true;
    }
}
