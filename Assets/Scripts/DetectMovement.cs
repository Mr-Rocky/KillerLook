using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMovement : MonoBehaviour {

    public GameObject playerCamara;
    public AudioSource audioFire;
    [Tooltip("Allowance of movement while staring.")]
    public float allowedMovementError;
    [Tooltip("Time that is needed to stare for activation of event.")]
    public float waitTime;
    [Tooltip("Rate of increasing infected area.")]
    public float speed;
    [Tooltip("Value for multiplying transperency.")]
    public float alpha;
    [Tooltip("Chance of fire to procede to next pixel.")]
    public float chanceOfFire;
    [HideInInspector]
    public bool gameOn = true;

    private Camera cam;
    private Transform playerTransform;
    private Quaternion previousRotation;
    private float waittingTime;
    private float radiusOfCircle;
    // for clearing fire ring
    private bool didHit;
    private Vector2 priviousPoint;
    private Texture2D priviousTex;

    // Use this for initialization
    void Start () {
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
            Debug.Log("nimam kamere");
        playerTransform = playerCamara.GetComponent<Transform>();
        if (float.IsNaN(allowedMovementError))
            allowedMovementError = 10.0f;
        if (float.IsNaN(waitTime))
            waitTime = 1.0f;
        if (float.IsNaN(speed))
            speed = 1.0f;
        if (float.IsNaN(chanceOfFire))
            speed = 0.25f;
        waittingTime = waitTime;
        radiusOfCircle = 0.0f;
        didHit = false;
	}
	

	void Update () {
        // Check if player moved
        // if not increse waiting time
        Quaternion rotation = playerTransform.rotation;
        float changeInRotation = Mathf.Abs(Quaternion.Angle(rotation, previousRotation));
        if (changeInRotation > allowedMovementError)
        {
            waittingTime = waitTime;
        }
        else if (gameOn)
        {
            waittingTime -= Time.deltaTime;
        }
        // update previos rotation to curent rotation
        previousRotation = rotation;

        // In case wait time passed start dissolve
        // find hit point
        if (waittingTime < 0)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast((playerCamara.transform.position + playerCamara.transform.forward*500), -playerCamara.transform.forward, out hitInfo))
            {
                Renderer rend = hitInfo.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hitInfo.collider as MeshCollider;

                if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                {
                    Debug.Log("error on hit");
                    return;
                }

                Texture2D tex = rend.material.mainTexture as Texture2D;
                Vector2 pixelUV = hitInfo.textureCoord;
                pixelUV.x *= -tex.width;
                pixelUV.y *= tex.height;

                radiusOfCircle += speed;
                //drawCircleFull(pixelUV, tex, (int)radiusOfCircle - 2);
                //drawCircle(pixelUV, tex, Color.red, (int)radiusOfCircle - 1);
                //drawCircle(pixelUV, tex, Color.yellow, (int)radiusOfCircle);
                //burningArea(pixelUV, tex, radiusOfCircle);
                transparencyCircle(pixelUV, tex, radiusOfCircle);
                if (!didHit)
                {
                    //audioFire.time = 1.9f;
                    //audioFire.Play();
                    didHit = true;
                }
                priviousPoint = pixelUV;
                priviousTex = tex;
            }
        }
        else if (didHit)
        {
            didHit = false;
            //drawCircleFull(priviousPoint, priviousTex, (int)radiusOfCircle);
            //burningAreaClear(priviousPoint, priviousTex, radiusOfCircle);
            radiusOfCircle = 0.0f;
            //audioFire.Stop();
        }
    }

    void drawCircle(Vector2 point, Texture2D tex, Color ringColor, int radius)
    {
        int x = radius;
        int y = 0;
        int err = 0;

        while (x >= y)
        {
            if (tex.GetPixel((int)point.x + x, (int)point.y + y).a == 1)
                tex.SetPixel((int)point.x + x, (int)point.y + y, ringColor);
            if (tex.GetPixel((int)point.x + y, (int)point.y + x).a == 1)
                tex.SetPixel((int)point.x + y, (int)point.y + x, ringColor);
            if (tex.GetPixel((int)point.x - y, (int)point.y + x).a == 1)
                tex.SetPixel((int)point.x - y, (int)point.y + x, ringColor);
            if (tex.GetPixel((int)point.x - x, (int)point.y + y).a == 1)
                tex.SetPixel((int)point.x - x, (int)point.y + y, ringColor);
            if (tex.GetPixel((int)point.x - x, (int)point.y - y).a == 1)
                tex.SetPixel((int)point.x - x, (int)point.y - y, ringColor);
            if (tex.GetPixel((int)point.x - y, (int)point.y - x).a == 1)
                tex.SetPixel((int)point.x - y, (int)point.y - x, ringColor);
            if (tex.GetPixel((int)point.x + y, (int)point.y - x).a == 1)
                tex.SetPixel((int)point.x + y, (int)point.y - x, ringColor);
            if (tex.GetPixel((int)point.x + x, (int)point.y - y).a == 1)
                tex.SetPixel((int)point.x + x, (int)point.y - y, ringColor);

            y += 1;
            if (err <= 0)
            {
                err += 2 * y + 1;
            }
            if (err > 0)
            {
                x -= 1;
                err -= 2 * x + 1;
            }
        }

        tex.Apply();
    }

    void drawCircleFull(Vector2 point, Texture2D tex, int radius)
    {
        Color ringColor = Color.clear;

        for (int i = -radius; i < radius; i++)
        {
            for (int j = -radius; j < radius; j++)
            {
                float distance = Mathf.Sqrt(i * i + j * j);
                if (distance < radius && distance >= Mathf.Min(0, radius - 2))
                {
                    int x = (int)point.x + i;
                    int y = (int)point.y + j;
                    tex.SetPixel(x, y, ringColor);
                }
            }
        }

        tex.Apply();
    }

    void burningArea(Vector2 point, Texture2D tex, float radius)
    {
        if (radius < 2 * speed)
        {
            tex.SetPixel((int)point.x, (int)point.y, Color.yellow);
        }
        else
        {
            int radiusInt = (int)radius;
            Texture2D tmpTex = tex;

            for (int i = -radiusInt; i <= radiusInt; i++)
            {
                for (int j = -radiusInt; j <= radiusInt; j++)
                {
                    // for speed up of a program
                    /*if ((Mathf.Abs(i) < (radius / 2)) && (Mathf.Abs(j) < (radius / 2)))
                    {
                        Debug.Log("i and j lower then half of radius \n i: "+i+" j: "+j);
                        continue;
                    }*/

                    int x = (int)point.x + i;
                    int y = (int)point.y + j;
                    Color pixelColor = tex.GetPixel(x, y);

                    if (pixelColor == Color.clear)
                        continue;
                    else if (pixelColor == Color.red)
                    {
                        if (Random.value < chanceOfFire)
                        {
                            tmpTex.SetPixel(x, y, Color.clear);
                            flameOn(x, y, tmpTex);
                        }
                    }
                    else if (pixelColor == Color.yellow)
                    {
                        if (Random.value < chanceOfFire)
                        {
                            tmpTex.SetPixel(x, y, Color.red);
                            flameOn(x, y, tmpTex);
                        }
                    }
                }
            }
            tex = tmpTex;
        }
        tex.Apply();
    }

    void flameOn(int x, int y, Texture2D tex)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                Color pixelColor = tex.GetPixel(x + i, y + j);
                if (pixelColor == Color.clear)
                    continue;
                else if (pixelColor == Color.yellow)
                {
                    tex.SetPixel(x + i, y + j, Color.red);
                }
                else if (pixelColor == Color.red)
                {
                    tex.SetPixel(x + i, y + j, Color.clear);
                }
                else
                {
                    tex.SetPixel(x + i, y + j, Color.yellow);
                }
            }
        }
        //tex.Apply();
    }

    void burningAreaClear(Vector2 point, Texture2D tex, float radius)
    {
        int radiusInt = (int)radius;

        for (int i = -radiusInt; i <= radiusInt; i++)
        {
            for (int j = -radiusInt; j <= radiusInt; j++)
            {
                int x = (int)point.x + i;
                int y = (int)point.y + j;
                Color pixelColor = tex.GetPixel(x, y);

                if (pixelColor == Color.red || pixelColor == Color.yellow)
                    tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
    }

    void transparencyCircle(Vector2 point, Texture2D tex, float radius)
    {
        int radiusInt = (int)radius;
        for (int i = -radiusInt; i < radiusInt; i++)
        {
            for (int j = -radiusInt; j < radiusInt; j++)
            {
                float distance = Mathf.Sqrt(i * i + j * j);
                
                if (distance < radius)
                {
                    int x = (int)point.x + i;
                    int y = (int)point.y + j;
                    Color pixelColor = tex.GetPixel(x, y);
                    //pixelColor.a *= (1 - distance / radius);
                    pixelColor.a *= alpha;
                    tex.SetPixel(x, y, pixelColor);
                }
            }
        }

        tex.Apply();
    }
}
