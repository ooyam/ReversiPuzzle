using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

public class ScreenTap : MonoBehaviour
{
    const float RAY_DISTANCE = 10.0f;
    Camera mMainCamera;  //���C���J����

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
    Touch mTouch;
#endif

    /// <summary>
    /// ������
    /// </summary>
    public void Initialize()
    {
        mMainCamera = Camera.main;
    }

    /// <summary>
    /// �X�V
    /// </summary>
    void Update()
    {
        //����\
        if (IsOperable())
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            //��ʂɐG��Ă���
            if (Input.touchCount > 0)
            {
                //�^�b�v���擾
                mTouch = Input.GetTouch(0);

                //�^�b�v�𗣂���
                if (mTouch.phase == TouchPhase.Ended) FlyRay();
            }
#else
            //�}�E�X�𗣂���
            if (Input.GetMouseButtonUp(0)) FlyRay();
#endif
        }
    }

    /// <summary>
    /// ���C���΂�
    /// </summary>
    void FlyRay()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        Vector3 touchPos = mMainCamera.ScreenToWorldPoint(mTouch.position);
#else
        Vector3 touchPos = mMainCamera.ScreenToWorldPoint(Input.mousePosition);
#endif
        Ray ray = new Ray(touchPos, Vector3.forward);
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
