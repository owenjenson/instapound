namespace Instapound.Web.Models;

public record EditProfileViewModel(
    string? Avatar,
    ChangePasswordViewModel ChangePassword,
    ChangeUserNameViewModel ChangeUserName,
    ChangeProfileInfoViewModel ChangeProfileInfo);