using UnityEngine;
using System.Collections;
using DragonBones;

public class handleKingfisher : MonoBehaviour {
	public Rigidbody2D rgbd;
	public UnityArmatureComponent kingFisher;
	public UnityEngine.Transform fish;

	private Vector2 targetPos, dir, lastPos, attackDir, initPos;
	private float maxRange, currTime, totalTime, targetAngle, fromAngle;
	private bool birdRoaming, birdFollowing, birdInitialized, birdAttack, birdFlyBack, canEat;

	void Start(){
		targetPos = new Vector2(0f, Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.85f)).y);
		lastPos = rgbd.position;
		initPos = rgbd.position;
		maxRange = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.2f, 0f)).y;
		totalTime = currTime = 2f;
		birdFollowing = true;
	}

	void Update(){
		if(!birdFollowing){
			currTime += Time.deltaTime;
			if(currTime >= totalTime){
				currTime = 0f;
				dir = (rgbd.position - lastPos).normalized * -1f;
			}
		}
		if(fish.position.y > 0.9f){
			canEat = false;
		}else
		{
			canEat = true;
		}
	}

	void OnTriggerEnter2D(Collider2D hit){
		if(!persistentData.Instance.gameOver && hit.gameObject.tag == "Fish" && canEat){
			persistentData.Instance.hUI.onKilled();
			kingFisher.animation.Stop();
			kingFisher.animation.Play("flying_withfish", 0);
			persistentData.Instance.mSound.playSpider();
		}
	}

	void FixedUpdate(){
		if(birdInitialized){
			if(Vector2.Distance(rgbd.position, targetPos) > 0.1f){
				rgbd.MovePosition(rgbd.position + ( dir * 3f * Time.deltaTime));
			}else
			{
				float centerX = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).x;
				dir = rgbd.position.x >= centerX ? Vector2.left : Vector2.right;
				birdInitialized = false;
				birdRoaming = true;
			}
		}
		if(birdRoaming){
			if(!persistentData.Instance.shieldEnabled && fish.position.y >= maxRange){
				birdFollowing = true;
				targetPos.x = Mathf.Lerp(rgbd.position.x, rgbd.position.x >= fish.position.x ? fish.position.x + 3f : fish.position.x - 3f, 0.02f);
				rgbd.MovePosition(targetPos);
				dir = (rgbd.position - lastPos).normalized;
				if(!IsInvoking("attack") && !persistentData.Instance.gameOver){
					Invoke("attack", 2f);
				}
			}else
			{
				if(IsInvoking("attack")){
					CancelInvoke("attack");
				}
				birdFollowing = false;
				rgbd.MovePosition(rgbd.position + ( dir * 3f * Time.deltaTime));
			}
			if(dir.x > 0f && kingFisher.armature.flipX){
				kingFisher.armature.flipX = false;
			}
			if(dir.x < 0f && !kingFisher.armature.flipX){
				kingFisher.armature.flipX = true;
			}
			lastPos = rgbd.position;
		}
		if(birdAttack){
			if(currTime != totalTime){
				currTime += Time.deltaTime;
				if(currTime >= totalTime){
					currTime = totalTime;
				}
			}
			float t = currTime / totalTime;
			rgbd.MoveRotation(Mathf.Lerp(fromAngle, targetAngle, t));
			Vector2 temp = rgbd.GetRelativeVector(Vector2.right);
			rgbd.MovePosition(rgbd.position + (temp * 12f * Time.deltaTime));
			if(!birdFlyBack && rgbd.position.y < persistentData.Instance.waterInstance.waterLevel){
				persistentData.Instance.mSound.playKingIn();
				persistentData.Instance.waterInstance.Splash(rgbd.position.x, 12f * Time.deltaTime);
				birdFlyBack = true;
				currTime = 0f;
				fromAngle = targetAngle;
				targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				kingFisher.animation.Stop();
				kingFisher.animation.Play("flying-nofish", 0);
			}
			if(birdFlyBack && rgbd.position.y > persistentData.Instance.waterInstance.waterLevel){
				persistentData.Instance.mSound.playKingOut();
				birdFlyBack = false;
				persistentData.Instance.waterInstance.Splash(rgbd.position.x, 12f * Time.deltaTime);				
			}
			if(rgbd.position.y > initPos.y){
				reset();
			}
		}
	}

	void attack(){
		birdRoaming = false;
		birdFollowing = true;
		Vector2 fPos = fish.position;
		fPos.x = rgbd.position.x >= fish.position.x ? fish.position.x + 1.1f : fish.position.x - 1.1f;
		fPos.y = fish.position.y >= (rgbd.position.y - 1f) ? maxRange : fish.position.y;
		attackDir = (fPos - rgbd.position).normalized;
		if(attackDir.x == 0f){
			attackDir.x = (float)Random.Range(-1, 1);
		}
		kingFisher.armature.flipX = attackDir.x > 0f ? false : true;
		kingFisher.animation.Stop();
		kingFisher.animation.Play("flying_attack", 0);
		if(kingFisher.armature.flipX)
		{
			kingFisher.armature.flipX = false;
			kingFisher.armature.flipY = true;
			rgbd.MoveRotation(180f);
			fromAngle = 180f;
		}else
		{
			fromAngle = 0f;
		}
		targetAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg + (fromAngle * 2f);
		dir = Vector2.Reflect(attackDir, Vector2.up);
		currTime = 0f;
		totalTime = 0.2f;
		birdAttack = true;
	}

	void reset(){
		rgbd.MovePosition(initPos);
		rgbd.MoveRotation(0f);
		kingFisher.animation.Stop();
		kingFisher.animation.Play("flying_normal", 0);
		birdFollowing = true;
		totalTime = currTime = 2f;
		birdAttack = false;
		birdRoaming = false;
		birdFlyBack = false;
		kingFisher.armature.flipX = false;
		kingFisher.armature.flipY = false;
		StartCoroutine(initBird());
	}

	IEnumerator initBird(){
		yield return new WaitForSeconds(Random.Range(5f, 8f));
		targetPos.x = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.2f, 0.8f), 0f)).x;
		dir = (targetPos - rgbd.position).normalized;
		birdInitialized = true;
	}

	public void initKingFisher(){
		StartCoroutine(initBird());
	}
}
