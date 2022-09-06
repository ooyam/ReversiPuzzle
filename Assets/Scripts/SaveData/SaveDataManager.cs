using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveDataManager
{
	public class SaveDate
	{
		//保存内容
		public int ClearStageNumber = 0;        //クリアステージ番号
		public int ViewedTutorialNumber = -1;   //表示済みチュートリアル番号
	}

	static SaveDate sSaveDate  = new SaveDate();

	public static int ClearStageNum { get; private set; }		//クリアステージ番号
	public static int ViewedTutorialNum { get; private set; }	//表示済みチュートリアル番号

	//保存ファイル名
	static readonly string DIRECTORY_NAME = Application.persistentDataPath;
	static readonly string FILE_PATH = DIRECTORY_NAME + "/SaveDate.json";


	//=========================================//
	//---------------データの設定--------------//
	//=========================================//

	/// <summary>
	/// クリアステージの設定
	/// </summary>
	/// <param name="_stageNum">ステージ番号</param>
	public static void SetClearStageNum(int _stageNum)
    {
		if (ClearStageNum < _stageNum) ClearStageNum = _stageNum;
	}

	/// <summary>
	/// 表示済みチュートリアル番号の設定
	/// </summary>
	/// <param name="_tutorialNum">チュートリアル番号</param>
	public static void SetViewedTutorialNum(int _tutorialNum)
    {
		if (ViewedTutorialNum < _tutorialNum) ViewedTutorialNum = _tutorialNum;
	}


	//=========================================//
	//-------------書き込み/読み込み-----------//
	//=========================================//

	/// <summary>
	/// データ書き込み
	/// </summary>
	public static void DataSave()
	{
		//データのセット
		sSaveDate.ClearStageNumber = ClearStageNum;
		sSaveDate.ViewedTutorialNumber = ViewedTutorialNum;

		//データ書き込み
		StreamWriter writer;
		string jsonstr = JsonUtility.ToJson(sSaveDate);
		writer = new StreamWriter(FILE_PATH, false);
		writer.Write(jsonstr);
		writer.Flush();
		writer.Close();
	}

	/// <summary>
	/// データ読み込み
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

		//データを変数に格納
		ClearStageNum = sSaveDate.ClearStageNumber;
		ViewedTutorialNum = sSaveDate.ViewedTutorialNumber;
	}
}
