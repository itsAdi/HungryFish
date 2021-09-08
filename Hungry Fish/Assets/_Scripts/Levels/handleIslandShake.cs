using UnityEngine;

public class handleIslandShake : MonoBehaviour {
	private Rigidbody2D selfRgbd;
	private Vector2 rPos, iPos;
	void Start(){
		selfRgbd = GetComponent<Rigidbody2D>();
		iPos = selfRgbd.position;
	}

	void FixedUpdate(){
		float theta = Time.timeSinceLevelLoad / 20f; // Adjust this number for speed of movement
        float distance = 10f * Mathf.Sin(theta); // Adjust this number for distance of shake
        rPos.x = iPos.x + 1f * distance;
		theta = Time.timeSinceLevelLoad / 0.2f; // Adjust this number for speed of movement
        distance = 0.03f * Mathf.Sin(theta); // Adjust this number for distance of shake
		rPos.y = iPos.y + 1f * distance;
		selfRgbd.MovePosition(rPos);
	}
}
