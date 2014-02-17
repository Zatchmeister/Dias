using UnityEngine;
using System.Collections;

public class BoxScript : AbilityHitListScript 
{
	Vector3 realPosition;
	Quaternion realRotation;
	float networkSmoothRate = 0.3f;
	float duration = 3f;

	void Awake()
	{
		realPosition = transform.position;
		realRotation = transform.rotation;
	}

	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if(networkView.isMine)
		{
			StartCoroutine(DestroyAfter(duration));
		}
	}

	void FixedUpdate()
	{
		if(networkView.isMine)						//if this is our rigidbody to keep track of, we need to keep it awake
		{
			rigidbody.WakeUp();
		}
	}

	void Update()
	{
		if(!networkView.isMine) 					//if this isn't our rigidbody, we need to smooth it's network movement
		{
			transform.position = Vector3.Lerp(transform.position, realPosition, networkSmoothRate);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, networkSmoothRate);
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if(stream.isWriting)
		{
			//this is our player
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
		}
		else
		{
			//this is another player
			stream.Serialize(ref realPosition);
			stream.Serialize(ref realRotation);
		}
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
