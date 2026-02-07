using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class DeviceTypeChecker
{
    const int WIDTH = 600;
    const int HEIGHT = 900;
    const int DPI = 200; // Dots Per Inch

    const string IPAD = "ipad";
    const string TAB = "tab";
    const string KINDLE = "kindle";

    private const int SCREENLAYOUT_SIZE_MASK = 0x0F;
    private const int SCREENLAYOUT_SIZE_LARGE = 0x03;
    private const int SCREENLAYOUT_SIZE_XLARGE = 0x04;

    public EDeviceType GetDeviceType()
    {
#if UNITY_EDITOR
        return GetDeviceTypeEditor();
#elif UNITY_ANDROID
        return GetDeviceTypeAndroid();
#elif UNITY_IOS
        return GetDeviceTypeIOS();
#else
        return GetDeviceTypeFallback();
#endif
    }

    private EDeviceType GetDeviceTypeFallback()
    {
        bool isTabletBySize = CheckByScreenSize();
        bool isTabletByDPI = CheckByDPI();
        bool isTabletByModel = CheckByDeviceModel();

        if ((isTabletBySize && isTabletByDPI) || isTabletByModel)
        {
            return EDeviceType.Tablet;
        }
        else
        {
            return EDeviceType.Phone;
        }
    }

    private EDeviceType GetDeviceTypeEditor()
    {
        bool isTabletCheck1 = CheckByScreenSize();
        bool isTabletCheck2 = CheckByDPI();
        bool isTabletCheck3 = CheckByDeviceModel();

        if (isTabletCheck1 && isTabletCheck2 && isTabletCheck3)
        {
            return EDeviceType.Tablet;
        }
        else if (isTabletCheck1 && isTabletCheck2 && !isTabletCheck3)
        {
            return EDeviceType.UnityEditor;
        }
        else
        {
            return EDeviceType.Phone;
        }
    }

    private EDeviceType GetDeviceTypeAndroid()
    {
#if UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject resources = activity.Call<AndroidJavaObject>("getResources");
        AndroidJavaObject configuration = resources.Call<AndroidJavaObject>("getConfiguration");

        int screenLayout = configuration.Get<int>("screenLayout");
        screenLayout &= SCREENLAYOUT_SIZE_MASK;

        if (screenLayout == SCREENLAYOUT_SIZE_LARGE || screenLayout == SCREENLAYOUT_SIZE_XLARGE)
        {
            return EDeviceType.Tablet;
        }
        else
        {
            return EDeviceType.Phone;
        }
#else
        return GetDeviceTypeFallback();
#endif
    }

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern int Device_get_generation();
#endif

    private EDeviceType GetDeviceTypeIOS()
    {
#if UNITY_IOS
        List<string> iosDevices = Enum.GetNames(typeof(DeviceGeneration)).ToList();
        string deviceGen = Device.generation.ToString();

        if (iosDevices.Contains(deviceGen))
        {
            if (deviceGen.Contains("iPad"))
            {
                return EDeviceType.Tablet;
            }
            else if (deviceGen.Contains("iPhone"))
            {
                return EDeviceType.Phone;
            }
            else return GetDeviceTypeFallback();
        }
        else 
        {
            return GetDeviceTypeFallback();
        }
#else
        return GetDeviceTypeFallback();
#endif
    }

    private bool CheckByScreenSize() => Screen.width > WIDTH && Screen.height > HEIGHT;
    private bool CheckByDPI() => Screen.dpi > DPI;
    private bool CheckByDeviceModel()
    {
        string deviceModel = SystemInfo.deviceModel.ToLower();
        return deviceModel.Contains(IPAD) || deviceModel.Contains(TAB) || deviceModel.Contains(KINDLE);
    }
}
