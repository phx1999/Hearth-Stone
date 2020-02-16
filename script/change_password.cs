using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class change_password : MonoBehaviour
{
    Connection connection;
    GameObject wrong_message;
    Text message;
    // Start is called before the first frame update
    void Start()
    {
        connection = GameObject.Find("Connection").GetComponent<Connection>();
        wrong_message = GameObject.Find("wrong_message");
        wrong_message.SetActive(false);
        message = wrong_message.transform.Find("close_message").transform.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (connection.wrong_message)
        {
            wrong_message.SetActive(true);
            message.text = connection.message;
            connection.wrong_message = false;
        }
    }
    public void close_message()
    {
        wrong_message.SetActive(false);
    }
    public void change_pass()
    {
        if (!GameObject.Find("newpassword").GetComponent<InputField>().text.Equals(GameObject.Find("newpassword_check").GetComponent<InputField>().text))
        {
            message.text = "两次输入密码不一致";
            wrong_message.SetActive(true);
        }
        connection.Change_passport(GameObject.Find("userid").GetComponent<InputField>().text,
             GameObject.Find("password").GetComponent<InputField>().text,
            GameObject.Find("newpassword").GetComponent<InputField>().text
            );
    }

    public void move_to_login()
    {
        SceneManager.LoadScene("login");

    }
}
