using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VP : MonoBehaviour {

    public VideoPlayer vp;
    public AudioSource audioSource;
    float time = 0;
    public GameObject Btn;
    public Image ChooseHeroImg;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        time += Time.deltaTime;
        if(time>=46)
        {
            vp.Stop();
            audioSource.Stop();
        }

	}

    public void Stop()
    {
        vp.Stop();
        audioSource.Stop();
        Btn.SetActive(false);
    }

    public void PlayerGame()
    {
        //Debug.Log("开始游戏！");
        ChooseHeroImg.transform.DOMoveX(0, 1);
    }

    public void LoadScence()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync("Game");
    }

}
