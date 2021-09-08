using UnityEngine;

public class handleCamera : MonoBehaviour {
	public Rigidbody2D fishObject;
	public Transform camTransform;
	public Transform[] lastLayer, secondLastLayer;

	private Vector3 camPos, tergetPos;
	private float[] margins;

	void Start(){
		Camera cam = Camera.main;
		camPos = camTransform.position;
		tergetPos = fishObject.position;
		margins = new float[2];
		margins[0] = cam.ScreenToWorldPoint(new Vector3(0f,0f,0f)).x;
		margins[1] = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth,0f,0f)).x;
	}

	void LateUpdate(){
			Vector3 tempPos;
			tempPos.z = 0f;
			float lastCamPosX = camPos.x;
			tergetPos.x = Mathf.Clamp(fishObject.position.x, margins[0], margins[1]);
			camPos.x = Mathf.Lerp(camTransform.position.x, tergetPos.x, 0.125f);
			lastCamPosX = camPos.x - lastCamPosX;
			for (int i = 0; i < lastLayer.Length; i++)
			{
				tempPos.y = lastLayer[i].position.y;
				tempPos.x = lastLayer[i].position.x;
				tempPos.x += lastCamPosX * 0.7f;
				lastLayer[i].position = tempPos;
				tempPos.y = secondLastLayer[i].position.y;
				tempPos.x = secondLastLayer[i].position.x;
				tempPos.x += lastCamPosX * 0.5f;
				secondLastLayer[i].position = tempPos;
			}
			camTransform.position = camPos;
			tergetPos = camTransform.position;
	}
}
