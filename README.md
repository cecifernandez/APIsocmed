# Social Media API

## Overview

This is a C# API for a social media currently under development. This API is built using JWT for authentication, Entity Framework as ORM and SQL Server as a database.

### Features

- Log in.
- Register.
- Create posts.
- Follow and unfollow users.
- Connect account with Spotify.
- Update user info.
- Delete posts.

### Technologies Used

- **ASP.NET Core**: Framework for building the API.
- **C#**: Main programming language.
- **JWT**: To authorize logged users to access the posts.
- **Postman**: For API documentation and testing.

### Endpoints

| HTTP Method | Endpoint                | Description                             |
|-------------|-------------------------|-----------------------------------------|
| POST         | `api/Access/Register`     | Register a new user.     |
| GET         | `api/Access/Login`  | Log in user. |
| GET         | `api/Posts/Posts`             | Retrieves a list of all posts.          |
| POST         | `api/Posts/CreatePost`             | Creates a new post.          |
| DELETE         | `api/Posts/{postId}`             | Deletes post.          |
| GET         | `api/User/GetUserById`             | Get user info by Id.         |
| GET         | `api/User/GetUserByUsername`             | Get user info by Username.          |
| POST         | `api/User/FollowUser`             | Follow user.          |
| POST         | `api/User/UnfollowUser`             | Unfollow user.          |
| GET         | `api/User/ConnectSpotify`             | Connect account to Spotify.          |
| PUT         | `api/User/{id}`             | Updates user info.          |

## Further development

- Implement likes and comments
- Improve exception handling and clener code
- Implement posting images
