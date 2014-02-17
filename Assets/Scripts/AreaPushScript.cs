using UnityEngine;
using System.Collections;

public class AreaPushScript : AbilityHitListScript 
{
	float duration = 0.02f;	//Maximum time this collider will exist, recommended to be really, REALLY small
	float forceMagnitude = 1.5f;
	
	// Use this for initialization
	void Start ()
	{
		Destroy(gameObject, duration);
	}
	
	void OnDestroy()
	{
		foreach(GameObject character in playersHit)
		{
			if(character.networkView.viewID != caster && character.networkView.isMine)	//ignore the caster
			{
				//do stuff for the ability here
				Vector3 forceStrength = (character.transform.position - transform.position).normalized * forceMagnitude;
				character.rigidbody.AddForce(forceStrength, ForceMode.Impulse);

				//try to report a hit on a target
				TryHitTarget(character);
			}
		}
	}
}
