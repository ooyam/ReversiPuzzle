using UnityEngine;
using System.Collections;
using static Sound.SoundManager;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class BGM_DataData
{
  [SerializeField]
  BGM_Type bgm_type;
  public BGM_Type BGM_TYPE { get {return bgm_type; } set { this.bgm_type = value;} }
  
  [SerializeField]
  string clipname;
  public string Clipname { get {return clipname; } set { this.clipname = value;} }
  
  [SerializeField]
  float volume;
  public float Volume { get {return volume; } set { this.volume = value;} }
  
}