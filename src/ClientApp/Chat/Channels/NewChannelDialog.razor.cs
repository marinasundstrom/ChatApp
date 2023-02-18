using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using MudBlazor;

using System.ComponentModel.DataAnnotations;

namespace ChatApp.Chat.Channels;

public partial class NewChannelDialog
{
    FormModel model = new FormModel();
    
    public class FormModel
    {
        [Required]
        [StringLength(60, ErrorMessage = "Title length can't be more than 8.")]
        public string Title { get; set; } = null !;
    }

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    async Task OnValidSubmit() 
    {
        var channel = await ChannelsClient.CreateChannelAsync(new CreateChannelRequest() { Name = model.Title });
        MudDialog.Close(DialogResult.Ok(channel));
    }
}