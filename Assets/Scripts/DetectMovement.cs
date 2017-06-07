using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMovement : MonoBehaviour {

    public GameObject playerCamara;
    public float allowedMovementError;
    public float waitTime;
    public float speed;

    private Camera cam;
    private Transform playerTransform;
    private Quaternion previousRotation;
    private float waittingTime;
    private float radiusOfCircle;

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
        waittingTime = waitTime;
        radiusOfCircle = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        // Check if player moved
        // if not increse waiting time
        Quaternion rotation = playerTransform.rotation;
        float changeInRotation = Mathf.Abs(Quaternion.Angle(rotation, previousRotation));
        if (changeInRotation > allowedMovementError)
        {
            waittingTime = waitTime;
            radiusOfCircle = 0.0f;
        }
        else
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

            if (Physics.Raycast((playerCamara.transform.position + playerCamara.transform.forward*1000), -playerCamara.transform.forward, out hitInfo))
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
                drawCircleFull(pixelUV, tex, (int)radiusOfCircle);
                drawCircle(pixelUV, tex, Color.yellow, (int)radiusOfCircle);
                drawCircle(pixelUV, tex, Color.red, (int)radiusOfCircle-1);
            }
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
                if (distance < (radius - 2) && distance >= Mathf.Min(0, radius - 3))
                {
                    int x = (int)point.x + i;
                    int y = (int)point.y + j;
                    tex.SetPixel(x, y, ringColor);
                }
            }
        }

        tex.Apply();
    }
}
