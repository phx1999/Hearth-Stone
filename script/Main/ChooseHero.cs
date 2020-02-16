using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChooseHero : MonoBehaviour,IPointerClickHandler {

    public Image image;
    public Text text;

    public void OnPointerClick(PointerEventData eventData)
    {
        image.sprite = eventData.pointerEnter.GetComponent<Image>().sprite;
        VS._instance.PlayerImg.sprite = image.sprite;
        //Debug.Log("当前点击的UI是：" + eventData.pointerEnter.name);
        switch(eventData.pointerEnter.name)
        {
            case "hero1":
                text.text = "吉安娜·普罗德摩尔";
                break;
            case "hero2":
                text.text = "雷克萨";
                break;
            case "hero3":
                text.text = "乌瑟尔•光明使者";
                break;
            case "hero4":
                text.text = "加尔鲁什•地狱咆哮";
                break;
            case "hero5":
                text.text = "玛法里奥•怒风";
                break;
            case "hero6":
                text.text = "古尔丹";
                break;
            case "hero7":
                text.text = "萨尔";
                break;
            case "hero8":
                text.text = "安度因•乌瑞恩";
                break;
            case "hero9":
                text.text = "瓦莉拉•萨古纳尔";
                break;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
