# Design

## UI

The User Interface is based on code from the "[Messenger](https://github.com/marinasundstrom/YourBrand/tree/main/Messenger/Messenger.UI)" sub-project of the larger "YourBrand" project. Though all features have not been ported as of yet.

## Features

There are two main features: ```Channels``` and ```Messages```. If you don't include ```Users```, otherwise there are 3.

The server-side is organized into Requests (Commands and Queries) and Domain Events.

### Post a Message

When a sender posts a Message, the ```PostMessage``` command will create a ``Message``. Once that Message has been persisted, then a ```MessagePosted``` event will be published. 

On the ```MessagePosted``` event, the sender of the ``Message`` will get a confirmation, and the other clients that subscribe to the same ```Channel``` that the message been posted in will be notified about there being a new Message.

### Admin commands

There are a number of admin commands which was added as part of the assignment. Simply post the following commands in a channel and a response will be sent back. Only the sender will see those posts, and no one else in the channel.

Get the total number of posts:

```
/admin getNumberPosts
```

Get number of posts in the current channel:

```
/admin getNumberPosts /channel
```

### User management

Upon first login, the user is prompted to give their Name and Email.
