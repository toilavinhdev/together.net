namespace Infrastructure.SharedKernel.Constants;

public static class ErrorCodes
{
    public static class Server
    {
        public const string BadRequest = nameof(BadRequest);
        public const string Unauthorized = nameof(Unauthorized);
        public const string Forbidden = nameof(Forbidden);
        public const string NotFound = nameof(NotFound);
        public const string UnsupportedMediaType = nameof(UnsupportedMediaType);
        public const string InternalServer = nameof(InternalServer);
    }
    
    public static class File
    {
        public const string FileNotFound = nameof(FileNotFound);
        public const string UploadFailed = nameof(UploadFailed);
    }
    
    public static class Notification
    {
        public const string NotificationNotFound = nameof(NotificationNotFound);
    }
    
    public static class User
    {
        public const string AccountHasBeenBanned = nameof(AccountHasBeenBanned);
        public const string UserNotFound = nameof(UserNotFound);
        public const string UserNameAlreadyExists = nameof(UserNameAlreadyExists);
        public const string UserEmailAlreadyExists = nameof(UserEmailAlreadyExists);
        public const string IncorrectPassword = nameof(IncorrectPassword);
        public const string ForgotPasswordTokenInvalid = nameof(ForgotPasswordTokenInvalid);
        public const string RefreshTokenFailed = nameof(RefreshTokenFailed);
    }
    
    public static class Role
    {
        public const string RoleNotFound = nameof(RoleNotFound);
        public const string RoleNameAlreadyExists = nameof(RoleNameAlreadyExists);
    }
    
    public static class Forum
    {
        public const string ForumNotFound = nameof(ForumNotFound);
    }
    
    public static class Topic
    {
        public const string TopicNotFound = nameof(TopicNotFound);
    }
    
    public static class Prefix
    {
        public const string PrefixNotFound = nameof(PrefixNotFound);
    }
    
    public static class Post
    {
        public const string PostNotFound = nameof(PostNotFound);
        public const string PostReportNotFound = nameof(PostReportNotFound);
    }
    
    public static class Reply
    {
        public const string ReplyNotFound = nameof(ReplyNotFound);
    }
    
    public static class Conversation
    {
        public const string ConversationNotFound = nameof(ConversationNotFound);
        public const string PrivateConversationAlreadyExists = nameof(PrivateConversationAlreadyExists);
        public const string HaveNotJoinedConversation = nameof(HaveNotJoinedConversation);
    }
}