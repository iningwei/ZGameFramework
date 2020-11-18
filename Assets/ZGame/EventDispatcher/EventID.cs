public class EventID
{

    public const int OnABResLoaded = 100;
    public const int OnRootObjDestroy = 101;
    public const int OnChildObjDestroy = 102;


    public const int OnGarbageDestroy = 300;

    public const int OnDistanceTravelled = 310;
    public const int OnADSpeedingCleanerTrigger = 320;

    public const int OnCleanerArriveAtGarbage = 400;

    public const int OnCurCityCollectGarbageFinished = 500;//当前城市垃圾收集完毕


    public const int OnServerGameDataChanged = 1000;



    public const int OnBeginDownloadResFiles = 10000;
    public const int OnResFileDownloading = 10001;
    public const int OnResFileDownloaded = 10002;
}
