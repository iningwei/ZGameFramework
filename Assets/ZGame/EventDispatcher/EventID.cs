namespace ZGame.Event
{
    public class EventID
    {
        public const string OnBeginDownloadResFiles = "1";
        public const string OnResFileDownloading = "2";
        public const string OnResFileDownloaded = "3";



        public const string SDKLoginSuccess = "10";
        public const string SDKLoginFail = "11";
        public const string SDKLoginCMP = "12";


        public const string OnFCMTokenReceived = "20";


        public const string OnSoundSliderValueChange = "30";
        public const string OnPlayRollSound = "31";


        public const string OnBeginDownloadHotResFiles = "50";
        public const string OnHotResFileDownloaded = "51";
        public const string OnHotResFileDownloadFail = "52";
        public const string OnHotResFileDownloading = "53";




        public const string OnABResLoaded = "100";
        public const string OnRootObjDestroy = "101";
        public const string OnChildObjDestroy = "102";

        public const string OnGameObjectInstantiate = "110";



        public const string OnFCMJump = "410";



        public const string OnMobileJumpClicked = "1000";




        //
        public const string OnGameStart = "10000";
        public const string OnGameEnd = "10001";
        public const string OnRestartGame = "10002";

        public const string OnPlayerSpawn = "20000";
        public const string OnPlayerOnLineStateChange = "20001";
        public const string OnActorSpawn = "21000";
        public const string OnActorBeginDead = "21001";
        public const string OnActorFinishDead = "21002";

        public const string OnTurnbasedSummonActorSuccess = "21200";//回合招兵成功

        public const string OnPlayerCheck = "21500";
        public const string OnRealtimeRankClickedPlayer = "21501";


        public const string OnPlayerDispatchTroopsTimeAdd = "22001";

        public const string OnPlayerEnergyAdd = "22100";

        public const string OnTurnbasedChang = "23000";

    }
}