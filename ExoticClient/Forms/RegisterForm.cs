﻿using System.Text;
using System;
using System.Windows.Forms;

using ExoticClient.App;
using ExoticClient.App.UI;
using ExoticClient.Classes;
using ExoticClient.Classes.Client;
using ExoticClient.Classes.Client.Authentication;
using ExoticClient.Classes.Client.PacketSystem;
using ExoticClient.Classes.Utils;

using Newtonsoft.Json;


namespace ExoticClient.Forms
{
    public partial class RegisterForm : CustomForm
    {
        private ExoticTcpClient _tcpClient;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, System.EventArgs e)
        {
            _tcpClient = ChronicApplication.Instance.TcpClient;
        }

        private async void RegisterButton_Click(object sender, System.EventArgs e)
        {
            string hashedPassword = PasswordHelper.HashPassword(txtPassword.Text);

            string userID = Guid.NewGuid().ToString();
            UserDetails userDetails = new UserDetails()
            {
                UserID = userID,
                Username = txtUsername.Text,
                Email = txtEmail.Text,
            };

            UserAuthDetails userAuthDetails = new UserAuthDetails()
            {
                UserID = userID,
                Username = txtUsername.Text,
                PasswordHash = hashedPassword,
                PasswordSalt = ""
            };

            string userDetailsJsonString = JsonConvert.SerializeObject(userDetails);
            string userAuthDetailsJsonString = JsonConvert.SerializeObject(userAuthDetails);

            string userAuthAndDetails = $"{userDetailsJsonString}-newpacket-{userAuthDetailsJsonString}";

            byte[] userAuthAndDetailsData = Encoding.UTF8.GetBytes(userAuthAndDetails);

            Packet userAuthDetailsPacket = _tcpClient.PacketHandler.CreateNewPacket(userAuthAndDetailsData, "User Registration Packet", true);
            await _tcpClient.PacketHandler.SendPacketAsync(userAuthDetailsPacket, _tcpClient.ClientHandler.GetNetworkStream());
        }

        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            _tcpClient.DisconnectFromServer();
            ChronicApplication.Instance.Shutdown();
        }

        private void MinimizeButton_Click(object sender, System.EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void BackToLoginButton_Click(object sender, EventArgs e)
        {

        }
    }
}
