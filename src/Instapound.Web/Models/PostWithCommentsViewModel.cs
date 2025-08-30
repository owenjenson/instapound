namespace Instapound.Web.Models;

public record PostWithCommentsViewModel(
    PostViewModel Post,
    List<CommentViewModel> Comments);