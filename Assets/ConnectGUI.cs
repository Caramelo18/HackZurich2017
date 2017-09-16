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

    Vehicle vehicle;
    bool isConnected;
    float throttle, brake;
    char gear = 'D';
    float turningAngle;
    string carIP;

    public Text shownGear;
    public Button setGear;
    public Slider throtleSlider, brakeSlider;
    public Text speed;
    public InputField ipCarAddress;

    VehicleStatus status;

    // Use this for initialization
    void Start () 
    {
        Input.gyro.enabled = true;
        imageTexture = new Texture2D(255, 255);
        shownGear.text = "D";
        setGear.GetComponentInChildren<Text>().text = "Gear";
    }

    // Update is called once per frame
    void Update () {
        if (vehicle != null)
        {
            if (!isConnected && vehicle.Connected)
            {
                Debug.Log("VehicleAvailable");
                isConnected = true;
                vehicle.SetGear(GearDirection.GEAR_DIRECTION_FORWARD);
            }
            
            if (vehicle.Connected)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    vehicle.SetGear(GearDirection.GEAR_DIRECTION_NEUTRAL);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    vehicle.SetGear(GearDirection.GEAR_DIRECTION_FORWARD);
                }        
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    vehicle.SetGear(GearDirection.GEAR_DIRECTION_BACKWARD);
                }

                turningAngle = Input.acceleration.x;
                vehicle.SetSteeringAngle(turningAngle);


                //vehicle.SetThrottle(Input.GetAxis("Vertical"));
                //vehicle.SetBrake(Mathf.Clamp01(-Input.GetAxis("Vertical")));
                vehicle.SetThrottle(throttle);
                vehicle.SetBrake(brake);
                //vehicle.SetSteeringAngle(Input.GetAxis("Horizontal"));
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

                status = vehicle.Status;
                updateSpeedDisplay();
            }
        }
    }
        
    void OnGUI()
    {
        if (!isConnected)
        {
            if (GUI.Button(new Rect(Screen.width * 0.5f - 300, Screen.height * 0.5f - 100, 600, 200), "Connect"))
            {
                //vehicle = Kopernikus.Instance.Vehicle("172.30.6.145");
                //vehicle = Kopernikus.Instance.Vehicle("192.168.137.1");
                vehicle = Kopernikus.Instance.Vehicle(carIP);
            }
        }
    }

    public void Throttle_Changed(float newValue)
    {
        this.throttle = newValue / 100;

        if(brake > 0)
        {
            brake = 0;
            brakeSlider.value = 0;
        }
    }

    public void Brake_Changed(float newValue)
    {
        this.brake = newValue;

        if(throttle > 0)
        {
            throttle = 0;
            throtleSlider.value = 0;
        }

    }

    public void changeGear()
    {
        if(gear == 'D')
        {
            vehicle.SetGear(GearDirection.GEAR_DIRECTION_BACKWARD);
            gear = 'R';
            shownGear.text = "R";
        }
        else
        {
            vehicle.SetGear(GearDirection.GEAR_DIRECTION_FORWARD);
            gear = 'D';
            shownGear.text = "D";
        }
    }

    private void updateSpeedDisplay()
    {
        int intSpeed = (int)status.Velocity;
        speed.text = intSpeed.ToString() + "m/s";
    }

    private void updateCarIP(string ip)
    {
        carIP = ip;
    }
}