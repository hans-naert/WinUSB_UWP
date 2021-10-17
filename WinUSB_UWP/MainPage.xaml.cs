using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WinUSB_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        UsbDevice usbDevice;

        private async void connectButtonClick(object sender, RoutedEventArgs e)
        {
            UInt32 vid = 0xC251;
            UInt32 pid = 0x4307;

            string aqs = UsbDevice.GetDeviceSelector(vid, pid);

            var myDevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(aqs);

            try
            {
                usbDevice = await UsbDevice.FromIdAsync(myDevices[0].Id);
            }
            catch (Exception exception)
            {
                USBFound.Text = exception.Message.ToString();
            }
            finally
            {
                USBFound.Text = "Opened device for communication.";
            }

        }
    }
}
