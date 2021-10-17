using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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

        private async void connectDevice()
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
                USBFound.Text = "Opened device for communication."+ ((usbDevice!=null)?myDevices[0].Name: "Warning usbDevice is Null");
            }
        }

        private void connectButtonClick(object sender, RoutedEventArgs e)
        {
            connectDevice();
        }

        private async void BulkWrite(byte dataByte)
        {
            UInt32 bytesWritten = 0;

            UsbBulkOutPipe writePipe = usbDevice.DefaultInterface.BulkOutPipes[0];
            writePipe.WriteOptions |= UsbWriteOptions.ShortPacketTerminate;

            var stream = writePipe.OutputStream;

            DataWriter writer = new DataWriter(stream);

            writer.WriteByte(dataByte);

            try
            {
                bytesWritten = await writer.StoreAsync();
            }
            catch (Exception exception)
            {
                USBFound.Text = exception.Message.ToString();
            }
            finally
            {
                //MessageBox.Show("Data written: " + bytesWritten + " bytes.");
            }
        }


        private async void BulkRead()
        {
            UInt32 bytesRead = 0;

            UsbBulkInPipe readPipe = usbDevice.DefaultInterface.BulkInPipes[0];
            readPipe.ReadOptions |= UsbReadOptions.IgnoreShortPacket;

            var stream = readPipe.InputStream;
            DataReader reader = new DataReader(stream);

            try
            {
                bytesRead = await reader.LoadAsync(readPipe.EndpointDescriptor.MaxPacketSize);
            }
            catch (Exception exception)
            {
                USBFound.Text = exception.Message.ToString();
            }
            finally
            {
                //MessageBox.Show("Number of bytes: " + bytesRead);

                IBuffer buffer = reader.ReadBuffer(bytesRead);

                var dataReader = DataReader.FromBuffer(buffer);
                byte[] bytes = new byte[64];
                dataReader.ReadBytes(bytes);

                //potProgressBar.Value = bytes[1] == 1 ? 0 : 1023;

                //this.BeginInvoke(new Action(() => label1.Text = bytes[1] == 1 ? "not pressed" : "pressed"));
                USBFound.Text=buffer.ToString();

            }
        }

        private void transferButtonClick(object sender, RoutedEventArgs e)
        {
            BulkWrite(0x81);
            BulkRead();
        }
    }
}
