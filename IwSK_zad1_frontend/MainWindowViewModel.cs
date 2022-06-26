using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IwSK_zad1_frontend
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public string Message { get; set; }

        public ObservableCollection<string> ReceivedList { get; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
