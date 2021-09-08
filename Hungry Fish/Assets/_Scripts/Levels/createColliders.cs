using UnityEngine;

public class createColliders : MonoBehaviour {
	public EdgeCollider2D colliderObject;
	void Start(){
		Camera cam = Camera.main;
		Vector2 bottomLeft = cam.ScreenToWorldPoint(new Vector3(-cam.pixelWidth / 2f, 0f, cam.nearClipPlane));
		Vector2 topLeft = cam.ScreenToWorldPoint(new Vector3(-cam.pixelWidth / 2f, cam.pixelHeight, cam.nearClipPlane));
		Vector2 topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth + (cam.pixelWidth / 2f), cam.pixelHeight, cam.nearClipPlane));
		Vector2 bottomRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth + (cam.pixelWidth / 2f), 0f, cam.nearClipPlane));
		Vector2[] pnts = new Vector2[5]{bottomLeft, bottomRight, topRight, topLeft, bottomLeft};
		colliderObject.points = pnts; 
	}
}
