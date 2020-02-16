using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;

public class menu : MonoBehaviour
{
    // Start is called before the first frame update
    Connection connection;
    GameObject wrong_message;
    Text message;
    List<string> friends_online;
    List<string> friends_offline;
    int friends_num;
    List<string> friends_request_name;
    GameObject friend_accept_list;
    int friends_request_num;
    GameObject[] button = new GameObject[15];
    GameObject[] friends_request_list = new GameObject[15];
    bool opend_request_list = false;
    GameObject room;
    GameObject pp;
    GameObject quit_room_list;
    GameObject creat_room;
    GameObject[] room_list=new GameObject[14];
    GameObject room_inner;
    bool room_list_opened = false;
    bool is_ready = false;
    GameObject enemy_ready;
    GameObject already_ready;
    GameObject Ready;
    GameObject start_game;
    void Start()
    {   
        connection = GameObject.Find("Connection").GetComponent<Connection>();
        already_ready = GameObject.Find("Already_ready");
        Ready= GameObject.Find("ready");
        start_game= GameObject.Find("Startgame");
        friends_online = connection.friends_online;
        friends_offline = connection.friends_offline;
        friends_num = connection.friends_num;
        friends_request_name = connection.friends_request_name;
        friends_request_num = connection.friends_request_num;
        wrong_message = GameObject.Find("wrong_message");
        wrong_message.SetActive(false);
        message = wrong_message.transform.Find("close_message").transform.Find("Text").GetComponent<Text>();
        connection.List_rooms();
        prepare_to_room();
        room_inner = GameObject.Find("room");
        room_inner.SetActive(false);
        enemy_ready = room_inner.transform.Find("enemy_ready").gameObject;
        GameObject text = GameObject.Find("score");
        
        quit_room_list = GameObject.Find("quit_room_list");
        quit_room_list.SetActive(false);
        creat_room= GameObject.Find("creat_room");
        creat_room.SetActive(false);
        room = GameObject.Find("roombg");
        pp = GameObject.Find("pp");
        room.SetActive(false);
        pp.SetActive(false);
        Text t = text.GetComponent<Text>();
        t.text = Math.Round(connection.win_rate, 2) + " " + connection.total_game_num;
        GameObject.Find("friends_accept_num").GetComponent<Text>().text = friends_request_num + "";
        hide_firend_request();
        hide_friends();
        show_friends();

    }

    public void end_game()
    {
        connection.Disconnect();
        print("end");
        Application.Quit();
    }

    public void start()
    {
        if (connection.enemy_ready)
        {
            connection.Start_game();
        }
        else
        {
            print("玩家未准备");
        }
    }

    public void ready()
    {
        
        if (is_ready)
        {
            is_ready = false;
            connection.cancel_Ready();
            already_ready.SetActive(false);
        }
        else
        {
            is_ready = true ;
            already_ready.SetActive(true);
            connection.Ready();
        }
    }

    public void close_message()
    {
        wrong_message.SetActive(false);
    }

    public void move_to_game()
    {
        if (connection.room_info.Count >= 13)
        {
            wrong_message.SetActive(true);
            message.text = "房间数已满";
        }
        else {
            connection.Creat_room();
            room_inner.SetActive(true);
            Ready.SetActive(false);
            already_ready.SetActive(false);
            start_game.SetActive(true);
        }
    }

    public void quit_room_list_move_to_menu()
    {
        room.SetActive(false);
        pp.SetActive(false);
        quit_room_list.SetActive(false);
        creat_room.SetActive(false);
        room_list_opened = false;
        connection.roomate = "";
    }
    public void prepare_to_room()
    {
        
        for (int i = 0; i < 14; i++)
        {
            room_list[i] = GameObject.Find("r" + i);
            room_list[i].SetActive(false);
        }
        
    }


    public void show_rooms()
    {
        pp.SetActive(true);
        room.SetActive(true);
        quit_room_list.SetActive(true);
        creat_room.SetActive(true);
        room_list_opened = true;
        for (int i = 0; i < 14; i++)
        {
            room_list[i].SetActive(false);
        }
        List<string> room_info = connection.room_info;
        for (int i = 0; i < room_info.Count; i++)
        {
            int num = int.Parse(Regex.Split(room_info[i], " ", RegexOptions.IgnoreCase)[0]);
            room_list[num - 1].SetActive(true);
            Text tt = room_list[num - 1].transform.Find("text").gameObject.GetComponent<Text>();
            string st = Regex.Split(room_info[i], " ", RegexOptions.IgnoreCase)[1];
            if (st.Equals("0"))
                tt.text = "房间 " + num + " 1/2";
            else if (st.Equals("1"))
                tt.text = "房间 " + num + " 2/2";
            else 
                tt.text = "房间 " + num + " 游戏中";
            tt.fontSize = 40;
            tt.color = Color.white;
        }
        connection.room_return = false;
    }
    public void Join_room(int num)
    {
        Text tt = room_list[num-1].transform.Find("text").gameObject.GetComponent<Text>();
        if (!Regex.Split(tt.text, " ", RegexOptions.IgnoreCase)[2].Equals("游戏中")) {
            
            room_inner.SetActive(true);
            is_ready = false;
            already_ready.SetActive(false);
            connection.Add_room(num);
            start_game.SetActive(false);
            Ready.SetActive(true);
        }

    }

    public void show_roomate()
    {
        quit_room_list.SetActive(false);
        creat_room.SetActive(false);
        room_list_opened = false;
        string roomate = connection.roomate;
        Text t = room_inner.transform.Find("roomate").gameObject.GetComponent<Text>();
        print(t);
        t.text = roomate;
        if (connection.enemy_ready)
        {
            enemy_ready.SetActive(true);
        }
        else
        {
            enemy_ready.SetActive(false);
        }
        connection.roomate_change = false;
        
    }
    public void quit_room()
    {
        connection.Quit_room();
        room_inner.SetActive(false);
        room_list_opened = true;
        connection.is_room_owner = false;
        connection.enemy_ready = false;
        connection.roomate = "";
        show_rooms();
    }

    public void move_to_cardlist()
    {
        SceneManager.LoadScene("cardlist");
    }
    public void hide_firend_request()
    {
        friend_accept_list = GameObject.Find("friend_accept_list");
        for (int i = 0; i < 15; i++)
        {
            friends_request_list[i] = GameObject.Find("Text (" + i+")");
            friends_request_list[i].SetActive(false);
        }
        friend_accept_list.SetActive(false);
        opend_request_list = false;
    }
    public void show_firend_request()
    {
        if (opend_request_list)
        {
            friend_accept_list.SetActive(false);
            opend_request_list = false;
        }
        else { 
            friend_accept_list.SetActive(true);
            for(int i = 0; i < friends_request_num; i++)
            {
                friends_request_list[i].SetActive(true);
                friends_request_list[i].GetComponent<Text>().text = friends_request_name[i];
            }
            opend_request_list = true;
        }
    }

    public void accept_friend_request(int oder)
    {
        connection.Accept_friendship(friends_request_name[oder]);
    }

    public void request_friend()
    {
        string request_name = GameObject.Find("friend_request").GetComponent<InputField>().text;
        connection.Ask_friendship(request_name);
    }

    public void delet_friend(int friend_order)
    {
        if (friend_order < friends_online.Count)
        {
            connection.Delet_friend(friends_online[friend_order]);

        }
        else
        {
            connection.Delet_friend(friends_offline[friend_order - friends_online.Count]);
        }
    }

    void reject_friend_request(int friend_order)
    {
        connection.Reject_friendship(friends_request_name[friend_order]);    
    }

    void hide_friends()
    {
        for (int i = 0; i < 15; i++)
        {
            button[i] = GameObject.Find("friend" + i);
            button[i].SetActive(false);
        }
    }

    void show_friends()
    {   
        int online_num = friends_online.Count;
        Button b;
        GameObject text;
        Text t;
        for (int i = 0; i < 15; i++)
        {
            if (i < online_num)
            {
                button[i].SetActive(true);
                text = GameObject.Find("friends_name" + i);
                t = text.GetComponent<Text>();
                if ((int)connection.friend_state[friends_online[i]]==0)
                    t.text = friends_online[i]+ "     online";
                else if((int)connection.friend_state[friends_online[i]] == 1)
                    t.text = friends_online[i] + "     in room";
                else
                    t.text = friends_online[i] + "     in game";
                t.fontSize = 30;
                t.fontStyle = FontStyle.Bold;
                t.color = Color.white;
                b = button[i].GetComponent<Button>();
                b.onClick.AddListener(Show);
            }
            else if(i<friends_num){
                button[i].SetActive(true);
                text = GameObject.Find("friends_name" + i);
                t = text.GetComponent<Text>();
                t.text = friends_offline[i-online_num]+"     offline";
                t.fontSize = 30;
                t.fontStyle = FontStyle.Bold;
                t.color = Color.white;
            }
        }
    }
    void Show()
    {
        print("show");
    }


    void refresh_request()
    {
        friends_request_name = connection.friends_request_name;
        friends_request_num = connection.friends_request_num;
        GameObject.Find("friends_accept_num").GetComponent<Text>().text = friends_request_num + "";
        if (friend_accept_list.activeSelf)
        {
            for(int i = 0; i < 15; i++)
            {
                if (friends_request_list[i].activeSelf)
                    friends_request_list[i].SetActive(false);
            }
            show_firend_request();
        }
        connection.friend_request = false;
    }

    void refresh_friend()
    {
        friends_online = connection.friends_online;
        friends_offline = connection.friends_offline;
        friends_num = connection.friends_num;
        for (int i = 0; i < 15; i++)
        {
            if (button[i].activeSelf)
                button[i].SetActive(false);
        }
        show_friends();
        connection.friend_change = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (connection.friend_request)
        {
            refresh_request();
        }
        if (connection.friend_change)
        {
            refresh_friend();
        }
        if (connection.room_return)
        {   if(room_list_opened)
                show_rooms();
        }
        if (connection.roomate_change)
        {  
            show_roomate();
        }
        if (connection.start_game)
        {
            SceneManager.LoadScene("Game");
            connection.start_game = false;
        }
        if (connection.roomate.Equals(""))
        {
            Ready.SetActive(false);
            enemy_ready.SetActive(false);
            start_game.SetActive(true);
        }
        if (connection.is_room_owner)
        {
            Ready.SetActive(false);
            already_ready.SetActive(false);
            start_game.SetActive(true);
        }
        else
        {
            Ready.SetActive(true);
            if (is_ready)
                already_ready.SetActive(true);
            else
                already_ready.SetActive(false);
            start_game.SetActive(false);
        }
        if (connection.wrong_message)
        {
            wrong_message.SetActive(true);
            message.text = connection.message;
            connection.wrong_message = false;
        }

        if (connection.reconnect)
        {
            SceneManager.LoadScene("Game");
        }
    }
}
