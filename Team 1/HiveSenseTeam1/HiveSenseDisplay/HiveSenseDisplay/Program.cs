using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;
using System.Text;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.LoveElectronics;

namespace HiveSenseDisplay
{
    public partial class Program
    {
        public Gadgeteer.Modules.IngenuityMicro.RfPipe RfPipe;
        public static Window mainWindow;
        public int LeftMargin = 5;
        public Text HiveTime;

        public Text HiveTemp;
        public Text HiveHumidity;
        public Text HiveAlert;
        public GT.Timer timer;
        public string SensorMessage = string.Empty;
        
        public Border Facepic;


        Bitmap BG;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            RfPipe = new GTM.IngenuityMicro.RfPipe(11);
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/

            timer = new GT.Timer(1000);
            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");
            SetupUI();

            RfPipe.DataReceived += new GTM.IngenuityMicro.RfPipe.RfPipeReceivedHandler(RfPipe_DataReceived);
            HappyButton.ButtonPressed += new GTM.LoveElectronics.Button.ButtonEventHandler(HappyButton_ButtonPressed);
            Mehbutton.ButtonPressed += new GTM.LoveElectronics.Button.ButtonEventHandler(Mehbutton_ButtonPressed);
            SadButton.ButtonPressed += new GTM.GHIElectronics.Button.ButtonEventHandler(SadButton_ButtonPressed);
        }

        void RfPipe_DataReceived(string val)
        {
            SensorMessage = val;
            if (!timer.IsRunning)
            {
                timer.Start();
            }

        }

        void SadButton_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
           
            var BeeFace = Resources.GetBitmap(Resources.BitmapResources.SadBee);
            Facepic.Background = new ImageBrush(BeeFace);
            Facepic.Invalidate();
        }

        void Mehbutton_ButtonPressed(GTM.LoveElectronics.Button sender, GTM.LoveElectronics.Button.ButtonState state)
        {
            var BeeFace = Resources.GetBitmap(Resources.BitmapResources.UncdertainBee);
            Facepic.Background = new ImageBrush(BeeFace);
            Facepic.Invalidate();
        }

        void HappyButton_ButtonPressed(GTM.LoveElectronics.Button sender, GTM.LoveElectronics.Button.ButtonState state)
        {
            var BeeFace = Resources.GetBitmap(Resources.BitmapResources.HappyBee);
            Facepic.Background = new ImageBrush(BeeFace);
            Facepic.Invalidate();
        }

        void timer_Tick(GT.Timer timer)
        {
            HiveTemp.TextContent = SensorMessage;
            HiveTime.Invalidate();
         }

        void SetupUI()
        {
            BG = Resources.GetBitmap(Resources.BitmapResources.UncdertainBee);

            var BeeFace = Resources.GetBitmap(Resources.BitmapResources.UncdertainBee);

            // setup the display window
            mainWindow = display_T35.WPFWindow;
            // create the canvas
            Canvas Layout = new Canvas();
            //// create the background

            Border background = new Border();
            //background.Background = new SolidColorBrush(Colors.DarkGray);
            background.Background = new ImageBrush(BG);
            background.Height = 240;
            background.Width = 320;
            ////add the BG to the layout
            Layout.Children.Add(background);


            Facepic = new Border();
            Facepic.Background = new ImageBrush(BeeFace);
            Facepic.Height = 240;
            Facepic.Width = 320;
            Layout.Children.Add(Facepic);

            HiveTime = new Text("Time: 00:00:00");
            HiveTime.ForeColor = Colors.Yellow;
            HiveTime.Font = Resources.GetFont(Resources.FontResources.Calibri24);
            Layout.Children.Add(HiveTime);
            //Set the text position
            Canvas.SetLeft(HiveTime, 85);
            Canvas.SetTop(HiveTime, 2);



            HiveTemp = new Text("00");
            HiveTemp.ForeColor = Colors.White;
            HiveTemp.Font = Resources.GetFont(Resources.FontResources.Calibri24);
            Layout.Children.Add(HiveTemp);
            //Set the text position
            Canvas.SetLeft(HiveTemp, LeftMargin);
            Canvas.SetTop(HiveTemp, 2);

            HiveHumidity = new Text("20");
            HiveHumidity.ForeColor = Colors.White;
            HiveHumidity.Font = Resources.GetFont(Resources.FontResources.Calibri24);
            Layout.Children.Add(HiveHumidity);
            //Set the text position
            Canvas.SetLeft(HiveHumidity, LeftMargin);
            Canvas.SetTop(HiveHumidity, 85);

            // draw to screen
            mainWindow.Child = Layout;

        }
    }
}
