using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Card : MonoBehaviour, IPointerDownHandler
{


    GameObject go;
    bool SureBool;
    float time = 0;
    public int ConsumeNum;
    public int Attack;
    public int Hp;
    public int index;
    public Text AttText;
    public Text HpText;
    public Image HurtImg;
    public Text HurtText;

    float TXtime = 0;
    float hurtTime = 0;
    public int SkillType;

    public AudioSource ComeOnAS;
    public AudioSource SkillAS;
    public AudioSource AttAS;
    Connection connection;

    public bool FengNuAlready;
    public bool isMagic;

    void Start()
    {
        connection = GameObject.Find("Connection").GetComponent<Connection>();
    }
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (Game._instance.round == Game.Round.ToCard)
        {
            if (eventData.pointerEnter.tag != "EnemyCard")
            {
                print("非对方卡牌");
                Game._instance.magicCard = null;
            }
            if (eventData.pointerEnter.tag == "ShouPai")
            {
                print("点击手牌");
                Game._instance.clear();
                Game._instance.Aggressor = null;
                Game._instance.ToAggressor = null;
                go = eventData.pointerEnter;
                print(go);
                SureBool = true;
                go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 20, 0.2f);
                eventData.pointerEnter.transform.parent.GetComponent<Image>().enabled = true;
                print(go.GetComponent<Card>().index);
                go.tag = "ZB";
                if (go.GetComponent<Card>().index >= 29)
                {
                    Game._instance.magicCard = go;
                }
                else
                {
                    Game._instance.magicCard = null;
                }
                time = 0;

            }
            
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                SureBool = false;
                go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y - 20, 0.2f);
                eventData.pointerEnter.transform.parent.GetComponent<Image>().enabled = false;
                go.tag = "ShouPai";
                time = 0;
            }
            //if (eventData.pointerEnter.tag != "ShouPai" && eventData.pointerEnter.tag != "ZB" || EventSystem.current.IsPointerOverGameObject() == false)
            //{
            //    Game._instance.clear();
            //}

            if (eventData.pointerEnter.tag == "ZB" && time >= 0.5f & ConsumeNum <= Game._instance.AtCryaralNum)
            {
                //go.transform.parent = GameObject.Find("Canvas").transform;
                if (go.GetComponent<Card>().isMagic && go.GetComponent<Card>().SkillType == 10)
                {
                    Camera.main.GetComponent<Game>().ReduceCrystal(ConsumeNum);
                    int index = go.GetComponent<Card>().index;
                    connection.Play_card(go.GetComponent<Card>().index);
                    print("play one card " + index);
                    Game._instance.ShouPaiArray.Remove(go.transform.parent);
                    Game._instance.num--;
                    Sequence sequence = DOTween.Sequence();
                    Tweener m1 = go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 297, 0.5f);
                    Tweener s1 = go.transform.parent.DOScale(new Vector3(1f, 0.8f, 1), 0.5f);
                    sequence.Append(m1);
                    sequence.Join(s1);
                    sequence.AppendInterval(0.5f);
                    Game._instance.magicCard = go;

                    StartCoroutine(ComeMagicDelay());
                }
                if(!go.GetComponent<Card>().isMagic)
                {
                    Camera.main.GetComponent<Game>().ReduceCrystal(ConsumeNum);
                    int index = go.GetComponent<Card>().index;
                    print("play one card " + index);
                    connection.Play_card(index);//传输打牌
                    Game._instance.ShouPaiArray.Remove(go.transform.parent);
                    Game._instance.num--;
                    Sequence sequence = DOTween.Sequence();
                    Tweener m1 = go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 215, 0.5f);
                    Tweener m2 = go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 295.7f, 0.5f);
                    Tweener s1 = go.transform.parent.DOScale(new Vector3(1.2f, 1f, 1), 0.5f);
                    Tweener s2 = go.transform.parent.DOScale(new Vector3(1f, 0.8f, 1), 0.5f);
                    sequence.Append(m1);
                    sequence.Join(s1);
                    sequence.AppendInterval(0.5f);
                    sequence.Append(m2);
                    sequence.Join(s2);
                    StartCoroutine(ComeCardDelay());
                    SureBool = false;
                    time = 0;
                }
                
            }
            if (eventData.pointerEnter.tag == "ZB" && time >= 0.5f &&
                ConsumeNum > Game._instance.AtCryaralNum)
            {
                Game._instance.HintText.text = "水晶不足";
                Game._instance.HintText.enabled = true;
            }

            if (eventData.pointerEnter.tag == "ZhuoPai")
            {
                Game._instance.Aggressor = eventData.pointerEnter;
                go = null;
                Game._instance.clear();
            }

            print(eventData.pointerEnter.tag);
            if(Game._instance.magicCard!=null)
                print(Game._instance.magicCard);
            if (eventData.pointerEnter.tag == "Enemy")
            {
                Game._instance.checkTaunt();
                //Game._instance.ToAggressor = eventData.pointerEnter;
                Game._instance.ToAggressor = Game._instance.EnemyImg.gameObject;
                Debug.Log("attack hero");
                if (Game._instance.hasTaunt == false)
                {
                    connection.Attack_card(Game._instance.Aggressor.GetComponent<Card>().index, -1);
                    StartCoroutine(DelayHurt());
                    Game._instance.Aggressor.GetComponent<Card>().AttAS.Play();
                }
                else
                {
                    Game._instance.HintText.text = "请攻击具有嘲讽的随从";
                    Game._instance.HintText.enabled = true;
                    StartCoroutine(release());
                }


            }
            if (eventData.pointerEnter.tag == "EnemyCard")
            {
                print("点击enemycard");
                if(Game._instance.magicCard != null)
                {
                    go = Game._instance.magicCard;
                    print(go.GetComponent<Card>().index);
                }
                if (go != null && go.GetComponent<Card>().isMagic == true &&go.GetComponent<Card>().SkillType!=10)
                {
                    print("打出法术");
                    Camera.main.GetComponent<Game>().ReduceCrystal(ConsumeNum);
                    int index = go.GetComponent<Card>().index;
                    print("play one card " + index);
                    Game._instance.ShouPaiArray.Remove(go.transform.parent);
                    Game._instance.num--;
                    Sequence sequence = DOTween.Sequence();
                    Tweener m1 = go.transform.parent.DOMoveY(go.transform.parent.position.y + 297, 0.5f);
                    Tweener s1 = go.transform.parent.DOScale(new Vector3(1f, 0.8f, 1), 0.5f);
                    sequence.Append(m1);
                    sequence.Join(s1);
                    sequence.AppendInterval(0.5f);
                    Game._instance.magicCard = go;
                    StartCoroutine(ComeMagicDelay());
                    Game._instance.magicTarget = eventData.pointerEnter;
                    connection.Attack_card(Game._instance.magicCard.GetComponent<Card>().index, Game._instance.magicTarget.GetComponent<Card>().index);
                    sequence = DOTween.Sequence();
                    Tweener r1 = Game._instance.magicTarget.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
                    Tweener r2 = Game._instance.magicTarget.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
                    Tweener r3 = Game._instance.magicTarget.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
                    sequence.Append(r1);
                    sequence.Append(r2);
                    sequence.Append(r3);
                    StartCoroutine(magicPower());

                    
                }
                
                if (Game._instance.Aggressor != null)
                {
                    Game._instance.checkTaunt();
                    Game._instance.ToAggressor = eventData.pointerEnter;
                    print("攻击的我方随从编号：" + Game._instance.Aggressor.GetComponent<Card>().index + ",被攻击的敌方随从编号：" + Game._instance.ToAggressor.GetComponent<Card>().index);
                    if (Game._instance.hasTaunt == false || Game._instance.hasTaunt == true && Game._instance.ToAggressor.GetComponent<Card>().SkillType == 3)
                    {
                        int ToAggresorIndex = Game._instance.ToAggressor.GetComponent<Card>().index;
                        connection.Attack_card(Game._instance.Aggressor.GetComponent<Card>().index, ToAggresorIndex);
                        StartCoroutine(DelayHurtCard());
                        Game._instance.Aggressor.GetComponent<Card>().AttAS.Play();
                    }
                    else
                    {
                        Game._instance.HintText.text = "请攻击具有嘲讽的随从";
                        Game._instance.HintText.enabled = true;
                        StartCoroutine(release());
                    }
                }
                

            }
            if (eventData.pointerEnter.tag == "AlreadyAtt")
            {

                Game._instance.HintText.text = "该随从已经攻击过了";
                Game._instance.HintText.enabled = true;
            }
        }


    }

    // Use this for initialization

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

    //void Grave()
    //{
    //    Game._instance.Aggressor.GetComponent<Card>().HurtImg.enabled = false;
    //    Game._instance.Aggressor.GetComponent<Card>().HurtText.enabled = false;
    //    Game._instance.Aggressor.transform.SetParent(GameObject.Find("Grave").transform);
    //    Game._instance.Aggressor.transform.position = Vector2.zero;
    //    Destroy(Game._instance.Aggressor.GetComponent<Card>());//删除进入废牌区的牌的脚本
    //    Camera.main.GetComponent<Game>().GraveManage();
    //   Debug.Log("进入废牌区");
    //}

    public IEnumerator ComeCardDelay()
    {
        yield return new WaitForSeconds(1.4f);
        //int index = go.GetComponent<Card>().index;
        //print("play one card " + index);
        //connection.Play_card(index);//传输打牌
        go.transform.parent.parent = GameObject.Find("ZhuoPanel").transform;
        go.tag = "ZhuoPai";
        go.GetComponent<Card>().ComeOnAS.Play();
        go.transform.parent.GetComponent<Image>().sprite = Game._instance.Pos.GetComponent<Image>().sprite;
        Game._instance.ZhuoPaiArray.Add(go.transform.parent);//添加的是Card的parent，也就是外面那层框
        if (go.GetComponent<Card>().SkillType == 2)
        {
            Camera.main.GetComponent<Skill>().ZhanHong();
            //go.GetComponent<Card>().SkillAS.Play();
        }

    }

    IEnumerator ComeMagicDelay()
    {
        yield return new WaitForSeconds(1.4f);
        //go.GetComponent<Card>().ComeOnAS.Play();

        if (go.GetComponent<Card>().SkillType == 10)
        {
            Game._instance.buildAdditionalCard(-1);
            yield return new WaitForSeconds(1);
            Destroy(go.transform.parent.gameObject);
        }
    }

    public IEnumerator DelayHurt()
    {
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = Game._instance.Aggressor.transform.DOMove(Game._instance.ToAggressor.transform.position, 0.1f);
        Tweener m2 = Game._instance.Aggressor.transform.DOMove(Game._instance.Aggressor.transform.position, 0.4f);
        //Tweener r1 = Game._instance.EnemyImg.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
        // Tweener r2 = Game._instance.EnemyImg.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
        //Tweener r3 = Game._instance.EnemyImg.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
        sequence.Append(m1);
        sequence.Append(m2);
        //sequence.Append(r1);
        //sequence.Append(r2);
        //sequence.Append(r3);
        yield return new WaitForSeconds(0.5f);
        Game._instance.ToAggressor.GetComponent<Enemy>().Hurt(Game._instance.Aggressor.GetComponent<Card>().Attack);
        if (Game._instance.Aggressor.GetComponent<Card>().SkillType != 1)
        {
            Game._instance.Aggressor.tag = "AlreadyAtt";
            Debug.Log("记录已经攻击过一次了");
            StartCoroutine(release());
        }

        else
        {

            Debug.Log("开始结算风怒。结算风怒前的FengNuAlready:" + FengNuAlready);
            if (!FengNuAlready)
            {
                print("风怒牌的tag:" + Game._instance.Aggressor.tag);
                Camera.main.GetComponent<Skill>().FengNu();
                //Game._instance.Aggressor.GetComponent<Card>().SkillAS.Play();
                FengNuAlready = true;
                print("风怒结束。FengNuAlready:" + Game._instance.Aggressor.GetComponent<Card>().FengNuAlready);
            }
            else
            {

                Game._instance.Aggressor.tag = "AlreadyAtt";
                print("发动技能后，风怒牌的tag:" + Game._instance.Aggressor.tag);
                Debug.Log("风怒第二次攻击结束");
                FengNuAlready = false;
                StartCoroutine(release());
            }
        }


        //Game._instance.Aggressor = null;
        //Grave();
    }

    public void CardHurt(int Att)
    {
        Debug.Log("执行了一次CardHurt");
        Hp -= Att;
        HurtImg.enabled = true;
        HurtText.enabled = true;
        HurtText.text = "-" + Att.ToString();
        //gameObject.transform.Find("HurtCardImage").gameObject.GetComponent<Text>().text= "-" + Att.ToString();
        hurtTime += Time.deltaTime;
        StartCoroutine(hide());
        if (Hp <= 0)
        {

            //Hp = MinHp;

        }
        //gameObject.transform.Find("Hp").gameObject.GetComponent<Text>().text = Hp.ToString();
        HpText.text = Hp.ToString();
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

    public IEnumerator release()
    {
        yield return new WaitForSeconds(0.5f);
        Game._instance.Aggressor = null;
        Game._instance.ToAggressor = null;
    }

    public IEnumerator DelayHurtCard()
    {
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
        yield return new WaitForSeconds(0.5f);
        Game._instance.ToAggressor.GetComponent<Card>().CardHurt(Game._instance.Aggressor.GetComponent<Card>().Attack);
        yield return new WaitForSeconds(1.1f);
        if (Game._instance.ToAggressor.GetComponent<Card>().Hp <= 0)
        {
            if (Game._instance.ToAggressor.GetComponent<Card>().SkillType == 4)
            {
                Game._instance.PlayerImg.GetComponent<Player>().Hurt(2);
                print("亡语：对英雄造成2点伤害");
            }
            print("销毁死亡的敌方随从" + Game._instance.ToAggressor.GetComponent<Card>().index);
            Game._instance.EnemyZhuoArray.Remove(Game._instance.ToAggressor.transform);
            Destroy(Game._instance.ToAggressor);


        }
        Game._instance.Aggressor.GetComponent<Card>().CardHurt(Game._instance.ToAggressor.GetComponent<Card>().Attack);
        if (Game._instance.Aggressor.GetComponent<Card>().Hp <= 0)
        {
            if (Game._instance.Aggressor.GetComponent<Card>().SkillType == 4)
            {

                Game._instance.EnemyImg.GetComponent<Enemy>().Hurt(2);
                print("亡语：对英雄造成2点伤害");
            }
            print("销毁死亡的己方随从" + Game._instance.Aggressor.GetComponent<Card>().index);
            Game._instance.ZhuoPaiArray.Remove(Game._instance.Aggressor.transform.parent);
            Destroy(Game._instance.Aggressor.transform.parent.gameObject);


        }
        if (Game._instance.Aggressor.GetComponent<Card>().SkillType != 1)
        {
            Game._instance.Aggressor.tag = "AlreadyAtt";
            Debug.Log("记录已经攻击过一次了");
            StartCoroutine(release());
        }
        else
        {
            Debug.Log("开始结算风怒。结算风怒前的FengNuAlready:" + FengNuAlready);
            if (!Game._instance.Aggressor.GetComponent<Card>().FengNuAlready)
            {
                print("风怒牌的tag:" + Game._instance.Aggressor.tag);
                Camera.main.GetComponent<Skill>().FengNu();
                //Game._instance.Aggressor.GetComponent<Card>().SkillAS.Play();
                Game._instance.Aggressor.GetComponent<Card>().FengNuAlready = true;

            }
            else
            {

                Game._instance.Aggressor.tag = "AlreadyAtt";
                print("发动技能后，风怒牌的tag:" + Game._instance.Aggressor.tag);
                print("结算风怒hou的FengNuAlready:" + FengNuAlready);
                Debug.Log("风怒第二次攻击结束");
                Game._instance.Aggressor.GetComponent<Card>().FengNuAlready = true;
                StartCoroutine(release());
            }
        }

        //Game._instance.Aggressor = null;
    }

    IEnumerator magicPower()
    {
        yield return new WaitForSeconds(1);
        switch (Game._instance.magicCard.GetComponent<Card>().SkillType)
        {
            case 5:
                Camera.main.GetComponent<Skill>().TaoKeZiXue();
                break;
            case 6:
                Camera.main.GetComponent<Skill>().MianKao();
                break;
            case 7:
                Camera.main.GetComponent<Skill>().QingJiaoLaoShi();
                break;
            case 8:
                Camera.main.GetComponent<Skill>().QingJiaoTongXue();
                break;
            case 9:
                Camera.main.GetComponent<Skill>().TuiKe();
                break;

        }
    }

    public void decreaseAtt(int att)
    {
        Attack -= att;
        if (Attack < 0)
        {
            Attack = 0;
        }
        AttText.text = Attack.ToString();
    }
    public void addAtt(int att)
    {
        Attack += att;
        if (Attack < 0)
        {
            Attack = 0;
        }
        AttText.text = Attack.ToString();
    }

    public void decreaseHp(int hp)
    {
        Hp -= hp;
        if (Hp < 0)
        {

        }
        HpText.text = Hp.ToString();
    }

}
