using UnityEngine;
using System.Collections;

public class handleSpider : MonoBehaviour {
	public Transform spiderTransform;
	public GameObject spiderGrab;
	public GameObject animatedSpider;

	private Vector2[] webLinePoints;
	private Material webLineMat;
	private bool moveDown, moveUp, initialized, canEat;
	private Rigidbody2D rgbd;
	private float finalY;

	void FixedUpdate(){
		if(moveDown){
			if(rgbd.position.y >= finalY){
				rgbd.MovePosition(rgbd.position + (Vector2.up * -3f * Time.deltaTime));
			}else{
				moveDown = false;
				canEat = true;
			}
		}
		if(moveUp){
			if(rgbd.position.y <= finalY){
				rgbd.MovePosition(rgbd.position + (Vector2.up * 3f * Time.deltaTime));
			}else{
				if(!persistentData.Instance.gameOver){
					StartCoroutine(moveSpider());
				}
				moveUp = false;
				canEat = false;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D fishColl){
		if(!persistentData.Instance.shieldEnabled && canEat && fishColl.gameObject.tag == "Fish"){
			spiderGrab.SetActive(true);
			animatedSpider.SetActive(false);
			StopCoroutine(moveSpider());
			finalY = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight + 300, 0f)).y;
			moveUp = true;
			persistentData.Instance.hUI.onKilled();
			persistentData.Instance.mSound.playSpider();
		}
	}

	void OnRenderObject(){
		if(initialized){
			webLinePoints[1].y = rgbd.position.y;
			GL.PushMatrix();
			webLineMat.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Color(Color.white);
			GL.Vertex(webLinePoints[0]);
			GL.Vertex(webLinePoints[1]);
			GL.End();
			GL.PopMatrix();
		}
	}

	IEnumerator moveSpider(){
		float rndX = Random.Range(0.2f, 0.8f);
		Vector3 tmpPOS = spiderTransform.position;
		tmpPOS.y = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight + 300, 0f)).y;
		tmpPOS.x = Camera.main.ViewportToWorldPoint(new Vector3(rndX, 0f, 0f)).x;
		webLinePoints[0].x = tmpPOS.x;
		webLinePoints[1].x = tmpPOS.x;
		spiderTransform.position = tmpPOS;
		float waitSec = Random.Range(1f,3f);
		yield return new WaitForSeconds(waitSec);
		finalY = Camera.main.ViewportToWorldPoint(new Vector3(0f, Random.Range(0.6f, 0.9f), 0f)).y;
		moveDown = true;
		waitSec = Random.Range(5f,8f);
		yield return new WaitForSeconds(waitSec);
		finalY = tmpPOS.y;
		moveUp = true;
	}

	public void initSpider(){
		webLineMat = new Material(Shader.Find("Hidden/Internal-Colored"));
		webLineMat.hideFlags = HideFlags.HideAndDontSave;
		webLineMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		webLineMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		webLineMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
		webLineMat.SetInt("_ZWrite", 0);
		webLinePoints = new Vector2[2];
		webLinePoints[0] = new Vector3(spiderTransform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight + 300, 0f)).y, -2f);
		webLinePoints[1] = new Vector3(spiderTransform.position.x, spiderTransform.position.y, -2f);
		rgbd = spiderTransform.gameObject.GetComponent<Rigidbody2D>();
		initialized = true;
		StartCoroutine(moveSpider());
	}
}
