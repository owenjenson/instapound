namespace Instapound.Web.Models;

public record ProfileViewModel(
    ProfileInfoViewModel Info,
    List<PostViewModel> Posts);