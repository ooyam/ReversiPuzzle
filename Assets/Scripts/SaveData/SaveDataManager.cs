using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveDataManager
{
	public class SaveDate
	{
		//�ۑ����e
		public int ClearStageNumber = 0;        //�N���A�X�e�[�W�ԍ�
		public int ViewedTutorialNumber = -1;   //�\���ς݃`���[�g���A���ԍ�
	}

	static SaveDate sSaveDate  = new SaveDate();

	public static int ClearStageNum { get; private set; }		//�N���A�X�e�[�W�ԍ�
	public static int ViewedTutorialNum { get; private set; }	//�\���ς݃`���[�g���A���ԍ�

	//�ۑ��t�@�C����
	static readonly string DIRECTORY_NAME = Application.persistentDataPath;
	static readonly string FILE_PATH = DIRECTORY_NAME + "/SaveDate.json";


	//=========================================//
	//---------------�f�[�^�̐ݒ�--------------//
	//=========================================//

	/// <summary>
	/// �N���A�X�e�[�W�̐ݒ�
	/// </summary>
	/// <param name="_stageNum">�X�e�[�W�ԍ�</param>
	public static void SetClearStageNum(int _stageNum)
    {
		if (ClearStageNum < _stageNum) ClearStageNum = _stageNum;
	}

	/// <summary>
	/// �\���ς݃`���[�g���A���ԍ��̐ݒ�
	/// </summary>
	/// <param name="_tutorialNum">�`���[�g���A���ԍ�</param>
	public static void SetViewedTutorialNum(int _tutorialNum)
    {
		if (ViewedTutorialNum < _tutorialNum) ViewedTutorialNum = _tutorialNum;
	}


	//=========================================//
	//-------------��������/�ǂݍ���-----------//
	//=========================================//

	/// <summary>
	/// �f�[�^��������
	/// </summary>
	public static void DataSave()
	{
		//�f�[�^�̃Z�b�g
		sSaveDate.ClearStageNumber = ClearStageNum;
		sSaveDate.ViewedTutorialNumber = ViewedTutorialNum;

		//�f�[�^��������
		StreamWriter writer;
		string jsonstr = JsonUtility.ToJson(sSaveDate);
		writer = new StreamWriter(FILE_PATH, false);
		writer.Write(jsonstr);
		writer.Flush();
		writer.Close();
	}

	/// <summary>
	/// �f�[�^�ǂݍ���
	/// </summary>
	public static void DataLoad()
	{
		if (File.Exists(FILE_PATH))
		{
			string dataStr;
			StreamReader reader;
			reader = new StreamReader(FILE_PATH);
			dataStr = reader.ReadToEnd();
			reader.Close();
			sSaveDate = JsonUtility.FromJson<SaveDate>(dataStr);
		}

		//�f�[�^��ϐ��Ɋi�[
		ClearStageNum = sSaveDate.ClearStageNumber;
		ViewedTutorialNum = sSaveDate.ViewedTutorialNumber;
	}
}
