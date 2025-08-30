# Instapound

Instapound is a social media web application inspired by platforms like Instagram, designed for sharing posts, images, and comments among users. This project was created as part of a university course assignment.

## Features

- **User Profiles:** Each user has a profile and can follow other users.
- **Posts:** Share text posts with optional images.
- **Comments:** Comment on posts and interact with other users.
- **Likes:** Like posts and comments.
- **Follow System:** Follow/unfollow users and view followers/following lists.
- **Live Chat:** Real-time chat (SignalR).
- **Demo/Test Data:** Automatically seeds with test users, posts, and comments in development mode.

## Tech Stack

- **Backend:** ASP.NET Core MVC, SignalR
- **Frontend:** Razor Views, JavaScript, Tailwind CSS
- **Database:** Entity Framework Core, SQLite
- **Authentication:** ASP.NET Core Identity

## Showcase

Here are some screenshots of Instapound in action:

<p align="center">
  <img src="screenshots/feed.png" alt="Feed screenshot" width="600" />
  <br>
  <img src="screenshots/profile.png" alt="Profile screenshot" width="600" />
  <br>
  <img src="screenshots/post-details.png" alt="Post details screenshot" width="600" />
</p>

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/RadekVyM/instapound.git
   ```

2. Navigate to the `src/Instapound.Web` folder and apply migrations to set up the database:
   ```bash
   dotnet ef database update
   ```

3. Navigate to the `src/Instapound.Web` folder and run the application:
   ```bash
   dotnet run
   ```

4. Navigate to the URL displayed in the console.

> [!NOTE]  
> When running in development, the app seeds demo users, posts, and comments automatically for testing.
