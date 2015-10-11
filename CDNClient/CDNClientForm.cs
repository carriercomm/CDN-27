﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDN
{
    public partial class CDNClientForm : Form
    {
        public CDNClientForm()
        {
            InitializeComponent();
            server = new CDNClient(CDNNetWork.GetLocalIPPoint(), this);
            server.Start();
            server.Idle();
            this.localIpAddressControl.Value = server.LocalEndpoint as IPEndPoint;
            this.localIpAddressControl.ReadOnly = true;
            this.clientTreeView.Nodes.Add(server.localRoot);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            server.Send(remoteIpAddressControl.Value, CDNMessage.MSGID.TEST);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            server.Send(remoteIpAddressControl.Value, CDNMessage.MSGID.SHOW);
        }

        private void serverFile_DbClick(object sender, EventArgs e)
        {
            if(serverTreeView.SelectedNode is FileNode)
            {
                server.Send(remoteIpAddressControl.Value, 
                    CDNMessage.MSGID.DOWNLOAD,
                    Serializer<FileNode>.Serialize<SoapFormatter>(serverTreeView.SelectedNode as FileNode));
            }
        }

        private void displayFile_DbClick(object sender, EventArgs e)
        {
            try
            {
                if (clientTreeView.SelectedNode is FileNode)
                {
                    using (FileStream fs = new FileStream((clientTreeView.SelectedNode as FileNode).info.FullName, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string content = sr.ReadToEnd();
                            this.fileTextBox.Text = content;
                        }
                    }
                }
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private CDNClient server;
    }
}