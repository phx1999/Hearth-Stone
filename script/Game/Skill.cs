using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Skill : MonoBehaviour
{

    public Image FNImg;
    public GameObject ZHImg;
    public Text CFText;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FengNu()
    {
        FNImg.GetComponent<Image>().enabled = true;
        RectTransform rt = FNImg.rectTransform;//获取透明度
        Color c = FNImg.color;
        c.a = 0;
        FNImg.color = c;
        Sequence sequence = DOTween.Sequence();
        Tweener a1 = FNImg.DOColor(new Color(c.r, c.g, c.b, 1), 1);//风怒图出现
        Tweener a2 = FNImg.DOColor(new Color(c.r, c.g, c.b, 0), 1);
        sequence.Append(a1);
        sequence.AppendInterval(0.5f);
        sequence.Append(a2);
        StartCoroutine(FengNuThen());
        //StartCoroutine(FengNuDelay());
    }

    public void ZhanHong()
    {
        ZHImg.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        Tweener s1 = ZHImg.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.5f);
        Tweener s2 = ZHImg.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        sequence.Append(s1);
        sequence.AppendInterval(0.3f);
        sequence.Append(s2);
        StartCoroutine(ZhanHongDelay());
    }



    IEnumerator ZhanHongDelay()
    {
        yield return new WaitForSeconds(1.3f);
        GameObject.Find("ImagePlayer").GetComponent<Player>().Blood(2);
        ZHImg.SetActive(false);
    }

    IEnumerator FengNuThen()
    {
        yield return new WaitForSeconds(2.5f);
        FNImg.GetComponent<Image>().enabled = false;
        //Game._instance.Aggressor.GetComponent<Card>().FengNuAlready = true;
        //print("风怒结束。FengNuAlready:"+Game._instance.Aggressor.GetComponent<Card>().FengNuAlready);
    }

    IEnumerator FengNuDelay()
    {
        yield return new WaitForSeconds(2.5f);
        FNImg.GetComponent<Image>().enabled = false;
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = Game._instance.Aggressor.transform.DOMove(Game._instance.ToAggressor.transform.position, 0.1f);
        Tweener m2 = Game._instance.Aggressor.transform.DOMove(Game._instance.Aggressor.transform.position, 0.4f);

        sequence.Append(m1);
        sequence.Append(m2);
        yield return new WaitForSeconds(0.5f);
        if (Game._instance.ToAggressor.tag == "Enemy")
        {
            Game._instance.ToAggressor.GetComponent<Enemy>().Hurt(Game._instance.Aggressor.GetComponent<Card>().Attack);
        }

        else if (Game._instance.ToAggressor.tag == "EnemyCard")
        {
            Tweener r1 = Game._instance.ToAggressor.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
            Tweener r2 = Game._instance.ToAggressor.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
            Tweener r3 = Game._instance.ToAggressor.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
            sequence.Append(r1);
            sequence.Append(r2);
            sequence.Append(r3);

            Game._instance.ToAggressor.GetComponent<Card>().CardHurt(Game._instance.Aggressor.GetComponent<Card>().Attack);
            if (Game._instance.ToAggressor.GetComponent<Card>().Hp <= 0)
            {
                Destroy(Game._instance.ToAggressor);
                Game._instance.EnemyZhuoArray.Remove(Game._instance.ToAggressor.transform);
            }
            Game._instance.Aggressor.GetComponent<Card>().CardHurt(Game._instance.ToAggressor.GetComponent<Card>().Attack);
            if (Game._instance.Aggressor.GetComponent<Card>().Hp <= 0)
            {
                Destroy(Game._instance.Aggressor.transform.gameObject);
                Game._instance.ZhuoPaiArray.Remove(Game._instance.Aggressor.transform.parent);
            }
            Game._instance.Aggressor.tag = "AlreadyAtt";
            Debug.Log("记录攻击第二次了");
            StartCoroutine(GetComponent<Card>().release());
        }


    }

    public void TaoKeZiXue()
    {
        Debug.Log("逃课自学");
        Game._instance.magicTarget.GetComponent<Card>().addAtt(1);
        Game._instance.magicTarget.GetComponent<Card>().decreaseHp(1);
        if (Game._instance.magicTarget.GetComponent<Card>().Hp <= 0)
        {
            Game._instance.EnemyZhuoArray.Remove(Game._instance.magicTarget.transform);
            Destroy(Game._instance.magicTarget);
        }
        Destroy(Game._instance.magicCard.transform.parent.gameObject);
        Debug.Log("销毁法术牌");
    }

    public void MianKao()
    {
        Debug.Log("免考");
        Destroy(Game._instance.magicTarget);
        Game._instance.EnemyZhuoArray.Remove(Game._instance.magicTarget.transform);
        Destroy(Game._instance.magicCard.transform.parent.gameObject);
        Debug.Log("销毁法术牌");
    }

    public void QingJiaoLaoShi()
    {
        Debug.Log("请教老师");
        Game._instance.magicTarget.GetComponent<Card>().decreaseAtt(2);
        Game._instance.magicTarget.GetComponent<Card>().decreaseHp(2);
        if (Game._instance.magicTarget.GetComponent<Card>().Hp <= 0)
        {
            Game._instance.EnemyZhuoArray.Remove(Game._instance.magicTarget.transform);
            Destroy(Game._instance.magicTarget);
        }
        Destroy(Game._instance.magicCard.transform.parent.gameObject);
        Debug.Log("销毁法术牌");
    }

    public void QingJiaoTongXue()
    {
        Debug.Log("请教同学");
        Game._instance.magicTarget.GetComponent<Card>().decreaseAtt(1);
        Game._instance.magicTarget.GetComponent<Card>().decreaseHp(1);
        if (Game._instance.magicTarget.GetComponent<Card>().Hp <= 0)
        {
            Game._instance.EnemyZhuoArray.Remove(Game._instance.magicTarget.transform);
            Destroy(Game._instance.magicTarget);
        }
        Destroy(Game._instance.magicCard.transform.parent.gameObject);
        Debug.Log("销毁法术牌");
    }

    public void TuiKe()
    {
        Debug.Log("退课");
        //发报文让对方重新抽一张与被选择的随从序号相同的牌
        Destroy(Game._instance.magicTarget);
        Game._instance.EnemyZhuoArray.Remove(Game._instance.magicTarget.transform);
        Destroy(Game._instance.magicCard.transform.parent.gameObject);
        Debug.Log("销毁法术牌");
    }

}
