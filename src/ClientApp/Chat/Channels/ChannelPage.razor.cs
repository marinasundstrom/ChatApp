using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using ChatApp.Theming;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Microsoft.AspNetCore.Components.Authorization;

namespace ChatApp.Chat.Channels
{
    public partial class ChannelPage
    {
        bool isDarkMode = false;
        string myUserId = "BS";
        bool isInAdminRole = false;

        List<Post> posts = new List<Post>();
       
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

            myUserId = await CurrentUserService.GetUserIdAsync();
            isInAdminRole = await CurrentUserService.IsInRoleAsync("admin");

            await LoadChannel();
        }

        private async Task LoadChannel()
        {
            Id = Id ?? "73b202c5-3ef1-4cd8-b1ed-04c05f47e981";

            var result = await MessagesClient.GetMessagesAsync(Guid.Parse(Id!), 1, 10, null, null);

            posts.Clear();

            foreach (var item in result.Items.Reverse())
            {
                AddMessage(item);
            }

            StateHasChanged();

            await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");

            try
            {
                if(hubConnection is not null && hubConnection.State != HubConnectionState.Disconnected) 
                {
                    await hubConnection.DisposeAsync();
                }

                hubConnection = new HubConnectionBuilder().WithUrl($"https://localhost:5001/hubs/chat?channelId={Id}", options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        return await AccessTokenProvider.GetAccessTokenAsync();
                    };
                }).WithAutomaticReconnect().Build();

                hubConnection.On<ChatApp.Message>("MessagePosted", OnMessagePosted);
                hubConnection.On<string>("MessagePostedConfirmed", OnMessagePostedConfirmed);
                hubConnection.On<string, string>("MessageDeleted", OnMessageDeleted);

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

        private void OnMessagePostedConfirmed(string messageId)
        {
            var post = posts.First(x => x.Id == Guid.Parse(messageId));
            post.Confirmed = true;

            StateHasChanged();
        }

        private void AddMessage(ChatApp.Message message)
        {
            var post = posts.FirstOrDefault(x => x.Id == message.Id);

            if(post is not null) 
            {
                post.Published = message.Created;
                
                return;
            }

            posts.Add(new Post
            {
                Id = message.Id,
                Sender = message.CreatedBy.Id,
                SenderInitials = GetInitials(message.CreatedBy.Name),
                Published = message.Created,
                Content = message.Content,
                IsCurrentUser = message.CreatedBy.Id == myUserId,
                Confirmed = true
            });
        }

        private async void OnMessagePosted(ChatApp.Message message)
        {            
            AddMessage(message);

            StateHasChanged();

            await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");
        }

        private void OnMessageDeleted(string channelId, string messageId) 
        {
            var post = posts.FirstOrDefault(x => x.Id.ToString() == messageId);

            if(post is not null) 
            {
                posts.Remove(post);

                StateHasChanged();
            }
        }

        async void OnLocationChanged(object? sender, LocationChangedEventArgs eventArgs)
        {
            await LoadChannel();

            StateHasChanged();
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
            public Guid Id { get; set; } 
            public string Sender { get; set; } = default !;
            public string SenderInitials { get; set; } = default!;
            public DateTimeOffset Published { get; set; }

            public string Content { get; set; } = default !;
            public bool IsCurrentUser { get; set; } = default !;
            public bool Confirmed { get; set; }
        }

        [Required]
        public string Text { get; set; } = default !;

        async Task Send()
        {
            var message = new Post() 
            {
                Id = Guid.Empty,
                Published = DateTimeOffset.UtcNow,
                Sender = myUserId,
                SenderInitials = GetInitials("Foo"), // TODO: Fix with my name,
                IsCurrentUser = true,
                Content = Text
            };

            posts.Add(message);

            message.Id = await hubConnection.InvokeAsync<Guid>("PostMessage", Id, Text);

            Text = string.Empty;

            await JSRuntime.InvokeVoidAsyncIgnoreErrors("helpers.scrollToBottom");
        }

        async Task DeleteMessage(Post post) 
        {
            await MessagesClient.DeleteMessageAsync(post.Id);

            if(post is not null) 
            {
                posts.Remove(post);

                StateHasChanged();
            }
        }

        private bool IsFirst(Post post)
        {
            var index = posts.IndexOf(post);
            if (index == 0)
            {
                return true;
            }

            var previousPost = posts[index - 1];

            if(previousPost.Sender != post.Sender) 
            {
                return true;
            }

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

            if(nextPost.Sender != post.Sender) 
            {
                return true;
            }

            if (!(post.Published.Year == nextPost.Published.Year && post.Published.Month == nextPost.Published.Month && post.Published.Day == nextPost.Published.Day && post.Published.Hour == nextPost.Published.Hour && post.Published.Minute == nextPost.Published.Minute))
            {
                return true;
            }

            return nextPost.Sender != post.Sender;
        }

        static string GetInitials(string name)
        {                       
            // StringSplitOptions.RemoveEmptyEntries excludes empty spaces returned by the Split method

            string[] nameSplit = name.Split(new string[] { "," , " "}, StringSplitOptions.RemoveEmptyEntries);
                        
            string initials = "";

            foreach (string item in nameSplit)
            {                
                initials += item.Substring(0, 1).ToUpper();
            }

            return initials;           
        }
    }
}