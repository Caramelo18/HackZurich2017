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
    string carIP = "192.168.137.1";

    public Text shownGear;
    public Button setGear;
    public Button connectVehicle;
    public Slider throtleSlider, brakeSlider;
    public Text speed;
    public InputField ipCarAddress;

    VehicleStatus status;

    // Use this for initialization
    void Start ()
    {
        Input.gyro.enabled = true;
        imageTexture = new Texture2D(255, 255);

        setGear.gameObject.SetActive(false);

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

                changeUItoDriveMode();

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
        } else
        {
            setGear.gameObject.SetActive(false);
            brakeSlider.gameObject.SetActive(false);
            throtleSlider.gameObject.SetActive(false);
            shownGear.gameObject.SetActive(false);
            speed.gameObject.SetActive(false); ;

}
    }

    void OnGUI()
    {
     
    }

    void changeUItoDriveMode() {
        setGear.gameObject.SetActive(true);
        brakeSlider.gameObject.SetActive(true);
        throtleSlider.gameObject.SetActive(true);
        shownGear.gameObject.SetActive(true);
        speed.gameObject.SetActive(true);

        ipCarAddress.gameObject.SetActive(false);
        connectVehicle.gameObject.SetActive(false);
    }

    public void connectCar() {
        vehicle = Kopernikus.Instance.Vehicle(carIP);
        
    }

    public void Throttle_Changed(float newValue)
    {
        //newValue between 0 and 100
        //this.throttle = newValue / 100;
        this.throttle = Mathf.Pow(2, newValue / 10) / 1024;

        if (newValue > 0)
            this.throttle += (float) 0.3;
            

        if(brake > 0)
        {
            brake = 0;
            brakeSlider.value = 0;
        }
    }

    public void Brake_Changed(float newValue)
    {
        //newValue between 0 and 1
        this.brake = Mathf.Pow(2, newValue * 10) / 1024;

        if (newValue > 0)
            this.brake += (float)0.3;

        if (throttle > 0)
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
