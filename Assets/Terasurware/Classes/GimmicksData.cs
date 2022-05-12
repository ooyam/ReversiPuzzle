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
		public bool in_square;
		public float position_x;
		public float position_y;
		public float scale_x;
		public float scale_y;
	}
}