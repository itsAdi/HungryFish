using UnityEngine;
using UnityEngine.EventSystems;

public class handleJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler{
	public RectTransform joyStick;
	public delegate void fishDel(Vector2 dir, float speedMultiplier);
	public event fishDel RegisterFish;

	private RectTransform selfRect;
	private Vector2 screenPos, basePos;

	void Start(){
		selfRect = gameObject.GetComponent<RectTransform>();
		basePos = selfRect.sizeDelta / 2f * -1f;
	}

	void Update(){
		if(basePos == Vector2.zero){
			basePos.x = selfRect.sizeDelta.x / 2f * -1f;
			basePos.y = selfRect.sizeDelta.y / 2f;
		}
	}

	public void OnPointerDown(PointerEventData e){
	}


	public void OnDrag(PointerEventData e){
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRect, e.position, e.pressEventCamera, out screenPos)){
			setJoystickPos();
		}
	}

	public void OnPointerUp(PointerEventData e){
		joyStick.anchoredPosition = Vector2.zero;
		if(RegisterFish != null){
			RegisterFish(Vector2.zero, 0f);
		}
	}

	void setJoystickPos(){
		float dist = Mathf.Clamp(Vector2.Distance(screenPos, basePos), 0f, 125f);
		joyStick.anchoredPosition = (screenPos - basePos).normalized * dist;
		//joyStick.anchoredPosition = basePos;
		if(RegisterFish != null){
			RegisterFish((joyStick.anchoredPosition - Vector2.zero).normalized, dist / 125f);
		}
	}
}
