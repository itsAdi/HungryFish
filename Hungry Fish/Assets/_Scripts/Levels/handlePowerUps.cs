using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class handlePowerUps : MonoBehaviour {
	public Transform[] powerUpTransforms; // 0: Life    1: Shield    2: scoreMultiplier
	public Rigidbody2D fishPos;

	private List<int> cumulativeProbability;
	private int powerUpIndex;
	private Vector2 minPos, maxPos;
	private IEnumerator hpuCoroutine;

	void Start(){
		minPos = Camera.main.ScreenToWorldPoint(new Vector3(-(Camera.main.pixelWidth / 2f) + 20f, 20f, 0f));
		maxPos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth + (Camera.main.pixelWidth / 2f) - 20f, Camera.main.pixelHeight - 20f, 0f));
		cumulativeProbability = new List<int>();
		cumulativeProbability.Add(2); // Life
		cumulativeProbability.Add(28); // Shield
		cumulativeProbability.Add(70); // ScoreMultiplier
		for (int i = 1; i < cumulativeProbability.Count; i++)
		{
			cumulativeProbability[i] += cumulativeProbability[i - 1];
		}
		StartCoroutine(enumeratePowerUp());
	}

	void Update(){
		if(powerUpIndex != -1){
			if(Vector3.Distance(fishPos.position, powerUpTransforms[powerUpIndex].position) < 0.7f){
				powerUpTransforms[powerUpIndex].localPosition = Vector3.zero;
				StopCoroutine(hpuCoroutine);
				switch (powerUpIndex)
				{
					case 0:
					persistentData.Instance.hUI.toggleSkill(powerUpIndex);
					StartCoroutine(enumeratePowerUp());
					break;
					default:
					StartCoroutine(toggleSkill(5f));
					break;
				}
			}
		}
	}

	void showPowerUp(){
		int r = (int)Mathf.Floor(Random.Range(0f, 10f) * 10f);
		foreach (int item in cumulativeProbability)
		{
			powerUpIndex++;
			if(r <= cumulativeProbability[powerUpIndex]){
				Vector3 tempPos = new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
				powerUpTransforms[powerUpIndex].position = tempPos;
				break;
			}
		}
	}

	IEnumerator enumeratePowerUp(){
		int lastLife = persistentData.Instance.lives;
		yield return new WaitForSecondsRealtime(5f);
		if(lastLife == persistentData.Instance.lives){
			powerUpIndex = -1;
			showPowerUp();
			hpuCoroutine = hidePowerUp();
			StartCoroutine(hpuCoroutine);
		}
	}

	IEnumerator hidePowerUp(){
		int lastLife = persistentData.Instance.lives;
		yield return new WaitForSecondsRealtime(8f);
		if(lastLife == persistentData.Instance.lives){
			powerUpTransforms[powerUpIndex].localPosition = Vector3.zero;
			powerUpIndex = -1;
			StartCoroutine(enumeratePowerUp());
		}
	}

	IEnumerator toggleSkill(float withTime){
		persistentData.Instance.hUI.toggleSkill(powerUpIndex);
		int lastLife = persistentData.Instance.lives;
		yield return new WaitForSecondsRealtime(withTime);
		if(lastLife == persistentData.Instance.lives){
			persistentData.Instance.hUI.toggleSkill(powerUpIndex);
			StartCoroutine(enumeratePowerUp());
		}
	}
}
