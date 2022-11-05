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

    [Header("GlobalCanvas")]
    [SerializeField]
    Canvas mGlobalCanvas;
    RectTransform mGlobalCanvasTra;
    Camera mGlobalCamera;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
    Touch mTouch;
#endif

    readonly WaitForSeconds DESTROY_TIME = new WaitForSeconds(2f);
    const float POS_Z = -5f;

    /// <summary>
    /// ������
    /// </summary>
    public void Initialize()
    {
        mGlobalCanvasTra = mGlobalCanvas.GetComponent<RectTransform>();
        mGlobalCamera = mGlobalCanvas.worldCamera;
    }

    /// <summary>
    /// �X�V
    /// </summary>
    void Update()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        //��ʂɐG��Ă���
        if (Input.touchCount > 0)
        {
            //�^�b�v���擾
            mTouch = Input.GetTouch(0);

            //�^�b�v�𗣂���
            if (mTouch.phase == TouchPhase.Ended) StartCoroutine(EffectGenerate());
        }
#else
        //�}�E�X�𗣂���
        if (Input.GetMouseButtonUp(0)) StartCoroutine(EffectGenerate());
#endif
    }

    /// <summary>
    /// �G�t�F�N�g����
    /// </summary>
    IEnumerator EffectGenerate()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mGlobalCanvasTra, mTouch.position, mGlobalCamera, out Vector2 localpoint);
#else
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mGlobalCanvasTra, Input.mousePosition, mGlobalCamera, out Vector2 localpoint);
#endif
        GameObject effect = Instantiate(mEffect);
        RectTransform effectTra = effect.GetComponent<RectTransform>();
        effectTra.SetParent(mEffectBox);
        effectTra.localPosition = new Vector3(localpoint.x, localpoint.y, POS_Z);

        //Effect�I�u�W�F�N�g�j��
        yield return DESTROY_TIME;
        Destroy(effect);
    }
}
