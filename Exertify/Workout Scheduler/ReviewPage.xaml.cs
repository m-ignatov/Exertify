using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Workout_Scheduler.DataModel;
using Workout_Scheduler.Tiles;

namespace Workout_Scheduler
{
    public sealed partial class ReviewPage : Page
    {
        Exercise ex;
        short sets;
        short reps;

        public int maximum_s { get; set; }
        public int maximum_r { get; set; }
        public int minimum_r { get; set; }
        public int minimum_s { get; set; }
        UpdateTiles update;

        public ReviewPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            update = new UpdateTiles();
            maximum_r = 40;
            minimum_r = 5;
            minimum_s = 1;
            maximum_s = 10;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ex = (Exercise)e.Parameter;
            reps = short.Parse(ex.Reps);
            sets = short.Parse(ex.Sets);
            this.DataContext = ex;
            FrameImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Exercise_Images/" + ex.Title + ".jpg"));

            if (reps == minimum_r) removeReps.IsEnabled = false;
            else if (reps == maximum_r) addReps.IsEnabled = false;
            if (sets == minimum_s) removeSets.IsEnabled = false;
            else if (sets == maximum_s) addSets.IsEnabled = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            update.setTile(ex);
            base.OnNavigatedFrom(e);
        }

        private async void add_remove_button(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "removeSets":
                    ex.Sets = (--sets).ToString();
                    break;
                case "addSets":
                    ex.Sets = (++sets).ToString();
                    break;
                case "removeReps":
                    ex.Reps = (--reps).ToString();
                    break;
                case "addReps":
                    ex.Reps = (++reps).ToString();
                    break;
            }
            await App.DataModel.saveExerciseDataAsync();
            check(btn);
        }

        private void check(Button btn)
        {
            string name = btn.Name;
            reps = short.Parse(ex.Reps);
            sets = short.Parse(ex.Sets);
            if (name == "addReps" && reps <= maximum_r)
            {
                if (reps == maximum_r) addReps.IsEnabled = false;
                removeReps.IsEnabled = true;
                return;
            }
            else if (name == "removeReps" && reps >= minimum_r)
            {
                if (reps == minimum_r) removeReps.IsEnabled = false;
                addReps.IsEnabled = true;
                return;
            }
            else if (name == "addSets" && sets <= maximum_s)
            {
                if (sets == maximum_s) addSets.IsEnabled = false;
                removeSets.IsEnabled = true;
                return;
            }
            else if (sets >= minimum_s)
            {
                if (sets == minimum_s) removeSets.IsEnabled = false;
                addSets.IsEnabled = true;
                return;
            }
        }
    }
}
