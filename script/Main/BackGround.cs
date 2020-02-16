using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BackGround : MonoBehaviour {

    Image image;
    float time = 0;

	// Use this for initialization
	void Start () {
        image = gameObject.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {

        time += Time.deltaTime;
        if (time >= 4)
        {
            ControlImage(image);
        }

    }

    public void ControlImage(Graphic graphic)
    {
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = graphic.transform.DOScale(new Vector3(0.6f, 0.6f, 1), 2f);
        Tweener m2 = graphic.transform.DOScale(new Vector3(1, 1, 1), 2f);
        sequence.Append(m1);
        //sequence.AppendInterval(2f);
        sequence.Append(m2);
        time = 0;
    }

}
