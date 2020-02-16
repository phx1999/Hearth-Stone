using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class register : MonoBehaviour
{
    Connection connection;
    GameObject fail_to_register;
    // Start is called before the first frame update
    void Start()
    {
        connection = GameObject.Find("Connection").GetComponent<Connection>();
        fail_to_register = GameObject.Find("fail_to_register");
        fail_to_register.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (connection.wrong_message)
        {
            fail_to_register.SetActive(true);
            fail_to_register.transform.Find("Text").gameObject.GetComponent<Text>().text = connection.message;
            connection.wrong_message = false;
        }
    }
    public void registe()
    {
        connection.Register(GameObject.Find("userid").GetComponent<InputField>().text,
            GameObject.Find("password").GetComponent<InputField>().text,
            GameObject.Find("email").GetComponent<InputField>().text,
            GameObject.Find("username").GetComponent<InputField>().text
            );
    }

    public void close_message()
    {
        fail_to_register.SetActive(false);
    }
    public void move_to_login()
    {

        SceneManager.LoadScene("login");

    }

    
}
