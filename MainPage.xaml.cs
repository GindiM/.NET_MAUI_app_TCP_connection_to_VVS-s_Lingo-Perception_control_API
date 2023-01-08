using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics.Text;
using System.Diagnostics;
//using Android.Media;
using System.IO;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection.Metadata;
//using static Android.Print.PrintAttributes;
using static System.Net.Mime.MediaTypeNames;
//using Android.Graphics.Fonts;

namespace Wand;

public partial class MainPage : ContentPage
{

    //----------- Global declarations
    string address = "";
    int port = 0;
    string username = "";
    string password = "";

    string incomingMsg = "";

    string[] strArr;

    string[] serverArr;
    string[] presetArr;

    TcpClient client;
    NetworkStream stream;

    //----------- Global GUI declarations-----------------------------------\\

    ScrollView scrollView = new ScrollView();
    VerticalStackLayout verticalStackLayout = new VerticalStackLayout();
    Microsoft.Maui.Controls.Image image = new Microsoft.Maui.Controls.Image();


    //----------- Global GUI declarations - MainPage Children

    Label label = new Label();
    Entry entryAddress = new Entry();
    Entry entryPort = new Entry();
    Entry entryUsername = new Entry();
    Entry entryPassword = new Entry();

    HorizontalStackLayout hsl0 = new HorizontalStackLayout();
    CheckBox cb0 = new CheckBox();
    Label lb0 = new Label();

    HorizontalStackLayout hsl1 = new HorizontalStackLayout();
    CheckBox cb1 = new CheckBox();
    Label lb1 = new Label();
    //----------------------------------------------------------------------//


    public MainPage()
    {
        InitializeComponent();
        RenderMainPage();
    }


    void RenderMainPage()
    {
        verticalStackLayout.Children.Clear();
        hsl0.Children.Clear();
        hsl1.Children.Clear();

        verticalStackLayout.Spacing = 20;
        verticalStackLayout.Padding = 30;
        //verticalStackLayout.VerticalOptions = LayoutOptions.Center;

        image.Source = "vvslogo.png";
        image.HeightRequest = 150;
        image.WidthRequest = 180;
        image.HorizontalOptions = LayoutOptions.Center;
        image.Margin = new Thickness(0, -30, 0, -30);

        //label.Text = "Mobile Remote";
        label.Text = "Sign in";
        label.FontSize = 32;
        label.FontAttributes = FontAttributes.Bold;
        label.HorizontalOptions = LayoutOptions.Center;

        entryAddress.Placeholder = "Address";
        entryPort.Placeholder = "Port";
        entryPort.Keyboard = Keyboard.Numeric;
        entryUsername.Placeholder = "Username";
        entryPassword.Placeholder = "Password";


        //----------------remove----------------\\\
        //entryAddress.Text = "192.168.1.12";     //|
        //entryPort.Text = "10001";               //|
        //entryUsername.Text = "admin";           //|
        //entryPassword.Text = "1234";            //|
        //--------------------------------------///


        //------------------------------------/DETAIL SAVING\-------------------------------------------------
        string[] splitStr = new string[6];

        if (!File.Exists(DetailFile.fullPath))
        {
            DetailFile.CreateDefaultFile(DetailFile.fullPath);
        }
        else
        {
            string file = DetailFile.ReadFile(DetailFile.fullPath);
            splitStr = file.Split('|');
            if (splitStr[4] == "true") //details
            {
                entryAddress.Text = splitStr[0];
                entryPort.Text = splitStr[1];
                entryUsername.Text = splitStr[2];
                cb0.IsChecked = true;
            }
            else if (splitStr[4] == "false" || splitStr[4] == "" || splitStr[4] == null)
            {
                cb0.IsChecked = false;
            }
            if (splitStr[5] == "true") //password
            {
                entryPassword.Text = splitStr[3];
                cb1.IsChecked = true;
            }
            else if (splitStr[5] == "false" || splitStr[5] == "" || splitStr[5] == null)
            {
                cb1.IsChecked = false;
            }
        }
        //------------------------------------\DETAIL SAVING/-------------------------------------------------


        cb0.HorizontalOptions = LayoutOptions.Start;
        lb0.Text = "Remember Details";
        lb0.VerticalOptions = LayoutOptions.Center;

        cb1.HorizontalOptions = LayoutOptions.Start;
        lb1.Text = "Remember Password";
        lb1.VerticalOptions = LayoutOptions.Center;

        Button button = new Button
        {
            Text = "Connect",
            FontSize = 16,
            ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Right, 10),
            IsEnabled = true,
            IsVisible = true,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        button.Clicked += Login;


        //---Append children to parents---
        //verticalStackLayout.Children.Add(image);
        verticalStackLayout.Children.Add(label);
        verticalStackLayout.Children.Add(entryAddress);
        verticalStackLayout.Children.Add(entryPort);
        verticalStackLayout.Children.Add(entryUsername);
        verticalStackLayout.Children.Add(entryPassword);

        verticalStackLayout.Children.Add(hsl0);
        hsl0.Children.Add(cb0);
        hsl0.Children.Add(lb0);
        verticalStackLayout.Children.Add(hsl1);
        hsl1.Children.Add(cb1);
        hsl1.Children.Add(lb1);

        verticalStackLayout.Add(button);

        scrollView.Content = verticalStackLayout;
        Content = scrollView;

    }

    void RenderServerPage()
    {
        //1.Count servers
        //2.Create buttons
        //3.Onclick sends a preset_list request

        TcpReadWrite.WriteToStream(stream, "server_list");
        Thread.Sleep(400);
        incomingMsg = TcpReadWrite.ReadFromStream(stream);
        Debug.WriteLine(incomingMsg);
        strArr = incomingMsg.Split('|');//splits server_list response
        if (strArr[1].Contains("succes") | strArr[1].Contains("sucess") | strArr[1].Contains("success"))
        {
            stream.Flush();

            serverArr = new string[strArr.Length - 2];

            for (int i = 0; i < strArr.Length - 2; i++) //{server_list|success|server1|server2} -> {server1|server2}
            {
                serverArr[i] = strArr[i + 2];
            }
        }

        verticalStackLayout.Children.Clear();

        HorizontalStackLayout hsls = new HorizontalStackLayout(); hsls.HorizontalOptions = LayoutOptions.Center;
        Button btnUpdate = new Button(); btnUpdate.Text = "Update List"; btnUpdate.Clicked += (sender, args) => RenderServerPage(); btnUpdate.BackgroundColor = Color.FromArgb("#00CCCC"); btnUpdate.HorizontalOptions = LayoutOptions.Center; btnUpdate.Margin = new Thickness(0, 0, 3, 0);
        Button btnLogOut = new Button(); btnLogOut.Text = "Log out"; btnLogOut.Clicked += (sender, args) => LogOut(); btnLogOut.BackgroundColor = Color.FromArgb("#00CC66"); btnLogOut.HorizontalOptions = LayoutOptions.Center;
        hsls.Children.Add(btnUpdate);
        hsls.Children.Add(btnLogOut);
        verticalStackLayout.Children.Add(hsls);

        Label srvLbl = new Label();
        srvLbl.Text = "Servers";
        srvLbl.FontSize = 24;
        srvLbl.FontAttributes = FontAttributes.Bold;
        srvLbl.HorizontalOptions = LayoutOptions.Center;
        srvLbl.VerticalOptions = LayoutOptions.Start;
        verticalStackLayout.Children.Add(srvLbl);

        for (int i = 0; i < serverArr.Length; i++)
        {
            Button btnServer = new Button();
            btnServer.Text = serverArr[i];
            verticalStackLayout.Children.Add(btnServer);
            btnServer.Clicked += (sender, args) => RenderPresetPage(btnServer.Text);
        }
        Content = scrollView;
    }
    void RenderPresetPage(string buttonServerText)
    {
        //1.Count presets
        //2.Create buttons
        //3.Onclick send a play/stop request

        verticalStackLayout.Children.Clear();

        HorizontalStackLayout hsl = new HorizontalStackLayout(); hsl.HorizontalOptions = LayoutOptions.Center;
        Button btnBack = new Button(); btnBack.Text = "Server List"; btnBack.Clicked += (sender, args) => RenderServerPage(); btnBack.BackgroundColor = Color.FromArgb("#FF9933"); btnBack.HorizontalOptions = LayoutOptions.Center; btnBack.Margin = new Thickness(0, 0, 3, 0);
        Button btnUpdate = new Button(); btnUpdate.Text = "Update List"; btnUpdate.Clicked += (sender, args) => RenderPresetPage(buttonServerText); btnUpdate.BackgroundColor = Color.FromArgb("#00CCCC"); btnUpdate.HorizontalOptions = LayoutOptions.Center; btnUpdate.Margin = new Thickness(0, 0, 3, 0);
        Button btnLogOut = new Button(); btnLogOut.Text = "Log out"; btnLogOut.Clicked += (sender, args) => LogOut(); btnLogOut.BackgroundColor = Color.FromArgb("#00CC66"); btnLogOut.HorizontalOptions = LayoutOptions.Center;
        hsl.Children.Add(btnBack);
        hsl.Children.Add(btnUpdate);
        hsl.Children.Add(btnLogOut);
        verticalStackLayout.Children.Add(hsl);

        Label pstLbl = new Label();
        pstLbl.Text = buttonServerText + "'s Presets";
        pstLbl.FontSize = 24;
        pstLbl.FontAttributes = FontAttributes.Bold;
        pstLbl.HorizontalOptions = LayoutOptions.Center;
        pstLbl.VerticalOptions = LayoutOptions.Start;
        verticalStackLayout.Children.Add(pstLbl);

        stream.Flush();
        TcpReadWrite.WriteToStream(stream, "preset_list|" + buttonServerText);
        Thread.Sleep(100);
        incomingMsg = TcpReadWrite.ReadFromStream(stream);
        Debug.WriteLine(incomingMsg);

        strArr = incomingMsg.Split('|'); //Splits preset_list response
        if (strArr[1].Contains("succes") | strArr[1].Contains("sucess") | strArr[1].Contains("success"))
        {
            stream.Flush();

            presetArr = new string[strArr.Length - 3];

            for (int i = 0; i < strArr.Length - 3; i++) //{preset_list|success|preset1|preset2} -> {preset1|preset2}
            {
                presetArr[i] = strArr[i + 3];
            }
        }


        for (int i = 0; i < presetArr.Length; i++)
        {
            HorizontalStackLayout horizontalStackLayout = new HorizontalStackLayout(); // [preset name] [stop]

            Button btnStop = new Button(); btnStop.Text = "Stop"; btnStop.BackgroundColor = Color.FromArgb("#F24E4E"); btnStop.HorizontalOptions = LayoutOptions.Start;
            horizontalStackLayout.Children.Add(btnStop);
            Button btnPreset = new Button(); btnPreset.Text = presetArr[i]; btnPreset.Margin = new Thickness(3, 0, 0, 0); btnPreset.WidthRequest = 270;//todo: why wont work - (horizontalStackLayout.Width)-(btnStop.Width)
            horizontalStackLayout.Children.Add(btnPreset);


            verticalStackLayout.Children.Add(horizontalStackLayout);
            btnPreset.Clicked += (sender, args) => Play(buttonServerText, btnPreset.Text);
            btnStop.Clicked += (sender, args) => Stop(buttonServerText);
        }
        Content = scrollView;
    }


    void Play(string buttonServerText, string ButtonPresetText)
    {
        stream.Flush();
        TcpReadWrite.WriteToStream(stream, "play|" + buttonServerText + "|" + ButtonPresetText);
    }
    void Stop(string buttonServerText)
    {
        stream.Flush();
        TcpReadWrite.WriteToStream(stream, "stop|" + buttonServerText);
    }
    void LogOut()
    {
        stream.Flush();
        TcpReadWrite.WriteToStream(stream, "logout");
        RenderMainPage();
    }

    private void Login(object sender, EventArgs e)     //"address|port|username|password|saveDetails|savePassword";
    {

        if (entryAddress.Text != null && entryAddress.Text.Length > 0)
        {
            address = entryAddress.Text;
        }
        if (entryPort.Text != null && entryPort.Text.Length > 0)
        {
            port = Int32.Parse(entryPort.Text);
        }
        if (entryUsername.Text != null && entryUsername.Text.Length > 0)
        {
            username = entryUsername.Text;
        }
        if (entryPassword.Text != null && entryPassword.Text.Length > 0)
        {
            password = entryPassword.Text;
        }


        //------------------------------------/DETAIL SAVING\-------------------------------------------------

        if (cb0.IsChecked == true && cb1.IsChecked == true)//details
        {
            DetailFile.WriteToFile(DetailFile.fullPath, $"{entryAddress.Text}|{entryPort.Text}|{entryUsername.Text}|{entryPassword.Text}|true|true");
        }
        if (cb1.IsChecked == true && cb1.IsChecked == false)//password
        {
            DetailFile.WriteToFile(DetailFile.fullPath, $"{entryAddress.Text}|{entryPort.Text}|{entryUsername.Text}||true|false");
        }
        if (cb1.IsChecked == false && cb1.IsChecked == true)//password
        {
            DetailFile.WriteToFile(DetailFile.fullPath, $"|||{entryPassword.Text}|false|true");
        }
        if (cb1.IsChecked == false && cb1.IsChecked == false)//password
        {
            DetailFile.WriteToFile(DetailFile.fullPath, $"||||false|false");
        }
        //------------------------------------\DETAIL SAVING/-------------------------------------------------



        client = new TcpClient(address, port);
        stream = client.GetStream();

        Debug.WriteLine(TcpReadWrite.ReadFromStream(stream).ToString()); //should return "register"
        stream.Flush();
        Thread.Sleep(200); //gives a stronger chance for the streams to fill up
        TcpReadWrite.WriteToStream(stream, $"login|{username}|{password}");
        stream.Flush();
        Thread.Sleep(400);
        incomingMsg = (TcpReadWrite.ReadFromStream(stream).ToString()); //should return "login|succes"

        strArr = incomingMsg.Split('|'); //splits login response
        if (strArr[1].Contains("succes") | strArr[1].Contains("sucess") | strArr[1].Contains("success"))
        {
            stream.Flush();

            RenderServerPage();
        }
        else if ((strArr[1].Contains("use")))
        {
            stream.Flush();
            DisplayAlert("Alert", "Id in use", "OK");
            try
            {
                TcpReadWrite.WriteToStream(stream, "logout");
            }
            catch { }

        }
        else
        {
            DisplayAlert("Alert", "Shet, Problem with cridentials or network", "OK");
            try
            {
                TcpReadWrite.WriteToStream(stream, "logout");
            }
            catch { }
        }









        // Routing.RegisterRoute(nameof(ServerPage), typeof(ServerPage));
        // Shell.Current.GoToAsync(nameof(ServerPage));
    }



}

