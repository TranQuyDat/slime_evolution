using UnityEngine;

static class SettingBeforeInGame
{
    public static bool IsComputer { get; private set; }
    public static bool IsHandheld { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void ApplySettings()
    {
        DetectDevice();
        SetFrameRate();

        if (IsHandheld)
            SetMobileOrientation();
    }

    private static void DetectDevice()
    {
        IsHandheld = Application.isMobilePlatform ||
                     SystemInfo.deviceType == DeviceType.Handheld;
        IsComputer = !IsHandheld &&
                     SystemInfo.deviceType == DeviceType.Desktop;
    }

    private static void SetFrameRate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private static void SetMobileOrientation()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}
