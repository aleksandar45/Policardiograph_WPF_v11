using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Settings
{
    public class SettingProgram: SettingBase
    {
        public SettingProgram(string savePath, float maxBattVoltage, float minBattVoltage, float alertBattVoltage, int lastMeasTime) {
            SavePath = savePath;
            MaxBatteryVoltage = maxBattVoltage;
            MinBatteryVoltage = minBattVoltage;
            BatteryAlertVoltage = alertBattVoltage;
            BatteryLastMeasuredTime = lastMeasTime;
        }
        public string SavePath
        {
            get;
            private set;
        }
        public float MaxBatteryVoltage
        {
            get;
            private set;
        }
        public float MinBatteryVoltage
        {
            get;
            private set;
        }
        public float BatteryAlertVoltage
        {
            get;
            private set;
        }
        public int BatteryLastMeasuredTime
        {
            get;
            set;
        }
    }
}
