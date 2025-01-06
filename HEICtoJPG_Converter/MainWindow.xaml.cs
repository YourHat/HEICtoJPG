using ImageMagick;
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

                var converttasks = new List<Task>();
                

                foreach (var f in files)
                {
                    if (Path.GetExtension((String)f).ToLower() == ".heic")
                    {

                        string filepath = f;
                        string filepathOut = f + ".jpg";

                        var conversiontask = Task.Run(() =>
                        {

                            using (MagickImage image = new MagickImage(filepath))
                            {
                                image.Format = MagickFormat.Jpeg;
                                image.Write(filepathOut);
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    filedone++;
                                    fileamount.Text = $"( {filedone} / {filetotal} )";

                                });
                            }
                        });

                        converttasks.Add(conversiontask);
                    }
                    else
                    {
                        MessageBox.Show($"{f} is not heic file");
                    }
                    
                }

                await Task.WhenAll(converttasks);
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