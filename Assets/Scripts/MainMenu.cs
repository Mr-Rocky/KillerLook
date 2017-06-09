using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Canvas menuCanvas;
    public MeshRenderer mashAlphaSphere;

    public Texture cityTexture;
    public Texture mallTexture;
    public Texture homeTexture;

    public GameObject alphaSphere;
    public GameObject player;
    public GameObject VRcamera;
    

    public void citySelected()
    {
        //alphaSphere.GetComponent<PrepareSphere>().textureMain = cityTexture;
        mashAlphaSphere.material.SetTexture("_MainTex", cityTexture);
        player.GetComponent<DetectMovement>().gameOn = true;
        menuCanvas.enabled = false;
        Instantiate(VRcamera, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion());
    }

    public void mallSelected()
    {
        //alphaSphere.GetComponent<PrepareSphere>().textureMain = mallTexture;
        mashAlphaSphere.material.SetTexture("_MainTex", mallTexture);
        player.GetComponent<DetectMovement>().gameOn = true;
        menuCanvas.enabled = false;
        Instantiate(VRcamera, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion());
    }

    public void homeSelected()
    {
        //alphaSphere.GetComponent<PrepareSphere>().textureMain = homeTexture;
        mashAlphaSphere.material.SetTexture("_MainTex", homeTexture);
        player.GetComponent<DetectMovement>().gameOn = true;
        menuCanvas.enabled = false;
        Instantiate(VRcamera, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion());
    }
    
}
