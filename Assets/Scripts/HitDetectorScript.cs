using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Collider))]
public class HitDetectorScript : MonoBehaviour {

	AbilityHitListScript list;

	// Use this for initialization
	void Start ()
	{
		list = transform.parent.GetComponent<AbilityHitListScript>();
		if(!list)
			Debug.LogError("Error grabbing AbilityHitListScript from parent in HitDetectorScript for object (" + gameObject.name + ")");
	}
	
	void OnTriggerEnter(Collider collider)
	{
		list.AddHitPlayer(collider.gameObject);
	}
}
