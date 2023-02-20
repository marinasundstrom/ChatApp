# Chat App

Chat app based on the [TodoApp](https://github.com/marinasundstrom/todo-app) project.

Watch the [video](https://youtu.be/e1RcKaZ2TSk)

## Screenshot

![Screenshot](/images/screenshot.png)

## Features

Two features: ```Channels``` and ```Messages```.

Commands and Queries
Publishing domain event
### Sending a Message

When a sender posts a Message, the ```PostMessage``` command will create a ``Message``. Once that Message has been persisted, then a ```MessagePosted``` event will be published. 

On the ```MessagePosted``` event, the sender of the ``Message`` will get a confirmation, and the other clients that subscribe to the same ```Channel``` that the message been posted in will be notified about there being a new Message.

### Admin commands

Get the total number of posts

```
/admin getNumberPosts
```

Get number of posts in the current channel

```
/admin getNumberPosts /channel
```

## Running the project

### Tye

```
tye run
```

### Docker Compose

```
docker-compose up
```

### Seeding the databases

#### Web

When in the Web project:

```sh
dotnet run -- --seed
```

#### IdentityService

When in the IdentityService project:

```sh
dotnet run -- /seed
```

### Login credentials

```
Username: alice 
Password: alice

Username: bob 
Password: bob
```

### Swagger

https://localhost:5001/swagger/