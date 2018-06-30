using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml.Controls.Primitives;
using System.ComponentModel;
using Windows.UI.Popups;
using System.Diagnostics;
using System.Runtime.Serialization;
using Windows.Storage.Streams;
using System.Runtime.CompilerServices;

namespace Workout_Scheduler.DataModel
{
    public class Exercise : INotifyPropertyChanged
    {
        private string title;
        private string reps;
        private string sets;

        public string Title
        {
            get { return title; }
            set { this.SetProperty(ref this.title, value); }
        }

        public string Reps
        {
            get { return reps; }
            set { this.SetProperty(ref this.reps, value); }
        }

        public string Sets
        {
            get { return sets; }
            set { this.SetProperty(ref this.sets, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DataSource : INotifyPropertyChanged    
    {
        private ObservableCollection<Exercise> _exercises;
        public ObservableCollection<Exercise> _Exercises
        {
            get { return _exercises; }
            set { this.SetProperty(ref this._exercises, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        


        const string fileName = "exercises.json";

        public DataSource()
        {
            _Exercises = new ObservableCollection<Exercise>();
        }

        public async Task<ObservableCollection<Exercise>> GetExercises()
        {
            await ensureDataLoaded();
            return _exercises;
        }
        public int getCount()
        {
            return _exercises.Count();
        }

        private async Task ensureDataLoaded()
        {
            if (_exercises.Count == 0)
            {
                await getExerciseDataAsync();
                return;
            }
        }

        private async Task getExerciseDataAsync()
        {
            if (_exercises.Count != 0) return;

            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Exercise>));

            try
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName))
                {
                    _exercises = (ObservableCollection<Exercise>)jsonSerializer.ReadObject(stream);
                }
            }
            catch
            {
                _exercises = new ObservableCollection<Exercise>();
            }
        }

        public async void AddExercise(Exercise Exercise)
        {
            _exercises.Add(Exercise);
            await saveExerciseDataAsync();
        }
        public void AddExercise_NoSave(Exercise ex)
        {
            _exercises.Add(ex);
        }

        public async void DeleteExercise(Exercise exercise)
        {
            _exercises.Remove(exercise);
            await saveExerciseDataAsync();
        }
        public async void DeleteAll()
        {
            foreach (Exercise item in _exercises.ToList())
            {
                _exercises.Remove(item);
            }
            await saveExerciseDataAsync();
        }

        public async Task saveExerciseDataAsync()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Exercise>));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, _exercises);
            }
        }
    }

    public class ComboBoxList
    {
        private List<String> _list = new List<string>();

        public ComboBoxList()
        {
            _list = new List<string>();
            WriteToComboBox();
        }

        public async Task WriteToComboBox()
        {
            if (_list.Count == 0)
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///exercises_data.txt"));
                using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    while (!sRead.EndOfStream)
                    {
                        string fileContent = "";
                        fileContent = await sRead.ReadLineAsync();
                        _list.Add(fileContent);
                    }
                }
                _list.Sort();
            }
        }
        public async Task<List<string>> GetItems()
        {
            await WriteToComboBox();
            return _list;
        }

        public int getCount()
        {
            return _list.Count();
        }
    }
}

