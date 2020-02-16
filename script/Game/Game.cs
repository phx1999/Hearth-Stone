using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

public class Game : MonoBehaviour
{

    public static Game _instance;

    public GameObject csWin;
    public GameObject jrWin;

    public List<Transform> ShouPaiArray;
    public List<Transform> ZhuoPaiArray;
    public List<Transform> GraveArray;

    public List<Transform> EnemyShouArray;
    public List<Transform> EnemyZhuoArray;

    GameObject Grave;
    public GameObject PlayerImg;
    public GameObject EnemyImg;

    public Image[] CrystalArr;

    public Text CrystalText;
    public Text EnemyCrystalText;
    public int CrystalNum = 0;
    public int AtCryaralNum;
    public Text HintText;
    float HintTime = 0;

    bool start = true;
    public GameObject Pos;//卡牌生成位置
    public GameObject CardsObj;//卡牌预制体
    GameObject go;
    GameObject ec;
    //GameObject goo;
    //public GameObject EnemyCardObj;
    public GameObject EnemyCardsObj;

    //public Text OverBtnText;
    bool OverBool;
    public Button OverBtn;

    public int num = 0;
    public int EnemyNum = 0;
    int g = 0;

    public Image ClockImg;
    float ClockTime = 0;
    bool ClockBool;
    public Image ClockTXImg;

    public GameObject Aggressor;
    public GameObject ToAggressor;
    int m = 0;

    public GameObject head;

    public GameObject enemy_attack_card;
    public GameObject own_attacked_card;

    public GameObject magicCard;
    public GameObject magicTarget;

    public GameObject enemy_magic_card;
    public GameObject own_magic_target;

    public Button Surrender;
    public Button Chat;
    public Button ChatBubble1;
    public Button ChatBubble2;
    public Button ChatBubble3;
    public Button ChatBubble4;
    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;
    public GameObject Bubble;
    public GameObject EnemyBubble;
    public Text BubleText;

    bool option = false;
    bool display = false;
    bool EnemyDisplay = false;
    float optionTime = 0;
    float displayTime = 0;
    float EnemyDisplayTime = 0;
    public enum Round//代表回合信息
    {
        Player,//玩家回合
        ToCard,//玩家出牌回合
        Enemy,
        OverRound
    }

    public Round round = Round.Player;//默认玩家回合
    public int RoundNum = 1;
    int c = 0;
    float BuildCardTime = 2.5f;

    float RoundTime = 0;

    bool firstRound = true;
    public bool hasTaunt = false;

    public Connection connection;
    public List<string> card_information = new List<string>();
    public string hero_path;
    public string enemy_path;
    bool EnemyIsOff = false;
    // Use this for initialization
    void Start()
    {

        _instance = this;


        ChatBubble1.onClick.AddListener(chat1);
        ChatBubble2.onClick.AddListener(chat2);
        ChatBubble3.onClick.AddListener(chat3);
        ChatBubble4.onClick.AddListener(chat4);

        refresh_card();
        for (int i = 0; i < 3; i++)
        {
            GameObject goo = Instantiate(EnemyCardsObj);
            EnemyShouArray.Add(goo.transform);
            goo.transform.SetParent(GameObject.Find("EnemyShouPanel").transform);
        }
        connection = GameObject.Find("Connection").GetComponent<Connection>();

        GameObject a = Instantiate(head);
        a.tag = "Enemy";

        PlayerImg = GameObject.Find("ImagePlayer");
        EnemyImg = GameObject.Find("ImageEnemy");
        a.transform.SetParent(EnemyImg.transform);
        PlayerImg.GetComponent<Image>().sprite = Resources.Load("cs", typeof(Sprite)) as Sprite;
        EnemyImg.GetComponent<Image>().sprite = Resources.Load("jr", typeof(Sprite)) as Sprite;
        PlayerImg.transform.DOMove(new Vector3(PlayerImg.transform.position.x - 400, PlayerImg.transform.position.y - 440, 0), 1.5f);
        EnemyImg.transform.DOMove(new Vector3(EnemyImg.transform.position.x + 400, EnemyImg.transform.position.y + 360, 0), 1.5f);
        AddCrystal();
        //CrystalArr[0].enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (connection.gameover)
        {
            connection.gameover = false;
            game_over();
            
        }

        if (connection.enemy_hurt)
        {
            connection.enemy_hurt = false;
            EnemyImg.GetComponent<Enemy>().Hurt(2);
        }

        if (connection.reconnect)
        {
            if (connection.my_round)
            {
                round = Round.ToCard;

            }
            else
            {
                round = Round.Enemy;
            }
            reload();
         }
  
        if (HintText.enabled == true)
        {
            HintTime += Time.deltaTime;
            if (HintTime > 2f)
            {
                HintText.enabled = false;
                HintTime = 0;
            }
        }
        if (connection.add_enemy_card_in_ground)//敌方出牌
        {
            //AddEnemyCrystal();
            connection.add_enemy_card_in_ground = false;
            EnemyComeCard();

        }

        if (round == Round.Player)
        {
            Surrender.gameObject.SetActive(false);
            this.OverBtn.interactable = false;
            //OverBtnText.text = "结束回合";
            BuildCardTime += Time.deltaTime;
            BuildCard();
            for (int i = 0; i < ZhuoPaiArray.Count; i++)
            {
                ZhuoPaiArray[i].transform.GetChild(0).tag = "ZhuoPai";
            }
        }

        if (round == Round.ToCard)
        {
            Surrender.gameObject.SetActive(true );
            this.OverBtn.interactable = true;
            RoundTime += Time.deltaTime;
            if (RoundTime >= 50)
            {
                Clock();
                ClockBool = true;

            }

        }
        if (round == Round.Enemy)
        {
            if (!firstRound)
                AddEnemyCrystal();
            this.OverBtn.interactable = false;
            Surrender.gameObject.SetActive(true);
            RoundTime += Time.deltaTime;
            if (RoundTime >= 60.2)
            {
                EnemyIsOff = true;
            }
        }
        if (EnemyIsOff == true)
        {
            connection.Round_end();
            RoundTime = 0;
            EnemyIsOff = false;
        }

        if (connection.my_round)
        {
            if (firstRound == false)
            {
                AddCrystal();
            }
            RoundTime = 0;
            round = Round.Player;
            connection.my_round = false;
            this.OverBtn.interactable = true;
            OverBtn.transform.DORotate(new Vector3(360, 0, 0), 1f, RotateMode.FastBeyond360);
        }

        
        
        if (round == Round.Enemy && connection.enemy_attack)//对手随从发动攻击
        {
            connection.enemy_attack = false;

            int enemy_attack_card_num = connection.enemy_attack_num;//对手随从编号
            int own_attacked_card_num = connection.own_attacked_card_num;//自己被攻击的随从编号
            if (enemy_attack_card_num <= 29)
            {
                print("EnemyCard:" + enemy_attack_card_num + ", OwnCard:" + own_attacked_card_num);
                enemy_attack_card = getEnemyCard(enemy_attack_card_num).gameObject;//GameObject enemy_attack_card是通过编号找到的桌面上的敌方随从
                                                                                   //this.Aggressor = enemy_attack_card;//GameObject 攻击者设为 GameObject enemy_attack_card
                if (own_attacked_card_num == -1)//此处被攻击者为己方英雄
                {
                    StartCoroutine(EnemyAttDelay());
                }
                else
                {
                    own_attacked_card = getOwnCard(own_attacked_card_num).gameObject;//GameObject own_attacked_card是通过编号找到的己方随从
                    StartCoroutine(EnemyAttCardDelay());//执行敌方随从攻击己方随从的方法
                                                        //EnemyAttCard();
                }
            }
            else//法术牌
            {
                fit_enemy_card(enemy_attack_card_num);
                own_magic_target= getOwnCard(own_attacked_card_num).gameObject;
                ReduceEnemyCrystal(ec.GetComponent<Card>().ConsumeNum);
                ec.transform.GetChild(2).GetComponent<Image>().enabled = false;//去除卡背
                Sequence sequence = DOTween.Sequence();
                Debug.Log("敌方打出法术牌,技能类型为：" + ec.GetComponent<Card>().SkillType);
                ec.transform.SetParent(GameObject.Find("Canvas").transform);
                Tweener m1 = ec.transform.DOMove(new Vector3(890.3f, 800, 0), 0.5f);
                Tweener s1 = ec.transform.DOScale(new Vector3(1f, 0.8f, 1), 0.5f);
                sequence.Append(m1);
                sequence.Join(s1);

                sequence.AppendInterval(0.5f);
                StartCoroutine(EnemyMagicDelay());
            }
            

        }

        if (ClockBool == true)
        {
            ClockTime += Time.deltaTime;
            if (ClockTime >= 10.2f)
            {
                connection.Round_end();

                ClockBool = false;
                ClockImg.GetComponent<Animator>().enabled = false;
                ClockImg.GetComponent<Image>().enabled = false;
                ClockTXImg.GetComponent<Image>().enabled = false;
                //OverBtnText.text = "对手回合";
                OverBtn.transform.DORotate(new Vector3(360, 0, 0), 1f, RotateMode.FastBeyond360);
                ClockTime = 0;
                //OverBool = true;
                round = Round.Enemy;
                RoundTime = 0;
            }
        }

        if (option == true)
        {
            optionTime += Time.deltaTime;
            if (optionTime > 3f)
            {
                option = false;
                ChatBubble1.gameObject.SetActive(false);
                ChatBubble2.gameObject.SetActive(false);
                ChatBubble3.gameObject.SetActive(false);
                ChatBubble4.gameObject.SetActive(false);
                optionTime = 0;
            }
        }

        if (display == true)
        {
            displayTime += Time.deltaTime;
            optionTime = 0;
            if (displayTime > 2f)
            {
                display = false;
                Bubble.SetActive(false);
                displayTime = 0;

            }
        }

        if (connection.enemy_send)
        {
            connection.enemy_send = false;
            EnemyBubble.SetActive(true);
            EnemyDisplay = true;
            switch (connection.enemy_message)
            {
                case 1:
                    EnemyBubble.transform.Find("Text").gameObject.GetComponent<Text>().text = text1.text;
                    break;
                case 2:
                    EnemyBubble.transform.Find("Text").gameObject.GetComponent<Text>().text = text2.text;
                    break;
                case 3:
                    EnemyBubble.transform.Find("Text").gameObject.GetComponent<Text>().text = text3.text;
                    break;
                case 4:
                    EnemyBubble.transform.Find("Text").gameObject.GetComponent<Text>().text = text4.text;
                    break;
            }
               
            
        }

        if (EnemyDisplay == true)
        {
            EnemyDisplayTime += Time.deltaTime;
            if (EnemyDisplayTime > 2f)
            {
                EnemyDisplay = false;
                EnemyDisplayTime = 0;
                EnemyBubble.SetActive(false);
            }
        }

    }
    void refresh_card()
    {
        card_information.Add(" ");
        card_information.Add("1,7,7,4,3");
        card_information.Add("2,4,4,5,0");
        card_information.Add("3,4,5,4,0");
        card_information.Add("4,5,3,1,4");
        card_information.Add("5,2,3,2,4");
        card_information.Add("6,4,4,3,1");
        card_information.Add("7,6,7,1,0");
        card_information.Add("8,4,5,2,3");
        card_information.Add("9,2,0,4,3");
        card_information.Add("10,5,6,3,4");
        card_information.Add("11,5,5,5,4");
        card_information.Add("12,2,3,3,2");
        card_information.Add("13,2,3,1,1");
        card_information.Add("14,3,3,3,1");
        card_information.Add("15,3,5,3,3");
        card_information.Add("16,2,3,2,2");
        card_information.Add("17,3,4,3,4");
        card_information.Add("18,4,2,2,3");
        card_information.Add("19,6,6,5,0");
        card_information.Add("20,6,6,5,4");
        card_information.Add("21,1,2,1,0");
        card_information.Add("22,2,2,2,3");
        card_information.Add("23,2,3,1,0");
        card_information.Add("24,2,3,2,0");
        card_information.Add("25,1,2,1,0");
        card_information.Add("26,3,4,2,3");
        card_information.Add("27,9,9,6,0");
        card_information.Add("28,9,8,7,0");
        card_information.Add("29,7,7,5,0");
        card_information.Add("30,0,7,5,5");
        card_information.Add("31,5,7,5,6");
        card_information.Add("32,4,7,5,7");
        card_information.Add("33,3,7,5,8");
        card_information.Add("34,4,7,5,9");
        card_information.Add("35,2,7,5,10");

    }

    void game_over()
    {
        if (connection.win)
        {
            StartCoroutine(csWinShow());
            
        }
        else
        {
            StartCoroutine(jrWinShow());
        }
        connection.Quit_room();
        clear_room();
    }

    void clear_room()
    {
        connection.roomate = "";
        connection.enemy_ready = false;
        connection.inroom = false;
    }


    void reload()
    {
        connection.reconnect = false;
        PlayerImg.GetComponent<Player>().Hp = connection.my_hero_hp;
        EnemyImg.GetComponent<Enemy>().Hp = connection.enemy_hero_hp;
        resetCrystal();
        AtCryaralNum = connection.at_crystal;
        CrystalNum = connection.my_crystal;
        CrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
        EnemyCrystalText.text = connection.enemy_at_crystal.ToString() + "/" + connection.enemy_crystal.ToString();
        for (int i = 0; i < CrystalNum; i++)
        {
            AddCrystal();
        }
        if (round == Round.ToCard)
        {
            RoundTime += connection.rest_time;
        }
        for (int i = 0; i < connection.card_in_hand.Count; i++)
        {
            print(connection.card_in_hand[i]);
            fit_card(connection.card_in_hand[i]);
            c = 2;
            go.transform.SetParent(GameObject.Find("ShouPanel").transform);
            go.transform.GetChild(0).tag = "ShouPai";
        }
        for (int i = 0; i < connection.my_card_inground.Count; i++)
        {
            print(connection.my_card_inground[i]);
            go = Instantiate(CardsObj, Pos.transform.position, Quaternion.identity);
            go.transform.Find("card1").GetComponent<Image>().sprite = Resources.Load(int.Parse(Regex.Split(connection.my_card_inground[i], " ")[0]).ToString(), typeof(Sprite)) as Sprite;
            go.transform.Find("card1").gameObject.GetComponent<Card>().index = int.Parse(Regex.Split(card_information[int.Parse(Regex.Split(connection.my_card_inground[i], " ")[0])],",")[0]);
            go.transform.Find("card1").gameObject.GetComponent<Card>().Attack = int.Parse(Regex.Split(connection.my_card_inground[i], " ")[1]);
            go.transform.Find("card1").gameObject.GetComponent<Card>().Hp = int.Parse(Regex.Split(connection.my_card_inground[i], " ")[2]);
            go.transform.Find("card1").transform.Find("Att").gameObject.GetComponent<Text>().text = go.transform.Find("card1").gameObject.GetComponent<Card>().Attack.ToString();
            go.transform.Find("card1").GetComponent<Card>().AttText = go.transform.Find("card1").transform.Find("Att").gameObject.GetComponent<Text>();
            go.transform.Find("card1").transform.Find("Hp").gameObject.GetComponent<Text>().text = go.transform.Find("card1").gameObject.GetComponent<Card>().Hp.ToString();
            go.transform.Find("card1").GetComponent<Card>().HpText = go.transform.Find("card1").transform.Find("Hp").gameObject.GetComponent<Text>();
            go.tag = "ZhuoPai";
            go.transform.SetParent(GameObject.Find("ZhuoPanel").transform);
            ZhuoPaiArray.Add(go.transform);
        }
        for (int i = 0; i < connection.enemy_card_inground.Count; i++)
        {
            go = Instantiate(EnemyCardsObj, Pos.transform.position, Quaternion.identity);
            go.GetComponent<Image>().sprite = Resources.Load(int.Parse(Regex.Split(connection.enemy_card_inground[i], " ")[0]).ToString(), typeof(Sprite)) as Sprite;
            go.GetComponent<Card>().index = int.Parse(Regex.Split(card_information[int.Parse(Regex.Split(connection.enemy_card_inground[i], " ")[0])], ",")[0]); 
            go.transform.Find("Att").gameObject.GetComponent<Text>().text = go.GetComponent<Card>().Attack.ToString();
            go.GetComponent<Card>().AttText = go.transform.Find("Att").gameObject.GetComponent<Text>();
            go.transform.Find("Hp").gameObject.GetComponent<Text>().text = go.GetComponent<Card>().Hp.ToString();
            go.GetComponent<Card>().HpText = go.transform.Find("Hp").gameObject.GetComponent<Text>();
            go.transform.GetChild(2).GetComponent<Image>().enabled = false;
            go.GetComponent<Card>().Attack = int.Parse(Regex.Split(connection.enemy_card_inground[i], " ")[1]);
            go.GetComponent<Card>().Hp = int.Parse(Regex.Split(connection.enemy_card_inground[i], " ")[2]);
            EnemyZhuoArray.Add(go.transform);
            go.transform.SetParent(GameObject.Find("EnemyZhuoPanel").transform);
            go.tag = "EnemyCard";
        }
    }

    void fit_card(int number)
    {
        go = Instantiate(CardsObj, Pos.transform.position, Quaternion.identity);
        go.transform.Find("card1").GetComponent<Image>().sprite = Resources.Load(number.ToString(), typeof(Sprite)) as Sprite;
        go.transform.Find("card1").gameObject.GetComponent<Card>().index = int.Parse(Regex.Split(card_information[number], ",")[0]);
        go.transform.Find("card1").gameObject.GetComponent<Card>().ConsumeNum = int.Parse(Regex.Split(card_information[number], ",")[1]);
        go.transform.Find("card1").gameObject.GetComponent<Card>().Attack =int.Parse(Regex.Split( card_information[number],",")[2]);
        go.transform.Find("card1").gameObject.GetComponent<Card>().Hp =  int.Parse(Regex.Split(card_information[number], ",")[3]);
        go.transform.Find("card1").gameObject.GetComponent<Card>().SkillType =  int.Parse(Regex.Split(card_information[number], ",")[4]);
        go.transform.Find("card1").transform.Find("Att").gameObject.GetComponent<Text>().text = go.transform.Find("card1").gameObject.GetComponent<Card>().Attack.ToString();
        go.transform.Find("card1").GetComponent<Card>().AttText = go.transform.Find("card1").transform.Find("Att").gameObject.GetComponent<Text>();
        go.transform.Find("card1").transform.Find("Hp").gameObject.GetComponent<Text>().text = go.transform.Find("card1").gameObject.GetComponent<Card>().Hp.ToString();
        go.transform.Find("card1").GetComponent<Card>().HpText = go.transform.Find("card1").transform.Find("Hp").gameObject.GetComponent<Text>();
        if (number >= 30)
        {
            go.transform.Find("card1").gameObject.GetComponent<Card>().isMagic = true;
            go.transform.Find("card1").transform.Find("Att").gameObject.SetActive(false);
            go.transform.Find("card1").transform.Find("Hp").gameObject.SetActive(false);
        }
    }

    void fit_enemy_card(int number)
    {
        ec = Instantiate(EnemyCardsObj, new Vector3(890.3f, 1000, 0), Quaternion.identity);
        ec.GetComponent<Image>().sprite = Resources.Load(number.ToString(), typeof(Sprite)) as Sprite;
        ec.GetComponent<Card>().index = int.Parse(Regex.Split(card_information[number], ",")[0]);
        ec.GetComponent<Card>().ConsumeNum= int.Parse(Regex.Split(card_information[number], ",")[1]);
        ec.GetComponent<Card>().Attack= int.Parse(Regex.Split(card_information[number], ",")[2]);
        ec.GetComponent<Card>().Hp= int.Parse(Regex.Split(card_information[number], ",")[3]);
        ec.GetComponent<Card>().SkillType= int.Parse(Regex.Split(card_information[number], ",")[4]);
        ec.transform.Find("Att").gameObject.GetComponent<Text>().text = ec.GetComponent<Card>().Attack.ToString();
        ec.GetComponent<Card>().AttText = ec.transform.Find("Att").gameObject.GetComponent<Text>();
        ec.transform.Find("Hp").gameObject.GetComponent<Text>().text = ec.GetComponent<Card>().Hp.ToString();
        ec.GetComponent<Card>().HpText = ec.transform.Find("Hp").gameObject.GetComponent<Text>();

        if (number >= 30)
        {
            ec.GetComponent<Card>().isMagic = true;
            ec.transform.Find("Att").gameObject.SetActive(false);
            ec.transform.Find("Hp").gameObject.SetActive(false);
        }
    }

    public void EnemyComeCard()
    {

        int enemy_card_num = connection.enemy_card_num;//得到敌方卡牌的编号
        if (enemy_card_num == 35)
        {
            fit_enemy_card(35);
            ReduceEnemyCrystal(ec.GetComponent<Card>().ConsumeNum);
            ec.transform.GetChild(2).GetComponent<Image>().enabled = false;//去除卡背
            Sequence sequence = DOTween.Sequence();
            Debug.Log("敌方打出法术牌,技能类型为：" + ec.GetComponent<Card>().SkillType);
            ec.transform.SetParent(GameObject.Find("Canvas").transform);
            Tweener m1 = ec.transform.DOMove(new Vector3(890.3f, 800, 0), 0.5f);
            Tweener s1 = ec.transform.DOScale(new Vector3(1f, 0.8f, 1), 0.5f);
            sequence.Append(m1);
            sequence.Join(s1);
            sequence.AppendInterval(1f);
            StartCoroutine(delete());
        }
        else
        {
            fit_enemy_card(enemy_card_num);
            ReduceEnemyCrystal(ec.GetComponent<Card>().ConsumeNum);
            ec.transform.SetParent(GameObject.Find("Canvas").transform);
            ec.transform.GetChild(2).GetComponent<Image>().enabled = false;//去除卡背
            Sequence sequence = DOTween.Sequence();
            Debug.Log("敌方打出随从牌");
            Tweener m1 = ec.transform.DOMove(new Vector3(890.3f, 800, 0), 0.5f);
            Tweener m2 = ec.transform.DOMove(new Vector3(890.3f, 719, 0), 0.5f);
            Tweener s1 = ec.transform.DOScale(new Vector3(1f, 0.8f, 1), 0.5f);
            Tweener s2 = ec.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 0.5f);
            sequence.Append(m1);
            sequence.Join(s1);
            sequence.AppendInterval(0.5f);
            sequence.Append(m2);
            sequence.Join(s2);
            StartCoroutine(EnemyDelay());
        }
    }

    IEnumerator delete()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(ec);
    }

    public Transform getEnemyCard(int enemy_attack_card_num)
    {
        for (int i = 0; i < EnemyZhuoArray.Count; i++)
        {
            if (EnemyZhuoArray[i].GetComponent<Card>().index == enemy_attack_card_num)
            {
                return EnemyZhuoArray[i];
            }
        }
        return null;
    }

    public Transform getOwnCard(int own_attacked_card_num)
    {
        for (int i = 0; i < ZhuoPaiArray.Count; i++)
        {
            if (ZhuoPaiArray[i].GetChild(0).GetComponent<Card>().index == own_attacked_card_num)
            {
                return ZhuoPaiArray[i].GetChild(0);
            }
        }
        return null;
    }
    public void GraveManage()
    {
        if (g >= 13)
        {
            foreach (Transform child in Grave.transform)
            {
                GraveArray.Add(child);
            }
            Destroy(GraveArray[0].gameObject);
            foreach (Transform child in Grave.transform)
            {
                GraveArray.Remove(child);
            }
        }
        g++;
    }

    IEnumerator EnemyDelay()
    {
        yield return new WaitForSeconds(1.5f);
        EnemyZhuoArray.Add(ec.transform);
        ec.transform.SetParent(GameObject.Find("EnemyZhuoPanel").transform);
        ec.tag = "EnemyCard";
        switch (ec.GetComponent<Card>().SkillType)
        {
            case 2:
                Camera.main.GetComponent<EnemySkill>().ZhanHong();
                //ec.GetComponent<Card>().SkillAS.Play();
                break;
            case 3:
                hasTaunt = true;
                break;
        }
    }

    IEnumerator EnemyMagicDelay()
    {
        yield return new WaitForSeconds(1f);
        int skillType = ec.GetComponent<Card>().SkillType;
        Destroy(ec);
        Sequence sequence = DOTween.Sequence();
        Tweener r1 = own_magic_target.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
        Tweener r2 = own_magic_target.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
        Tweener r3 = own_magic_target.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
        sequence.Append(r1);
        sequence.Append(r2);
        sequence.Append(r3);
        yield return new WaitForSeconds(1);
        switch (skillType)
        {
            case 5:
                Camera.main.GetComponent<EnemySkill>().TaoKeZiXue();
                break;
            case 6:
                Camera.main.GetComponent<EnemySkill>().MianKao();
                break;
            case 7:
                Camera.main.GetComponent<EnemySkill>().QingJiaoLaoShi();
                break;
            case 8:
                Camera.main.GetComponent<EnemySkill>().QingJiaoTongXue();
                break;
            case 9:
                Camera.main.GetComponent<EnemySkill>().TuiKe();
                break;
        }


    }

    public void OverRound()
    {
        clear();
        //OverBool = true;

        //if (OverBool)
        //{
        //OverBtnText.text = "对手回合";
        round = Round.Enemy;
        connection.Round_end();
        OverBtn.transform.DORotate(new Vector3(360, 0, 0), 1f, RotateMode.FastBeyond360);//可以转到360以外的角度
        if (ClockBool == true)
        {
            ClockBool = false;
            clear();
            ClockImg.GetComponent<Animator>().enabled = false;
            ClockImg.GetComponent<Image>().enabled = false;
            ClockTXImg.GetComponent<Image>().enabled = false;
            ClockTime = 0;
            RoundTime = 0;
        }
        //}

    }

    void Clock()
    {
        ClockImg.GetComponent<Animator>().enabled = true;//激活动画组件
        ClockImg.GetComponent<Image>().enabled = true;//激活图片
        ClockTXImg.GetComponent<Image>().enabled = true;
    }

    public void checkTaunt()
    {
        hasTaunt = false;
        for (int i = 0; i < EnemyZhuoArray.Count; i++)
        {
            if (EnemyZhuoArray[i].GetComponent<Card>().SkillType == 3)
            {
                hasTaunt = true;
            }
        }

    }

    void resetCrystal()
    {
        for (int i = 0; i < CrystalArr.Length; i++)
        {
            CrystalArr[i].enabled = false;
        }
        CrystalNum = 0;
        AtCryaralNum = 0;
    }


    public void AddCrystal()
    {
        if (CrystalNum < 10)
        {
            for (int i = 0; i <= CrystalNum; i++)
            {
                CrystalArr[i].enabled = true;
            }
            CrystalNum++;
            AtCryaralNum = CrystalNum;
            CrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
        }
        else
        {
            CrystalNum = 10;
            AtCryaralNum = CrystalNum;
            CrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
        }
    }
    public void AddEnemyCrystal()
    {
        if (CrystalNum < 10)
        {
            AtCryaralNum = CrystalNum;
            EnemyCrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
        }
        else
        {
            CrystalNum = 10;
            AtCryaralNum = CrystalNum;
            CrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
        }
    }
    public void ReduceCrystal(int num)
    {
        for (int i = 0; i < AtCryaralNum; i++)
        {
            CrystalArr[i].enabled = false;
        }
        for (int i = 0; i < AtCryaralNum - num; i++)
        {
            CrystalArr[i].enabled = true;
        }
        AtCryaralNum -= num;
        CrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
    }
    public void ReduceEnemyCrystal(int num)
    {
        AtCryaralNum -= num;
        if (AtCryaralNum < 0)
        {
            AtCryaralNum = 0;
        }
        EnemyCrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
    }
    public void BuildCard()
    {
        if (num <= 9 && BuildCardTime >= 2.5)
        {
            int x = connection.get_card_num();
            if (x != -1)
            {
                fit_card(x);
                ShouPaiArray.Add(go.transform);
                go.transform.SetParent(GameObject.Find("Canvas").transform);//卡牌父体为画布，否则和画布并列 不会显示出来
                Sequence sequence = DOTween.Sequence();
                Tweener m1 = go.transform.DOMoveX(go.transform.position.x - 500, 0.7f);
                Tweener m2 = go.transform.DOMove(new Vector3(go.transform.position.x - 1185,
                    go.transform.position.y - 390, 0), 0.7f);
                Tweener s1 = go.transform.DOScale(new Vector3(1f, 0.8f, 0), 0.7f);
                sequence.Append(m1);//0.7s进场
                sequence.AppendInterval(0.7f);//停留0.7秒让玩家看一下抽的是什么牌
                sequence.Append(m2);//0.7s飞入手牌
                sequence.Join(s1);
                if (c >= 2)
                {
                    if (connection.priority == 0 || firstRound == false)
                    {
                        if (firstRound == true)
                        {
                            HintText.text = "你是先手";
                            HintText.enabled = true;
                        }
                        round = Round.ToCard;
                        firstRound = false;
                    }
                    else
                    {
                        HintText.text = "你是后手";
                        HintText.enabled = true;
                        round = Round.Enemy;
                        firstRound = false;
                    }
                }
                num++;
                BuildCardTime = 0;
                c++;
                StartCoroutine(Suspend());
            }
            else
            {
                PlayerImg.GetComponent<Player>().Hurt(2);
            }

        }
        if (num > 9)
        {

            Debug.Log("手牌已满！");
            round = Round.ToCard;
        }

    }

    public void buildAdditionalCard(int x)
    {
        if (x == -1)
        {
            x = connection.get_card_num();
        }
        Debug.Log("额外抽一张牌");
        if (num <= 9)
        {
            fit_card(x);//生成卡牌
            ShouPaiArray.Add(go.transform);
            go.transform.SetParent(GameObject.Find("Canvas").transform);//卡牌父体为画布，否则和画布并列 不会显示出来
            Sequence sequence = DOTween.Sequence();
            Tweener m1 = go.transform.DOMoveX(go.transform.position.x - 500, 0.7f);
            Tweener m2 = go.transform.DOMove(new Vector3(go.transform.position.x - 1185,
                go.transform.position.y - 460, 0), 0.7f);
            Tweener s1 = go.transform.DOScale(new Vector3(1f, 0.8f, 0), 0.7f);
            sequence.Append(m1);//1s 进场
            sequence.AppendInterval(0.7f);//停留一秒让玩家看一下抽的是什么牌
            sequence.Append(m2);//1s 飞向手牌区 同时缩小
            sequence.Join(s1);

            print("c:" + c);
            print("round:" + round);
            StartCoroutine(Suspend2());
            num++;
            //BuildCardTime = 0;
            c++;

        }
        if (num > 9)
        {
            Debug.Log("手牌已满！");
        }
    }

    public void buildSpecificCard(int index)
    {
        Debug.Log("抽取编号为" + index + "的牌");
        if (num <= 9)
        {
            int x = index ;
            fit_card(x);//生成卡牌
            ShouPaiArray.Add(go.transform);
            go.transform.SetParent(GameObject.Find("Canvas").transform);//卡牌父体为画布，否则和画布并列 不会显示出来
            Sequence sequence = DOTween.Sequence();
            Tweener m1 = go.transform.DOMoveX(go.transform.position.x - 500, 0.7f);
            Tweener m2 = go.transform.DOMove(new Vector3(go.transform.position.x - 1185,
                go.transform.position.y - 460, 0), 0.7f);
            Tweener s1 = go.transform.DOScale(new Vector3(0.5f, 0.4f, 0), 0.7f);
            sequence.Append(m1);//1s 进场
            sequence.AppendInterval(0.7f);//停留一秒让玩家看一下抽的是什么牌
            sequence.Append(m2);//1s 飞向手牌区 同时缩小
            sequence.Join(s1);

            print("c:" + c);
            print("round:" + round);
            StartCoroutine(Suspend2());
            num++;
            //BuildCardTime = 0;
            c++;

        }
        if (num > 9)
        {
            Debug.Log("手牌已满！");
        }

    }

    IEnumerator Suspend2()
    {
        yield return new WaitForSeconds(2.1f);
        switch (num)
        {
            case 6:
                Vector2 z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -120;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 7:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -150;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 8:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -170;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 9:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -190;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 10:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -210;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
        }
        go.transform.SetParent(GameObject.Find("ShouPanel").transform);
        go.transform.GetChild(0).tag = "ShouPai";
    }

    //public void BuildEnemyCard()
    //{
    //    Debug.Log("敌人发牌？");
    //    if (EnemyNum <= 9)
    //    {
    //        int s = connection.get_card_num();
    //        EnemyCardObj = EnemyCardsObj[s - 1];
    //        goo = Instantiate(EnemyCardObj);
    //        goo.tag = "EnemyCard";//Mine
    //        EnemyShouArray.Add(goo.transform);
    //        goo.transform.SetParent(GameObject.Find("EnemyShouPanel").transform);
    //        EnemyNum++;

    //        //int x = Random.Range(0, 16);
    //        // EnemyCardObj = EnemyCardsObj[x];
    //        //goo = Instantiate(EnemyCardObj, Pos.transform.position, Quaternion.identity);//生成卡牌
    //        // EnemyShouArray.Add(goo.transform);
    //        //goo.transform.SetParent(GameObject.Find("Canvas").transform);//卡牌父体为画布，否则和画布并列 不会显示出来
    //        // Sequence sequence = DOTween.Sequence();
    //        // Tweener m1 = goo.transform.DOMoveX(go.transform.position.x - 500, 1);
    //        // Tweener m2 = goo.transform.DOMove(new Vector3(go.transform.position.x - 1185,
    //        //     goo.transform.position.y + 460, 0), 1);
    //        //  Tweener s1 = goo.transform.DOScale(new Vector3(0.5f, 0.4f, 0), 1);
    //        // sequence.Append(m1);
    //        // sequence.AppendInterval(1);//停留一秒让玩家看一下抽的是什么牌
    //        //  sequence.Append(m2);
    //        //  sequence.Join(s1);
    //        //  StartCoroutine(EnemySuspend());
    //        //  EnemyNum++;
    //        //BuildCardTime = 0;
    //        //c++;
    //    }
    //    if (EnemyNum > 9)
    //    {
    //        Debug.Log("手牌已满！");
    //    }
    //}

    IEnumerator Suspend()
    {
        yield return new WaitForSeconds(2.1f);
        switch (num)
        {
            case 6:
                Vector2 z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -120;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 7:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -150;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 8:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -170;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 9:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -190;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 10:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -210;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
        }
        print("suspen");
        go.transform.SetParent(GameObject.Find("ShouPanel").transform);
        go.transform.GetChild(0).tag = "ShouPai";
        if (c >= 2)
        {
            StopAllCoroutines();
        }
    }

    IEnumerator EnemySuspend()
    {
        yield return new WaitForSeconds(3);
        switch (EnemyNum)
        {
            case 6:
                Vector2 z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -205;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 7:
                z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -222;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 8:
                z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -234;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 9:
                z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -241;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 10:
                z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -248;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
        }
        //if (c >= 3)
        //{
        //round = Round.ToCard;
        //}
        go.transform.SetParent(GameObject.Find("EnemyShouPanel").transform);
        go.transform.GetChild(0).tag = "EnemyShouPai";
    }


    IEnumerator EnemyAttDelay()
    {
        yield return new WaitForSeconds(1);//不立刻攻击 先延迟1s
        //int s = Random.Range(0, EnemyZhuoArray.Count);
        //m = s;

        Sequence sequence = DOTween.Sequence();
        Tweener m1 = enemy_attack_card.transform.DOMove(PlayerImg.transform.position, 0.5f);
        Tweener m2 = enemy_attack_card.transform.DOMove(enemy_attack_card.transform.position, 0.5f);
        sequence.Append(m1);
        sequence.Append(m2);
        yield return new WaitForSeconds(1);
        PlayerImg.GetComponent<Player>().Hurt(enemy_attack_card.GetComponent<Card>().Attack);
        switch (enemy_attack_card.GetComponent<Card>().SkillType)
        {
            case 1:
                print("敌方风怒随从");
                Camera.main.GetComponent<EnemySkill>().FengNu();
                //enemy_attack_card.GetComponent<Card>().SkillAS.Play();
                break;
        }
    }

    IEnumerator EnemyAttCardDelay()
    {
        yield return new WaitForSeconds(1);
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = enemy_attack_card.transform.DOMove(own_attacked_card.transform.position, 0.5f);
        Tweener m2 = enemy_attack_card.transform.DOMove(enemy_attack_card.transform.position, 0.5f);
        Tweener r1 = own_attacked_card.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
        Tweener r2 = own_attacked_card.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
        Tweener r3 = own_attacked_card.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
        sequence.Append(m1);
        sequence.Append(m2);
        sequence.Append(r1);
        sequence.Append(r2);
        sequence.Append(r3);
        yield return new WaitForSeconds(0.5f);
        own_attacked_card.GetComponent<Card>().CardHurt(enemy_attack_card.GetComponent<Card>().Attack);
        yield return new WaitForSeconds(1.1f);
        if (own_attacked_card.GetComponent<Card>().Hp <= 0)
        {
            if (own_attacked_card.GetComponent<Card>().SkillType == 4)
            {
                EnemyImg.GetComponent<Enemy>().Hurt(2);
                print("亡语：对英雄造成2点伤害");
            }
            print("销毁被敌方打死的随从" + (own_attacked_card.GetComponent<Card>().index));
            ZhuoPaiArray.Remove(own_attacked_card.transform.parent);
            Destroy(own_attacked_card.transform.parent.gameObject);

        }

        enemy_attack_card.GetComponent<Card>().CardHurt(own_attacked_card.GetComponent<Card>().Attack);
        if (enemy_attack_card.GetComponent<Card>().Hp <= 0)
        {
            if (enemy_attack_card.GetComponent<Card>().SkillType == 4)
            {
                PlayerImg.GetComponent<Player>().Hurt(2);
                print("亡语：对英雄造成2点伤害");
            }
            print("销毁自毙的敌方随从" + enemy_attack_card.GetComponent<Card>().index);
            EnemyZhuoArray.Remove(enemy_attack_card.transform);
            Destroy(enemy_attack_card);

        }
        switch (enemy_attack_card.GetComponent<Card>().SkillType)
        {
            case 1:
                print("敌方风怒随从");
                Camera.main.GetComponent<EnemySkill>().FengNu();
                //enemy_attack_card.GetComponent<Card>().SkillAS.Play();
                break;
        }
    }

    public void surrender()
    {
        //发报文认输
        
        connection.Surrender();

    }

    public IEnumerator csWinShow()
    {
        csWin.SetActive(true);
        csWin.GetComponent<Animator>().enabled = true;
        Camera.main.GetComponent<AudioSource>().Stop();
        GameObject.Find("csWin").GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene("menu");
    }

    public IEnumerator jrWinShow()
    {
        jrWin.SetActive(true);
        jrWin.GetComponent<Animator>().enabled = true;
        Camera.main.GetComponent<AudioSource>().Stop();
        GameObject.Find("jrWin").GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene("menu");
    }

    public void chatBubleList()
    {

        ChatBubble1.gameObject.SetActive(true);
        ChatBubble2.gameObject.SetActive(true);
        ChatBubble3.gameObject.SetActive(true);
        ChatBubble4.gameObject.SetActive(true);
        option = true;

    }

    public void chat1()
    {
        display = true;
        ChatBubble1.gameObject.SetActive(false);
        ChatBubble2.gameObject.SetActive(false);
        ChatBubble3.gameObject.SetActive(false);
        ChatBubble4.gameObject.SetActive(false);
        Bubble.SetActive(true);
        BubleText.text = text1.text;
        connection.Communication(1);
    }

    public void chat2()
    {
        display = true;
        ChatBubble1.gameObject.SetActive(false);
        ChatBubble2.gameObject.SetActive(false);
        ChatBubble3.gameObject.SetActive(false);
        ChatBubble4.gameObject.SetActive(false);
        Bubble.SetActive(true);
        BubleText.text = text2.text;
        connection.Communication(2);
    }

    public void chat3()
    {
        display = true;
        ChatBubble1.gameObject.SetActive(false);
        ChatBubble2.gameObject.SetActive(false);
        ChatBubble3.gameObject.SetActive(false);
        ChatBubble4.gameObject.SetActive(false);
        Bubble.SetActive(true);
        BubleText.text = text3.text;
        connection.Communication(3);
    }

    public void chat4()
    {
        display = true;
        ChatBubble1.gameObject.SetActive(false);
        ChatBubble2.gameObject.SetActive(false);
        ChatBubble3.gameObject.SetActive(false);
        ChatBubble4.gameObject.SetActive(false);
        Bubble.SetActive(true);
        BubleText.text = text4.text;
        connection.Communication(4);
    }

    public void clear()
    {
        for (int i = 0; i < Game._instance.ShouPaiArray.Count; i++)
        {
            Game._instance.ShouPaiArray[i].GetComponent<Image>().enabled = false;
            Game._instance.ShouPaiArray[i].GetChild(0).tag = "ShouPai";
        }
    }

}
