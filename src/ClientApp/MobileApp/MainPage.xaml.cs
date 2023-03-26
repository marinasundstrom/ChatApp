using ChatApp.Auth0;
using ChatApp.Chat.Messages;

namespace ChatApp;

public partial class MainPage : ContentPage
{
    private readonly Auth0Client auth0Client;
    // 👆 new code

    public MainPage(Auth0Client client)
    // 👆 changed code
    {
        InitializeComponent();
        auth0Client = client;    // 👈 new code
    }

    // 👇 new code
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var loginResult = await auth0Client.LoginAsync();

        if (!loginResult.IsError)
        {
            LoginView.IsVisible = false;
            HomeView.IsVisible = true;
        }
        else
        {
            await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
        }
    }
    // 👆 new code
}

