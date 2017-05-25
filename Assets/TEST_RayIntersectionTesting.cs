using UnityEngine;
using System.Collections;

public class TEST_RayIntersectionTesting : MonoBehaviour
{
	
	private dfGUIManager uiRoot;
	
	/// <summary>
	/// Initialization
	/// </summary>
	public void Start()
	{
		// Obtain a reference to the attached dfGUIManager instance
		// for testing purposes
		uiRoot = GameObject.Find ("UI Root").GetComponent<dfGUIManager>();
	}
	
	/// <summary>
	/// Tests the IsControlUnderMouse function
	/// </summary>
	public void Update()
	{
		var controlFound = GetControlUnderMouse( uiRoot, Input.mousePosition );
		if( controlFound != null )
		{
			//transform.position = new Vector3(controlFound.transform.position.x, controlFound.transform.position.y + 0.11f, 0);
			//Debug.Log( "Control under mouse: " + controlFound + ", Tooltip: " + controlFound.Tooltip );
			gameObject.GetComponent<dfLabel>().Text = controlFound.Tooltip;
		}
		else
		{
			gameObject.GetComponent<dfLabel>().Text = "";
		}
	}
	
	/// <summary>
	/// Returns TRUE if an active and enabled control overlaps the given mouse
	/// position, FALSE otherwise
	/// </summary>
	/// <param name="uiRoot">A reference to the dfGUIManager instance used to
	/// manage the active UI controls</param>
	/// <param name="mousePosition">The Vector2 value of the current mouse position (ie: Input.mousePosition)</param>
	/// <returns></returns>
	public dfControl GetControlUnderMouse( dfGUIManager uiRoot, Vector2 mousePosition )
	{
		
		var renderCamera = uiRoot.RenderCamera;
		
		var viewportMousePos = renderCamera.ScreenToViewportPoint( mousePosition );
		if( viewportMousePos.x < 0f || viewportMousePos.x > 1f || viewportMousePos.y < 0f || viewportMousePos.y > 1f )
			return null;
		
		var ray = renderCamera.ScreenPointToRay( mousePosition );
		var maxDistance = renderCamera.farClipPlane - renderCamera.nearClipPlane;
		
		var hits = Physics.RaycastAll( ray, maxDistance, renderCamera.cullingMask );
		
		return clipCast( uiRoot, hits );
		
	}
	
	/// <summary>
	/// Refines the list of RaycastHit results and narrows it down to a single
	/// unclipped dfControl object if possible
	/// </summary>
	/// <param name="hits"></param>
	/// <returns></returns>
	private dfControl clipCast( dfGUIManager uiRoot, RaycastHit[] hits )
	{
		
		if( hits == null || hits.Length == 0 )
			return null;
		
		var match = (dfControl)null;
		var modalControl = dfGUIManager.GetModalControl();
		
		for( int i = hits.Length - 1; i >= 0; i-- )
		{
			
			var hit = hits[ i ];
			var control = hit.transform.GetComponent<dfControl>();
			var skipControl =
				control == null ||
					( modalControl != null && !control.transform.IsChildOf( modalControl.transform ) ) ||
					!control.enabled ||
					control.Opacity < 0.01f ||
					!control.IsEnabled ||
					!control.IsVisible ||
					!control.transform.IsChildOf( uiRoot.transform );
			
			if( skipControl )
				continue;
			
			if( isInsideClippingRegion( hit, control ) )
			{
				if( match == null || control.RenderOrder > match.RenderOrder )
				{
					match = control;
				}
			}
			
		}
		
		return match;
		
	}
	
	/// <summary>
	/// Determines whether the raycast point on the given control is
	/// inside of the control's clip region hierarchy
	/// </summary>
	/// <param name="control"></param>
	/// <returns></returns>
	private bool isInsideClippingRegion( RaycastHit hit, dfControl control )
	{
		
		var point = hit.point;
		
		while( control != null )
		{
			
			var planes = control.ClipChildren ? control.GetClippingPlanes() : null;
			if( planes != null && planes.Length > 0 )
			{
				for( int i = 0; i < planes.Length; i++ )
				{
					if( !planes[ i ].GetSide( point ) )
					{
						return false;
					}
				}
			}
			
			control = control.Parent;
			
		}
		
		return true;
		
	}
	
}