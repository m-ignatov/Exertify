using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Workout_Scheduler.DataModel;
using System.Text;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Controls;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Media.Animation;


namespace Workout_Scheduler
{
    public sealed partial class AddExercise : Page
    {
        public AddExercise()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled; // Added by Momchil
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {            
            var items = await App.ComboBoxModel.GetItems();
            this.DataContext = items;
            myComboBox.SelectedIndex = -1;
            repCounter.Value = 1;
            setCounter.Value = 1;

        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Exercise newExercise = new Exercise();
            newExercise.Title = myComboBox.SelectedValue.ToString();
            newExercise.Reps = repCounter.Value.ToString();
            newExercise.Sets = setCounter.Value.ToString();
            App.DataModel.AddExercise(newExercise);
            Frame.Navigate(typeof(MainPage));
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
