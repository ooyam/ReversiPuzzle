using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GimmicksData : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int id;
		public string name;
		public bool free_fall;
		public int damage_times;
		public bool continuous;
	}
}