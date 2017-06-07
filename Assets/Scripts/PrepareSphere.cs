using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareSphere : MonoBehaviour {

    public Texture textureMain;

	// Use this for initialization
	void Start () {
		if (textureMain != null)
        {
            MeshRenderer mash = GetComponent<MeshRenderer>();
            mash.material.SetTexture("_MainTex", textureMain);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
