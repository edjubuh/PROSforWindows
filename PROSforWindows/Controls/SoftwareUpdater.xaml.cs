using PROSforWindows.Commands;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PROSforWindows.Controls
{
    /// <summary>
    /// Interaction logic for SoftwareUpdater.xaml
    /// </summary>
    public partial class SoftwareUpdater : UserControl, INotifyPropertyChanged
    {
        public string CurrentVersion
        {
            get { return (string)GetValue(CurrentVersionProperty); }
            set { SetValue(CurrentVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVersionProperty =
            DependencyProperty.Register("CurrentVersion", typeof(string), typeof(SoftwareUpdater), new PropertyMetadata("0.0.0"));



        public string ItemName
        {
            get { return (string)GetValue(ItemNameProperty); }
            set { SetValue(ItemNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemNameProperty =
            DependencyProperty.Register("ItemName", typeof(string), typeof(SoftwareUpdater), new PropertyMetadata("Software"));



        public ICommand CheckForUpdatesCommand
        {
            get { return (ICommand)GetValue(CheckForUpdatesCommandProperty); }
            set { SetValue(CheckForUpdatesCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckForUpdatesCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckForUpdatesCommandProperty =
            DependencyProperty.Register("CheckForUpdatesCommand", typeof(ICommand), typeof(SoftwareUpdater), new PropertyMetadata(new RelayCommand(_ => { })));



        public ICommand InstallLatestUpdateCommand
        {
            get { return (ICommand)GetValue(InstallLatestUpdateCommandProperty); }
            set { SetValue(InstallLatestUpdateCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InstallLatestUpdateCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstallLatestUpdateCommandProperty =
            DependencyProperty.Register("InstallLatestUpdateCommand", typeof(ICommand), typeof(SoftwareUpdater), new PropertyMetadata(new RelayCommand(_ => { })));

        

        public bool HaveNewVersionToInstall
        {
            get { return (bool)GetValue(HaveNewVersionToInstallProperty); }
            set { SetValue(HaveNewVersionToInstallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HaveNewVersionToInstall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HaveNewVersionToInstallProperty =
            DependencyProperty.Register("HaveNewVersionToInstall", typeof(bool), typeof(SoftwareUpdater), new PropertyMetadata(false));



        public string NewVersion
        {
            get { return (string)GetValue(NewVersionProperty); }
            set { SetValue(NewVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NewVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewVersionProperty =
            DependencyProperty.Register("NewVersion", typeof(string), typeof(SoftwareUpdater), new PropertyMetadata("0.2.0"));


        private bool isCheckingForUpdates = false;
        public bool IsCheckingForUpdates
        {
            get { return isCheckingForUpdates; }
            set
            {
                if (isCheckingForUpdates != value)
                    OnPropertyChanged(nameof(IsCheckingForUpdates));
            }
        }


        public SoftwareUpdater()
        {
            InitializeComponent();
        }


        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
