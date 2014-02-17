using UnityEngine;
using System.Collections;

[RequireComponent (typeof(NetworkView))]
public class NetworkCharacterScript : MonoBehaviour 
{
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	float networkSmoothRate = 0.3f;

	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if(networkView.isMine)
		{
			PlayerInputScript inputScript = GetComponent<PlayerInputScript>();
			GameObject game = GameObject.FindGameObjectWithTag("GameController");
			game.GetComponent<CameraControllerScript>().RegisterPlayer(gameObject);
			game.GetComponent<PauseMenuScript>().RegisterInputScript(inputScript);
			inputScript.enabled = true;
			GetComponent<CharacterMoveScript>().enabled = true;
			transform.Find("CharacterCamera").gameObject.SetActive(true);
			transform.Find("TelescopeCamera").gameObject.SetActive(true);
		}
		else
		{
			GetComponent<Rigidbody>().isKinematic = true;
			GetComponent<Rigidbody>().useGravity = false;
		}
	}

	void Update()
	{
		if(networkView.isMine)
		{
			//this is our object, and the player move script will move us
		}
		else
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

	public void SetTransform(Vector3 pos, Quaternion rot)
	{
		transform.position = realPosition = pos;
		transform.rotation = realRotation = rot;
		gameObject.SetActive(true);
	}
}