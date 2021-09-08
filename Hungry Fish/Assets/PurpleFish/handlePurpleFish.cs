using UnityEngine;
using DragonBones;
using System.Collections;

public class handlePurpleFish : MonoBehaviour {
	public UnityArmatureComponent fishInstance;
	public Rigidbody2D selfRgbd;
	public enum dir{
		Left,
		Right
	}
	public dir fishDirection = dir.Left;

	private bool fishEating;
	private Vector2 fishDir, initPos;
	private float speed, maxDistance;

	void Start(){
		fishInstance.animation.Play("fe1sw", 0);
		fishDir = fishDirection == dir.Left ? Vector2.left : Vector2.right;
		Camera cam = Camera.main;
		maxDistance = fishDirection == dir.Left ? cam.ScreenToWorldPoint(new Vector2(- (cam.pixelWidth + (cam.pixelWidth / 2f + 10f)), 0f)).x : cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth + (cam.pixelWidth / 2f + 10f) ,0f)).x;
		initPos = selfRgbd.gameObject.transform.position;
	}

	void Update(){
		if(fishEating){
			if(fishInstance.animation.lastAnimationState.isCompleted){
				fishEating = false;
				fishInstance.animation.Play("fe1sw", 0);
			}
		}
	}

	void FixedUpdate(){
		selfRgbd.MovePosition(selfRgbd.position + (speed * fishDir * Time.deltaTime));	
		if (fishDirection == dir.Left)
		{
			if(selfRgbd.position.x < maxDistance){
				StartCoroutine(resetFish());	
			}
		}else
		{
			if(selfRgbd.position.x > maxDistance){
				StartCoroutine(resetFish());	
			}
		}
	}

	void OnTriggerEnter2D(Collider2D hit){
		if(!persistentData.Instance.shieldEnabled && hit.gameObject.tag == "Fish"){
			fishEating = true;
			fishInstance.animation.Stop("fe1sw");
			fishInstance.animation.Play("fe1e", 1);
			persistentData.Instance.hUI.onKilled();
			persistentData.Instance.mSound.playFish();
		}
	}

	IEnumerator resetFish(){
		yield return new WaitForSeconds(Random.Range(0.2f, 3f));
		selfRgbd.gameObject.transform.position = initPos;
		speed = Random.Range(2f, 6f);
	}

	public void initFish(){
		StartCoroutine(resetFish());	
	}
}
