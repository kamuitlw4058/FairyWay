using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityGameFramework.Runtime;

namespace FairyWay.Hotfix
{
    public class GameBridge
    {
        public void Start()
        {
            Log.Info("热更主循环启动!");

            // HotfixEntry.InitHotfixEntry();

            // HotfixEntry.Procedure.StartProcedure<ProcedurePreload>();
        }
    }
}
