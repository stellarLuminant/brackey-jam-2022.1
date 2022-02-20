using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasScaler))]
public class CanvasPixelScaling : MonoBehaviour
{
	public RectTransform BasePixelCanvas;

	Camera cam;
	CanvasScaler scaler;
	PixelPerfectCamera pixelCamera;

	void OnEnable()
	{
		Assert.IsNotNull(BasePixelCanvas, "BasePixelCanvas wasn't set!");

		pixelCamera = FindObjectOfType<PixelPerfectCamera>();
		Assert.IsNotNull(pixelCamera, "PixelPerfectCamera wasn't set!");
		
		cam = pixelCamera.GetComponent<Camera>();
		Assert.IsNotNull(cam, "Camera wasn't found!");

		scaler = GetComponent<CanvasScaler>();
		Assert.IsNotNull(scaler, "CanvasScaler wasn't set!");

		if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize)
		{
			Debug.LogWarning("scaler.uiScaleMode didn't equal ConstantPixelSize! Forcing to ConstantPixelSize.");
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
		}
	}

	// Update is called once per frame
	void Update()
	{
		var shouldScalePixelPerfectly = Application.isPlaying;
		
		#if UNITY_EDITOR
		// Editor specific code to be able to get pixelCamera.runInEditMode
		shouldScalePixelPerfectly = shouldScalePixelPerfectly 
			|| (pixelCamera && pixelCamera.runInEditMode);
		#endif

		if (shouldScalePixelPerfectly && pixelCamera)
		{
			Scale();
		}
	}

	public void Scale()
    {
		// Sets the correct scale factor
		var scaleX = Math.Floor((double)(Screen.width / pixelCamera.refResolutionX));
		var scaleY = Math.Floor((double)(Screen.height / pixelCamera.refResolutionY));
		//scaler.scaleFactor = pixelCamera.pixelRatio;
		scaler.scaleFactor = (float)Math.Min(scaleX, scaleY);

		// Sets the BasePixelCanvas size to match the pixel perfect camera's size, multiplied by ratio
		// Note that in order for this to work, the rect needs to be in the center.
		var rect = BasePixelCanvas.rect;
		rect.width = pixelCamera.refResolutionX * pixelCamera.pixelRatio;
		rect.height = pixelCamera.refResolutionY * pixelCamera.pixelRatio;
	}
}
