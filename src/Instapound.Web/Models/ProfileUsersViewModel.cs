namespace Instapound.Web.Models;

public record ProfileUsersViewModel(
    ProfileInfoViewModel Info,
    List<ProfileInfoViewModel> Users);