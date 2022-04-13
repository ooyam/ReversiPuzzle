using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;

public class ScreenTap : MonoBehaviour
{
    [Header("PieceManagerの取得")]
    [SerializeField]
    PieceManager pieceMan;

    Camera mainCamera;   //メインカメラ
    string[] pieceTags;  //駒のタグ
    float rayDistance = 10.0f;

    void Start()
    {
        //メインカメラ取得
        mainCamera = Camera.main;

        //駒のタグ取得
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
                    //Rayが当たったオブジェクトのtagがPlayerだったら
                    string cllidTag = hitCollider.tag;
                    if (Array.IndexOf(pieceTags, cllidTag) >= 0)
                        StartCoroutine(pieceMan.PutPieceToSquare(hitCollider.gameObject));
                }
            }
        }
    }

    //タップ可能判定
    bool TappableJudgment()
    {
        if (NOW_PUTTING_PIECES)    return false;  //駒配置中
        if (NOW_REVERSING_PIECES)  return false;  //駒反転中
        if (NOW_DESTROYING_PIECES) return false;  //駒破壊中
        if (NOW_FALLING_PIECES)    return false;  //駒落下中

        return true;
    }
}
