using UnityEngine;
using google.service.game;
using admob;
//using UnityEngine.Advertisements;
using MiniJSON;
using System.IO;
using System.Collections.Generic;

public class persistentData : MonoBehaviour {
	[HideInInspector]
	public bool gameOver, fishKilled, soundOn, shieldEnabled;
	[HideInInspector]
	public int lives, score, difficultyLevel = -1, bestScore, scorePerInsect;
	[HideInInspector]
	public float barSeekPos;

	[HideInInspector]
	public handleUI hUI;
	[HideInInspector]
	public waterManager waterInstance;
	[HideInInspector]
	public handleDifficulty difficulty;
	[HideInInspector]
	public manageSound mSound;
	public static persistentData Instance;

	private Admob gameAds;
	private int intstlAdLoadTry, rwrdAdLoadTry, maxLives;

	public delegate void scoreDel(int score);
	public scoreDel OnScore;

	private bool adCompleted;

	void Awake(){
		if(Instance == null){
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	void Start(){
		GoogleGame.Instance().login(false, false);
		//Advertisement.Initialize("1674222");
		lives = 3;
		scorePerInsect = 10;
		barSeekPos = 1f;
		gameAds = Admob.Instance();
		//gameAds.setTesting(true);
		gameAds.interstitialEventHandler += OnInterstitial;
		gameAds.rewardedVideoEventHandler += onReward;
		gameAds.loadRewardedVideo("ca-app-pub-8347424776413444/1638514410");
		gameAds.initAdmob("","ca-app-pub-8347424776413444/9291662827");
		gameAds.loadInterstitial();
		try
		{
			if(!File.Exists(Application.persistentDataPath + "/KS_gameData.json"))
			{
				File.Create(Application.persistentDataPath + "/KS_gameData.json").Dispose();
				File.WriteAllText(Application.persistentDataPath + "/KS_gameData.json", "{\"best\":0, \"sound\":1}");
				soundOn = true;
				bestScore = -1;
			}else{
				string fetchedData = File.ReadAllText(Application.persistentDataPath + "/KS_gameData.json");
				var best = Json.Deserialize(fetchedData) as Dictionary<string, object>;
				int.TryParse(best["best"].ToString(), out bestScore);
				int tempSound;
				int.TryParse(best["sound"].ToString(), out tempSound);
				soundOn = tempSound == 0 ? false : true;
			}
		}
		catch
		{
			Debug.LogError("Assessing JSON failed");
			throw;
		}
	}

	public void increaseScore(int byScore){
		if(!gameOver){
			OnScore(byScore);
		}
	}

	void OnInterstitial(string eventName, string msg){
		switch (eventName)
		{
			case "onAdClosed":
				gameAds.loadInterstitial();
				UnityMainThreadDispatcher.Instance().Enqueue(() => {
					UnityEngine.SceneManagement.SceneManager.LoadScene(0);
				});
			break;
			case "onAdFailedToLoad":
				intstlAdLoadTry++;
				if (intstlAdLoadTry < 4)
				{
					gameAds.loadInterstitial();					
				}
			break;
		}
	}

	void onReward(string eventName, string msg){
		if(string.Equals(eventName, AdmobEvent.onAdFailedToLoad)){
			rwrdAdLoadTry++;
			if(rwrdAdLoadTry < 4){
				gameAds.loadRewardedVideo("ca-app-pub-8347424776413444/1638514410");
			}
		}
		if(string.Equals(eventName, AdmobEvent.onAdClosed)){
			if(adCompleted){
				gameAds.loadRewardedVideo("ca-app-pub-8347424776413444/1638514410");
				adCompleted = false;
				lives++;
				UnityMainThreadDispatcher.Instance().Enqueue(()=>{
					gameOver = false;
					UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
				});
			}else
			{
				showError("Video Skipped", "Extra life could not be rewarded");
			}
		}
		if(string.Equals(eventName, AdmobEvent.onRewarded)){
			adCompleted = true;
		}
	}

	// void OnUnityReward(ShowResult res){
	// 	switch (res)
	// 	{
	// 		case ShowResult.Finished:
	// 			 lives++;
	// 			 gameOver = false;
	// 			 UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	// 		break;
	// 		case ShowResult.Skipped:
	// 			 showError("Video Skipped", "Extra life could not be rewarded");
	// 		break;
	// 		case ShowResult.Failed:
	// 			showError("Sorry", "Could not load reward video");					
	// 		break;
	// 	}
	// }

	// void OnUnityInterstitial(ShowResult res){
	// 	UnityEngine.SceneManagement.SceneManager.LoadScene(0);		
	// }

	void showError(string msgTitle, string withMsg){
		hUI.panelTitle.text = msgTitle;
		hUI.showMsgBox(msgClbk, withMsg, false, false, true, false);
	}

	void msgClbk(char r){
		showIntr();
	}
	public int MaxLives{
		get{ return maxLives; }
		set{ maxLives = value;}
	}

	public bool handleLives(){
		lives = Mathf.Clamp(--lives, 0, maxLives);
		OnScore = null;
		hUI = null;
		return lives == 0 ? true : false;
	}

	public bool isRewardVideoReady(){
		if(gameAds.isInterstitialReady()){
			return true;
		}/*else
		{
			if(Advertisement.isSupported){
				if(Advertisement.isInitialized){
					if(Advertisement.IsReady("rewardedVideo")){
						return true;
					}
				}
			}
		}*/
		return false;
	}

	public void showReward(){
		
		if(gameAds.isInterstitialReady()){
			gameAds.showRewardedVideo();
		}/*else
		{
			ShowOptions opt = new ShowOptions();
			opt.resultCallback = OnUnityReward;
			Advertisement.Show("rewardedVideo", opt);
		}*/
	}

	public void showIntr(){
		hUI = null;
		lives = maxLives;
		bestScore = score;
		score = 0;
		scorePerInsect = 10;
		barSeekPos = 1f;
		OnScore = null;
		difficultyLevel = -1;
		if(gameAds.isInterstitialReady()){
			gameAds.showInterstitial();
		}else{
			UnityEngine.SceneManagement.SceneManager.LoadScene(0); // Remove this if enabling unity ads ******************************************************************************* 
			/*if(Advertisement.isSupported){
				if(Advertisement.isInitialized){
					if (Advertisement.IsReady("video"))
					{
						ShowOptions opt = new ShowOptions();
						opt.resultCallback = OnUnityInterstitial;
						Advertisement.Show("video", opt);
					}else
					{
						UnityEngine.SceneManagement.SceneManager.LoadScene(0);					
					}
				}else
				{
					UnityEngine.SceneManagement.SceneManager.LoadScene(0);					
				}
			}else
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			}*/
		}
	}

	public void showLeaderboard(){
		if(GoogleGame.Instance().isConnected()){
			GoogleGame.Instance().showLeaderboard("CgkIuYzauO4CEAIQAA");
		}
	}

	public void writeFile(string withSCore){
		try{
			if(File.Exists(Application.persistentDataPath + "/KS_gameData.json"))
			{
				File.WriteAllText(Application.persistentDataPath + "/KS_gameData.json", "{\"best\":" + withSCore + ", \"sound\":" + (soundOn == true ? "1" : "0") + "}");
			}
			if(GoogleGame.Instance().isConnected()){
				long tempScore;
				long.TryParse(withSCore, out tempScore);
				GoogleGame.Instance().submitLeaderboardScore("CgkIuYzauO4CEAIQAA", tempScore);
			}
		}catch{
			throw;
		}
	}
}
