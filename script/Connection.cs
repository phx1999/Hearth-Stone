using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

public class Connection : MonoBehaviour
{
    //connection
    private Socket socket;
    private static SocketBehaviour _singleton;
    private const int BUFFER_SIZE = 2048;
    private byte[] buffer;

    //user_message
    public string user_id;
    public string password;
    public string user_name;

    //return message
    public bool wrong_message;
    public string message;

    //return user_memssage
    public int total_game_num;
    public double win_rate;

    //return friends_info
    public int friends_num;
    public List<string> friends_online;
    public List<string> friends_offline;
    public int friends_request_num;
    public List<string> friends_request_name;
    public bool friend_request = false;
    public bool friend_change = false;
    public Hashtable friend_state;

    //return room_info
    public List<string> room_info;
    public bool room_return = false;
    public bool inroom = false;
    public int room_id;
    public bool is_room_owner;
    public bool enemy_ready = false;
    public bool roomate_change = false;
    public string roomate;

    //return in game
    public string my_hero;
    public string enemy_hero;
    public List<int> card_to_get;
    public bool my_round = false;
    public int priority;
    public int enemy_card_num;
    public bool add_enemy_card_in_ground = false;
    public int enemy_attack_num;
    public int own_attacked_card_num;
    public bool enemy_attack=false;
    public bool gameover = false;
    public bool win = false;
    public bool enemy_hurt = false;

    //scence change
    public bool start_game = false;
    public bool log = false;
    public bool reconnect = false;

    //status in game
    public int my_hero_hp ;
    public int enemy_hero_hp ;
    public int at_crystal;
    public int my_crystal;
    public int enemy_at_crystal;
    public int enemy_crystal ;
    public int rest_time;
    public List<int> card_in_hand;
    public List<string> my_card_inground;
    public List<string> enemy_card_inground;
    public bool enemy_send = false;
    public int enemy_message;
    

    Scene scene;
    /// <summary>
    /// connection
    /// </summary>
    public static SocketBehaviour Singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = FindObjectOfType<SocketBehaviour>();
            }
            return _singleton;
        }
    }
    public void Connect()
    {
        try
        {
            string host = "122.51.26.166";
            int port = 14290;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(host, port);
            print(host);
        }
        catch (Exception e)
        {
            print(e.Message);
        }

        if (socket.Connected)
        {
            print("Connected");
        }
        else
        {
            print("Connect fail");
        }
    }
    public void Send(string message)
    {
        print(message);
        if (!socket.Connected)
            return;
        byte[] msg = Encoding.ASCII.GetBytes(message);
        socket.Send(msg);
    }

    public void Disconnect()
    {
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
    private long get_time()
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));  // 当地时区
        DateTime localNow = DateTime.Now;
        long now = (long)((localNow - startTime).Ticks / 10000000);
        return now;
    }
    private void Receive()
    {
        if (!socket.Connected)
            return;

        buffer = new byte[BUFFER_SIZE];

        try
        {
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, new AsyncCallback(Receive_Callback), socket);

        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

    private void Receive_Callback(IAsyncResult ar)
    {
        if (!socket.Connected)
        {
            return;
        }

        int read = socket.EndReceive(ar);

        if (read > 0)
        {
            string responses = Encoding.UTF8.GetString(buffer);
            print(responses);
            string[] ss = Regex.Split(responses, "\r\n\r\n", RegexOptions.IgnoreCase);
            for (int i = 0; i < ss.Length; i++)
            {
                resolver(ss[i]);
            }
            Receive();
        }
    }

    /// <summary>
    /// connection
    /// </summary>

    /// <summary>
    /// 提供数据
    /// </summary>
    

    public int get_card_num()
    {
        if (card_to_get.Count == 0)
        {
            print("no card");
            return (-1);
        }
        else
        {
            int x = card_to_get[0];
            print("get card " + x);
            card_to_get.RemoveAt(0);
            return x;
        }
        
    }
    /// <summary>
    /// 提供数据
    /// </summary>
    

    // Use this for initialization
    void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        scene = SceneManager.GetActiveScene();
    }
    private void Update()
    {
        
        scene = SceneManager.GetActiveScene();
       
        if (scene.name =="Game" && !socket.Connected)
        {
            //断线了
            Login();
            //reconnect();
        }
        if(scene.name== "menu" && !socket.Connected)
        {
            SceneManager.LoadScene(0);
            print("duanxian");
        }
    }
    
  
    public void Register(string id,string pass,string name,string email)
    {
        Connect();
        Receive();   
        string message = "";
        message += "200 " + get_time() + "\r\n";
        message += id + " " + pass + "\r\n";
        message += name + "\r\n";
        message += email + "\r\n";
        message += "\r\n";
        Send(message);
        
    }



    public void Login()
    {
        Connect();
        Receive();
        string message = "";
        message += "210 " + get_time() + "\r\n";
        message += user_id + " " + password + "\r\n";
        message += "\r\n";
        Send(message);
        
    }

    public void Change_passport(string u_id,string pass,string new_pass)
    {
        Connect();
        Receive();
        string message = "";
        message += "220 " + get_time() + "\r\n";
        message += u_id + " " +pass+" " + new_pass+"\r\n";
        message += "\r\n";
        Send(message);
    }



    public void Send_email()
    {
        Connect();
        Receive();
        string message = "";
        message += "230 " + get_time() + "\r\n";
        message += user_id + "\r\n";
        message += "\r\n";
        Send(message);
        
    }

    public void Buy_packet(string user_id)
    {
        string message = "";
        message += "300 " + get_time() + "\r\n";
        message += user_id + "\r\n";
        message += "\r\n";
        Send(message);
    }
    public void Draw_card(string user_id)
    {
        string message = "";
        message += "305 " + get_time() + "\r\n";
        message += user_id + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Get_personal_card(string user_id)
    {
        
        string message = "";
        message += "310 " + get_time() + "\r\n";
        message += user_id + "\r\n";
        message += "\r\n";
        Send(message);

    }



    public void Get_list(string user_id)
    {
        
        string message = "";
        message += "315 " + get_time() + "\r\n";
        message += user_id + "\r\n";
        message += "\r\n";
        Send(message);
        
    }

    public void Creat_list(string user_id, string list_name)
    {
        
        string message = "";
        message += "320 " + get_time() + "\r\n";
        message += user_id + " " + list_name + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Get_card_inlist(string user_id, string list_name)
    {
        
        string message = "";
        message += "325 " + get_time() + "\r\n";
        message += user_id + " " + list_name + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Delet_list(string user_id, int list_name)
    {
        
        string message = "";
        message += "330 " + get_time() + "\r\n";
        message += user_id + " " + list_name + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void List_add_card(string user_id, int list_name, int personal_card_num)
    {
        
        string message = "";
        message += "340 " + get_time() + "\r\n";
        message += user_id + " " + list_name + "\r\n";
        message += personal_card_num + "\r\n";
        message += "\r\n";
        Send(message);
    }
    public void List_delete_card(string user_id, int list_name, int card_num_inlist)
    {

        string message = "";
        message += "350 " + get_time() + "\r\n";
        message += user_id + " " + list_name + "\r\n";
        message += card_num_inlist + "\r\n";
        message += "\r\n";
        Send(message);
    }



    public void Ask_friendship(string receiver_id)
    {
        
        string message = "";
        message += "400 " + get_time() + "\r\n";
        message += user_id + " " + receiver_id  + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Accept_friendship(string asker_id)
    {
        
        string message = "";
        message += "410 " + get_time() + "\r\n";
        message += user_id + " " + asker_id +  "\r\n";
        message += "\r\n";
        Send(message);

    }

    public void Reject_friendship(string asker_id)
    {
        friends_request_name.Remove(asker_id);
        friends_request_num -= 1;
        friend_request = true;
        string message = "";
        message += "415 " + get_time() + "\r\n";
        message += user_id + " " + asker_id + "\r\n";
        message += "\r\n";
        Send(message);

    }



    public void Send_message(string sender_id, string receiver_id, string messages)
    {
        
        string message = "";
        message += "420 " + get_time() + "\r\n";
        message += sender_id + " " + receiver_id + "\r\n";
        message += messages + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Delet_friend( string be_deleted_id)
    {
        
        string message = "";
        message += "430 " + get_time() + "\r\n";
        message += user_id + " " + be_deleted_id +  "\r\n";
        message += "\r\n";
        Send(message);

    }








    //// in the game using the card
    /// <summary>
    /// 对战系统
    /// </summary>
    /// <returns></returns>
    public void List_rooms()
    {
        string message = "";
        message += "500 " + get_time() + "\r\n";
        message += user_id + "\r\n";
        message += "\r\n";
        Send(message);
        
    }

    public void Creat_room()
    {
        is_room_owner = true;
        string message = "";
        message += "510 " + get_time() + "\r\n";
        message += user_id + "\r\n";
        message += "\r\n";
        Send(message);
    }   
    public void Add_room(int room_num)
    {
        is_room_owner = false;
        string message = "";
        message += "520 " + get_time() + "\r\n";
        message += user_id +" "+ room_num + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Ready()
    {
        string message = "";
        message += "530 " + get_time() + "\r\n";
        message += user_id+" "+room_id + "\r\n";
        message += "\r\n";
        Send(message);
    }
    public void cancel_Ready()
    {
        string message = "";
        message += "535 " + get_time() + "\r\n";
        message += user_id+" "+room_id + "\r\n";
        message += "\r\n";
        Send(message);
    }


    public void Quit_room()
    {
        string message = "";
        message += "545 " + get_time() + "\r\n";
        message += user_id+" "+room_id + "\r\n";
        message += "\r\n";
        Send(message);
    }


    public void Start_game()
    {
        string message = "";
        message += "550 " + get_time() + "\r\n";
        message += user_id+ " "+room_id + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Change_card_ingame(string user_id, string card_number)
    {
        string message = "";
        message += "560 " + get_time() + "\r\n";
        message += user_id + " " + card_number + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Get_card_ingame()
    {
        string message = "";
        message += "570 " + get_time() + "\r\n";
        message += user_id+" "+room_id + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Play_card(int card_num)
    {
        string message = "";
        message += "580 " + get_time() + "\r\n";
        message += user_id+" "+room_id + "\r\n";
        message += card_num + "\r\n";
        message += "\r\n";
        Send(message);
        
    }

    public void Play_card(int card_num,int target)
    {
        string message = "";
        message += "585 " + get_time() + "\r\n";
        message += user_id + " " + room_id + "\r\n";
        message += card_num +" " + target+ "\r\n";
        message += "\r\n";
        Send(message);

    }

    public void Attack_card(int card_attacker_num,int card_attacked_num)
    {
        
        string message = "";
        message += "590 " + get_time() + "\r\n";
        message += user_id + " " + room_id + "\r\n";
        message += card_attacker_num +" "+card_attacked_num+ "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Round_end()
    {
        string message = "";
        message += "600 " + get_time() + "\r\n";
        message += user_id + " " + room_id + "\r\n";
        message += priority + "\r\n";
        message += "\r\n";
        Send(message);
    }

    //public void Remove_card(string user_id, string card_num)
    //{

    //    string message = "";
    //    message += "600 " + get_time() + "\r\n";
    //    message += user_id + " " +  card_num + "\r\n";
    //    message += "\r\n";
    //    Send(message);
    //}
    

    public void Communication( int messages)
    {
        string message = "";
        message += "570 " + get_time() + "\r\n";
        message +=user_id+" "+ room_id + "\r\n";
        message += messages +"\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Surrender()
    {
        string message = "";
        message += "710 " + get_time() + "\r\n";
        message += user_id + " " +room_id+ "\r\n";
        message += "\r\n";
        Send(message);
    }

    ////finish the game

    public void Add_game_record(string user_id, bool win)
    {
        string message = "";
        message += "720 " + get_time() + "\r\n";
        message += user_id + " " + win + "\r\n";
        message += "\r\n";
        Send(message);
    }

    public void Add_gold(string user_id, int gold_num)
    {
        string message = "";
        message += "730 " + get_time() + "\r\n";
        message += user_id + " " + gold_num + "\r\n";
        message += "\r\n";
        Send(message);
    }


    //public void reconnect()
    //{
    //    string message = "";
    //    message += "800 " + get_time() + "\r\n";
    //    message += user_id +"\r\n";
    //    message += "\r\n";
    //    Send(message);
    //}


    //Update is called once per frame

   
    //private string Receive()
    //{
    //    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback((ar) =>
    //        {
    //    var length = socket.EndReceive(ar);
    //    //读取出来消息内容
    //    this.reply = Encoding.Unicode.GetString(buffer, 0, length);
    //    //显示消息
    //    Console.WriteLine(reply);

    //}), null);
    //    return this.reply;
    //}

    
    public void resolver(string resonses)
    {
        resonses = Regex.Replace(resonses, @"[\0]", "");
        string[] line = Regex.Split(resonses, "\r\n", RegexOptions.IgnoreCase);
        string First_line = line[0];
        string Method_code = Regex.Split(First_line, " ", RegexOptions.IgnoreCase)[0];
        print(Method_code);
        if (Method_code.Equals("000"))
        {
            //错误信息
            wrong_message = true;
            message = line[1];
        }
        else if (Method_code.Equals("020"))
        {
            //断线重连信息
            my_hero_hp = int.Parse(Regex.Split(line[1], " ")[0]);
            enemy_hero_hp = int.Parse(Regex.Split(line[1], " ")[1]);
            print(0);
            at_crystal = int.Parse(Regex.Split(line[2], " ")[0]);
            my_crystal = int.Parse(Regex.Split(line[2], " ")[1]);
            enemy_at_crystal = int.Parse(Regex.Split(line[2], " ")[2]);
            enemy_crystal= int.Parse(Regex.Split(line[2], " ")[3]);
            print(1);
            if (int.Parse(Regex.Split(line[3], " ")[0]) == 1)
            {
                my_round = true;
                rest_time = int.Parse(Regex.Split(line[3], " ")[1]);
            }else
            {
                my_round = false;
            }
            print(2);
            string[] card_message = Regex.Split(line[4], " ");
            card_to_get = new List<int>();
            for (int i = 0; i < card_message.Length-1; i++)
            {
                card_to_get.Add(int.Parse(card_message[i]));
            }
            card_message = Regex.Split(line[5], " ");
            card_in_hand = new List<int>();
            print(card_message.Length);
            for (int i = 0; i < card_message.Length-1; i++)
            {
                card_in_hand.Add(int.Parse(card_message[i]));
            }
            print(3);
            int num = int.Parse(line[6]);
            my_card_inground = new List<string>();
            for (int i = 0; i < num; i++)
            {
                my_card_inground.Add(line[i+6]);
            }
            int line_order = 6 + num+1;
            num = int.Parse(line[line_order]);
            enemy_card_inground = new List<string>();
            print(4);
            for (int i = 0; i < num; i++)
            {
                enemy_card_inground.Add(line[line_order+i]);
            }
            print("reconnect");
            reconnect = true;
        }
        else if (Method_code.Equals("100"))
        {
            string friends_name = line[1].Replace("['", "");
            friends_name = friends_name.Replace("'", "");
            friends_name = friends_name.Replace("]", "");
            friends_name = Regex.Split(friends_name, ",")[0];
            friends_offline.Remove(friends_name);
            print(friends_name);
            if (!friends_online.Contains(friends_name))
                friends_online.Add(friends_name);
            friend_state.Add(friends_name, 0);
            friend_change = true;
        }
        else if (Method_code.Equals("101"))
        {
            friends_online.Remove(line[1]);
            friends_offline.Add(line[1]);
            friend_state.Remove(line[1]);
            friend_change = true;
        }
        else if (Method_code.Equals("102"))
        {
            friend_state.Remove(line[1]);
            friend_state.Add(line[1], 1);
            friend_change = true;
        }
        else if (Method_code.Equals("103"))
        {
            friend_state.Remove(line[1]);
            friend_state.Add(line[1], 0);
            friend_change = true;
        }
        else if (Method_code.Equals("104"))
        {
            friend_state.Remove(line[1]);
            friend_state.Add(line[1], 2);
            friend_change = true;
        }
        else if (Method_code.Equals("105"))
        {
            friend_state.Remove(line[1]);
            friend_state.Add(line[1], 1);
            friend_change = true;
        }
        else if (Method_code.Equals("106"))
        {
            
            string str = Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[0] + " 0";
            room_info.Add(str);
            room_return = true;
        }
        else if (Method_code.Equals("107"))
        {
          
            room_info.Remove(line[1] + " 0");
            room_return = true;
        }
        else if (Method_code.Equals("108"))
        {
           
            room_info.Remove(line[1] + " 0");
            room_info.Add(line[1] + " 1");
            room_return = true;
        }
        else if (Method_code.Equals("109"))
        {
          
            room_info.Remove(line[1] + " 1");
            room_info.Add(line[1] + " 0");
            room_return = true;
        }
        else if (Method_code.Equals("110"))
        {
  
            room_info.Remove(line[1] + " 1");
            room_info.Add(line[1] + " 2");
            room_return = true;
        }
        else if (Method_code.Equals("111"))
        {
           
            room_info.Remove(line[1] + " 2");
            room_info.Add(line[1] + " 1");
            room_return = true;
        }
        else if (Method_code.Equals("112")) {

            is_room_owner = true;
        }
        else if (Method_code.Equals("113"))
        {
            roomate_change = true;
            roomate = Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[1];
        }
        else if (Method_code.Equals("114"))
        {
            roomate_change = true;
            roomate = "";
        }
        else if (Method_code.Equals("115"))
        {
            roomate_change = true;
            enemy_ready = true;
        }
        else if (Method_code.Equals("116"))
        {
            roomate_change = true;
            enemy_ready = false;
        }
        else if (Method_code.Equals("201"))
        {
            //拒绝注册
            message = line[1];
            wrong_message = true;
            print("拒绝注册");
        }
        else if (Method_code.Equals("202"))
        {
            print("注册成功");
            //注册成功
        }
        else if (Method_code.Equals("211"))
        {
            wrong_message = true;
            message = line[1];
            print("拒绝登陆");

            //拒绝登陆
        }
        else if (Method_code.Equals("212"))
        {
            print("登陆成功");
            user_name = Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[0];
            total_game_num = int.Parse(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[1]);
            win_rate = double.Parse(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[2]);
            friends_num = int.Parse(line[2]);
            friends_online = new List<string>();
            friends_offline = new List<string>();
            friend_state = new Hashtable();
            int online_num = 0;
            int offline_num = 0;
            for (int i = 0; i < friends_num; i++)
            {
                if (Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[2].Equals("1"))
                {
                    friends_online.Add(Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[0]);
                    friend_state.Add(Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[0], 0);
                    online_num += 1;
                }
                else if (Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[2].Equals("3") || Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[2].Equals("2"))
                {
                    friends_online.Add(Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[0]);
                    friend_state.Add(Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[0], 1);
                    online_num += 1;
                }
                else if (Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[2].Equals("4"))
                {
                    friends_online.Add(Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[0]);
                    friend_state.Add(Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[0], 2);
                    online_num += 1;
                }
                else
                {
                    friends_offline.Add(Regex.Split(line[3 + i], " ", RegexOptions.IgnoreCase)[0]);
                    offline_num += 1;
                }
            }
            friends_request_name = new List<string>();
            friends_request_num = int.Parse(line[3 + friends_num]);
            for (int i = 0; i < friends_request_num; i++)
            {
                friends_request_name.Add(line[4 + friends_num + i]);
            }
            log = true;
            //登陆成功

        }
        else if (Method_code.Equals("221"))
        {
            //密码修改失败
        }
        else if (Method_code.Equals("222"))
        {
            //密码修改成功
            wrong_message = true;
            message = "修改密码成功";
        }
        else if (Method_code.Equals("231"))
        {
            //邮箱修改密码失败
        }
        else if (Method_code.Equals("232"))
        {
            //邮箱修改密码成功
        }
        else if (Method_code.Equals("301"))
        {
            //购买卡包失败
        }
        else if (Method_code.Equals("302"))
        {
            //购买卡包成功
        }
        else if (Method_code.Equals("306"))
        {
            //抽卡成功
        }
        else if (Method_code.Equals("307"))
        {
            //抽卡失败
        }
        else if (Method_code.Equals("311"))
        {
            //获取个人拥有卡牌失败
        }
        else if (Method_code.Equals("312"))
        {
            //获取个人拥有卡牌成功

        }
        else if (Method_code.Equals("316"))
        {
            //获取卡组失败
        }
        else if (Method_code.Equals("317"))
        {
            //获取卡组成功
        }
        else if (Method_code.Equals("321"))
        {
            //新建卡组失败
        }
        else if (Method_code.Equals("322"))
        {
            //新建卡组成功
        }
        else if (Method_code.Equals("326"))
        {
            //获取卡组中卡牌失败
        }
        else if (Method_code.Equals("327"))
        {
            //获取卡组中卡牌成功
        }
        else if (Method_code.Equals("331"))
        {
            //删除卡组失败
        }
        else if (Method_code.Equals("332"))
        {
            //删除卡组成功
        }
        else if (Method_code.Equals("341"))
        {
            //卡组添加牌失败
        }
        else if (Method_code.Equals("342"))
        {
            //卡组添加牌成功
        }
        else if (Method_code.Equals("351"))
        {
            //卡组删除牌失败
        }
        else if (Method_code.Equals("352"))
        {
            //卡组删除牌成功
        }
        else if (Method_code.Equals("400"))
        {
            //接收请求好友
            friends_request_name.Add(line[1]);
            friends_request_num += 1;
            friend_request = true;
            print("有人申请好友");
        }
        else if (Method_code.Equals("401"))
        {
            //请求好友失败

        }
        else if (Method_code.Equals("402"))
        {
            //好友改变
            if (friends_request_name.Contains(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[0]))
            {
                friends_request_name.Remove(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[0]);
                friends_request_num -= 1;
                friend_request = true;
            }
            if (Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[1].Equals("1"))
            {
                friends_online.Add(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[0]);
                friend_state.Add(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[0], 0);
            }
            else
            {
                friends_offline.Add(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[0]);
            }
            friends_num += 1;
            friend_change = true;
            print("好友增加");
        }
        else if (Method_code.Equals("411"))
        {
            //接收好友申请失败
            wrong_message = true;
            message = line[1] + " " + line[2];
            print("接收好友失败");
        }
        else if (Method_code.Equals("412"))
        {
            //接收好友申请成功

            print("接受好友成功");

        }
        else if (Method_code.Equals("416"))
        {
            wrong_message = true;
            message = line[1] + " " + line[2];
            print("拒绝好友申请失败");
        }
        else if (Method_code.Equals("417"))
        {
            if (friends_request_name.Contains(line[1]))
            {
                friends_request_name.Remove(line[1]);
                friends_request_num -= 1;
                friend_request = false;
            }
            print("拒绝好友成功");
        }
        else if (Method_code.Equals("421"))
        {
            //发送消息失败
        }
        else if (Method_code.Equals("422"))
        {
            //发送消息成功
        }
        else if (Method_code.Equals("431"))
        {
            //删除好友成功
        }
        else if (Method_code.Equals("432"))
        {
            //删除好友失败
            if (friends_online.Contains(line[1]))
            {
                friends_online.Remove(line[1]);
                friend_state.Remove(line[1]);
            }
            else
            {
                friends_offline.Remove(line[1]);
            }
            friends_num -= 1;
            friend_change = true;

        }
        else if (Method_code.Equals("433"))
        {
            //被好友失败
            if (friends_online.Contains(line[1]))
            {
                friends_online.Remove(line[1]);
                friend_state.Remove(line[1]);
            }
            else
            {
                friends_offline.Remove(line[1]);
            }
            friends_num -= 1;
            friend_change = true;

        }
        else if (Method_code.Equals("501"))
        {
            //展示房间失败
        }
        else if (Method_code.Equals("502"))
        {
            //展示房间成功
            int room_num = int.Parse(line[1]);
            room_info = new List<string>();
            for (int i = 2; i < 2 + room_num; i++)
            {
                string str = Regex.Split(line[i], " ", RegexOptions.IgnoreCase)[0] + " " + Regex.Split(line[i], " ", RegexOptions.IgnoreCase)[1];
                room_info.Add(str);
            }
            room_return = true;
            print(room_info.Count);

        }
        else if (Method_code.Equals("511"))
        {
            //创建房间失败
            wrong_message = true;
            message = line[1];
        }
        else if (Method_code.Equals("512"))
        {
            //创建房间成功
            room_id = int.Parse(line[1]);
            roomate_change = true;
            inroom = true;
        }
        else if (Method_code.Equals("521"))
        {
            //加入房间失败
            wrong_message = true;
            message = line[1];
        }
        else if (Method_code.Equals("522"))
        {
            //加入房间成功
            inroom = true;
            room_id = int.Parse(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[0]);
            roomate = Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[1];
            roomate_change = true;
        }
        else if (Method_code.Equals("531"))
        {
            //准备游戏失败
        }
        else if (Method_code.Equals("532"))
        {
            //准备游戏成功
            wrong_message = true;
            message = line[2];
        }
        else if (Method_code.Equals("536"))
        {
            //取消准备失败
            wrong_message = true;
            message = line[2];
        }
        else if (Method_code.Equals("537"))
        {
            //取消准备成功
        }
        else if (Method_code.Equals("546"))
        {
            //离开房间失败
            wrong_message = true;
            message = line[2];
        }
        else if (Method_code.Equals("547"))
        {
            //离开房间成功
            room_return = true;
        }
        else if (Method_code.Equals("551"))
        {
            //开始游戏失败
            wrong_message = true;
            message = line[2];
        }
        else if (Method_code.Equals("552"))
        {
            //开始游戏成功x
            priority = int.Parse(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[1]);
            card_to_get = new List<int>();
            if (priority == 0)
            {
                my_round = true;
            }
            else
            {
                my_round = false;
            }
            my_hero = line[2];
            enemy_hero = line[3];
            for (int i = 0; i < 35; i++)
            {
                card_to_get.Add(int.Parse(line[i + 4]));
            }

            start_game = true;
        }
        else if (Method_code.Equals("561"))
        {
            //开局换卡失败
        }
        else if (Method_code.Equals("562"))
        {
            //开局换卡成功
        }
        else if (Method_code.Equals("571"))
        {
            //游戏抽卡失败
        }
        else if (Method_code.Equals("572"))
        {
            //游戏抽卡成功
            card_to_get.Add(int.Parse(line[2]));
            my_round = true;
        }
        else if (Method_code.Equals("581"))
        {
            //打出卡牌失败
        }
        else if (Method_code.Equals("582"))
        {
            //对手放牌
            enemy_card_num = int.Parse(line[1]);
            add_enemy_card_in_ground = true;

        }
        else if (Method_code.Equals("591"))
        {
            //卡牌攻击失败
        }
        else if (Method_code.Equals("592"))
        {
            //卡牌攻击成功
            enemy_attack_num = int.Parse(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[0]);
            own_attacked_card_num = int.Parse(Regex.Split(line[1], " ", RegexOptions.IgnoreCase)[1]);
            enemy_attack = true;
        }
        else if (Method_code.Equals("603"))
        {
            //回合结束失败
            Game._instance.round = Game.Round.Enemy;
        }
        else if (Method_code.Equals("602"))
        {
            //接收回合结束
            my_round = !my_round;
        }
        else if (Method_code.Equals("610"))
        {
            gameover = true;
            if (line[1].Equals(user_id))
            {
                win = true;
                win_rate += (1 + win_rate * total_game_num)/(total_game_num+1);
                total_game_num += 1;
            }
            else
            {
                win = false;
                win_rate += (win_rate * total_game_num) / (total_game_num + 1);
                total_game_num += 1;
            }
        }
        else if (Method_code.Equals(620))
        {
            enemy_hurt = true;
        }

        else if (Method_code.Equals("570"))
        {
            //接受游戏交流
            enemy_send = true;
            enemy_message = int.Parse(line[2]);
        }
        else if (Method_code.Equals("711"))
        {
            //游戏投降失败
        }
        else if (Method_code.Equals("712"))
        {
            //游戏投降成功
        }
        else if (Method_code.Equals("721"))
        {
            //增加战绩失败
        }
        else if (Method_code.Equals("722"))
        {
            //增加战绩成功
        }
        else if (Method_code.Equals("731"))
        {
            //增加金币失败
        }
        else if (Method_code.Equals("732"))
        {
            //增加金币成功
        }
        
    }


}
