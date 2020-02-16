using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public int Hp = 30;
    public int MaxHp = 30;
    public int MinHp = 0;
    public Text HpText;
    public Image HurtImg;
    public Text HurtText;
    float time = 0;

    //public GameObject LosePanel;
    public GameObject csWin;
    public GameObject jrWin;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HpText.text = Hp.ToString();
        if (HurtImg.enabled == true)
        {
            time += Time.deltaTime;
            if (time > 1f)
            {
                HurtImg.enabled = false;
                HurtText.enabled = false;
                time = 0;
            }
        }
    }

    public void Hurt(int Att)
    {
        Sequence sequence = DOTween.Sequence();
        Tweener r1 = gameObject.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
        Tweener r2 = gameObject.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
        Tweener r3 = gameObject.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
        sequence.Append(r1);
        sequence.Append(r2);
        sequence.Append(r3);
        Hp -= Att;
        HurtImg.enabled = true;
        HurtText.enabled = true;
        HurtText.text = "-" + Att.ToString();
        if (Hp <= MinHp)
        {
            Hp = MinHp;
            //LosePanel.SetActive(true);
            //LosePanel.GetComponent<Animator>().enabled = true;
            //GameObject.Find("LoseImage").GetComponent<Image>().sprite = Game._instance.PlayerImg.sprite;
            //Camera.main.GetComponent<AudioSource>().Stop();
            //GameObject.Find("LosePanel").GetComponent<AudioSource>().Play();
            if (Game._instance.hero_path == "jr")
            {
               StartCoroutine(Game._instance.csWinShow());
            }
            else if(Game._instance.hero_path == "cs")
            {
               StartCoroutine(Game._instance.jrWinShow());
            }
        }
        
    }

    public void Blood(int blood)
    {
        Hp += blood;
        if (Hp >= MaxHp)
        {
            Hp = MaxHp;
        }
    }

}
