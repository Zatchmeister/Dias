using UnityEngine;
using System.Collections;

public class WallScript : AbilityHitListScript {

	float duration = 5f;
	PlatformScript platfom;

	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if(networkView.isMine)
		{
			StartCoroutine(DestroyAfter(duration));
		}
		platfom = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlatformManagerScript>().FindNearestPlatform(transform.position);
		transform.parent = platfom.transform;
	}
	
	IEnumerator DestroyAfter(float time)
	{
		for(float x = 0f; x < time; x += Time.deltaTime)
		{
			yield return 0;
		}
		Network.Destroy(gameObject);
	}
}
