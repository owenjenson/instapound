namespace Instapound.Web.Models;

public record SimpleUserViewModel(
    Guid Id,
    string UserName,
    string Name,
    string? Avatar);