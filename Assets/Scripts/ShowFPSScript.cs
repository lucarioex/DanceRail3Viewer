using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFPSScript : MonoBehaviour {

    Text text;
	int counter = 0;
	WaitForSeconds one = new WaitForSeconds(1.0f);

	void Start () 
	{
        text = GetComponent<Text>();
		StartCoroutine(ShowCount());
	}

	// Update is called once per frame
	void Update()
	{
		counter++;

	}

	IEnumerator ShowCount()
	{
		while(true)
		{

			text.text = "FPS : "+counter;//"FPS : " + (int)(1.0f / Time.deltaTime);// + " IPS : " + (int)(1.0f / Time.fixedDeltaTime);
			counter = 0;
			yield return one;
		}
	}

}
