using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataManager : MonoBehaviour
{
	public class PuzzleModeSaveDate
	{
		public int puzzleModeStageNum = -1;
	}
	public class ShootModeSaveData
	{
		public int shootModeStageNum = -1;
	}

	PuzzleModeSaveDate puzzleModeSaveDate  = new PuzzleModeSaveDate();
	ShootModeSaveData shootModeSaveDate    = new ShootModeSaveData();
	public static SaveDataManager instance = null;

	//各クリアステージ番号
	[System.NonSerialized]
	public int puzzleModeStageNum = -1;
	[System.NonSerialized]
	public int shootModeStageNum  = -1;

	//ファイル名
	string puzzleModeFileName = "/PuzzleModeSaveData.json";
	string shootModeFileName  = "/ShootModeSaveData.json";

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

	//========================================================================
	//パズルモードセーブデータ書き込み
	//========================================================================
	//ClearStageNum; クリアステージ番号
	//========================================================================
	public void WritePuzzleModeSaveData(int ClearStageNum)
	{
		if(puzzleModeSaveDate.puzzleModeStageNum < ClearStageNum)
		{
			puzzleModeSaveDate.puzzleModeStageNum = ClearStageNum;

			StreamWriter writer;
			string jsonstr = JsonUtility.ToJson(puzzleModeSaveDate);

			writer = new StreamWriter(Application.persistentDataPath + puzzleModeFileName, false);
			writer.Write(jsonstr);
			writer.Flush();
			writer.Close();
		}
	}

	//========================================================================
	//シュートモードセーブデータ書き込み
	//========================================================================
	//ClearStageNum; クリアステージ番号
	//========================================================================
	public void WriteShootModeSaveData(int ClearStageNum)
	{
		if (shootModeSaveDate.shootModeStageNum < ClearStageNum)
		{
			shootModeSaveDate.shootModeStageNum = ClearStageNum;

			StreamWriter writer;
			string jsonstr = JsonUtility.ToJson(shootModeSaveDate);

			writer = new StreamWriter(Application.persistentDataPath + shootModeFileName, false);
			writer.Write(jsonstr);
			writer.Flush();
			writer.Close();
		}
	}

	//========================================================================
	//セーブデータ読み込み
	//========================================================================
	public void ReadSaveData()
	{
		//パズルモード
		if (File.Exists(Application.persistentDataPath + puzzleModeFileName))
		{
			string datastr = "";
			StreamReader reader;
			reader = new StreamReader(Application.persistentDataPath + puzzleModeFileName);
			datastr = reader.ReadToEnd();
			reader.Close();
			puzzleModeSaveDate = JsonUtility.FromJson<PuzzleModeSaveDate>(datastr);

			puzzleModeStageNum = puzzleModeSaveDate.puzzleModeStageNum;
		}

		//シュートモード
		if (File.Exists(Application.persistentDataPath + shootModeFileName))
		{
			string datastr = "";
			StreamReader reader;
			reader = new StreamReader(Application.persistentDataPath + shootModeFileName);
			datastr = reader.ReadToEnd();
			reader.Close();
			shootModeSaveDate = JsonUtility.FromJson<ShootModeSaveData>(datastr);

			shootModeStageNum = shootModeSaveDate.shootModeStageNum;
		}
	}
}
