using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveDataManager : MonoBehaviour
{
	public class SaveDate
	{
		//保存内容
		public int ClearStageNumber = 0;
	}

	static SaveDate mSaveDate  = new SaveDate();
	static SaveDataManager instance = null;

	//クリアステージ番号
	public static int ClearStageNum { get; private set; }

	//保存ファイル名
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
	/// データ書き込み
	/// </summary>
	/// <param name="_clearStageNum">クリアステージ番号</param>
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
	/// データ読み込み
	/// </summary>
	public static void DataLoad()
	{
		//シュートモード
		if (File.Exists(FILE_PATH))
		{
			string dataStr;
			StreamReader reader;
			reader = new StreamReader(FILE_PATH);
			dataStr = reader.ReadToEnd();
			reader.Close();
			mSaveDate = JsonUtility.FromJson<SaveDate>(dataStr);

			//データを変数に格納
			ClearStageNum = mSaveDate.ClearStageNumber;
		}
	}
}
