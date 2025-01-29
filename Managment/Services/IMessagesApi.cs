using Refit;

namespace Management.Services
{
    [Headers("Authorization: Bearer")]
    public interface IMessagesApi
    {
        [Patch("/RenameUser?oldUsername={oldUsername}&newUsername={newUsername}")]
        Task<ApiResponse<object>> RenameUser(string oldUsername, string newUsername);

        [Delete("/DeleteUserMessages/{username}")]
        Task<ApiResponse<object>> DeleteUserMessages(string username);
    }
}
