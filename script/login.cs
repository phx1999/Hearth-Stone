using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class login : MonoBehaviour
{
    // Start is called before the first frame update
    Connection connection;
    private GameObject fail_to_log;
    void Start()
    {
        connection = GameObject.Find("Connection").GetComponent<Connection>();
        fail_to_log = GameObject.Find("wrong_message");
        fail_to_log.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (connection.log)
        {
            print("move");
            move_to_menu();
            connection.log = false;
        }
        if (connection.wrong_message)
        {
            fail_to_log.SetActive(true);
            fail_to_log.transform.Find("close_message").transform.Find("Text").GetComponent<Text>().text = connection.message;
            connection.wrong_message = false;
        }
    }
    public void game_end()
    {
        connection.Disconnect();
        print("end");
        Application.Quit();
    }
    public void log()
    {
        connection.user_id = GameObject.Find("userid").GetComponent<InputField>().text;
        connection.password = GameObject.Find("password").GetComponent<InputField>().text;
        connection.Login();
    }

    public void move_to_change_password()
    {

        SceneManager.LoadScene("change_password");
    }
    public void move_to_register()
    {

        SceneManager.LoadScene("register");
    }

    private void move_to_menu()
    {
        SceneManager.LoadScene("menu");
    }
    public void close_message()
    {
        fail_to_log.SetActive(false);
    }

    public void refresh()
    {
        fail_to_log.SetActive(true);
        fail_to_log.transform.Find("close_message").transform.Find("Text").GetComponent<Text>().text = "滚，没做这个功能";
    }

}
