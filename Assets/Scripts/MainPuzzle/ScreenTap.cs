using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PuzzleMain;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

public class ScreenTap : MonoBehaviour
{
    PiecesManager piecesMgr;        //PiecesManager
    SupportItemsManager stItemsMgr; //SupportItemsManager
    Camera mainCamera;              //メインカメラ
    const float rayDistance = 10.0f;

    public void Initialize()
    {
        mainCamera = Camera.main;
        piecesMgr  = sPuzzleMain.GetPiecesManager();
        stItemsMgr = sPuzzleMain.GetSupportItemsManager();
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
                    //Rayが当たったオブジェクト
                    switch (hitCollider.tag)
                    {
                        //駒,ギミック
                        case PIECE_TAG:
                        case GIMMICK_TAG:
                            piecesMgr.TapObject(hitCollider.gameObject);
                            break;

                        //援護アイテム
                        case SUPPORT_ITEM_TAG:
                            stItemsMgr.TapItem(hitCollider.gameObject);
                            break;
                    }
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
        if (NOW_GIMMICK_DESTROY_WAIT) return false;  //ギミック破壊待機中
        if (NOW_GIMMICK_STATE_CHANGE) return false;  //ギミック状態変化中
        if (NOW_SUPPORT_ITEM_USE)     return false;  //援護アイテム使用中
        if (NOW_TURN_END_PROCESSING)  return false;  //ターン終了処理中

        return true;
    }
}
