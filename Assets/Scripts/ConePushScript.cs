using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConePushScript : AbilityHitListScript {

	float duration = 0.02f;	//Maximum time this collider will exist, recommended to be really, REALLY small
	float forceMagnitude = 3f;

	// Use this for initialization
	void Start ()
	{
		Destroy(gameObject, duration);
	}

	void OnDestroy()
	{
		foreach(GameObject character in playersHit)
		{
			if(character && character.rigidbody && character.networkView.viewID != caster && character.networkView.isMine)	//ignore the caster
			{
				//do stuff for the ability here
				Vector3 forceStrength = (transform.rotation * new Vector3(0f, 0f, 1f)).normalized * forceMagnitude;
				character.rigidbody.AddForce(forceStrength, ForceMode.Impulse);

				//try to report a hit on a target
				TryHitTarget(character);
			}
		}
	}
}