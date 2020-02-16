using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class Card2 : MonoBehaviour, IPointerClickHandler
{
    GameObject go;
    bool SureBool;
    float time = 0;
    public int ConsumeNum;
    public int Attack;
    public int Hp;
    public Text HpText;

    public Image HurtImg;
    public Text HurtText;

    float TXtime = 0;
    float hurtTime = 0;
    //public int SkillType;

    public AudioSource ComeOnAS;
    public AudioSource SkillAS;
    public AudioSource AttAS;

    GameObject ZHImg;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag == "ShouPai")
        {
            for (int i = 0; i < Game._instance.ShouPaiArray.Count; i++)
            {
                Game._instance.ShouPaiArray[i].GetComponent<Image>().enabled = false;
                Game._instance.ShouPaiArray[i].GetChild(0).tag = "ShouPai";
            }

            go = eventData.pointerEnter;
            SureBool = true;
            go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 20, 0.2f);
            eventData.pointerEnter.transform.parent.GetComponent<Image>().enabled = true;
            go.tag = "ZB";
            time = 0;
        }
        if (eventData.pointerEnter.tag == "ZB" && time >= 0.5f &&
            ConsumeNum <= Game._instance.AtCryaralNum)
        {
            //go.transform.parent = GameObject.Find("Canvas").transform;
            Camera.main.GetComponent<Game>().ReduceCrystal(ConsumeNum);

            Sequence sequence = DOTween.Sequence();
            Tweener m1 = go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 297, 0.5f);
            Tweener m2 = go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 377, 0.5f);
            Tweener s1 = go.transform.parent.DOScale(new Vector3(1f, 0.8f, 1), 0.5f);
            Tweener s2 = go.transform.parent.DOScale(new Vector3(0.5f, 0.4f, 1), 0.5f);
            sequence.Append(m1);
            sequence.Join(s1);
            sequence.AppendInterval(0.5f);
            sequence.Append(m2);
            sequence.Join(s2);
            StartCoroutine(ComeCardDelay());
            SureBool = false;
            time = 0;
        }
        if (eventData.pointerEnter.tag == "ZB" && time >= 0.5f &&
            ConsumeNum > Game._instance.AtCryaralNum)
        {
            Game._instance.HintText.enabled = true;
        }

        if (eventData.pointerEnter.tag == "ZhuoPai")
        {
            Game._instance.Aggressor = eventData.pointerEnter;
        }
        if (eventData.pointerEnter.tag == "Enemy")
        {
            Game._instance.ToAggressor = eventData.pointerEnter;
            Sequence sequence = DOTween.Sequence();
            Tweener m1 = Game._instance.Aggressor.transform.DOMove(Game._instance.ToAggressor.transform.position, 0.1f);
            Tweener m2 = Game._instance.Aggressor.transform.DOMove(Game._instance.Aggressor.transform.position, 0.4f);
            Tweener r1 = Game._instance.EnemyImg.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
            Tweener r2 = Game._instance.EnemyImg.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
            Tweener r3 = Game._instance.EnemyImg.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
            sequence.Append(m1);
            sequence.Append(m2);
            sequence.Append(r1);
            sequence.Append(r2);
            sequence.Append(r3);
            StartCoroutine(DelayHurt());
            Game._instance.Aggressor.GetComponent<Card2>().AttAS.Play();
        }
        if (eventData.pointerEnter.tag == "EnemyCard")
        {
            Game._instance.ToAggressor = eventData.pointerEnter;
            Sequence sequence = DOTween.Sequence();
            Tweener m1 = Game._instance.Aggressor.transform.DOMove(Game._instance.ToAggressor.transform.position, 0.1f);
            Tweener m2 = Game._instance.Aggressor.transform.DOMove(Game._instance.Aggressor.transform.position, 0.4f);
            Tweener r1 = Game._instance.ToAggressor.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
            Tweener r2 = Game._instance.ToAggressor.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
            Tweener r3 = Game._instance.ToAggressor.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
            sequence.Append(m1);
            sequence.Append(m2);
            sequence.Append(r1);
            sequence.Append(r2);
            sequence.Append(r3);
            StartCoroutine(DelayHurtCard());
            Game._instance.Aggressor.GetComponent<Card2>().AttAS.Play();
        }
        if (eventData.pointerEnter.tag == "AlreadyAtt")
        {

            Game._instance.HintText.text = "该随从已经攻击过了";
            Game._instance.HintText.enabled = true;
        }

    }

    // Use this for initialization
    void Start()
    {
        ZHImg = GameObject.Find("ImagePlayer/ZhanHongTX");
    }

    // Update is called once per frame
    void Update()
    {
        if (SureBool == true)
        {
            time += Time.deltaTime;
        }
        if (go == null)
        {
            return;
        }
        if (go.tag == "ZhuoPai")
        {
            TXtime += Time.deltaTime;
            if (TXtime >= 2)
            {
                go.transform.parent.GetComponent<Image>().enabled = false;
            }
        }

        if (HurtImg.enabled == true)
        {
            hurtTime += Time.deltaTime;
            if (hurtTime > 1f)
            {
                HurtImg.enabled = false;
                HurtText.enabled = false;
                hurtTime = 0;
            }
        }
    }

    void Grave()
    {
        Game._instance.Aggressor.GetComponent<Card2>().HurtImg.enabled = false;
        Game._instance.Aggressor.GetComponent<Card2>().HurtText.enabled = false;
        Game._instance.Aggressor.transform.SetParent(GameObject.Find("Grave").transform);
        Game._instance.Aggressor.transform.position = Vector2.zero;
        Destroy(Game._instance.Aggressor.GetComponent<Card2>());//删除进入废牌区的牌的脚本
        Camera.main.GetComponent<Game>().GraveManage();
        Debug.Log("进入废牌区");
    }

    IEnumerator ComeCardDelay()
    {
        yield return new WaitForSeconds(1.4f);
        go.transform.parent.parent = GameObject.Find("ZhuoPanel").transform;
        go.tag = "ZhuoPai";
        go.GetComponent<Card2>().ComeOnAS.Play();
        go.transform.parent.GetComponent<Image>().sprite = Game._instance.Pos.GetComponent<Image>().sprite;
        Game._instance.ShouPaiArray.Remove(go.transform.parent);
        Game._instance.num--;
        Camera.main.GetComponent<Skill>().ZhanHong();
        //go.GetComponent<Card2>().ZhanHong();
        go.GetComponent<Card2>().SkillAS.Play();
        
        
    }

    IEnumerator DelayHurt()
    {
        yield return new WaitForSeconds(0.5f);
        Game._instance.ToAggressor.GetComponent<Enemy>().Hurt(Game._instance.Aggressor.GetComponent<Card2>().Attack);
        Game._instance.Aggressor.tag = "AlreadyAtt";
        Debug.Log("记录已经攻击过一次了");
        StartCoroutine(release());
        //Grave();
    }

    public void CardHurt(int Att)
    {
        Hp -= Att;
        HurtImg.enabled = true;
        HurtText.enabled = true;
        HurtText.text = "-" + Att.ToString();
        hurtTime += Time.deltaTime;
        StartCoroutine(hide());
        if (Hp <= 0)
        {

            //Hp = MinHp;

        }
        HpText.text = Hp.ToString();
    }
    IEnumerator toGrave()
    {
        yield return new WaitForSeconds(1f);
        Grave();
    }

    IEnumerator hide()
    {
        yield return new WaitForSeconds(1f);
        if (HurtImg.enabled == true)
        {
            HurtImg.enabled = false;
            HurtText.enabled = false;
            hurtTime = 0;
        }
    }


    IEnumerator DelayHurtCard()
    {
        yield return new WaitForSeconds(0.5f);
        Game._instance.ToAggressor.GetComponent<Card2>().CardHurt(Game._instance.Aggressor.GetComponent<Card2>().Attack);
        if (Game._instance.ToAggressor.GetComponent<Card2>().Hp <= 0)
        {
            Game._instance.ToAggressor.SetActive(false);

        }

        Game._instance.Aggressor.GetComponent<Card2>().CardHurt(Game._instance.ToAggressor.GetComponent<Card2>().Attack);
        if (Game._instance.Aggressor.GetComponent<Card2>().Hp <= 0)
        {
            GameObject ca = Game._instance.Aggressor.transform.parent.gameObject;
            ca.SetActive(false);
            StartCoroutine(toGrave());
        }
        Game._instance.Aggressor.tag = "AlreadyAtt";
        Debug.Log("记录已经攻击过一次了");
        StartCoroutine(release());
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

    IEnumerator release()
    {
        yield return new WaitForSeconds(0.5f);
        Game._instance.Aggressor = null;
    }
}
