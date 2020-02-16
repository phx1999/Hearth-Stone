using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VS : MonoBehaviour {

    public static VS _instance;
    public Image vsImg;
    public Image PlayerImg;
    public Image EnemyImg;
    public GameObject vsPanel;

    // Use this for initialization
    void Start () {
        _instance = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move()
    { 
        vsPanel.SetActive(true);
        vsImg.transform.DOScale(new Vector3(1, 1, 1), 1);
        PlayerImg.transform.DOMoveY(PlayerImg.transform.position.y - 1120, 1);
        EnemyImg.transform.DOMoveY(EnemyImg.transform.position.y + 1120, 1);
    }

}
