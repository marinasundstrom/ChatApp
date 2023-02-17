using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using ChatApp.Theming;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Microsoft.AspNetCore.Components.Authorization;

namespace ChatApp.Messages
{
    public partial class Channel
    {
        bool isDarkMode = false;
        string MyUserId = "BS";
        List<Post> posts = new List<Post>()
        {
            new Post{Sender = "FB", Published = DateTime.UtcNow, Content = "Welcome!"}
        };
       
        [Parameter]
        public string? Id { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }  = default!;

        HubConnection hubConnection = null!;

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            ThemeManager.ColorSchemeChanged += ColorSchemeChanged;
            isDarkMode = ThemeManager.CurrentColorScheme == ColorScheme.Dark;
            
            StateHasChanged();

            var authenticationState = await AuthenticationStateTask;
            MyUserId = authenticationState.User?.FindFirst("sub")?.Value!;

            try
            {
                Id = Id ?? "test";

                hubConnection = new HubConnectionBuilder().WithUrl($"https://localhost:5001/hubs/chat?channelId={Id}", options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var tokenResult = await AccessTokenProvider.RequestAccessToken();
                        if(tokenResult.TryGetToken(out var token)) 
                        {
                            return token.Value;
                        }
                        return null;
                    };
                }).WithAutomaticReconnect().Build();

                hubConnection.On<string, string, string>("MessagePosted", OnMessagePosted);

                hubConnection.Closed += (error) =>
                {
                    if (error is not null)
                    {
                        Snackbar.Add($"{error.Message}", Severity.Error);
                    }

                    Snackbar.Add("Connection closed");
                    return Task.CompletedTask;
                };
                hubConnection.Reconnected += (error) =>
                {
                    Snackbar.Add("Reconnected");
                    return Task.CompletedTask;
                };
                hubConnection.Reconnecting += (error) =>
                {
                    Snackbar.Add("Reconnecting");
                    return Task.CompletedTask;
                };
                await hubConnection.StartAsync();
                Snackbar.Add("Connected");
            }
            catch (Exception exc)
            {
                Snackbar.Add(exc.Message.ToString(), Severity.Error);
            }
        }

        private void OnMessagePosted(string channelId, string senderId, string content)
        {
            Console.WriteLine(senderId);
            
            posts.Add(new Post{
                Sender = senderId, 
                Published = DateTime.UtcNow, 
                Content = content, 
                IsCurrentUser = senderId == MyUserId
            });

            StateHasChanged();
        }

        void OnLocationChanged(object? sender, LocationChangedEventArgs eventArgs)
        {

        }

        void ColorSchemeChanged(object? sender, ColorSchemeChangedEventArgs arg)
        {
            isDarkMode = arg.ColorScheme == ColorScheme.Dark;
            StateHasChanged();
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            ThemeManager.ColorSchemeChanged -= ColorSchemeChanged;
        }

        class Post
        {
            public string Sender { get; set; } = default !;
            public DateTime Published { get; set; }

            public string Content { get; set; } = default !;
            public bool IsCurrentUser { get; set; } = default !;
        }

        [Required]
        public string Text { get; set; } = default !;

        async Task Send()
        {
            await hubConnection.SendAsync("PostMessage", Id, Text);
            Text = string.Empty;
        }

        private bool IsFirst(Post post)
        {
            var index = posts.IndexOf(post);
            if (index == 0)
            {
                return true;
            }

            var previousPost = posts[index - 1];
            if (!(post.Published.Year == previousPost.Published.Year && post.Published.Month == previousPost.Published.Month && post.Published.Day == previousPost.Published.Day && post.Published.Hour == previousPost.Published.Hour && post.Published.Minute == previousPost.Published.Minute))
            {
                return true;
            }

            return previousPost.Sender != post.Sender;
        }

        private bool IsLast(Post post)
        {
            var index = posts.IndexOf(post);
            if (index == posts.Count - 1)
            {
                return true;
            }

            var nextPost = posts[index + 1];
            if (!(post.Published.Year == nextPost.Published.Year && post.Published.Month == nextPost.Published.Month && post.Published.Day == nextPost.Published.Day && post.Published.Hour == nextPost.Published.Hour && post.Published.Minute == nextPost.Published.Minute))
            {
                return true;
            }

            return nextPost.Sender != post.Sender;
        }
    }
}