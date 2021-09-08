using UnityEngine;

public class handleKillArea : MonoBehaviour {
	void OnCollisionEnter2D(Collision2D hit){
		if(!persistentData.Instance.gameOver && hit.gameObject.tag.Equals("Fish")){
			persistentData.Instance.hUI.onKilled();
		}
	}
}
