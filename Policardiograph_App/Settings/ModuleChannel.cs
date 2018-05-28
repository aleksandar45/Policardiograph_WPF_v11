using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Settings
{
    public class ModuleChannel
    {
        public ModuleChannel(string moduleName, int channelNumber) {
            ModuleName = moduleName;
            ChannelNumber = channelNumber;
        }
        public ModuleChannel(string moduleName, int channelNumber, string description)
        {
            ModuleName = moduleName;
            ChannelNumber = channelNumber;
            Description = description;
        }
        public ModuleChannel(string moduleName, int channelNumber, string description, string axis)
        {
            ModuleName = moduleName;
            ChannelNumber = channelNumber;
            Description = description;
            Axis = axis;
        }
        public ModuleChannel(ModuleChannel other)
        {
            ModuleName = other.ModuleName;
            ChannelNumber = other.ChannelNumber;
            Description = other.Description;
            Axis = other.Axis;
        }
        public string ModuleName{
            get;
            set;
        }        
        public int ChannelNumber
        {
            get;
            set;
        }
        public string Axis
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
    
    }
}
