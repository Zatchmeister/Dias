using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class CharacterMoveScript : MonoBehaviour
{
	enum MoveType { MOVE_POSITION, ADD_FORCE, NONE }

	public float moveSpeedModifier;
	public float jumpHeightModifier;
	public bool isGrounded;
	MoveType moveType;

	CharacterBuildScript character;

	void Awake () 
	{
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = true;
		moveSpeedModifier = 1f;
		jumpHeightModifier = 1f;
		isGrounded = true;
		ResetMoveType ();
	}

	void Start ()
	{
		character = GetComponent<CharacterBuildScript> ();
	}

	void Update ()
	{
		if (networkView.isMine) 
		{
			rigidbody.WakeUp ();
		}
	}

	public void ResetMoveType ()
	{
		moveType = MoveType.MOVE_POSITION;
	}

	public void ToggleMoveType ()
	{
		if (moveType == MoveType.MOVE_POSITION)
			moveType = MoveType.NONE;
		else
			moveType = MoveType.MOVE_POSITION;
	}

	public void SetYRotation (float rotation)
	{
		//transform.rotation = Quaternion.Euler(0, rotation, 0);
		rigidbody.MoveRotation (Quaternion.Euler (0, rotation, 0));
	}

	public void Move (float horz, float vert)
	{
		if (horz != 0f || vert != 0f) 
		{
			if (moveType == MoveType.MOVE_POSITION)
				MovePosition (horz, vert);
			else if (moveType == MoveType.MOVE_POSITION)
				MoveAddForce (horz, vert);
			//else if (moveType == MoveType.NONE)
			//The player cannot move

			/*
			Vector3 velocityChange = targetVelocity - rigidbody.velocity;
			velocityChange.y = 0f;
			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			*/
		}
	}

	void MovePosition (float horz, float vert)
	{
		Vector3 movement = new Vector3 (horz, 0f, vert);
		movement = transform.TransformDirection (movement).normalized;
		movement *= GameSettings.moveSpeed * moveSpeedModifier * Time.fixedDeltaTime;
		
		rigidbody.MovePosition (transform.position + movement);
	}

	void MoveAddForce (float horz, float vert)
	{
		Vector3 movement = new Vector3 (horz, 0f, vert);
		movement = transform.TransformDirection (movement).normalized;
		movement *= GameSettings.moveSpeed * moveSpeedModifier * Time.fixedDeltaTime;
		//rigidbody.AddForce(movement * 6f);
		movement *= 25f;

		if (movement.x > 0f && rigidbody.velocity.x > 0f) 
		{
			if (movement.x > rigidbody.velocity.x)
				rigidbody.AddForce (new Vector3 (movement.x, 0f, 0f), ForceMode.VelocityChange);
		}
		else if (movement.x < 0f && rigidbody.velocity.x < 0f) 
		{
			if (movement.x < rigidbody.velocity.x)
				rigidbody.AddForce (new Vector3 (movement.x, 0f, 0f), ForceMode.VelocityChange);
		}
		else 
		{
			rigidbody.AddForce (new Vector3 (movement.x, 0f, 0f), ForceMode.VelocityChange);
		}
		
		if (movement.z > 0f && rigidbody.velocity.z > 0f) 
		{
			if (movement.z > rigidbody.velocity.z)
				rigidbody.AddForce (new Vector3 (0f, 0f, movement.z), ForceMode.VelocityChange);
		}
		else if (movement.z < 0f && rigidbody.velocity.z < 0f) 
		{
			if (movement.z < rigidbody.velocity.z)
				rigidbody.AddForce (new Vector3 (0f, 0f, movement.z), ForceMode.VelocityChange);
		}
		else 
		{
			rigidbody.AddForce (new Vector3 (0f, 0f, movement.z), ForceMode.VelocityChange);
		} 
	}

	public void Jump (bool jumping)
	{
		if (jumping && character.CanJump ()) 
		{
			rigidbody.velocity = new Vector3 (rigidbody.velocity.x, CalculateJumpVerticalSpeed (), rigidbody.velocity.z);
			character.DoJump ();
		}
	}

	float CalculateJumpVerticalSpeed ()
	{
		// From the jump height and gravity we deduce the upwards speed for the character to reach at the apex.
		return Mathf.Sqrt (2 * GameSettings.standardJumpHeight * jumpHeightModifier * -Physics.gravity.y);
	}

	void CheckGrounded (Collision collision, bool isEnter)
	{
		foreach (ContactPoint point in collision.contacts) 
		{
			if (point.point.y < transform.position.y + GameSettings.characterGroundedHeight) //This assumes the player is standing upright
			{		
				isGrounded = true;
				if (isEnter && networkView.isMine)			//only modify the jump counter if it is our character, and we have entered the collision
					character.ResetJumpCount ();
				StopCoroutine ("NotGrounded");
				return;
			}
		}
	}

	void OnCollisionEnter (Collision collision)
	{
		CheckGrounded (collision, true);
	}

	void OnCollisionStay (Collision collision)
	{
		CheckGrounded (collision, false);
	}

	void OnCollisionExit (Collision collision)
	{
		StartCoroutine (NotGrounded ());
	}

	IEnumerator NotGrounded ()
	{
		yield return new WaitForSeconds (GameSettings.characterGroundedTime);
		isGrounded = false;
	}
}
