using UnityEngine;
using System.Collections;
using static Sound.SoundManager;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class SE_DataData
{
  [SerializeField]
  SE_Type se_type;
  public SE_Type SE_TYPE { get {return se_type; } set { this.se_type = value;} }
  
  [SerializeField]
  string clipname;
  public string Clipname { get {return clipname; } set { this.clipname = value;} }
  
  [SerializeField]
  float volume;
  public float Volume { get {return volume; } set { this.volume = value;} }
  
  [SerializeField]
  int playtimes;
  public int Playtimes { get {return playtimes; } set { this.playtimes = value;} }
  
  [SerializeField]
  float playspan;
  public float Playspan { get {return playspan; } set { this.playspan = value;} }
  
}