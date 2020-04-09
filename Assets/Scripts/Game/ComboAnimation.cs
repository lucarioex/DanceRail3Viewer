using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAnimation : MonoBehaviour {

    Vector3 start_pos;

	// Use this for initialization
	void Start ()
    {
        start_pos = transform.position;
        StartCoroutine(Scale());
	}

    IEnumerator Scale()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            transform.localScale -= (transform.localScale - Vector3.one) * Time.deltaTime * 10.0f;
            transform.position -= (transform.position - start_pos) * Time.deltaTime * 10.0f;

            yield return new WaitForEndOfFrame();
        }
    }

}
