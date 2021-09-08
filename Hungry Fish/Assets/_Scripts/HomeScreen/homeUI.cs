using UnityEngine;
using UnityEngine.UI;

public class homeUI : MonoBehaviour {
    public AudioSource menuMusicSource;
    public Image musicBtn;
    public Sprite[] btnStates;

    void Start(){
        persistentData.Instance.gameOver = false;
        if(persistentData.Instance.soundOn){
            menuMusicSource.Play();
            musicBtn.sprite = btnStates[0];
        }else
        {
            musicBtn.sprite = btnStates[1];            
        }
    }

    // Nutton Methods

    public void loadFB(){
        Application.OpenURL("https://www.facebook.com/KemothStudios/");
    }

    public void moreGames(){
        Application.OpenURL("https://play.google.com/store/apps/developer?id=Kemoth+Studios");
    }

    public void rateUs(){
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.kemothstudios.hungryfish");
    }

    public void startStory(){
        persistentData.Instance.writeFile(persistentData.Instance.bestScore.ToString());
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }

    public void toggleSound(){
        persistentData.Instance.soundOn = !persistentData.Instance.soundOn;
        if(persistentData.Instance.soundOn){
            musicBtn.sprite = btnStates[0];
            menuMusicSource.Play();
        }else
        {
            musicBtn.sprite = btnStates[1];
            menuMusicSource.Stop();
        }
    }

    public void loadLeaderboard(){
        persistentData.Instance.showLeaderboard();
    }

    // **************
}
