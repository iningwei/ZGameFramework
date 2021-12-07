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

        public const string OnABResLoaded = "100";
        public const string OnRootObjDestroy = "101";
        public const string OnChildObjDestroy = "102";

        public const string OnGameObjectInstantiate = "110";

        public const string OnBattleEnter = "200";
        public const string OnBattleExit = "201";


        public const string OnMachineBeginDestroy = "301";

        public const string OnMachineHPChange = "310";
        public const string OnMachineSpinCountChange = "311";
        public const string OnMachineSpinIdle = "312";
        public const string OnMachineSpinRolling = "313";
        public const string OnMachineSpinShowResult = "314";


        public const string OnPetDie = "400";

        public const string OnFCMJump = "410";

        public const string OnFirstPetSelected = "440";
        public const string OnFirstPetUnselected = "441";
    }
}