using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveDataManager : MonoBehaviour
{
	public class SaveDate
	{
		//�ۑ����e
		public int ClearStageNumber = 0;
	}

	static SaveDate mSaveDate  = new SaveDate();
	static SaveDataManager instance = null;

	//�N���A�X�e�[�W�ԍ�
	public static int ClearStageNum { get; private set; }

	//�ۑ��t�@�C����
	static readonly string DIRECTORY_NAME = Application.persistentDataPath;
	static readonly string FILE_PATH = DIRECTORY_NAME + "/SaveDate.json";

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	/// <summary>
	/// �f�[�^��������
	/// </summary>
	/// <param name="_clearStageNum">�N���A�X�e�[�W�ԍ�</param>
	public static void DataSave(int _clearStageNum)
	{
		if (mSaveDate.ClearStageNumber < _clearStageNum)
		{
			mSaveDate.ClearStageNumber = _clearStageNum;

			StreamWriter writer;
			string jsonstr = JsonUtility.ToJson(mSaveDate);

			writer = new StreamWriter(FILE_PATH, false);
			writer.Write(jsonstr);
			writer.Flush();
			writer.Close();
		}
	}

	/// <summary>
	/// �f�[�^�ǂݍ���
	/// </summary>
	public static void DataLoad()
	{
		//�V���[�g���[�h
		if (File.Exists(FILE_PATH))
		{
			string dataStr;
			StreamReader reader;
			reader = new StreamReader(FILE_PATH);
			dataStr = reader.ReadToEnd();
			reader.Close();
			mSaveDate = JsonUtility.FromJson<SaveDate>(dataStr);

			//�f�[�^��ϐ��Ɋi�[
			ClearStageNum = mSaveDate.ClearStageNumber;
		}
	}
}
