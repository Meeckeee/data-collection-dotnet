﻿/*******************************************************************************
  * Copyright 2018 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  http://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Commands;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels;
using Esri.ArcGISRuntime.Mapping.Popups;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.WPF.Views
{
    /// <summary>
    /// Interaction logic for AttachmentsView.xaml
    /// </summary>
    public partial class AttachmentsView : UserControl
    {
        public AttachmentsView()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                // list of file types supported as attachments
                // https://developers.arcgis.com/rest/services-reference/query-attachments-feature-service-layer-.htm
                dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                dialog.Title = "Please select an image as attachment.";
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    BroadcastMessenger.Instance.RaiseBroadcastMessengerValueChanged(dialog.FileName, BroadcastMessageKey.NewAttachmentFile);

                    // TODO: Remove tight coupling by setting this property from the view
                    ((MainViewModel)DataContext).AttachmentsViewModel.AttachmentMode = AttachmentMode.Edit;
                }
            }
        }


        /// <summary>
        /// Event handler for user clicking the attachment to open it
        /// </summary>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            var attachment = ((AttachmentWithThumbnail)image.DataContext).Attachment;

            var newFileName = attachment.Filename + Path.GetExtension(attachment.Name);

            // if the file exists, rename it to contain the proper extension
            if (File.Exists(attachment.Filename))
            {
                File.Move(attachment.Filename, newFileName);
            }

            // if the renamed file exists, open it in the user's preferred application
            if (File.Exists(newFileName))
            {
                System.Diagnostics.Process.Start(newFileName);
            }
            else
            {
                UserPromptMessenger.Instance.RaiseMessageValueChanged("File not found", "The attachment file you are trying to open could not be located. Please try restarting the application.", true);
                return;
            }
        }
    }
}
