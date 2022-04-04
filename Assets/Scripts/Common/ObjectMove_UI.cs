using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ObjectMove_UI
{
    public class ObjectMove_UI : MonoBehaviour
    {
        //========================================================================
        //�萔
        //========================================================================
        public static bool SCALING_INFINITE_END       = false;  //�g�k�����Đ��I���t���O
        public static bool COLOR_CHANGE_INFINITE_END  = false;  //2�F�_�Ŗ����Đ��I���t���O

        //========================================================================
        //�h��铮��
        //========================================================================
        //tra;        ����I�u�W�F�N�g��RectTransform
        //moveSpeed;  ���쑬�x
        //maxRot;     �h��p�x
        //moveCount;  1�T�C�N�������(�J�E���g���Ȃ��ꍇ��-1�w��)
        //stopTime;   ��~����
        //breakCount; �I���T�C�N����(�������[�v�̏ꍇ��-1�w��)
        //endTime;    �h��I������(���ԂŎ~�߂Ȃ��ꍇ��-1�w��)
        //========================================================================
        public static IEnumerator ShakeMovement(RectTransform tra, float moveSpeed, float maxRot, int moveCount, float stopTime, int breakCount, float endTime)
        {
            bool leftMove      = true;   //���ɉ�]�H
            bool rotReturn     = false;  //�p�x�߂����H
            bool shakeStop     = false;  //��~�H
            int loopTimes      = 0;      //�����
            int cycleTimes     = 0;      //�T�C�N����
            float playTime     = 0.0f;   //�h�ꓮ��Đ�����
            float oneFrameTime = 0.02f;  //1�t���[���̎���
            while (true)
            {
                yield return new WaitForFixedUpdate();
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
                    playTime += oneFrameTime;
                    if (playTime >= endTime && -0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        break;  //�h��I��
                    }
                }
            }
        }


        //========================================================================
        //�ړ�����
        //========================================================================
        //tra;        ����I�u�W�F�N�g��RectTransform
        //moveSpeed;  ���쑬�x
        //acceleRate; ������(�����ړ���1.0f�w��)
        //targetPos;  �ڕW���W
        //========================================================================
        public static IEnumerator MoveMovement(RectTransform tra, float moveSpeed, float acceleRate, Vector2 targetPos)
        {
            float minSpeed = 1.0f;                 //�Œᑬ�x�w��(�������[�v�΍�)
            float offset   = 0.5f;                 //��~�ꏊ�̃I�t�Z�b�g
            Vector2 nowPos = tra.anchoredPosition; //���݂̍��W
            bool sideways  = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X�����ɓ���H
            while (true)
            {
                yield return new WaitForFixedUpdate();
                moveSpeed *= acceleRate;
                if (0.0f <= moveSpeed && moveSpeed < minSpeed) moveSpeed = minSpeed;
                else if (-minSpeed < moveSpeed && moveSpeed <= 0.0f) moveSpeed = -minSpeed;
                tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, targetPos, moveSpeed);
                nowPos = tra.anchoredPosition;

                //---------------------------------------------
                //�ړ��I��
                //---------------------------------------------
                if ((sideways && targetPos.x - offset <= nowPos.x && nowPos.x <= targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset <= nowPos.y && nowPos.y <= targetPos.y + offset))
                {
                    tra.anchoredPosition = targetPos;
                    break;
                }
            }
        }


        //========================================================================
        //�ړ�����(MoveMovement)�ɗv���鎞�Ԍv�Z
        //========================================================================
        //tra;        ����I�u�W�F�N�g��RectTransform
        //moveSpeed;  ���쑬�x
        //acceleRate; ������(�����ړ���1.0f�w��)
        //targetPos;  �ڕW���W
        //========================================================================
        public static float GetMoveTime(RectTransform tra, float moveSpeed, float acceleRate, Vector2 targetPos)
        {
            //�N���[���쐻
            RectTransform cloneTra = CloneCreate(tra);

            //���Ԍv�Z�p
            float oneFrameTime = 0.02f;
            float moveTime     = 0.0f;

            float minSpeed = 1.0f;                      //�Œᑬ�x�w��(�������[�v�΍�)
            float offset   = 0.5f;                      //��~�ꏊ�̃I�t�Z�b�g
            Vector2 nowPos = cloneTra.anchoredPosition; //���݂̍��W
            bool sideways  = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X�����ɓ���H
            while (true)
            {
                moveTime  += oneFrameTime;
                moveSpeed *= acceleRate;
                if (0.0f <= moveSpeed && moveSpeed < minSpeed) moveSpeed = minSpeed;
                else if (-minSpeed < moveSpeed && moveSpeed <= 0.0f) moveSpeed = -minSpeed;
                cloneTra.anchoredPosition = Vector2.MoveTowards(cloneTra.anchoredPosition, targetPos, moveSpeed);
                nowPos = cloneTra.anchoredPosition;

                //---------------------------------------------
                //�ړ��I��
                //---------------------------------------------
                if ((sideways && targetPos.x - offset <= nowPos.x && nowPos.x <= targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset <= nowPos.y && nowPos.y <= targetPos.y + offset))
                {
                    cloneTra.anchoredPosition = targetPos;
                    break;
                }
            }

            Destroy(cloneTra.gameObject);  //�N���[���폜
            return moveTime;               //���Ԃ�Ԃ�
        }


        //========================================================================
        //��������
        //========================================================================
        //tra;        ����I�u�W�F�N�g��RectTransform
        //shakeSpeed; ���쑬�x
        //offsetX;    �ڕW���W�I�t�Z�b�gX
        //offsetY;    �ڕW���W�I�t�Z�b�gY
        //shakeTimes; �ړ���
        //delayTime;  �ړ��Ԃ̑ҋ@����
        //========================================================================
        public static IEnumerator SlideShakeMovement(RectTransform tra, float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            float offset       = 0.5f;                 //��~�ꏊ�̃I�t�Z�b�g
            Vector2 defaultPos = tra.anchoredPosition; //�������W�擾
            bool sideways      = Mathf.Abs(offsetX) >= Mathf.Abs(offsetY); //X�����ɓ���H

            //��������
            for (int moveCount = 0; moveCount < shakeTimes; moveCount++)
            {
                int vector = (moveCount % 2 == 0) ? 1 : -1;
                Vector2 tarPos = new Vector2(defaultPos.x + offsetX * vector, defaultPos.y + offsetY * vector);
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, tarPos, shakeSpeed);
                    Vector2 nowPos = tra.anchoredPosition;

                    //---------------------------------------------
                    //���̈ړ���
                    //---------------------------------------------
                    if ((sideways && tarPos.x - offset <= nowPos.x && nowPos.x <= tarPos.x + offset) ||
                        (!sideways && tarPos.y - offset <= nowPos.y && nowPos.y <= tarPos.y + offset))
                    {
                        tra.anchoredPosition = tarPos;
                        yield return new WaitForSeconds(delayTime);
                        break;
                    }
                }
            }

            //���̍��W�ɖ߂�
            while (true)
            {
                yield return new WaitForFixedUpdate();
                tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, defaultPos, shakeSpeed);
                Vector2 nowPos = tra.anchoredPosition;

                //---------------------------------------------
                //�ړ��I��
                //---------------------------------------------
                if ((sideways && defaultPos.x - offset <= nowPos.x && nowPos.x <= defaultPos.x + offset) ||
                    (!sideways && defaultPos.y - offset <= nowPos.y && nowPos.y <= defaultPos.y + offset))
                {
                    tra.anchoredPosition = defaultPos;
                    break;
                }
            }
        }


        //========================================================================
        //��������(SlideShakeMovement)�ɗv���鎞�Ԍv�Z
        //========================================================================
        //tra;        ����I�u�W�F�N�g��RectTransform
        //shakeSpeed; ���쑬�x
        //offsetX;    �ڕW���W�I�t�Z�b�gX
        //offsetY;    �ڕW���W�I�t�Z�b�gY
        //shakeTimes; �ړ���
        //delayTime;  �ړ��Ԃ̑ҋ@����
        //========================================================================
        public static float GetSlideShakeTime(RectTransform tra, float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            //�N���[���쐻
            RectTransform cloneTra = CloneCreate(tra);

            //���Ԍv�Z�p
            float oneFrameTime = 0.02f;
            float moveTime     = 0.0f;

            float offset = 0.5f;                 //��~�ꏊ�̃I�t�Z�b�g
            Vector2 defaultPos = cloneTra.anchoredPosition; //�������W�擾
            bool sideways = Mathf.Abs(offsetX) >= Mathf.Abs(offsetY); //X�����ɓ���H

            //��������
            for (int moveCount = 0; moveCount < shakeTimes; moveCount++)
            {
                int vector = (moveCount % 2 == 0) ? 1 : -1;
                Vector2 tarPos = new Vector2(defaultPos.x + offsetX * vector, defaultPos.y + offsetY * vector);
                while (true)
                {
                    moveTime += oneFrameTime;
                    cloneTra.anchoredPosition = Vector2.MoveTowards(cloneTra.anchoredPosition, tarPos, shakeSpeed);
                    Vector2 nowPos = cloneTra.anchoredPosition;

                    //---------------------------------------------
                    //���̈ړ���
                    //---------------------------------------------
                    if ((sideways && tarPos.x - offset <= nowPos.x && nowPos.x <= tarPos.x + offset) ||
                        (!sideways && tarPos.y - offset <= nowPos.y && nowPos.y <= tarPos.y + offset))
                    {
                        cloneTra.anchoredPosition = tarPos;
                        moveTime += delayTime;
                        break;
                    }
                }
            }

            //���̍��W�ɖ߂�
            while (true)
            {
                moveTime += oneFrameTime;
                cloneTra.anchoredPosition = Vector2.MoveTowards(cloneTra.anchoredPosition, defaultPos, shakeSpeed);
                Vector2 nowPos = cloneTra.anchoredPosition;

                //---------------------------------------------
                //�ړ��I��
                //---------------------------------------------
                if ((sideways && defaultPos.x - offset <= nowPos.x && nowPos.x <= defaultPos.x + offset) ||
                    (!sideways && defaultPos.y - offset <= nowPos.y && nowPos.y <= defaultPos.y + offset))
                {
                    cloneTra.anchoredPosition = defaultPos;
                    break;
                }
            }

            Destroy(cloneTra.gameObject);  //�N���[���폜
            return moveTime;               //���Ԃ�Ԃ�
        }


        //========================================================================
        //��]����
        //========================================================================
        //traArray;    ����I�u�W�F�N�g��RectTransform[]
        //rotSpeed;    �g�k���x
        //stopRot;     ��]��̊p�x(��Ίp)
        //========================================================================
        public static IEnumerator RotateMovement(RectTransform[] traArray, Vector3 rotSpeed, Vector3 stopRot)
        {
            //�ł��������삷�鎲����
            int axis = 0;
            if (rotSpeed.x < rotSpeed.y)
                axis = (rotSpeed.y > rotSpeed.z) ? 1 : 2;
            else if (rotSpeed.x < rotSpeed.z)
                axis = 2;

            //��]
            float tolerance = 5.0f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                foreach (RectTransform tra in traArray)
                { tra.Rotate(rotSpeed.x, rotSpeed.y, rotSpeed.z); }
                Vector3 nowRot   = traArray[0].localEulerAngles;
                float refRot     = nowRot.x;
                float refStopRot = stopRot.x;
                switch (axis)
                {
                    case 1:
                        refRot     = nowRot.y;
                        refStopRot = stopRot.y;
                        break;
                    case 2:
                        refRot     = nowRot.z;
                        refStopRot = stopRot.z;
                        break;
                }
                if (refStopRot - tolerance <= refRot && refRot <= refStopRot + tolerance) break;
            }

            //�ŏI�p�x�ɍ��킹��
            foreach (RectTransform tra in traArray)
            { tra.localRotation = Quaternion.Euler(stopRot.x, stopRot.y, stopRot.z); }
        }


        //========================================================================
        //��]����(RotateMovement)�ɗv���鎞�Ԍv�Z
        //========================================================================
        //tra;         ����I�u�W�F�N�g��RectTransform
        //rotSpeed;    �g�k���x
        //stopRot;     ��]��̊p�x(��Ίp)
        //========================================================================
        public static float GetRotateMoveTime(RectTransform tra, Vector3 rotSpeed, Vector3 stopRot)
        {
            //�N���[���쐻
            RectTransform cloneTra = CloneCreate(tra);

            //���Ԍv�Z�p
            float oneFrameTime = 0.02f;
            float moveTime     = 0.0f;

            //�ł��������삷�鎲����
            int axis = 0;
            if (rotSpeed.x < rotSpeed.y)
                axis = (rotSpeed.y > rotSpeed.z) ? 1 : 2;
            else if (rotSpeed.x < rotSpeed.z)
                axis = 2;

            //��]
            float tolerance    = 5.0f;
            while (true)
            {
                moveTime += oneFrameTime;
                cloneTra.Rotate(rotSpeed.x, rotSpeed.y, rotSpeed.z);
                Vector3 nowRot = cloneTra.localEulerAngles;
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
            }

            Destroy(cloneTra.gameObject);  //�N���[���폜
            return moveTime;               //���Ԃ�Ԃ�
        }


        //========================================================================
        //�g��k������
        //========================================================================
        //tra;          ����I�u�W�F�N�g��RectTransform
        //scalingSpeed; �g�k���x
        //changeScale;  �ύX���̊g�嗦
        //endScale;     �I�����̊g�嗦
        //scalingTimes; �g�k��(�z��1����1�J�E���g�A-1�w��Ŗ����Đ�)
        //========================================================================
        public static IEnumerator ScaleChange(RectTransform tra, Vector3 scalingSpeed, float changeScale, float endScale, int scalingTimes)
        {
            Vector3 nowScale = tra.localScale;          //���݂̊g�嗦
            float judgeAxis  = 0.0f;                    //���莲
            bool scaleChange = true;                    //�ύX���쒆�H

            //�ł������g�k���鎲����
            int axis    = 0;
            float mathX = Mathf.Abs(scalingSpeed.x);
            float mathY = Mathf.Abs(scalingSpeed.y);
            float mathZ = Mathf.Abs(scalingSpeed.z);
            if (mathX < mathY)
                axis = (mathY > mathZ) ? 1 : 2;
            else if (mathX < mathZ)
                axis = 2;

            //�g�唻��
            switch (axis)
            {
                case 0: judgeAxis = nowScale.x; break;
                case 1: judgeAxis = nowScale.y; break;
                case 2: judgeAxis = nowScale.z; break;
            }
            bool scaleUp = changeScale > judgeAxis;

            bool infinite = scalingTimes < 0;
            int loopTimes = 0;
            while (infinite || loopTimes < scalingTimes)
            {
                while (true)
                {
                    yield return new WaitForFixedUpdate();

                    //�g�k���X�V
                    tra.localScale = nowScale;

                    //�������w��
                    switch (axis)
                    {
                        case 0: judgeAxis = nowScale.x; break;
                        case 1: judgeAxis = nowScale.y; break;
                        case 2: judgeAxis = nowScale.z; break;
                    }

                    if (scaleChange)
                    {
                        //�ύX����
                        if ((scaleUp && judgeAxis >= changeScale) || (!scaleUp && judgeAxis <= changeScale)) scaleChange = false;
                        nowScale = new Vector3(nowScale.x + scalingSpeed.x, nowScale.y + scalingSpeed.y, nowScale.z + scalingSpeed.z);
                    }
                    else
                    {
                        //�I������
                        if ((scaleUp && judgeAxis <= endScale) || (!scaleUp && judgeAxis >= endScale)) break;
                        nowScale = new Vector3(nowScale.x - scalingSpeed.x, nowScale.y - scalingSpeed.y, nowScale.z - scalingSpeed.z);
                    }
                }

                //�ϐ����Z�b�g
                tra.localScale = Vector3.one * endScale;
                scaleChange = true;

                //�����Đ��I������
                if (infinite && SCALING_INFINITE_END)
                {
                    SCALING_INFINITE_END = false;
                    break;
                }
                loopTimes++;
            }
        }


        //========================================================================
        //�g��k������(ScaleChange)�ɗv���鎞�Ԍv�Z
        //========================================================================
        //tra;          ����I�u�W�F�N�g��RectTransform
        //scalingSpeed; �g�k���x
        //changeScale;  �ύX���̊g�嗦
        //endScale;     �I�����̊g�嗦
        //scalingTimes; �g�k��(��0�����̏ꍇ�͖߂�l0.0f)
        //========================================================================
        public static float GetScaleChangeTime(RectTransform tra, Vector3 scalingSpeed, float changeScale, float endScale, int scalingTimes)
        {
            //�N���[���쐻
            RectTransform cloneTra = CloneCreate(tra);

            //���Ԍv�Z�p
            float oneFrameTime = 0.02f;
            float moveTime     = 0.0f;

            Vector3 nowScale = cloneTra.localScale;     //���݂̊g�嗦
            float judgeAxis  = 0.0f;                    //���莲
            bool scaleChange = true;                    //�ύX���쒆�H
            bool scaleUp     = changeScale > endScale;  //�g��H

            //�ł������g�k���鎲����
            int axis = 0;
            if (scaleUp && (scalingSpeed.x < scalingSpeed.y) || !scaleUp && (scalingSpeed.x > scalingSpeed.y))
                axis = (scaleUp && (scalingSpeed.y > scalingSpeed.z) || !scaleUp && (scalingSpeed.y < scalingSpeed.z)) ? 1 : 2;
            else if (scaleUp && (scalingSpeed.x < scalingSpeed.z) || !scaleUp && (scalingSpeed.x > scalingSpeed.z))
                axis = 2;

            for (int loopTimes = 0; loopTimes < scalingTimes; loopTimes++)
            {
                while (true)
                {
                    moveTime += oneFrameTime;

                    //�g�k���X�V
                    cloneTra.localScale = nowScale;

                    //�������w��
                    switch (axis)
                    {
                        case 0: judgeAxis = nowScale.x; break;
                        case 1: judgeAxis = nowScale.y; break;
                        case 2: judgeAxis = nowScale.z; break;
                    }

                    if (scaleChange)
                    {
                        //�ύX����
                        if ((scaleUp && judgeAxis >= changeScale) || (!scaleUp && judgeAxis <= changeScale)) scaleChange = false;
                        nowScale = new Vector3(nowScale.x + scalingSpeed.x, nowScale.y + scalingSpeed.y, nowScale.z + scalingSpeed.z);
                    }
                    else
                    {
                        //�I������
                        if ((scaleUp && judgeAxis <= endScale) || (!scaleUp && judgeAxis >= endScale)) break;
                        nowScale = new Vector3(nowScale.x - scalingSpeed.x, nowScale.y - scalingSpeed.y, nowScale.z - scalingSpeed.z);
                    }
                }

                //�ϐ����Z�b�g
                cloneTra.localScale = Vector3.one * endScale;
                scaleChange = true;
            }

            Destroy(cloneTra.gameObject);  //�N���[���폜
            return moveTime;               //���Ԃ�Ԃ�
        }


        //========================================================================
        //�F�ύX����(Image.color)
        //========================================================================
        //ima;          �ύX�Ώ�Image
        //changeSpeed;  �ύX���x
        //colArray;     �ύX�F�̔z��(0:���݂̐F)
        //compArray;    ��r�ԍ��w��z��(0:R 1:G 2:B 3:A)
        //chengeCount;  ���[�v��(�z��1����1�J�E���g�A-1�w��Ŗ����Đ�)
        //========================================================================
        public static IEnumerator ImagePaletteChange(Image ima, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            float oneFrameTime = 0.02f;            //1�t���[������
            int loopTimes      = 0;                //�J��Ԃ���
            int colCount       = colArray.Length;  //�ύX�F�̐�
            bool infinite      = chengeCount < 0;  //�������[�v�H

            int nowIndex  = 0;    //���݂̐F
            int nextIndex = 1;    //���̐F
            float nextCompCol = colArray[nextIndex][compArray[nowIndex]];   //��r�F�w��
            float judgeRange  = 5.0f / 255.0f;                              //����͈�

            ima.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                ima.color = Color.Lerp(ima.color, colArray[nextIndex], changeSpeed);
                float nowCompCol = ima.color[compArray[nowIndex]];
                if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                {
                    nowIndex    = nextIndex;
                    nextIndex   = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                    nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                    loopTimes++;

                    //�������[�v�I������
                    if (infinite && COLOR_CHANGE_INFINITE_END && nowIndex == 0)
                    {
                        COLOR_CHANGE_INFINITE_END = false;
                        break;
                    }
                }
                yield return new WaitForSecondsRealtime(oneFrameTime);
            }
            ima.color = colArray[nowIndex];
        }

        //========================================================================
        //�F�ύX����(ImagePaletteChange) �ɗv���鎞�Ԍv�Z
        //========================================================================
        //ima;          �ύX�Ώ�Image
        //changeSpeed;  �ύX���x
        //colArray;     �ύX�F�̔z��(0:���݂̐F)
        //compArray;    ��r�ԍ��w��z��(0:R 1:G 2:B 3:A)
        //chengeCount;  ���[�v��(�z��1����1�J�E���g�A-1�w��Ŗ����Đ�)
        //========================================================================
        public static float GetImagePaletteChangeTime(Image ima, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            //�N���[���쐻
            Image cloneIma = CloneCreate_Image(ima);
            float moveTime = 0.0f;

            float oneFrameTime = 0.02f;            //1�t���[������
            int loopTimes      = 0;                //�J��Ԃ���
            int colCount       = colArray.Length;  //�ύX�F�̐�
            bool infinite      = chengeCount < 0;  //�������[�v�H

            int nowIndex       = 0;    //���݂̐F
            int nextIndex      = 1;    //���̐F
            float nextCompCol  = colArray[nextIndex][compArray[nowIndex]];   //��r�F�w��
            float judgeRange   = 5.0f / 255.0f;                              //����͈�

            cloneIma.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                cloneIma.color = Color.Lerp(cloneIma.color, colArray[nextIndex], changeSpeed);
                float nowCompCol = cloneIma.color[compArray[nowIndex]];
                if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                {
                    nowIndex    = nextIndex;
                    nextIndex   = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                    nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                    loopTimes++;

                    //�������[�v�I������
                    if (infinite && COLOR_CHANGE_INFINITE_END && nowIndex == 0)
                    {
                        COLOR_CHANGE_INFINITE_END = false;
                        break;
                    }
                }

                //���Ԍv�Z
                moveTime += oneFrameTime;
            }
            cloneIma.color = colArray[nowIndex];

            Destroy(cloneIma.gameObject); //�N���[���폜
            return moveTime;              //���Ԃ�Ԃ�
        }


        //========================================================================
        //�F�ύX����(Text.color)
        //========================================================================
        //tex;          �ύX�Ώ�Text
        //changeSpeed;  �ύX���x
        //colArray;     �ύX�F�̔z��(0:���݂̐F)
        //compArray;    ��r�ԍ��w��z��(0:R 1:G 2:B 3:A)
        //chengeCount;  ���[�v��(��F�ύX��1�J�E���g�A-1�w��Ŗ����Đ�)
        //========================================================================
        public static IEnumerator TextPaletteChange(Text tex, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            float oneFrameTime = 0.02f;            //1�t���[������
            int loopTimes      = 0;                //�J��Ԃ���
            int colCount       = colArray.Length;  //�ύX�F�̐�
            bool infinite      = chengeCount < 0;  //�������[�v�H

            int nowIndex       = 0;    //���݂̐F
            int nextIndex      = 1;    //���̐F
            float nextCompCol  = colArray[nextIndex][compArray[nowIndex]];   //��r�F�w��
            float judgeRange   = 5.0f / 255.0f;                              //����͈�

            tex.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                tex.color = Color.Lerp(tex.color, colArray[nextIndex], changeSpeed);
                float nowCompCol = tex.color[compArray[nowIndex]];
                if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                {
                    nowIndex    = nextIndex;
                    nextIndex   = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                    nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                    loopTimes++;

                    //�������[�v�I������
                    if (infinite && COLOR_CHANGE_INFINITE_END && nowIndex == 0)
                    {
                        COLOR_CHANGE_INFINITE_END = false;
                        break;
                    }
                }
                yield return new WaitForSecondsRealtime(oneFrameTime);
            }
            tex.color = colArray[nowIndex];
        }

        //========================================================================
        //�F�ύX����(TextPaletteChange) �ɗv���鎞�Ԍv�Z
        //========================================================================
        //tex;          �ύX�Ώ�Text
        //changeSpeed;  �ύX���x
        //colArray;     �ύX�F�̔z��(0:���݂̐F)
        //compArray;    ��r�ԍ��w��z��(0:R 1:G 2:B 3:A)
        //chengeCount;  ���[�v��(��F�ύX��1�J�E���g�A-1�w��Ŗ����Đ�)
        //========================================================================
        public static float GetTextPaletteChangeTime(Text tex, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            //�N���[���쐬
            Text cloneTex = CloneCreate_Text(tex);
            float moveTime = 0.0f;

            float oneFrameTime = 0.02f;            //1�t���[������
            int loopTimes      = 0;                //�J��Ԃ���
            int colCount       = colArray.Length;  //�ύX�F�̐�
            bool infinite      = chengeCount < 0;  //�������[�v�H

            int nowIndex       = 0;    //���݂̐F
            int nextIndex      = 1;    //���̐F
            float nextCompCol  = colArray[nextIndex][compArray[nowIndex]];   //��r�F�w��
            float judgeRange   = 5.0f / 255.0f;                              //����͈�

            cloneTex.color = colArray[nowIndex];
            while (infinite || loopTimes < chengeCount)
            {
                cloneTex.color   = Color.Lerp(cloneTex.color, colArray[nextIndex], changeSpeed);
                float nowCompCol = cloneTex.color[compArray[nowIndex]];
                if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                {
                    nowIndex    = nextIndex;
                    nextIndex   = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                    nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                    loopTimes++;

                    //�������[�v�I������
                    if (infinite && COLOR_CHANGE_INFINITE_END && nowIndex == 0)
                    {
                        COLOR_CHANGE_INFINITE_END = false;
                        break;
                    }
                }

                //���Ԍv�Z
                moveTime += oneFrameTime;
            }
            cloneTex.color = colArray[nowIndex];

            Destroy(cloneTex.gameObject);  //�N���[���폜
            return moveTime;               //���Ԃ�Ԃ�
        }


        //========================================================================
        //�N���[�����I���W�i���Ɠ����W�ɃZ�b�g
        //========================================================================
        //tra;  �@����I�u�W�F�N�g��RectTransform
        //========================================================================
        public static RectTransform CloneCreate(RectTransform tra)
        {
            GameObject clone       = GameObject.Instantiate(tra.gameObject) as GameObject;
            RectTransform cloneTra = clone.GetComponent<RectTransform>();
            Transform parentTra    = tra.parent.gameObject.transform;
            cloneTra.SetParent(parentTra, false);
            cloneTra.localRotation = tra.localRotation;
            return cloneTra;
        }
        //========================================================================
        //ima;  �@����I�u�W�F�N�g��Image
        //========================================================================
        public static Image CloneCreate_Image(Image ima)
        {
            GameObject clone = GameObject.Instantiate(ima.gameObject) as GameObject;
            Image cloneIma   = clone.GetComponent<Image>();
            cloneIma.color   = ima.color;
            return cloneIma;
        }
        //========================================================================
        //tex;  �@����I�u�W�F�N�g��Text
        //========================================================================
        public static Text CloneCreate_Text(Text tex)
        {
            GameObject clone = GameObject.Instantiate(tex.gameObject) as GameObject;
            Text cloneTex    = clone.GetComponent<Text>();
            cloneTex.color   = tex.color;
            return cloneTex;
        }
    }
}