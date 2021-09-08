using UnityEngine;

public class handleDifficulty : MonoBehaviour {
    public handleKingfisher[] kingfisher;
    public handlePurpleFish[] purpleFish;
    public handleSpider[] spider;

    private int difficultyLevel;

    void Start(){
        persistentData.Instance.difficulty = null;
        persistentData.Instance.difficulty = this;
        for (int i = 0; i <= persistentData.Instance.difficultyLevel; i++)
        {
            upgradeDifficulty(i);
        }
    }

    public void upgradeDifficulty(int withLevel){
        switch (withLevel)
        {
            case 0:
            purpleFish[1].initFish();
            purpleFish[3].initFish();
            break;

            case 1:
            spider[0].initSpider();
            break;

            case 2:
            kingfisher[0].initKingFisher();
            break;

            case 3:
            purpleFish[0].initFish();
            purpleFish[5].initFish();
            break;

            case 4:
            spider[1].initSpider();
            kingfisher[1].initKingFisher();
            break;

            case 5:
            purpleFish[4].initFish();
            purpleFish[2].initFish();
            break;
        }
    }
}
