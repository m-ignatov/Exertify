using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Workout_Scheduler.DataModel;
using NotificationsExtensions.TileContent;

namespace Workout_Scheduler.Tiles
{
    class UpdateTiles
    {
        public void setTile(Exercise ex)
        {
            string imagePath_Wide = "ms-appx:///assets/tiles_images/wide_tile/" + ex.Title + ".jpg";
            string imagePath_Square = "ms-appx:///assets/tiles_images/square_tile/" + ex.Title + ".jpg"; ;
            string title = ex.Title;
            string reps = "Reps: " + ex.Reps;
            string sets = "Sets: " + ex.Sets;

            ITileSquare310x310Image tileContent = TileContentFactory.CreateTileSquare310x310Image();
            tileContent.Image.Src = imagePath_Square;
            tileContent.Image.Alt = "image";

            // Create a notification for the Wide310x150 tile using one of the available templates for the size.
            ITileWide310x150PeekImage02 wideTile = TileContentFactory.CreateTileWide310x150PeekImage02();
            wideTile.TextHeading.Text = title;
            wideTile.TextBody2.Text = reps;
            wideTile.TextBody3.Text = sets;
            wideTile.Image.Src = imagePath_Wide;

            // Create a notification for the Square150x150 tile using one of the available templates for the size.
            ITileSquare150x150PeekImageAndText01 square150x150Content = TileContentFactory.CreateTileSquare150x150PeekImageAndText01();
            square150x150Content.Image.Src = imagePath_Square;
            square150x150Content.TextBody1.Text = title;
            square150x150Content.TextBody2.Text = reps;
            square150x150Content.TextBody3.Text = sets;

            // Create a notification for the Square71x71 tile using one of the available templates for the size.
            //ITileSquare71x71Image square71x71Content = TileContentFactory.CreateTileSquare71x71Image();
            //square71x71Content.Image.Src = imagePath;

            // Attach the Square71x71 template to the Square150x150 template.
            //square150x150Content.Square71x71Content = square71x71Content;

            // Attach the Square150x150 template to the Wide310x150 template.
            wideTile.Square150x150Content = square150x150Content;

            // Attach the Wide310x150 to the Square310x310 template.
            tileContent.Wide310x150Content = wideTile;

            // Send the notification to the application’s tile.
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());
        }
    }
}
