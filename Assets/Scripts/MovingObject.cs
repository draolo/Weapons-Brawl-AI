using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MovingObject : MonoBehaviour 
{
	/// <summary>
	/// The previous position.
	/// </summary>
	public Vector2 mOldPosition;
	/// <summary>
	/// The current position.
	/// </summary>
	public Vector2 mPosition;


	/// <summary>
	/// The AABB for collision queries.
	/// </summary>
	public AABB mAABB;
	
	/// <summary>
	/// The tile map.
	/// </summary>
	public Map mMap;

	

	/// <summary>
	/// True if the instance is on the ground.
	/// </summary>
	//[HideInInspector]
	public bool mOnGround = false;

	/// <summary>
	/// The previous state of atCeiling.
	/// </summary>
	//[HideInInspector]
	public bool mWasAtCeiling = false;


	
	/// <summary>
	/// Depth for z-ordering the sprites.
	/// </summary>
	public float mSpriteDepth = 0f;


	void OnDrawGizmos()
	{
		DrawMovingObjectGizmos ();
	}

	/// <summary>
	/// Draws the aabb and ceiling, ground and wall sensors .
	/// </summary>
	protected void DrawMovingObjectGizmos()
	{
		//calculate the position of the aabb's center
		var aabbPos = transform.position;
		
		//draw the aabb rectangle
		Gizmos.color = Color.yellow;
   		Gizmos.DrawWireCube(aabbPos, mAABB.HalfSize*2.0f);
		
		//draw the ground checking sensor
		Vector2 bottomLeft = aabbPos - new Vector3(mAABB.HalfSizeX, mAABB.HalfSizeY, 0.0f) - Vector3.up + Vector3.right;
		var bottomRight = new Vector2(bottomLeft.x + mAABB.HalfSizeX*2.0f - 2.0f, bottomLeft.y);
		
		Gizmos.color = Color.red;
		Gizmos.DrawLine(bottomLeft, bottomRight);
		
		//draw the ceiling checking sensor
		Vector2 topRight = aabbPos + new Vector3(mAABB.HalfSize.x, mAABB.HalfSize.y, 0.0f) + Vector3.up - Vector3.right;
		var topLeft = new Vector2(topRight.x - mAABB.HalfSize.x*2.0f + 2.0f, topRight.y);
		
		Gizmos.color = Color.red;
		Gizmos.DrawLine(topLeft, topRight);
		
		//draw left wall checking sensor
		bottomLeft = aabbPos - new Vector3(mAABB.HalfSize.x, mAABB.HalfSize.y, 0.0f) - Vector3.right;
		topLeft = bottomLeft;
		topLeft.y += mAABB.HalfSize.y * 2.0f;
		
		Gizmos.DrawLine(topLeft, bottomLeft);
		
		//draw right wall checking sensor
		
		bottomRight = aabbPos + new Vector3(mAABB.HalfSize.x, -mAABB.HalfSize.y, 0.0f) + Vector3.right;
		topRight = bottomRight;
		topRight.y += mAABB.HalfSize.y * 2.0f;
		
		Gizmos.DrawLine(topRight, bottomRight);
	}

}