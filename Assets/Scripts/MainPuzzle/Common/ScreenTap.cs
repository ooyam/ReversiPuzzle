using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

public class ScreenTap : MonoBehaviour
{
    Camera mainCamera;  //メインカメラ
    const float rayDistance = 10.0f;

    public void Initialize()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (IsOperable())
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
                            PiecesMgr.TapObject(hitCollider.gameObject);
                            break;

                        //援護アイテム
                        case SUPPORT_ITEM_TAG:
                            SupportItemsMgr.TapItem(hitCollider.gameObject);
                            break;
                    }
                }
            }
        }
    }
}
