using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour {

    Text text;
    float time = 0;

	// Use this for initialization
	void Start () {
        text = gameObject.GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        if(time>=2)
        {
            ControlText(text);
        }
	}

    public void ControlText(Graphic graphic)
    {
        Color c = graphic.color;
        Sequence sequence = DOTween.Sequence();
        Tweener a1 = graphic.DOColor(new Color(c.r, c.g, c.b, 0), 1f);
        Tweener a2 = graphic.DOColor(new Color(c.r, c.g, c.b, 1), 1f);
        sequence.Append(a1);
        sequence.Append(a2);
        time = 0;
    }

}
