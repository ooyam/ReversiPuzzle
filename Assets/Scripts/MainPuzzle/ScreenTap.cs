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
    Camera mainCamera;              //CJ
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
                    //Rayͺ½Α½IuWFNg
                    switch (hitCollider.tag)
                    {
                        //ξ,M~bN
                        case PIECE_TAG:
                        case GIMMICK_TAG:
                            piecesMgr.TapObject(hitCollider.gameObject);
                            break;

                        //μACe
                        case SUPPORT_ITEM_TAG:
                            stItemsMgr.TapItem(hitCollider.gameObject);
                            break;
                    }
                }
            }
        }
    }

    //^bvΒ\»θ
    bool TappableJudgment()
    {
        if (NOW_PUTTING_PIECES)       return false;  //ξzu
        if (NOW_REVERSING_PIECES)     return false;  //ξ½]
        if (NOW_DESTROYING_PIECES)    return false;  //ξjσ
        if (NOW_FALLING_PIECES)       return false;  //ξΊ
        if (NOW_GIMMICK_DESTROY_WAIT) return false;  //M~bNjσ?@
        if (NOW_GIMMICK_STATE_CHANGE) return false;  //M~bNσΤΟ»
        if (NOW_SUPPORT_ITEM_USE)     return false;  //μACegp
        if (NOW_TURN_END_PROCESSING)  return false;  //^[IΉ

        return true;
    }
}
