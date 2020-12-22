using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Window
{
    public class WindowLayer
    {
        public static string Hidden = "hidden";//Some windows may always be used,so after close window,we inactive them and move them to this layer for reuse
        public static string Hud3D = "hud3d";//3D ui，such as blood ui
        public static string Basic = "basic";//especially used for game's main ui window
        public static string Hud = "hud";//used for some pop up window
        public static string Msg = "msg";//used for some message box or message tips
        public static string SceneChange = "scenechange";//used for scene change
        public static string NetMask = "netmask";//used for net ring
        public static string Top = "top";//toppest layer,used for system notify or something you always want player to see.

        public static List<string> LayerList = new List<string>()
    {
        Hidden,
        Hud3D,
        Basic,
        Hud,
        Msg,
        SceneChange,
        NetMask,
        Top
    };
    }
}