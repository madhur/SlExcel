using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.SharePoint.Client;
using System.Threading;
using System.IO;
using System.Windows.Resources;
using excel_create.Controls;
using Common;


namespace excel_create.Controls
{

    public partial class Attachments : UserControl
    {
        private const string libName = "Idea Attachments";
        string folderName, newFolderName;
        private ClientContext myClContext;
        public SelectedFiles selectedFiles;


        public Attachments()
        {
            InitializeComponent();
            selectedFiles = new SelectedFiles();

            ConnectToSP();

            FileListBox.DataContext = selectedFiles;
            FileListBox.ItemsSource = selectedFiles;
           
            RemoveButton.IsEnabled = false;

        }

        private void FileListBox_Drop(object sender, DragEventArgs e)
        {

        }


        private void UploadFile(FileInfo fileToUpload, string libraryTitle, string folderName)
        {
            var web = myClContext.Web;
            List destinationList = web.Lists.GetByTitle(libraryTitle);

            var fciFileToUpload = new FileCreationInformation();

            Stream streamToUpload = fileToUpload.OpenRead();
            int length = (int)streamToUpload.Length;  // get file length

            fciFileToUpload.Content = new byte[length];

            int count = 0;                        // actual number of bytes read
            int sum = 0;                          // total number of bytes read

            while ((count = streamToUpload.Read(fciFileToUpload.Content, sum, length - sum)) > 0)
                sum += count;  // sum is a buffer offset for next reading
            streamToUpload.Close();

            fciFileToUpload.Url = fileToUpload.Name;

            Microsoft.SharePoint.Client.File clFileToUpload = null;
            if (string.IsNullOrEmpty(folderName))
            {
                clFileToUpload = destinationList.RootFolder.Files.Add(fciFileToUpload);

                myClContext.Load(clFileToUpload);

                myClContext.ExecuteQueryAsync((s, ee) =>
                {

                    Dispatcher.BeginInvoke(() =>
                    {
                        selectedFiles.Add(new FileEntry(fileToUpload.Name, fileToUpload.Name));
                        RemoveButton.IsEnabled = true;
                        
                    }
                        );



                },
                (s, ee) =>
                {
                    Console.WriteLine(ee.Message);

                });

            }
            else
            {
                FolderCollection folderCol = destinationList.RootFolder.Folders;
                //myClContext.Load(folderCol, items => items.Include(fldr => fldr.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase)));

                myClContext.Load(folderCol);


                myClContext.ExecuteQueryAsync((s, ee) =>
                {

                    for (int i = 0; i < folderCol.Count; ++i)
                    {
                        if (folderCol[i].Name.Equals(folderName, StringComparison.OrdinalIgnoreCase))
                        {
                            clFileToUpload = folderCol[i].Files.Add(fciFileToUpload);

                            myClContext.Load(clFileToUpload);
                            break;
                        }

                    }

                    myClContext.ExecuteQueryAsync((ss, eee) =>
                    {

                        Dispatcher.BeginInvoke(() =>
                        {
                            selectedFiles.Add(new FileEntry(fileToUpload.Name, fileToUpload.Name));
                            RemoveButton.IsEnabled = true;
                            
                        }
                            );



                    },
              (ss, eee) =>
              {
                  Console.WriteLine(eee.Message);

              });



                },
              (s, ee) =>
              {
                  Console.WriteLine(ee.Message);

              });
            }
        }

        private void ConnectToSP()
        {
            myClContext = ClientContext.Current;


        }

        public void CreateFolder(string siteUrl, string listName, string relativePath, string folderName)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                Folder rootFolder = list.RootFolder;

                clientContext.Load(rootFolder);



                ListItemCreationInformation newItem = new ListItemCreationInformation();
                newItem.UnderlyingObjectType = FileSystemObjectType.Folder;
                //newItem.FolderUrl = siteUrl + listName;
                if (!relativePath.Equals(string.Empty))
                {
                    newItem.FolderUrl += "/" + relativePath;
                }
                newItem.LeafName = folderName;
                
                ListItem item = list.AddItem(newItem);
                item["Title"] = folderName;
                item.Update();

                clientContext.Load(list);

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    Folder newFolder = rootFolder.Folders.Add(folderName);


                    Dispatcher.BeginInvoke(() =>
                    {
                        SelectButton.IsEnabled = true;
                      
                        
                        // MessageBox.Show("Created", "Created", MessageBoxButton.OK);
                    });

                },
          (s, ee) =>
          {
              Console.WriteLine(ee.Message);

          });
            }
        }

        public void RenameFolder(string siteUrl, string listName, string relativePath, string folderName, string folderNewName)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                //  string FolderFullPath = GetFullPath(listName, relativePath, folderName);

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View Scope=\"RecursiveAll\"> " +
                                "<Query>" +
                                    "<Where>" +
                     "<And>" +
                                            "<Eq>" +
                                                "<FieldRef Name=\"FSObjType\" />" +
                                                "<Value Type=\"Integer\">1</Value>" +
                                             "</Eq>" +
                     "<Eq>" +
                       "<FieldRef Name=\"Title\"/>" +
                       "<Value Type=\"Text\">" + folderName + "</Value>" +
                     "</Eq>" +
                     "</And>" +
                                     "</Where>" +
                                "</Query>" +
                                "</View>";

                /* if (relativePath.Equals(string.Empty))
                 {
                     query.FolderServerRelativeUrl = "/lists/" + listName;
                 }
                 else
                 {
                     query.FolderServerRelativeUrl = "/lists/" + listName + "/" + relativePath;
                 }*/

                //query.FolderServerRelativeUrl = "/"+listName;

                var folders = list.GetItems(query);

                clientContext.Load(list);
                clientContext.Load(list.Fields);
                clientContext.Load(folders, fs => fs.Include(fi => fi["Title"],
                    fi => fi["DisplayName"],
                    fi => fi["FileLeafRef"]));
                // clientContext.ExecuteQuery();

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    if (folders.Count == 1)
                    {

                        folders[0]["Title"] = folderNewName;
                        folders[0]["FileLeafRef"] = folderNewName;
                        folders[0].Update();
                        clientContext.ExecuteQueryAsync((ss, eee) =>
                        {

                            Dispatcher.BeginInvoke(() =>
                            {

                                MessageBox.Show("Success", "Success", MessageBoxButton.OK);
                            });




                        },
          (ss, eee) =>
          {
              Console.WriteLine(eee.Message);

          });

                    }
                },
          (s, ee) =>
          {
              Console.WriteLine(ee.Message);

          });


            }
        }


        public void SearchFolder(string siteUrl, string listName, string relativePath)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                string FolderFullPath = null;

                CamlQuery query = CamlQuery.CreateAllFoldersQuery();

                if (relativePath.Equals(string.Empty))
                {
                    FolderFullPath = "/lists/" + listName;
                }
                else
                {
                    FolderFullPath = "/lists/" + listName + "/" + relativePath;
                }
                if (!string.IsNullOrEmpty(FolderFullPath))
                {
                    query.FolderServerRelativeUrl = FolderFullPath;
                }
                IList<Folder> folderResult = new List<Folder>();

                var listItems = list.GetItems(query);

                clientContext.Load(list);
                clientContext.Load(listItems, litems => litems.Include(
                    li => li["DisplayName"],
                    li => li["Id"]
                    ));

                clientContext.ExecuteQuery();

                foreach (var item in listItems)
                {

                    Console.WriteLine("{0}----------{1}", item.Id, item.DisplayName);
                }
            }
        }


        public void DeleteFile(string siteUrl, string listName, string relativePath, string folderName, FileEntry fileName)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                + "<Query>"
                + "<Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='File'>" + fileName.FileName + "</Value></Eq></Where>"
                + "</Query>"
                + "</View>";

               if(!string.IsNullOrEmpty(folderName))
                {
                    query.FolderServerRelativeUrl = new Uri(siteUrl).AbsolutePath +"/" +libName+"/" + folderName + "/";
                }

                ListItemCollection listItems = list.GetItems(query);
                clientContext.Load(listItems);

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    foreach (ListItem listitem in listItems)
                    {


                        listitem.DeleteObject();


                        Dispatcher.BeginInvoke(() =>
                        {
                            
                            selectedFiles.Remove(fileName);
                            if(selectedFiles.Count==0)
                                RemoveButton.IsEnabled=false;

                        });


                        clientContext.ExecuteQueryAsync((ss, eee) =>
                       {

                       },
                        (ss, eee) =>
                        {
                            Console.WriteLine(eee.Message);

                        });


                    }

                },
         (s, ee) =>
         {
             Console.WriteLine(ee.Message);

         });



            }

        }

        public void DeleteFolder(string siteUrl, string listName, string relativePath, string folderName)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);

                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View Scope=\"RecursiveAll\"> " +
                               "<Query>" +
                                   "<Where>" +
                     "<And>" +
                                           "<Eq>" +
                                               "<FieldRef Name=\"FSObjType\" />" +
                                               "<Value Type=\"Integer\">1</Value>" +
                                            "</Eq>" +
                     "<Eq>" +
                       "<FieldRef Name=\"Title\"/>" +
                       "<Value Type=\"Text\">" + folderName + "</Value>" +
                     "</Eq>" +
                     "</And>" +
                                    "</Where>" +
                               "</Query>" +
                               "</View>";

                /*if (relativePath.Equals(string.Empty))
                {
                    query.FolderServerRelativeUrl = "/lists/" + listName;
                }
                else
                {
                    query.FolderServerRelativeUrl = "/lists/" + listName + "/" + relativePath;
                }*/

                var folders = list.GetItems(query);

                clientContext.Load(list);
                clientContext.Load(folders);

                clientContext.ExecuteQueryAsync((s, ee) =>
                {

                    if (folders.Count == 1)
                    {
                        folders[0].DeleteObject();
                        clientContext.ExecuteQueryAsync((ss, eee) =>
                        {

                            Dispatcher.BeginInvoke(() =>
                            {

                                MessageBox.Show("Deleted Folder", "Deleted", MessageBoxButton.OK);
                                selectedFiles.Clear();

                                RemoveButton.IsEnabled = false;
                            });

                        },
         (ss, eee) =>
         {
             Console.WriteLine(eee.Message);

         });
                    }


                    Dispatcher.BeginInvoke(() =>
                    {
                       


                    });

                },
         (s, ee) =>
         {
             Console.WriteLine(ee.Message);

         });



            }
        }

        private string GetFolderName()
        {
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = DateTime.Now.ToString("TyyyyMMddHHmmssfff");
                return folderName;
            }
            else if (!string.IsNullOrEmpty(newFolderName))
                return newFolderName;
            else if (!string.IsNullOrEmpty(folderName))
                return folderName;

            return string.Empty;
        }

        private void FileUpload_Click(object sender, RoutedEventArgs e)
        {
            //this.txtProgress.Text = string.Empty;

            string folderName = GetFolderName();
            if (!string.IsNullOrEmpty(folderName))
            {
                CreateFolder(ClientContext.Current.Url, libName, string.Empty, folderName);
            }

            OpenFileDialog oFileDialog = new OpenFileDialog();
            oFileDialog.Filter = "All Files|*.*";
            oFileDialog.FilterIndex = 1;
            oFileDialog.Multiselect = true;

            string data = string.Empty;

            if (oFileDialog.ShowDialog() == true && !string.IsNullOrEmpty(folderName))
            {

                foreach (FileInfo file in oFileDialog.Files)
                {
                    UploadFile(file, libName, GetFolderName());
                    

                }
            }

        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = GetFolderName();
            if (!string.IsNullOrEmpty(folderName))
            {
                CreateFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName);
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
         //   User Singleuser;

            ClientContext context = ClientContext.Current;
            List Idea = context.Web.Lists.GetByTitle("Idea");
            ListItem newItem = Idea.AddItem(new ListItemCreationInformation());

            //Singleuser = context.Web.EnsureUser("ads\\mahuj4");
            newItem["Idea_x0020_Status"] = "Draft";
            newItem["proj_comments"] = "Attachment Uploaded";
            newItem.Update();
            context.Load(Idea, list => list.Title);

            context.ExecuteQueryAsync((s, ee) =>
            {
                string itemId = newItem.Id.ToString();
                
                RenameFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName, itemId);

                Dispatcher.BeginInvoke(() =>
                {
                    newFolderName = itemId;
                    MessageBox.Show("Item created and folder renamed", "Item created and folder renamed", MessageBoxButton.OK);
                }
                    );


            },
     (s, ee) =>
     {
         Console.WriteLine(ee.Message);

     });


        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
                DeleteFolder(Utils.GetSiteUrl(), libName, string.Empty, GetFolderName());
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
             
            FileEntry selFile=FileListBox.SelectedItem as FileEntry;
            DeleteFile(Utils.GetSiteUrl(), libName, string.Empty, GetFolderName(), selFile);
            
           
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            // cleanup if there is a temp folder
            if (!string.IsNullOrEmpty(folderName))
            {
                DeleteFolder(Utils.GetSiteUrl(), libName, string.Empty, folderName);
            }
        }

    }
}

