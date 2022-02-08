using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class CustomController : MonoBehaviour
{
    public InputDeviceCharacteristics characteristics;
    [SerializeField]
    private List<GameObject> controllerModels;
    private GameObject controllerInstance;
    private InputDevice availableDevice;

    public bool renderController; // Hand와 Controller 사이를 변경할 변수
    public GameObject handModel; // 핸드 모델
    private  GameObject handInstance; // 핸드 인스턴스

    private Animator handModelAnimator;

    public GameObject HandGun;

    bool triggerButton;

    // Start is called before the first frame update
    void Start()
    {
        TryInitialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!availableDevice.isValid)
        {
            TryInitialize();
        }

        if (renderController)
        {
            handInstance.SetActive(false);
            controllerInstance.SetActive(true);
        }
        else
        {
            handInstance.SetActive(true);
            controllerInstance.SetActive(false);
            UpdateHandAnimation();
        }
        if (HandGun != null)
        {
            bool menuButtonvalue;
            if (availableDevice.TryGetFeatureValue(CommonUsages.triggerButton, out menuButtonvalue) && menuButtonvalue)
            {
                if(triggerButton == false)
                {
                    HandGun.GetComponent<SimpleShoot>().Shoot();
                    triggerButton = true;
                }
            }
            else
            {
                triggerButton = false;
            }
        }
    }

    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();
        // 오른쪽 컨트롤러를 입력받기 위해 사용하는 것
        InputDevices.GetDevicesWithCharacteristics(characteristics, devices);
        foreach (var device in devices)
        {
            Debug.Log($"Available Device Name: {device.name}, Characteristic: { device.characteristics}");
        }
        if (devices.Count > 0)
        {
            availableDevice = devices[0];
            GameObject currentControllerModel;
            if (availableDevice.name.Contains("Left"))
            {
                currentControllerModel = controllerModels[1];
            }
            else if (availableDevice.name.Contains("Right"))
            {
                currentControllerModel = controllerModels[2];
            }
            else
            {
                currentControllerModel = null;
            }

            if (currentControllerModel)
            {
                controllerInstance = Instantiate(currentControllerModel, transform);
            }
            else
            {
                Debug.LogError("Didn't get suitable controller model");
                controllerInstance = Instantiate(controllerModels[0], transform);
            }

            handInstance = Instantiate(handModel, transform);
            handModelAnimator = handInstance.GetComponent<Animator>();
        }
    }

    void UpdateHandAnimation()
    {
        if(availableDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handModelAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handModelAnimator.SetFloat("Trigger", 0);
        }

        if(availableDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handModelAnimator.SetFloat("Grip", gripValue);
        }
        else 
        { 
            handModelAnimator.SetFloat("Grip", 0); 
        }
    }
}
