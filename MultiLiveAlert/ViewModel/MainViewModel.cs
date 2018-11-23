using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
namespace MultiLiveAlert.ViewModel
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        protected BindableBase() { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
    }
    public interface IMainViewModel
    {
        event EventHandler CloseRequested;
        void RequestClose();
        Task InitializeAsync();
    }
    public class MainViewModel : BindableBase, IMainViewModel
    {
        public event EventHandler CloseRequested;
        private bool _isGrouping;
        public bool IsGrouping
        {
            get { return _isGrouping; }
            set
            {
                _isGrouping = value;
                if (_isGrouping)
                {
                    Collection.GroupDescriptions.Add(new PropertyGroupDescription("Broadcaster.Name"));
                    RaisePropertyChanged(nameof(Collection));
                }
                else
                {
                    Collection.GroupDescriptions.Clear();
                    RaisePropertyChanged(nameof(Collection));
                }
                RaisePropertyChanged(nameof(IsGrouping));
                RaisePropertyChanged(nameof(IsNotGrouping));
            }
        }

        public bool IsNotGrouping
        {
            get => !IsGrouping;
            set
            {
                IsGrouping = !value;
            }
        }

        public void RequestClose()
        {
            OnCloseRequested(EventArgs.Empty);
        }
        protected virtual void OnCloseRequested(EventArgs e)
        {
            CloseRequested?.Invoke(this, e);
        }

        public ICollectionView Collection { get; }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        private ObservableCollection<Data> _collection;
        public MainViewModel()
        {
            var ner = new Broadcaster { Name = "NER" };
            var kagawa = new Broadcaster { Name = "â¡êÏ" };
            var midori = new Broadcaster { Name = "â°éRóŒ" };
            _collection = new ObservableCollection<Data>
            {
                new Data{ Broadcaster = ner, Id = "123"},
                new Data{ Broadcaster=ner, Id = "555"},
                new Data{Broadcaster=kagawa, Id="999"},
                new Data{Broadcaster = midori, Id = "224"},
            };
            Collection = CollectionViewSource.GetDefaultView(_collection);
            IsGrouping = true;
        }
    }
    public class Data
    {
        public Broadcaster Broadcaster { get; set; }
        public string Id { get; set; }
    }
    public class Broadcaster
    {
        public string Name { get; set; }
    }
}