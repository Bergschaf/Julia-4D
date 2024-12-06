using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public ComputeShader juliaShader;
    public RenderTexture renderTexture;
    public Vector4 C;

    [System.Serializable]
    public struct CameraStruct
    {
        public Vector3 Position;
        public Vector3 Forward;
        public Vector3 Up;
        //public float Fov;
        public int Width;
        public int Height;
    }
    
    public CameraStruct Camera;

    public Main(CameraStruct camera)
    {
        Camera = camera;
    }

    // Start is called before the first frame update
    void Awake()
    {
        Camera.Width = Screen.width;
        Camera.Height = Screen.height;
        
    }

    private void Update()
    {
        Camera.Forward = Vector3.Normalize(Camera.Forward);
        Camera.Up = Vector3.Normalize(Camera.Up);
        // Control the camera
        if (Input.GetKey(KeyCode.W))
        {
            Camera.Position += Camera.Forward * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Camera.Position -= Camera.Forward * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Camera.Position -= Vector3.Cross(Camera.Forward, Camera.Up) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Camera.Position += Vector3.Cross(Camera.Forward, Camera.Up) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Camera.Position += Camera.Up * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Camera.Position -= Camera.Up * Time.deltaTime;
        }
        
        // if the mouse is pressed, the camera rotates
        if (Input.GetMouseButton(0))
        {
            Camera.Forward = Quaternion.AngleAxis(Input.GetAxis("Mouse X"), -Camera.Up) * Camera.Forward;
            Camera.Forward = Quaternion.AngleAxis(Input.GetAxis("Mouse Y"), Vector3.Cross(Camera.Forward, Camera.Up)) * Camera.Forward;

        }
        // mouse controls the orientation
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(Camera.Width, Camera.Height, 0);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
        juliaShader.SetTexture(0, "Result", renderTexture);
        
        juliaShader.SetFloats("C", new float[] { C.x, C.y, C.z, C.w });
        juliaShader.SetFloats("CameraPosition", new float[] { Camera.Position.x, Camera.Position.y, Camera.Position.z });
        juliaShader.SetFloats("CameraForward", new float[] { Camera.Forward.x, Camera.Forward.y, Camera.Forward.z });
        juliaShader.SetFloats("CameraUp", new float[] { Camera.Up.x, Camera.Up.y, Camera.Up.z });
        juliaShader.SetInt("Width", Camera.Width);
        Debug.Log(Camera.Width);
        juliaShader.SetInt("Height", Camera.Height);
        Debug.Log(Camera.Height);
        
        juliaShader.Dispatch(0, Camera.Width / 8, Camera.Height / 8, 1);
        Graphics.Blit(renderTexture, destination);
    }
}
