using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Policardiograph_App.Properties;

namespace Policardiograph_App.ViewModel
{
    
    public class Description : ViewModelBase {

        public Description(string descriptionString) {
            _descriptionString = descriptionString;
        }
        private string _descriptionString = string.Empty;
        public string DescriptionString
        {
            get
            {
                return _descriptionString;
            }
            set
            {
                _descriptionString = value;
                OnPropertyChanged("DescriptionString");
            }
        }   
    }
    public class Channel : ViewModelBase  
    {
        private MainWindowViewModel _parent;

        public Channel(string channelname)
        {
            _channel = channelname;
            channelDescriptions = new List<Description>() { new Description("Aca") };
         //   channelDescriptions.Add(new Description("Aca"));
        }
        public void addParent(MainWindowViewModel _parent) {
            this._parent = _parent;
        }

        private List<Description> channelDescriptions;
        public List<Description> ChannelDescriptions
        {
            get {
                return channelDescriptions;
            }
            set {
                channelDescriptions = value;
                OnPropertyChanged("ChannelDescriptions");
            }
        }

        private string _channel = string.Empty;  
        public string ChannelName  
        {  
            get  
            {  
                return _channel;  
            }  
            set  
            {  
                _channel = value;
                OnPropertyChanged("ChannelName");  
            }  
        }       
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
    }
    public class Module : ViewModelBase
    {
        private MainWindowViewModel _parent;
        private byte _id;

        public Module(byte id, string modulename, int no_channels)
        {
            _id = id;
            _moduleName = modulename;
            channels = new List<Channel>();
            for(int i=0;i<no_channels;i++){
                channels.Add(new Channel("CH"+(i+1).ToString()));
            }
        }
        public void addParent(MainWindowViewModel _parent) {
            this._parent = _parent;
            for (int i = 0; i < channels.Count; i++)
            {
                channels.ElementAt(i).addParent(_parent);
            }
        }

        private List<Channel> channels;
        public List<Channel> Channels
        {
            get
            {
                return channels;
            }
            set
            {
                channels = value;
                OnPropertyChanged("Channels");
            }
        }

        private string _moduleName;
        public string ModuleName
        {
            get {
                return _moduleName;
            }
            set{
                _moduleName = value;
                OnPropertyChanged("ModuleName");
            }
        }
        private Brush _moduleStatusColor = new SolidColorBrush(Colors.Red);
        public Brush ModuleStatusColor {
            get {
                return _moduleStatusColor;
            }
            set {
                _moduleStatusColor = value;
                OnPropertyChanged("ModuleStatusColor");
            }
        }
        private bool _isSelected = false;
        public bool IsSelected {
            get {
                return _isSelected;
            }
            set {
                _parent.DataGridIsVisible = true;
               /* if ((_parent.ModuleDescriptions.Count > 0) && (_parent.ModuleDescriptions.Count < 2))
                {
                    _parent.ModuleDescriptions.RemoveAt(0);
                }*/
                switch (_id){
                    case 0:
                        _parent.ModuleDescriptions = new List<ModuleDescription>{new ModuleDescription(Resources.MASTERMODULE_LABEL, _parent.micModuleStatusString, _parent.micNoChannels, _parent.micSampleRate)};
                        break;
                    case 1:
                        _parent.ModuleDescriptions = new List<ModuleDescription>{new ModuleDescription(Resources.FBGAMODULE_LABEL, _parent.fbgaModuleStatusString, _parent.fbgaNoChannels, _parent.fbgaSampleRate)};
                        break;
                    case 2:
                        _parent.ModuleDescriptions = new List<ModuleDescription>{new ModuleDescription(Resources.ECGMODULE_LABEL, _parent.ecgModuleStatusString, _parent.ecgNoChannels, _parent.ecgSampleRate)};
                        break;
                    case 3:
                        _parent.ModuleDescriptions = new List<ModuleDescription>{new ModuleDescription(Resources.ACCMODULE_LABEL, _parent.accModuleStatusString, _parent.accNoChannels, _parent.accSampleRate)};
                        break;
                    case 4:
                        _parent.ModuleDescriptions= new List<ModuleDescription>{new ModuleDescription(Resources.PPGMODULE_LABEL, _parent.ppgModuleStatusString, _parent.ppgNoChannels, _parent.ppgSampleRate)};
                        break;

                }
                _isSelected = value;
                OnPropertyChanged("IsSelected");
               
            }
        }
        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }
        private bool _isEnabled = false;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
        private bool _focusable = true;
        public bool Focusable
        {
            get
            {
                return _focusable;
            }
            set
            {
                _focusable = value;
                OnPropertyChanged("Focusable");
            }
        }

    }
    public class TreeViewViewModel : ViewModelBase
    {
        
        private MainWindowViewModel _parent;

        public TreeViewViewModel()  
        {                    
           
        }
        public void addParent(MainWindowViewModel _parent) {
            this._parent = _parent;           

            Modules = new List<Module>()  
            {  
                new Module(0,Resources.MASTERMODULE_LABEL,_parent.micNoChannels),  
                new Module(1,Resources.FBGAMODULE_LABEL,_parent.fbgaNoChannels),
                new Module(2,Resources.ECGMODULE_LABEL,_parent.ecgNoChannels),
                new Module(3,Resources.ACCMODULE_LABEL,_parent.accNoChannels),
                new Module(4,Resources.PPGMODULE_LABEL,_parent.accNoChannels)
  
            };
            for (int i = 0; i < modules.Count; i++)
            {
                modules.ElementAt(i).addParent(_parent);
            }
            //for (int i = 0; i < Modules.Count; i++) Modules.ElementAt(i).IsEnabled = true;   
        }
        public void expand(bool b, string module) {
            if(String.Compare(module,"MIC")==0){
                if (Modules.Count > 0) Modules.ElementAt(0).IsExpanded = b;
            }
            if (String.Compare(module, "FBGA") == 0)
            {
                if (Modules.Count > 1) Modules.ElementAt(1).IsExpanded = b;
            }
            if (String.Compare(module, "ECG") == 0)
            {
                if (Modules.Count > 2) Modules.ElementAt(2).IsExpanded = b;
            }
            if (String.Compare(module, "ACC") == 0)
            {
                if (Modules.Count > 3) Modules.ElementAt(3).IsExpanded = b;
            }
            if (String.Compare(module, "PPG") == 0)
            {
                if (Modules.Count > 4) Modules.ElementAt(4).IsExpanded = b;
            }
        }
        private List<Module> modules;
        public List<Module> Modules  
        {  
            get  
            {  
                return modules;  
            }  
            set  
            {
                modules = value;
                OnPropertyChanged("Modules");  
            }  
        }  
    }
}
