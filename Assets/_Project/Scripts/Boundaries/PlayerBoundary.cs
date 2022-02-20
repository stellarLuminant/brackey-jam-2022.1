using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is to be attached to a Player script, to handle boundary transitions.
/// </summary>
public class PlayerBoundary : MonoBehaviour
{
	BoundaryManager boundaryManager;

	// Start is called before the first frame update
	void Start()
	{
		// Finds the boundary manager
		boundaryManager = FindObjectOfType<BoundaryManager>();
		if (!boundaryManager)
		{
			Debug.LogWarning(
				"PlayerBoundary.cs Start() was called, " + 
				"but there is no BoundaryManager in Scene"
			);
		} else
		{
			// Sets all boundaries to follow player through code
			foreach (var boundary in boundaryManager.Boundaries)
			{
				boundary.VirtualCamera.Follow = transform;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		foreach (var boundary in boundaryManager.Boundaries)
		{
			var collider = boundary.Collider;

			// If the player is inside the collider's bounds:
			if (
				collider.bounds.min.x < transform.position.x
				&& collider.bounds.max.x > transform.position.x
				&& collider.bounds.min.y < transform.position.y
				&& collider.bounds.max.y > transform.position.y)
			{
				// Update for when the boundary is shown
				boundary.UpdateShow();
			} else
			{
				// Update for when the boundary isn't shown
				boundary.UpdateHide();
			}
		}
	}
}
