using System;
using math = System.Math;
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
using Gadgeteer.Modules.IngenuityMicro;

namespace HiveSenseDisplay
{
    public partial class Program
    {
        private Gadgeteer.Modules.IngenuityMicro.RfPipe RfPipe;
        private static Window mainWindow;
        private int LeftMargin = 5;
        private Text HiveTime;
        private Text HiveTemp;
        private Text HiveHumidity;
        private Text HiveAlert;
        private GT.Timer timer;
        private Border Facepic;
        private Queue messageQueue = new Queue();

        private Bitmap BG;

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
        }

        void RfPipe_DataReceived(string val)
        {
            messageQueue.Enqueue(val);

            if (!timer.IsRunning)
            {
                timer.Start();
            }
        }

        void timer_Tick(GT.Timer timer)
        {
            while (messageQueue.Count > 0)
            {
                var message = (string)messageQueue.Dequeue();
                if (TryParseAndDisplayMessage(message, "TempDegC", HiveTemp, true))
                {
                    return;
                }
                if (TryParseAndDisplayMessage(message, "HumidityPc", HiveHumidity, true))
                {
                    return;
                }
                if (TryParseAndDisplayMessage(message, "Time", HiveTime))
                {
                    return;
                }

                if (TryParseAndDisplayMessage(message, "Alert", HiveAlert))
                {
                    var BeeFace = Resources.GetBitmap(Resources.BitmapResources.SadBee);
                    Facepic.Background = new ImageBrush(BeeFace);
                    Facepic.Invalidate();
                }
            }
        }

        private bool TryParseAndDisplayMessage(string message, string identifier, Text textArea, bool numericValue = false)
        {
            string payload;
            if (TryExtractPayloadFromSensorMessage(message, identifier, out payload))
            {
                var displayPayload = payload;

                double numeric;
                if (numericValue)
                {
                    double.TryParse(payload, out numeric);
                    displayPayload = (math.Round(numeric * 100) / 100).ToString().Substring(0, 5);
                }

                textArea.TextContent = displayPayload;
                textArea.Invalidate();
                return true;
            }
            return false;
        }

        private bool TryExtractPayloadFromSensorMessage(string message, string identifier, out string payload)
        {
            if (message.Length < identifier.Length)
            {
                payload = string.Empty;
                return false;
            }

            if (message.Substring(0, identifier.Length) == identifier)
            {
                payload = message.TrimStart(identifier.ToCharArray());
                return true;
            }
            payload = string.Empty;
            return false;
        }

        void SetupUI()
        {
            BG = Resources.GetBitmap(Resources.BitmapResources.UncertainBee);

            var BeeFace = Resources.GetBitmap(Resources.BitmapResources.UncertainBee);

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
            HiveTime.ForeColor = Colors.White;
            HiveTime.Font = Resources.GetFont(Resources.FontResources.Calibri24);
            Layout.Children.Add(HiveTime);
            //Set the text position
            Canvas.SetLeft(HiveTime, 115);
            Canvas.SetTop(HiveTime, 2);

            HiveAlert = new Text("Test");
            HiveAlert.ForeColor = Colors.White;
            HiveAlert.Font = Resources.GetFont(Resources.FontResources.NinaB);
            Layout.Children.Add(HiveAlert);
            //Set the text position
            Canvas.SetLeft(HiveAlert, 115);
            Canvas.SetTop(HiveAlert, 40);

            HiveTemp = new Text("00.00");
            HiveTemp.ForeColor = Colors.White;
            HiveTemp.Font = Resources.GetFont(Resources.FontResources.Calibri24);
            Layout.Children.Add(HiveTemp);
            //Set the text position
            Canvas.SetLeft(HiveTemp, LeftMargin);
            Canvas.SetTop(HiveTemp, 35);

            HiveHumidity = new Text("20.00");
            HiveHumidity.ForeColor = Colors.White;
            HiveHumidity.Font = Resources.GetFont(Resources.FontResources.Calibri24);
            Layout.Children.Add(HiveHumidity);
            //Set the text position
            Canvas.SetLeft(HiveHumidity, LeftMargin);
            Canvas.SetTop(HiveHumidity, 125);

            // draw to screen
            mainWindow.Child = Layout;

        }
    }
}
