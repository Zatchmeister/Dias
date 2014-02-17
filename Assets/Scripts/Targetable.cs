using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct HitStruct
{
	public CharacterBuildScript playerHit;
	public CharacterBuildScript attacker;
	public float timestamp;
	
	public HitStruct(float ts)
	{
		playerHit = null;
		attacker = null;
		timestamp = ts;
	}
	
	public HitStruct(CharacterBuildScript atkr, CharacterBuildScript dfnd)
	{
		playerHit = dfnd;
		attacker = atkr;
		timestamp = Time.time;
	}
}

public class Targetable : MonoBehaviour 
{
	List<HitStruct> hits;
	CharacterBuildScript build;

	void Awake()
	{
		hits = new List<HitStruct>();
	}

	void Start()
	{
		build = GetComponent<CharacterBuildScript>();
	}

	HitStruct GetMostRecentHit(CharacterBuildScript player)
	{
		HitStruct mostRecent = new HitStruct(-1);
		foreach(HitStruct hit in hits)
		{
			if(hit.attacker == player && hit.timestamp > mostRecent.timestamp)
				mostRecent = hit;
		}
		return mostRecent;
	}

	public List<HitStruct> GetHits(float seconds)
	{
		List<HitStruct> recentHits = new List<HitStruct>();
		List<CharacterBuildScript> playersWithHits = new List<CharacterBuildScript>();

		foreach(HitStruct hit in hits)
		{
			//if the attack happened within 'seconds' of our death, it wasn't from someone on our team, and this player hasn't already been added to the hit list, report the hit
			if(hit.timestamp > Time.time - seconds && hit.attacker.build.team != build.build.team && !playersWithHits.Contains(hit.attacker))
			{
				//HACK this is approaching an O(n^2) solution to find the most recent unique attack by a player
				recentHits.Add(GetMostRecentHit(hit.attacker));
				playersWithHits.Add(hit.attacker);
			}
		}
		hits.Clear();
		return recentHits;
	}

	public void HitBy(CharacterBuildScript attacker)
	{
		hits.Add(new HitStruct(attacker, build));
	}
}
