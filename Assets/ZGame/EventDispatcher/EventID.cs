public class EventID
{

    public const string OnABResLoaded = "100";
    public const string OnRootObjDestroy = "101";
    public const string OnChildObjDestroy = "102";


    public const string OnBannerAdShowed = "200";
    public const string OnGarbageDestroy = "300";

    public const string OnDistanceTravelled = "310";

    public const string OnADSpeedingUpTrigger = "320";
    public const string onADSpeedingUpEnd = "321";//广告加速时间用尽


    public const string OnCleanerArriveAtGarbage = "400";

    public const string OnCurCityCollectGarbageFinished = "500";//当前城市垃圾收集完毕


    public const string OnServerGameDataChanged = "1000";



    public const string OnBeginDownloadResFiles = "10000";
    public const string OnResFileDownloading = "10001";
    public const string OnResFileDownloaded = "10002";
}
