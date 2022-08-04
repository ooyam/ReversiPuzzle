using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CommonDefine;

namespace ObjectMove_UI
{
    public class ObjectMove_UI : MonoBehaviour
    {
        /// <summary>
        /// �h��铮��
        /// </summary>
        /// <param name="tra">       ����I�u�W�F�N�g��RectTransform</param>
        /// <param name="moveSpeed"> ���쑬�x</param>
        /// <param name="maxRot">    �h��p�x</param>
        /// <param name="moveCount"> 1�T�C�N�������(�J�E���g���Ȃ��ꍇ��-1�w��)</param>
        /// <param name="stopTime">  ��~����</param>
        /// <param name="breakCount">�I���T�C�N����(�������[�v�̏ꍇ��-1�w��)</param>
        /// <param name="endTime">   �h��I������(���ԂŎ~�߂Ȃ��ꍇ��-1�w��)</param>
        public static IEnumerator ShakeMovement(RectTransform tra, float moveSpeed, float maxRot, int moveCount, float stopTime, int breakCount, float endTime)
        {
            bool leftMove = true;   //���ɉ�]�H
            bool rotReturn = false;  //�p�x�߂����H
            bool shakeStop = false;  //��~�H
            int loopTimes = 0;      //�����
            int cycleTimes = 0;      //�T�C�N����
            float playTime = 0.0f;   //�h�ꓮ��Đ�����
            while (true)
            {
                if (tra == null) yield break;
                float rotZ = tra.localRotation.eulerAngles.z;
                rotZ = (rotZ >= 180.0f) ? rotZ - 360.0f : rotZ;
                if (!rotReturn)
                {
                    if (leftMove)
                    {
                        //---------------------------------------------
                        //���ɉ�]
                        //---------------------------------------------
                        tra.Rotate(0.0f, 0.0f, moveSpeed);
                        if (rotZ > maxRot) leftMove = false;
                    }
                    else
                    {
                        //---------------------------------------------
                        //�E�ɉ�]
                        //---------------------------------------------
                        tra.Rotate(0.0f, 0.0f, -moveSpeed);
                        if (rotZ < -maxRot)
                        {
                            loopTimes++;
                            if (0 < moveCount && moveCount <= loopTimes) rotReturn = true;  //�I���T�C�N�����𒴂������ɗh����~�߂�
                            else leftMove = true;
                        }
                    }
                }
                else
                {
                    //---------------------------------------------
                    //�p�x��0�ɖ߂�
                    //---------------------------------------------
                    tra.Rotate(0.0f, 0.0f, moveSpeed);
                    if (-0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        shakeStop = true;
                    }

                    //---------------------------------------------
                    //�h����~�߂�
                    //---------------------------------------------
                    if (shakeStop)
                    {
                        cycleTimes++;
                        if (0 < breakCount && breakCount <= cycleTimes) break;  //�h��I��
                        else yield return new WaitForSeconds(stopTime);         //�ꎞ��~
                        loopTimes = 0;
                        rotReturn = false;
                        shakeStop = false;
                    }
                }

                //---------------------------------------------
                //���ԂŒ�~����ꍇ�̏���
                //---------------------------------------------
                if (0 < endTime)
                {
                    playTime += ONE_FRAME_TIMES;
                    if (playTime >= endTime && -0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        break;  //�h��I��
                    }
                }
                yield return FIXED_UPDATE;
            }
        }


        /// <summary>
        /// �����ړ�
        /// </summary>
        /// <param name="tra">       ����I�u�W�F�N�g��RectTransform</param>
        /// <param name="moveSpeed"> ���쑬�x</param>
        /// <param name="targetPos"> �ڕW���W</param>
        public static IEnumerator DecelerationMovement(RectTransform tra, float moveSpeed, Vector3 targetPos)
        {
            float offset = 0.1f;              //��~�ꏊ�̃I�t�Z�b�g
            Vector3 nowPos = tra.localPosition; //���݂̍��W
            bool sideways = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X�����ɓ���H
            while (true)
            {
                if (tra == null) yield break;
                tra.localPosition = Vector3.Lerp(tra.localPosition, targetPos, moveSpeed);
                nowPos = tra.localPosition;

                //---------------------------------------------
                //�ړ��I��
                //---------------------------------------------
                if ((sideways && targetPos.x - offset < nowPos.x && nowPos.x < targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset < nowPos.y && nowPos.y < targetPos.y + offset))
                {
                    tra.localPosition = targetPos;
                    break;
                }
                yield return FIXED_UPDATE;
            }
        }

        /// <summary>
        /// �����ړ�
        /// </summary>
        /// <param name="tra">       ����I�u�W�F�N�g��RectTransform</param>
        /// <param name="moveSpeed"> ���쑬�x</param>
        /// <param name="targetPos"> �ڕW���W</param>
        /// <param name="acceleRate">������(�����ړ���0.0f�w��)</param>
        const float DEFAULT_ACCELE_RATE = 0.0f;
        public static IEnumerator ConstantSpeedMovement(RectTransform tra, float moveSpeed, Vector3 targetPos, float acceleRate = DEFAULT_ACCELE_RATE)
        {
            float minSpeed = 0.01f;            //�Œᑬ�x�w��(�������[�v�΍�)
            float offset = 0.05f;             //��~�ꏊ�̃I�t�Z�b�g
            Vector3 nowPos = tra.localPosition; //���݂̍��W
            bool sideways = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X�����ɓ���H
            while (true)
            {
                if (tra == null) yield break;
                moveSpeed += acceleRate;
                if (acceleRate != DEFAULT_ACCELE_RATE)
                {
                    if (0.0f <= moveSpeed && moveSpeed < minSpeed) moveSpeed = minSpeed;
                    else if (-minSpeed < moveSpeed && moveSpeed <= 0.0f) moveSpeed = -minSpeed;
                }
                tra.localPosition = Vector3.MoveTowards(tra.localPosition, targetPos, moveSpeed);
                nowPos = tra.localPosition;

                //---------------------------------------------
                //�ړ��I��
                //---------------------------------------------
                if ((sideways && targetPos.x - offset < nowPos.x && nowPos.x < targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset < nowPos.y && nowPos.y < targetPos.y + offset))
                {
                    tra.localPosition = targetPos;
                    break;
                }
                yield return FIXED_UPDATE;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="tra">       ����I�u�W�F�N�g��RectTransform</param>
        /// <param name="shakeSpeed">���쑬�x</param>
        /// <param name="offsetX">   �ڕW���W�I�t�Z�b�gX</param>
        /// <param name="offsetY">   �ڕW���W�I�t�Z�b�gY</param>
        /// <param name="shakeTimes">�ړ���</param>
        /// <param name="delayTime"> �ړ��Ԃ̑ҋ@����</param>
        public static IEnumerator SlideShakeMovement(RectTransform tra, float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            float offset = 0.5f;                 //��~�ꏊ�̃I�t�Z�b�g
            Vector2 defaultPos = tra.localPosition; //�������W�擾
            bool sideways = Mathf.Abs(offsetX) >= Mathf.Abs(offsetY); //X�����ɓ���H

            //��������
            for (int moveCount = 0; moveCount < shakeTimes; moveCount++)
            {
                int vector = (moveCount % 2 == 0) ? 1 : -1;
                Vector2 tarPos = new Vector2(defaultPos.x + offsetX * vector, defaultPos.y + offsetY * vector);
                while (true)
                {
                    if (tra == null) yield break;
                    tra.localPosition = Vector2.MoveTowards(tra.localPosition, tarPos, shakeSpeed);
                    Vector2 nowPos = tra.localPosition;

                    //---------------------------------------------
                    //���̈ړ���
                    //---------------------------------------------
                    if ((sideways && tarPos.x - offset <= nowPos.x && nowPos.x <= tarPos.x + offset) ||
                        (!sideways && tarPos.y - offset <= nowPos.y && nowPos.y <= tarPos.y + offset))
                    {
                        tra.localPosition = tarPos;
                        yield return new WaitForSeconds(delayTime);
                        break;
                    }
                    yield return FIXED_UPDATE;
                }
            }

            //���̍��W�ɖ߂�
            while (true)
            {
                if (tra == null) yield break;
                tra.localPosition = Vector2.MoveTowards(tra.localPosition, defaultPos, shakeSpeed);
                Vector2 nowPos = tra.localPosition;

                //---------------------------------------------
                //�ړ��I��
                //---------------------------------------------
                if ((sideways && defaultPos.x - offset <= nowPos.x && nowPos.x <= defaultPos.x + offset) ||
                    (!sideways && defaultPos.y - offset <= nowPos.y && nowPos.y <= defaultPos.y + offset))
                {
                    tra.localPosition = defaultPos;
                    break;
                }
                yield return FIXED_UPDATE;
            }
        }

        /// <summary>
        /// ��]����
        /// </summary>
        /// <param name="tra">����I�u�W�F�N�g��RectTransform</param>
        /// <param name="rotSpeed">�g�k���x</param>
        /// <param name="stopRot"> ��]��̊p�x(��Ίp)</param>
        public static IEnumerator RotateMovement(RectTransform tra, Vector3 rotSpeed, Vector3 stopRot)
        {
            //�ł��������삷�鎲����
            int axis = 0;
            if (rotSpeed.x < rotSpeed.y)
                axis = (rotSpeed.y > rotSpeed.z) ? 1 : 2;
            else if (rotSpeed.x < rotSpeed.z)
                axis = 2;

            //��]
            float tolerance = 10.0f;
            while (true)
            {
                if (tra == null) yield break;
                tra.Rotate(rotSpeed.x, rotSpeed.y, rotSpeed.z);
                Vector3 nowRot = tra.localEulerAngles;
                float refRot = nowRot.x;
                float refStopRot = stopRot.x;
                switch (axis)
                {
                    case 1:
                        refRot = nowRot.y;
                        refStopRot = stopRot.y;
                        break;
                    case 2:
                        refRot = nowRot.z;
                        refStopRot = stopRot.z;
                        break;
                }
                if (refStopRot - tolerance <= refRot && refRot <= refStopRot + tolerance) break;
                yield return FIXED_UPDATE;
            }

            //�ŏI�p�x�ɍ��킹��
            tra.localRotation = Quaternion.Euler(stopRot.x, stopRot.y, stopRot.z);
        }

        /// <summary>
        /// �S���̊g��k������
        /// </summary>
        /// <param name="tra">         ����I�u�W�F�N�g��RectTransform</param>
        /// <param name="scalingSpeed">�g�k���x(���ׂĐ��̐��Ŏw��)</param>
        /// <param name="changedScale"> �ύX��̊g�嗦</param>
        /// <returns></returns>
        public static IEnumerator AllScaleChange(RectTransform tra, float scalingSpeed, float changedScale)
        {
            Vector3 nowScale = tra.localScale;         //���݂̃X�P�[��
            bool scaleUp = nowScale.x < changedScale;  //�g��H

            //�k���̏ꍇ�͑��x�𔽓]
            if (!scaleUp) scalingSpeed *= -1;

            while (true)
            {
                if (tra == null) yield break;

                //�g�k���X�V
                float x = nowScale.x + scalingSpeed;
                float y = nowScale.y + scalingSpeed;
                tra.localScale = new Vector3(x, y, nowScale.z);

                //�I������
                if ((scaleUp && (x > changedScale || y > changedScale)) ||
                    (!scaleUp && (x < changedScale || y < changedScale)))
                    break;

                //���݂̃X�P�[���X�V
                nowScale = tra.localScale;
                yield return FIXED_UPDATE;
            }
        }

        /// <summary>
        /// �F�ύX����(Image.color)
        /// </summary>
        /// <param name="img">        �ύX�Ώ�Image</param>
        /// <param name="changeSpeed">�ύX���x</param>
        /// <param name="colArray">   �ύX�F�̔z��(0:���݂̐F)</param>
        /// <param name="chengeCount">���[�v��(�z��1����1�J�E���g�A-1�w��Ŗ����Đ�)</param>
        /// <returns></returns>
        public static IEnumerator ImagePaletteChange(Image img, float changeSpeed, Color32[] colArray, int chengeCount = 1)
        {
            int loopTimes = 0;                  //�J��Ԃ���
            int colCount = colArray.Length;     //�ύX�F�̐�
            bool infinite = chengeCount < 0;    //�������[�v�H

            int nowIndex = 0;                   //���݂̐F
            int nextIndex = 1;                  //���̐F
            int judgeRange = 5;                 //����͈�

            img.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                if (img == null) yield break;

                //�F�ύX�J�n
                img.color = Color.Lerp(img.color, colArray[nextIndex], changeSpeed);

                //�ύX�I��
                Color32 nowColor = img.color;
                if (nowColor.r + judgeRange >= colArray[nextIndex].r && colArray[nextIndex].r >= nowColor.r - judgeRange && //R
                    nowColor.g + judgeRange >= colArray[nextIndex].g && colArray[nextIndex].g >= nowColor.g - judgeRange && //G
                    nowColor.b + judgeRange >= colArray[nextIndex].b && colArray[nextIndex].b >= nowColor.b - judgeRange && //B
                    nowColor.a + judgeRange >= colArray[nextIndex].a && colArray[nextIndex].a >= nowColor.a - judgeRange)   //A
                {
                    loopTimes++;
                    nowIndex = nextIndex;
                    nextIndex = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                }
                yield return new WaitForSecondsRealtime(ONE_FRAME_TIMES);
            }
            img.color = colArray[nowIndex];
        }
    }
}