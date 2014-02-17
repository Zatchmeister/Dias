using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityHitListScript : MonoBehaviour
{
	protected IList<GameObject> playersHit = new List<GameObject> ();
	protected NetworkViewID caster;			//the actual object that cast the ability
	protected NetworkViewID casterClient;	//the client from which the ability was cast
	protected PlayerDataScript playerData;

	void Awake ()
	{
		playerData = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PlayerDataScript> ();
	}

	protected void TryHitTarget (GameObject player)
	{
		//tell the person that they were hit by us
		Targetable target = player.GetComponent<Targetable> ();
		if (target) {
			target.HitBy (playerData.GetPlayerData (casterClient).character);
		}
	}

	[RPC]
	public void SetCaster (NetworkViewID id)
	{
		caster = id;
	}

	[RPC]
	public void SetCasterClient (NetworkViewID id)
	{
		casterClient = id;
	}

	public void AddHitPlayer (GameObject player)
	{
		playersHit.Add (player);
	}
}
