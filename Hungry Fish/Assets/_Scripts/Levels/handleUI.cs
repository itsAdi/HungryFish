using UnityEngine;
using UnityEngine.UI;
using System;

public class handleUI : MonoBehaviour {
	[Serializable]
	public class uiRenderers{
		public Image[] uiImage;
	}

	public Image barMask;
	public Text scoreText;
	public GameObject[] lives;
	public uiRenderers overlayWindow;
	public Animator alertAnim, starvAnim, skillAnim;
	[Header("Pause and Gameover windows")]
	public uiRenderers pauseWindow;
	public uiRenderers gameoverWindow;
	[Header("Message Box")]
	public GameObject messageboxPanel;
	public Button[] mbBtns;
	public Text messageText;
	public Text panelTitle;
	[Header("Power Ups")]
	public Sprite[] powerUpSprite; // 0: Life    1: Shield    2: ScoreMultiplier
	public Image powerUpIcon;

	public delegate void mbListener(char res); // res -> 'y' : YES ... 'n' : NO ... 'o' : OK ... 'c' : CANCEL
	public mbListener OnMessageBoxDismissed;

	private int currentScore = 0;

	private float currentBarSeekPos;
	void Start(){
		persistentData.Instance.OnScore += updateScore;
		persistentData.Instance.hUI = this;
		currentScore = persistentData.Instance.score;
		scoreText.text = currentScore.ToString();
		persistentData.Instance.MaxLives = lives.Length;
		for (int i = 0; i < persistentData.Instance.lives; i++)
		{
			lives[i].SetActive(true);
		}
		powerUpIcon.enabled = false;
	}

	void Update(){
		if(!persistentData.Instance.fishKilled && !persistentData.Instance.gameOver){
			barMask.fillAmount = persistentData.Instance.barSeekPos;
			persistentData.Instance.barSeekPos = Mathf.Clamp(persistentData.Instance.barSeekPos - 0.0004f, 0f, 1f);
			if(persistentData.Instance.hUI != null && persistentData.Instance.barSeekPos == 0f){
				if(persistentData.Instance.lives > 1){
					starvAnim.SetTrigger("showStarv");
				}
				persistentData.Instance.hUI.onKilled();
			}
		}
	}

	void updateScore(int score){
		currentScore += score;
		scoreText.text = currentScore.ToString();
		persistentData.Instance.score = currentScore;
		if(persistentData.Instance.bestScore != -1 && currentScore > persistentData.Instance.bestScore){
			persistentData.Instance.bestScore = -1;
			persistentData.Instance.mSound.playHighScore();
			alertAnim.SetTrigger("alert");
		}
		if(currentScore == 20 || currentScore == 50 || currentScore == 100 || currentScore == 130 || currentScore == 150 || currentScore == 180){
			persistentData.Instance.difficulty.upgradeDifficulty(persistentData.Instance.difficultyLevel);
			persistentData.Instance.difficultyLevel++;
		}
	}

	public void showMsgBox(mbListener callback ,string withMessage, bool yesButton, bool noButton, bool okButton, bool cancelButton){
		messageText.text = withMessage;
		OnMessageBoxDismissed = callback;
		messageboxPanel.SetActive(true);
		if (yesButton)
		{
			mbBtns[0].gameObject.SetActive(true);
		}

		if (noButton)
		{
			mbBtns[1].gameObject.SetActive(true);
		}

		if (okButton)
		{
			mbBtns[2].gameObject.SetActive(true);
		}

		if (cancelButton)
		{
			mbBtns[3].gameObject.SetActive(true);
		}
	}

	void hideMsgBox(){
		for (int i = 0; i < mbBtns.Length; i++)
		{
			if(mbBtns[i].gameObject.activeInHierarchy){
				mbBtns[i].gameObject.SetActive(false);
			}
		}
		messageboxPanel.SetActive(false);
	}

	void fromGameover(char res){
		switch (res)
		{
			case 'o': Time.timeScale = 1f;
					persistentData.Instance.showIntr();
					break;
			case 'c':hideMsgBox();
					toggleGameoverWindow();
					break;
			case 'y':Application.Quit();
					break;
			case 'n':hideMsgBox();
					toggleGameoverWindow();
					break;
		}
	}

	void fromPause(char res){
		switch (res)
		{
			case 'o': Time.timeScale = 1f;
					persistentData.Instance.showIntr();
					break;
			case 'c':hideMsgBox();
					togglePauseWindow();
					break;
			case 'y':Application.Quit();
					break;
			case 'n':hideMsgBox();
					togglePauseWindow();
					break;
		}
	}

	void fromHome(char res){
		switch (res)
		{
			case 'y':
			persistentData.Instance.showReward();
			break;
			case 'n':
			persistentData.Instance.showIntr();
			break;
		}
	}

	void noAdFound(char res){
		hideMsgBox();
		toggleGameoverWindow();
	}

	void reloadScene(){
		persistentData.Instance.fishKilled = false;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	}

	void togglePauseWindow(){
		foreach (Image item in pauseWindow.uiImage)
		{
			item.enabled = !item.enabled;
		}
	}

	void  toggleGameoverWindow(){
		foreach (Image item in gameoverWindow.uiImage)
		{
			item.enabled = !item.enabled;
		}
	}

	public void onKilled(){
		persistentData.Instance.barSeekPos = 1f;
		persistentData.Instance.scorePerInsect = 10;
		persistentData.Instance.shieldEnabled = false;
		if(!persistentData.Instance.handleLives()){
			 Invoke("reloadScene", 2f);
			 persistentData.Instance.fishKilled = true;
		}else{
			lives[0].SetActive(false);
			panelTitle.text = "Game Over";
			overlayWindow.uiImage[0].enabled = !overlayWindow.uiImage[0].enabled;
			toggleGameoverWindow();
			persistentData.Instance.gameOver = true;
			persistentData.Instance.mSound.playGameOver();
		}
		persistentData.Instance.writeFile(currentScore.ToString());
	}

	public void loadHome(){
		if(gameoverWindow.uiImage[0].enabled){
			if(persistentData.Instance.isRewardVideoReady()){
				showMsgBox(fromHome, "You can earn a life and can continue. Do you want to continue ?", true, true, false, false);
				panelTitle.text = "Are you sure ?";
				toggleGameoverWindow();	
			}else
			{
				persistentData.Instance.showIntr();
			}		
		}
		if(pauseWindow.uiImage[0].enabled){
			showMsgBox(fromPause, "All session progress will be lost", false, false, true, true);
			panelTitle.text = "Are you sure ?";
			togglePauseWindow();			
		}
	}

	public void togglePause(){
		if(overlayWindow.uiImage[0].enabled){
			togglePauseWindow();
			overlayWindow.uiImage[0].enabled = false;
			Time.timeScale = 1f;
		}else
		{
			panelTitle.text = "Paused";
			togglePauseWindow();
			overlayWindow.uiImage[0].enabled = true;
			Time.timeScale = 0f;
		}
	}

	public void quitGame(){
		if(gameoverWindow.uiImage[0].enabled){
			showMsgBox(fromGameover, "Are you sure you want to quit ?", true, true, false, false);
			panelTitle.text = "Quit Game";
			toggleGameoverWindow();	
		}
		if(pauseWindow.uiImage[0].enabled){
			showMsgBox(fromPause, "Are you sure you want to quit ?", true, true, false, false);
			panelTitle.text = "Quit Game";
			togglePauseWindow();			
		}
	}

	public void getLife(){
		if(persistentData.Instance.isRewardVideoReady()){
			persistentData.Instance.showReward();
		}else
		{
			showMsgBox(noAdFound, "Video not available at this time", false, false, true, false);
			panelTitle.text = "Sorry";
			toggleGameoverWindow();	
		}
	}

	public void toggleSkill(int withIndex){
		switch (withIndex)
		{
			case 0:
			persistentData.Instance.lives = Mathf.Clamp(persistentData.Instance.lives + 1, 0, persistentData.Instance.MaxLives);
			lives[persistentData.Instance.lives - 1].SetActive(true);
			break;
			case 1:
			powerUpIcon.enabled = !powerUpIcon.enabled;
			if(powerUpIcon.enabled) skillAnim.SetTrigger("shootAnim");
			persistentData.Instance.shieldEnabled = powerUpIcon.enabled;
			powerUpIcon.sprite = powerUpSprite[withIndex];
			break;
			case 2:
			powerUpIcon.enabled = !powerUpIcon.enabled;
			if(powerUpIcon.enabled) skillAnim.SetTrigger("shootAnim");
			persistentData.Instance.scorePerInsect = powerUpIcon.enabled ? 20 : 10;
			powerUpIcon.sprite = powerUpSprite[withIndex];
			break;
		}
	}

	public void mbButton(string withRes){
		OnMessageBoxDismissed(withRes[0]);
	}
}
