using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

public class ScreenTap : MonoBehaviour
{
    const float RAY_DISTANCE = 10.0f;
    Camera mMainCamera;  //���C���J����

    //�X�}�z���ƂȂ���Input.GetMouseButtonUp�������ɂ����ב�p
    bool mIsPush = false;
    bool mIsRelease = false;

    public void Initialize()
    {
        mMainCamera = Camera.main;
    }

    void Update()
    {
        if (mIsPush)
        {
            if (!Input.GetMouseButton(0))
            {
                mIsPush = false;
                mIsRelease = true;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            mIsPush = true;
        }

        //�w�𗣂���
        if (mIsRelease)
        {
            //����\
            if (IsOperable())
            {
                FlyRay();
            }
            mIsRelease = false;
        }

    }

    /// <summary>
    /// ���C���΂�
    /// </summary>
    void FlyRay()
    {
        Vector3 mousePos = mMainCamera.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = new Ray(mousePos, Vector3.forward);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, RAY_DISTANCE);
        var hitCollider = hit.collider;
        if (hitCollider)
        {
            //Ray�����������I�u�W�F�N�g
            switch (hitCollider.tag)
            {
                //��,�M�~�b�N
                case PIECE_TAG:
                case GIMMICK_TAG:
                    PiecesMgr.TapObject(hitCollider.gameObject);
                    break;

                //����A�C�e��
                case SUPPORT_ITEM_TAG:
                    SupportItemsMgr.TapItem(hitCollider.gameObject);
                    break;
            }
        }
    }
}
