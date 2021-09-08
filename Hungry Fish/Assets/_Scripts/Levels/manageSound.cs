using UnityEngine;

public class manageSound : MonoBehaviour {
	public AudioSource fishEating, bgMusic, spiderEating, gameOver, fishIn, fishOut, kingIn, kingOut, highScore;

	void Start(){
		persistentData.Instance.mSound = null;
		persistentData.Instance.mSound = this;
		if(persistentData.Instance.soundOn){
			bgMusic.Play();
		}
	}

	public void playFish(){
		if(persistentData.Instance.soundOn){
			fishEating.Play();
		}
	}

	public void playSpider(){
		if(persistentData.Instance.soundOn){
			spiderEating.Play();
		}
	}

	public void playFishIn(){
		if(persistentData.Instance.soundOn){
			fishIn.Play();
		}
	}

	public void playFishOut(){
		if(persistentData.Instance.soundOn){
			fishOut.Play();
		}
	}

	public void playKingIn(){
		if(persistentData.Instance.soundOn){
			kingIn.Play();
		}
	}

	public void playKingOut(){
		if(persistentData.Instance.soundOn){
			kingOut.Play();
		}
	}

	public void playGameOver(){
		if(persistentData.Instance.soundOn){
			if(bgMusic.isPlaying){
				bgMusic.Stop();
			}
			gameOver.Play();
		}
	}

	public void playHighScore(){
		if(persistentData.Instance.soundOn){
			highScore.Play();
		}
	}
}
