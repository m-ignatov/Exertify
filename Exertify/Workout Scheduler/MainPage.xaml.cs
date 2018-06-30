using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit;
using Workout_Scheduler.DataModel;
using Workout_Scheduler.Tiles;


namespace Workout_Scheduler
{
    public sealed partial class MainPage : Page
    {
        object datacontext;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var exercises = await App.DataModel.GetExercises();
            this.DataContext = exercises;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            //Frame frame = Window.Current.Content as Frame;
            var lastPage = Frame.BackStack.Last().SourcePageType;

            if (lastPage.Name == "MainPage" && Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
            return;
        }

        #region Methods


        ListViewBase lvb;
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            lvb = sender as ListViewBase;
            ContinuumNavigationTransitionInfo.SetExitElementContainer(lvb, true);
            Frame.Navigate(typeof(ReviewPage), e.ClickedItem);
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (lvb != null) ContinuumNavigationTransitionInfo.SetExitElementContainer(lvb, false);
            Frame.Navigate(typeof(AddExercise));
        }

        private async void flyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var msg = new Windows.UI.Popups.MessageDialog("Are you sure?", "Exertify");
            msg.Commands.Add(new UICommand(
                "Delete",
                new UICommandInvokedHandler(this.CommandInvokedHandler)));
            msg.Commands.Add(new UICommand(
                 "Cancel",
                 new UICommandInvokedHandler(this.CommandInvokedHandler)));
            msg.DefaultCommandIndex = 0;
            msg.CancelCommandIndex = 1;

            await msg.ShowAsync();
        }
        private async void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Delete")
            {
                App.DataModel.DeleteExercise((Exercise)datacontext);
            }
            else if (command.Label == "Remove All")
            {
                App.DataModel.DeleteAll();
            }
            else if (command.Label == "Yes")
            {
                List<string> list = await App.ComboBoxModel.GetItems();
                ListView myListView = FindChildControl<ListView>(this, "myListView") as ListView;
                
                int size = list.Count();

                if (myListView.Items.Count != 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        string title = list[i];
                        int flag = 0;

                        foreach (Exercise ex in myListView.Items.ToList())
                        {
                            if (ex.Title == list[i]) { flag = 1; break; }
                        }
                        if (flag == 0)
                        {
                            Exercise temp = new Exercise();
                            temp.Title = list[i].ToString();
                            temp.Reps = "5";
                            temp.Sets = "5";
                            App.DataModel.AddExercise_NoSave(temp);
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < size; i++)
                    {
                        Exercise temp = new Exercise();
                        temp.Title = list[i].ToString();
                        temp.Reps = "5";
                        temp.Sets = "5";
                        App.DataModel.AddExercise_NoSave(temp);
                    }
                }
                await App.DataModel.saveExerciseDataAsync();
            }
        }

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);

            datacontext = (e.OriginalSource as FrameworkElement).DataContext;
        }

        private void Hub_SectionsInViewChanged(object sender, SectionsInViewChangedEventArgs e)
        {
            var section = Hub.SectionsInView[0];

            if (section.Header.ToString() != "exercises")
            {
                myCommandBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                myCommandBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private async void addAllBarButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = new Windows.UI.Popups.MessageDialog("Are you sure to add new exercises?", "Exertify");
            msg.Commands.Add(new UICommand(
                "Yes",
                new UICommandInvokedHandler(this.CommandInvokedHandler)));
            msg.Commands.Add(new UICommand(
                 "Cancel",
                 new UICommandInvokedHandler(this.CommandInvokedHandler)));
            msg.DefaultCommandIndex = 0;
            msg.CancelCommandIndex = 1;

            await msg.ShowAsync();
        }

        private DependencyObject FindChildControl<T>(DependencyObject control, string ctrlName)
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                FrameworkElement fe = child as FrameworkElement;
                // Not a framework element or is null
                if (fe == null) return null;

                if (child is T && fe.Name == ctrlName)
                {
                    // Found the control so return
                    return child;
                }
                else
                {
                    // Not found it - search children
                    DependencyObject nextLevel = FindChildControl<T>(child, ctrlName);
                    if (nextLevel != null)
                        return nextLevel;
                }
            }
            return null;
        }
                
        private void myListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            int size = App.ComboBoxModel.getCount();
            if (App.DataModel.getCount() == size)
            {
                addAllBarButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                addAllBarButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        #endregion
    }

    #region Use This To Implement Swipe Gestures
    /*
    public class MyListView : ListView
    {
        protected override Windows.UI.Xaml.DependencyObject GetContainerForItemOverride()
        {
            // Use the 'MyListViewItem' in order to obtain Pointer Position information
            return new MyListViewItem();
        }
    }
    public class MyListViewItem : ListViewItem
    {
        IList<PointerPoint> point;
        object pressedItem;

        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            point = e.GetIntermediatePoints(this);

            pressedItem = (e.OriginalSource as FrameworkElement).DataContext;
        }
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
        }
        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            IList<PointerPoint> list = e.GetIntermediatePoints(this);
            if (e.GetIntermediatePoints(this).Count > 0 && pressedItem!=null)
            {
                int start = (int)point[0].Position.X;
                int end = (int)list[0].Position.X;
                int dif = end - start;

                Debug.WriteLine(start);
                Debug.WriteLine(end);

                if (dif>69)
                {                   
                    App.DataModel.DeleteExercise((Exercise)pressedItem);
                }
            }
            //Debug.WriteLine("OnPointerMoved:{0} {1}", DateTime.Now, e.GetIntermediatePoints(this).Count > 0 ? list[0].Position : new Point(0, 0));
            base.OnPointerMoved(e);
        }
    }
    */
    #endregion
}
