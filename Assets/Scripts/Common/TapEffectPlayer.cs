using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectPlayer : MonoBehaviour
{
    [Header("�^�b�v�G�t�F�N�g")]
    [SerializeField]
    GameObject mEffect;

    [Header("�G�t�F�N�g�i�[��")]
    [SerializeField]
    RectTransform mEffectBox;

    Canvas mCanvas;
    RectTransform mCanvasTra;
    Camera mCamera;

    readonly WaitForSeconds DESTROY_TIME = new WaitForSeconds(2f);
    const float POS_Z = -5f;

    //�X�}�z���ƂȂ���Input.GetMouseButtonUp�������ɂ����ב�p
    bool mIsPush = false;
    bool mIsRelease = false;

    void Start()
    {
        mCanvas = GetComponent<Canvas>();
        mCanvasTra = mCanvas.GetComponent<RectTransform>();
        mCamera = mCanvas.worldCamera;
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
            mIsRelease = false;
            StartCoroutine(EffectGenerate());
        }
    }

    /// <summary>
    /// �G�t�F�N�g�j��
    /// </summary>
    IEnumerator EffectGenerate()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mCanvasTra, Input.mousePosition, mCamera, out Vector2 localpoint);
        GameObject effect = Instantiate(mEffect);
        RectTransform effectTra = effect.GetComponent<RectTransform>();
        effectTra.SetParent(mEffectBox);
        effectTra.localPosition = new Vector3(localpoint.x, localpoint.y, POS_Z);

        //Effect�I�u�W�F�N�g�j��
        yield return DESTROY_TIME;
        Destroy(effect);
    }
}
