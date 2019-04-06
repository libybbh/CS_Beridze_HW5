using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Beridze_5.Tools;
using Beridze_5.Data;
using Beridze_5.View;

namespace Beridze_5.Models
{ 
    //acccidentaly called it Model, but it`s ViewModel
    internal class DataTableModel : INotifyPropertyChanged
    {
        private ObservableCollection<SystemProcess> _processes;
        private readonly Thread _updateThread;
        private SystemProcess _selectedProcess;
        private RelayCommand _endTaskCommand;
        private RelayCommand _getInfoCommand;
        private RelayCommand _openFileLocationCommand;
        private InformWindoView _infoWindow;

        public bool IsItemSelected => SelectedProcess != null;

        public SystemProcess SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
                OnPropertyChanged("IsItemSelected");
            }
        }

        public ObservableCollection<SystemProcess> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        internal DataTableModel()
        {
            //_showLoaderAction = showLoaderAction;
            _updateThread = new Thread(UpdateUsers);
            Thread initializationThread = new Thread(InitializeProcesses);
            initializationThread.Start();
        }

        public RelayCommand EndTaskCommand => _endTaskCommand ?? (_endTaskCommand = new RelayCommand(EndTaskImpl));
        public RelayCommand GetInfoCommand => _getInfoCommand ?? (_getInfoCommand = new RelayCommand(GetInfoImpl));
        public RelayCommand OpenFileLocationCommand => _openFileLocationCommand ?? (_openFileLocationCommand = new RelayCommand(OpenFileLocationImpl));

        private void EndTaskImpl(object o)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    process.Kill();
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private async void GetInfoImpl(object o)
        {
            try
            {
                await Task.Run(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                    {
                        System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
                        _infoWindow?.Close();
                        try
                        {
                            _infoWindow = new InformWindoView(process);
                            _infoWindow.Show();
                        }
                        catch (Win32Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OpenFileLocationImpl(object o)
        {
            await Task.Run(() =>
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    string fullPath = process.MainModule.FileName;
                    System.Diagnostics.Process.Start("explorer.exe", fullPath.Remove(fullPath.LastIndexOf('\\')));
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private async void UpdateUsers()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                    {
                        try
                        {
                            lock (Processes)
                            {
                                List<SystemProcess> toRemove =
                                    new List<SystemProcess>(
                                        Processes.Where(proc => !SystemProcessDB.Processes.ContainsKey(proc.Id)));
                                foreach (SystemProcess proc in toRemove)
                                {
                                    Processes.Remove(proc);
                                }

                                List<SystemProcess> toAdd =
                                    new List<SystemProcess>(
                                        SystemProcessDB.Processes.Values.Where(proc => !Processes.Contains(proc)));
                                foreach (SystemProcess proc in toAdd)
                                {
                                    Processes.Add(proc);
                                }
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (ArgumentNullException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (InvalidOperationException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    });
                });
                Thread.Sleep(4000);
            }
        }

        private async void InitializeProcesses()
        {
            await Task.Run(() =>
            {
                Processes = new ObservableCollection<SystemProcess>(SystemProcessDB.Processes.Values);
            });
            _updateThread.Start();
            while (SystemProcessDB.Processes.Count == 0)
                Thread.Sleep(3000);
        }

        internal void Close()
        {
            _updateThread.Join(3000);
        }

        #region Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
