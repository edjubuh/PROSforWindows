﻿using PROSforWindows.Commands;
using Shell32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PROSforWindows.Controls
{
    /// <summary>
    /// Interaction logic for FolderPicker.xaml
    /// </summary>
    public partial class FolderPicker : UserControl
    {
        public static readonly DependencyProperty SelectButtonTextProperty =
                DependencyProperty.Register(nameof(SelectButtonText), typeof(string), typeof(FolderPicker), new PropertyMetadata("Select"));

        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register(nameof(SelectCommand), typeof(ICommand), typeof(FolderPicker));

        public static readonly DependencyProperty SelectedFolderProperty =
            DependencyProperty.Register(nameof(SelectedFolder), typeof(string), typeof(FolderPicker));

        public string SelectButtonText
        {
            get { return (string)GetValue(SelectButtonTextProperty); }
            set { SetValue(SelectButtonTextProperty, value); }
        }

        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        public string SelectedFolder
        {
            get { return (string)GetValue(SelectedFolderProperty); }
            set { SetValue(SelectedFolderProperty, value); }
        }

        public ObservableCollection<Folder> Items { get; set; } = new ObservableCollection<Folder>();

        public FolderPicker()
        {
            DataContext = this;

            // Add shortcuts from %userprofile%\Links
            foreach (string path in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Links").Where(p => p.EndsWith(".lnk")))
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)((new Shell()).
                    NameSpace(System.IO.Path.GetDirectoryName(path)).
                    ParseName(System.IO.Path.GetFileName(path)).
                    GetLink);
                if (!string.IsNullOrEmpty(link.Path) && File.GetAttributes(link.Path).HasFlag(FileAttributes.Directory)) Items.Add((new Folder(link.Path)).LoadChildren(null, new RoutedEventArgs()));
            }

            // Add user profile
            Items.Add((new Folder(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).LoadChildren(null, new RoutedEventArgs())));

            // Add root drives (C:\, etc.)
            foreach (string path in Directory.GetLogicalDrives())
                Items.Add((new Folder(path)).LoadChildren(null, new RoutedEventArgs()));

            if (Items.Count == 1)
                Items[0].IsExpanded = true;

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectCommand?.Execute(SelectedFolder);
        }

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedFolder = ((Folder)e.NewValue).Path;
        }

    }

    public class Folder : INotifyPropertyChanged
    {
        string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                    if (IsExpanded)
                    {
                        IList<Folder> remove = null;
                        foreach (var f in Subfolders)
                        {
                            try
                            {
                                f.LoadChildren(this, new RoutedEventArgs());
                            }
                            catch (UnauthorizedAccessException)
                            {
                                if (remove == null) remove = new List<Folder>();
                                remove.Add(f);
                            }
                        }
                        if (remove != null) foreach (var f in remove) Subfolders.Remove(f);
                    }
                }
            }
        }

        public ObservableCollection<Folder> Subfolders { get; set; } = new ObservableCollection<Folder>();

        public Folder(string path)
        {
            Path = path;
            OpenInExplorerCommand = new RelayCommand(openInExplorer);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Folder LoadChildren(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var s in Directory.GetDirectories(Path))
                    Subfolders.Add(new Folder(s));
            }
            catch (UnauthorizedAccessException) { throw; }
            return this;
        }


        public ICommand OpenInExplorerCommand { get; set; }
        void openInExplorer(object o)
        {
            System.Diagnostics.Process.Start(o as string);
        }
    }
}
