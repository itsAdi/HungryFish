using UnityEngine;
using System.Collections;
using DragonBones;

public class handleFish : MonoBehaviour {
	public handleJoystick joystickInstance;
	public Rigidbody2D selfRgbd;
	public UnityArmatureComponent fishInstanse;

	private Vector2 fishDirection, currentVel, lastPos;
	private bool canMoveFish, pushFish;
	private float totalTime, currentTime, rotTime, lastRotation, targetRotation, targetPositiveRotation, lastPositiveRotation, fromVelMultiplier, currentVelMultiplier, speed;
	private int rotDir;

	void Start(){
		joystickInstance.RegisterFish += moveFish;
		canMoveFish = true;
		totalTime = 0.5f;
		rotTime = 0.2f;
		lastPos = selfRgbd.position;
	}

	void Update(){
		if(!persistentData.Instance.gameOver && !persistentData.Instance.fishKilled){
			if((fishInstanse.transform.rotation.eulerAngles.z < 90f && fishInstanse.transform.rotation.eulerAngles.z >= 0f) || (fishInstanse.transform.rotation.eulerAngles.z <= 360f && fishInstanse.transform.rotation.eulerAngles.z > 270f)){
				if(fishInstanse.armature.flipY){
					fishInstanse.armature.flipY = false;
				}
			}else
			{
				if(!fishInstanse.armature.flipY){
					fishInstanse.armature.flipY = true;
				}
			}
		}else
		{
			if (gameObject.activeInHierarchy)
			{
				gameObject.SetActive(false);
			}
		}
	}

	void FixedUpdate(){
		if(!persistentData.Instance.gameOver && !persistentData.Instance.fishKilled){
			if((selfRgbd.position.y > persistentData.Instance.waterInstance.waterLevel && canMoveFish) || (selfRgbd.position.y < persistentData.Instance.waterInstance.waterLevel && !canMoveFish)){
				toggleFishBodyType();
			}
			if(canMoveFish){
				currentVel = (selfRgbd.position - lastPos) / Time.fixedDeltaTime;
				if(selfRgbd.gravityScale == 1f){
					persistentData.Instance.waterInstance.Splash(selfRgbd.position.x, currentVel.y * 1f / 40f);
					selfRgbd.gravityScale = 0f;
				}									
				currentTime += Time.deltaTime;
				if(currentTime >= totalTime){
					currentTime = totalTime;
				}
				float t = currentTime / totalTime;
				if(fishDirection != Vector2.zero){
					t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
					currentVelMultiplier = Mathf.Lerp(fromVelMultiplier, speed, t);
				}else{
					t = Mathf.Sin(t * Mathf.PI * 0.5f);
					currentVelMultiplier = Mathf.Lerp(fromVelMultiplier, 0f, t);
				}
				Vector2 rightDir = selfRgbd.gameObject.transform.right;
				selfRgbd.MovePosition(selfRgbd.position + (rightDir * currentVelMultiplier * Time.deltaTime));
				if(rotTime < 0.2f){
					rotTime += Time.deltaTime;
					if(rotTime >= 0.2f){
						rotTime = 0.2f;
					}
					float k = rotTime / 0.2f;
					k = Mathf.Sin(k * Mathf.PI * 0.5f);
					selfRgbd.MoveRotation(Mathf.Lerp(rotDir == 0 ? lastRotation : lastPositiveRotation, rotDir == 0 ? targetRotation : targetPositiveRotation, k));
				}
			}else
			{
				if(pushFish){
					selfRgbd.gravityScale = 1f;
					pushFish = false;
					selfRgbd.velocity = Vector2.zero;
					selfRgbd.AddForce(currentVel * 1.2f, ForceMode2D.Impulse);
					persistentData.Instance.waterInstance.Splash(selfRgbd.position.x, currentVel.y * 1f / 40f);
				}
				Vector2 velRef = selfRgbd.velocity;
				selfRgbd.MoveRotation(Mathf.Atan2(velRef.y, velRef.x) * Mathf.Rad2Deg);
				fishInstanse.animation.Stop();
			}
			lastPos = selfRgbd.position;
		}
	}

	void OnParticleCollision(GameObject other){
		persistentData.Instance.increaseScore(persistentData.Instance.scorePerInsect);
		persistentData.Instance.barSeekPos = Mathf.Clamp(persistentData.Instance.barSeekPos + 0.05f, 0f, 1f);
		persistentData.Instance.mSound.playFish();
	}

	void moveFish(Vector2 dir, float multiplier){
		speed = 8.5f * multiplier;
		if(dir != Vector2.zero){
			if(fishDirection == Vector2.zero){
				fromVelMultiplier = currentVelMultiplier;		
				currentTime = 0f;
			}
			fishDirection = dir;
			setRotation();
		}else{
			fishDirection = dir;
			fromVelMultiplier = currentVelMultiplier;		
			currentTime = 0f;
		}
	}

	void toggleFishBodyType(){
		if (selfRgbd.gravityScale == 1f)
		{
			persistentData.Instance.mSound.playFishIn();
			fishInstanse.animation.Play("hero_swim", 0);
			rotTime = 0.2f;
			Vector2 refVel = (selfRgbd.position - lastPos) / Time.deltaTime;
			targetRotation = Mathf.Atan2(refVel.y, refVel.x) * Mathf.Rad2Deg;
			if(fishDirection != Vector2.zero){
				setRotation();
				currentTime = totalTime;
			}else
			{
				currentTime = 0f;
			}
			canMoveFish = true;
		}else
		{
			persistentData.Instance.mSound.playFishOut();
			canMoveFish = false;
			pushFish = true;
		}
	}

	void setRotation(){
		rotTime = 0f;
		lastRotation = targetRotation;
		targetRotation = Mathf.Atan2(fishDirection.y, fishDirection.x) * Mathf.Rad2Deg;
		rotDir = 0;
		if(lastRotation > 90f){
			if(targetRotation < -90f){
				rotDir = 1;
				targetPositiveRotation = 360f - Mathf.Abs(targetRotation);
				lastPositiveRotation = lastRotation;
			}
		}

		if(lastRotation < -90f){
			if(targetRotation > 90f){
				rotDir = 1;
				targetPositiveRotation = targetRotation;
				lastPositiveRotation = 360f - Mathf.Abs(lastRotation);
			}
		}
	}
}
