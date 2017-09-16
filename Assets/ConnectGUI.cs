using UnityEngine;
using System.Collections;
using KopernikusWrapper;
using UnityEngine.UI;

public class ConnectGUI : MonoBehaviour {
    public enum ConnectionState
    {
        NotConnected,
        AttemptingConnect,
        Connected
    }

    public Renderer cameraDisplay;
    Texture2D imageTexture;
    //public CustomButton accelerator;
    //public Transform panel;

    

    Vehicle vehicle;
    bool isConnected;
    float throttle, brake;

    // Use this for initialization
    void Start () 
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;
        imageTexture = new Texture2D(255, 255);
        //panel = new Transform(123, 0, 1);
        //accelerator.transform.SetParent(panel) ;
    }


    // Update is called once per frame
    void Update () {
        if (vehicle != null)
        {
            if (!isConnected && vehicle.Connected)
            {
                Debug.Log("VehicleAvailable");
                isConnected = true;
            }

            if (vehicle.Connected)
            {
                //vehicle.SetThrottle(Input.GetAxis("Vertical"));
                vehicle.SetThrottle(throttle);
                vehicle.SetBrake(Mathf.Clamp01(-Input.GetAxis("Vertical")));
                vehicle.SetSteeringAngle(Input.GetAxis("Horizontal"));
                vehicle.Update();

                CameraSensor s = (CameraSensor) vehicle.Sensor(SensorType.SENSOR_TYPE_CAMERA_FRONT);
                if (s != null)
                {
                    CameraSensorData d = (CameraSensorData) s.Data;
                    if (d != null)
                    {
                        imageTexture.LoadImage(d.ImageData);
                        cameraDisplay.material.mainTexture = imageTexture;
                    }
                }
            }
        }
    }
        
    void OnGUI()
    {
        if (!isConnected)
        {
            if (GUI.Button(new Rect(Screen.width * 0.5f - 300, Screen.height * 0.5f - 100, 600, 200), "Connect"))
            {
                vehicle = Kopernikus.Instance.Vehicle("172.30.6.145");
                //vehicle = Kopernikus.Instance.Vehicle("192.168.137.193");
            }
        }
    }


    public void Throttle_Changed(float newValue)
    {
        this.throttle = newValue;
    }

    public void Brake_Changed(float newValue)
    {
        this.brake = newValue;
    }
}