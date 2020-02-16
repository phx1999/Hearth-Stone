using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemySkill : MonoBehaviour
{
    public Image FNImg;
    public GameObject ZHImg;
    public GameObject script;
    // Start is called before the first frame update
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
        //if (Game._instance.script.GetComponent<Connection>().own_attacked_card_num != -1)
        //{
        // StartCoroutine(FengNuDelayCard());
        //}
        //else
        //{
        // StartCoroutine(FengNuDelay());
        //}

    }

    IEnumerator FengNuThen()
    {
        yield return new WaitForSeconds(2.5f);
        FNImg.GetComponent<Image>().enabled = false;
        print("风怒特效结束");
    }

    IEnumerator FengNuDelay()
    {
        yield return new WaitForSeconds(2.5f);
        FNImg.GetComponent<Image>().enabled = false;
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = Game._instance.enemy_attack_card.transform.DOMove(Game._instance.own_attacked_card.transform.position, 0.1f);
        Tweener m2 = Game._instance.enemy_attack_card.transform.DOMove(Game._instance.enemy_attack_card.transform.position, 0.4f);
        sequence.Append(m1);
        sequence.Append(m2);
        Game._instance.PlayerImg.GetComponent<Player>().Hurt(Game._instance.enemy_attack_card.GetComponent<Card>().Attack);
        Debug.Log("我方英雄被攻击第二次了");
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
        GameObject.Find("ImageEnemy").GetComponent<Enemy>().Blood(2);
        ZHImg.SetActive(false);
    }

    public void TaoKeZiXue()
    {
        Debug.Log("敌方发动逃课自学,目标为：" + (Game._instance.own_magic_target.GetComponent<Card>().index));
        Game._instance.own_magic_target.GetComponent<Card>().addAtt(1);
        Game._instance.own_magic_target.GetComponent<Card>().decreaseHp(1);
        if (Game._instance.own_magic_target.GetComponent<Card>().Hp <= 0)
        {
            Game._instance.ZhuoPaiArray.Remove(Game._instance.own_magic_target.transform.parent);
            Destroy(Game._instance.own_magic_target.transform.parent.gameObject);
        }
    }

    public void MianKao()
    {
        Debug.Log("敌方发动免考,目标为：" + (Game._instance.own_magic_target.GetComponent<Card>().index));
        Destroy(Game._instance.own_magic_target.transform.parent.gameObject);
        Game._instance.ZhuoPaiArray.Remove(Game._instance.own_magic_target.transform.parent);
    }

    public void QingJiaoLaoShi()
    {
        Debug.Log("请教老师");
        Game._instance.own_magic_target.GetComponent<Card>().decreaseAtt(2);
        Game._instance.own_magic_target.GetComponent<Card>().decreaseHp(2);
        if (Game._instance.own_magic_target.GetComponent<Card>().Hp <= 0)
        {
            Game._instance.ZhuoPaiArray.Remove(Game._instance.own_magic_target.transform.parent);
            Destroy(Game._instance.own_magic_target.transform.parent.gameObject);
        }
    }

    public void QingJiaoTongXue()
    {
        Debug.Log("请教同学");
        Game._instance.own_magic_target.GetComponent<Card>().decreaseAtt(1);
        Game._instance.own_magic_target.GetComponent<Card>().decreaseHp(1);
        if (Game._instance.own_magic_target.GetComponent<Card>().Hp <= 0)
        {
            Game._instance.ZhuoPaiArray.Remove(Game._instance.own_magic_target.transform.parent);
            Destroy(Game._instance.own_magic_target.transform.parent.gameObject);
        }
    }

    public void TuiKe()
    {
        Debug.Log("退课");
        //发报文让自己重新抽一张与被选择的随从序号相同的牌
        Game._instance.buildAdditionalCard(Game._instance.own_magic_target.GetComponent<Card>().index);//卡牌编号由服务器决定
        Destroy(Game._instance.own_magic_target.transform.parent.gameObject);
        Game._instance.ZhuoPaiArray.Remove(Game._instance.own_magic_target.transform.parent);
    }
}
