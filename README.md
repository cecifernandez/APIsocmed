# Social Media API

## Overview

This is a C# API for a social media currently in development. This API is built using JWT for authentication, Entity Framework as ORM and SQL Server as a database.

### Features

- Log in.
- Register.
- Create posts.

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

