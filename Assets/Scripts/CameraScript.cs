using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	Camera c;
	RectTransform gameRect;
	float offset = 2;

	void Start () {
		this.c = this.GetComponent<Camera> ();
		this.gameRect = transform.Find("ViewRect").GetComponent<RectTransform>();
		// set the desired aspect ratio (the values in this example are
		// hard-coded for 16:9, but you could make them into public
		// variables instead so you can set them at design time)
		//float targetaspect = 16.0f / 9.0f;
		//float targetaspect = 9.0f / 16.0f;
		float targetaspect = gameRect.rect.width/gameRect.rect.height;

		// determine the game window's current aspect ratio
		float windowaspect = (float)Screen.width / (float)Screen.height;
		//Debug.LogFormat("Window aspect currently: {0}", windowaspect);

		// current viewport height should be scaled by this amount
		float scaleheight = windowaspect / targetaspect;
		// Debug.LogFormat("Scale ht {0}", scaleheight);

		if (scaleheight < 1.0f) // if scaled height is less than current height, add letterbox
		{  
			Rect rect = this.c.rect;

			rect.width = 1.0f;
			rect.height = scaleheight;
			rect.x = 0;
			rect.y = (1.0f - scaleheight) / 2.0f;

			this.c.rect = rect;
		}
		else // add pillarbox
		{
			float scalewidth = 1.0f / scaleheight;

			Rect rect = this.c.rect;

			rect.width = scalewidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scalewidth) / 2.0f;
			rect.y = 0;

			this.c.rect = rect;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
