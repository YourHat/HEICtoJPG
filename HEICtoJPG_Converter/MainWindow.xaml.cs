using FileFormat.Heic.Decoder;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace HEICtoJPG_Converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] files;
        public MainWindow()
        {
            InitializeComponent();
            
        }
        private void fileDropFunc(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string filename = "";
                foreach (var f in files)
                {
                    filename += f + "\n";
                }
                
                
                filePathName.Content = filename;
            }
        }
        private void OpenFileDialogFunc(object sender, DragEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "HEIC files | *.heic";
            fileDialog.Title = "Please Pick a HEIC file....";
            bool? success = fileDialog.ShowDialog();
            if(success == true)
            {
                string path = fileDialog.FileName;
                filePathName.Content = path;
            }
        }
        private async void ConvertHEICtoJPG(object sender, RoutedEventArgs e)
        {
            if ((string)filePathName.Content != "Drag and Drop Files Here")
            {
                Storyboard blinkingStoryboard = (Storyboard)this.Resources["BlinkingStoryboard"];
                blinkingTextBlock.Text = "Converting...";
                blinkingStoryboard.Begin();
                int filetotal = files.Count();
                int filedone = 0;
                fileamount.Foreground = new SolidColorBrush(Colors.Black);
                fileamount.Text = $"( {filedone} / {filetotal} )";
                foreach (var f in files)
                {
                    if (Path.GetExtension((String)f).ToLower() == ".heic")
                    {

                        string filepath = f;
                        string filepathOut = f + ".jpg";

                        await Task.Run(() =>
                        {

                            using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                HeicImage image = HeicImage.Load(fs);
                                var pixels = image.GetByteArray(FileFormat.Heic.Decoder.PixelFormat.Bgra32);
                                var width = (int)image.Width;
                                var height = (int)image.Height;

                                var wbitmap = new WriteableBitmap(width, height, 72, 72, PixelFormats.Bgra32, null);
                                var rect = new Int32Rect(0, 0, width, height);
                                wbitmap.WritePixels(rect, pixels, 4 * width, 0);

                                using FileStream saveStream = new FileStream(filepathOut, FileMode.OpenOrCreate);
                                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(wbitmap));
                                encoder.Save(saveStream);

                            }
                        });

                    }
                    else
                    {
                        MessageBox.Show($"{f} is not heic file");
                    }
                    filedone++;
                    fileamount.Text = $"( {filedone} / {filetotal} )";
                }
                filePathName.Content = "Drag and Drop Files Here";
                blinkingStoryboard.Pause();
                blinkingTextBlock.Foreground = new SolidColorBrush(Colors.Black);
                blinkingTextBlock.Text = "Done!";
            }
            else
            {
                MessageBox.Show("Drag and Drop Files first");
            }

        }

        }

}