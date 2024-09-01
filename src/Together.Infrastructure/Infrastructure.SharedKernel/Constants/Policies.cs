namespace Infrastructure.SharedKernel.Constants;

public static class Policies
{
    public const string All = nameof(All);

    public static class Management
    {
        public const string Access = "Management:Access";
        public const string ViewDashboard = "Management:ViewDashboard";
    }
    
    public static class User
    {
        public const string Get = "User:Get";
        public const string List = "User:List";
    }
    
    public static class Role
    {
        public const string View = "Role:View";
        public const string Create = "Role:Create";
        public const string Update = "Role:Update";
        public const string Assign = "Role:Assign";
        public const string Delete = "Role:Delete";
    }
    
    public static class Forum
    {
        public const string View = "Forum:View";
        public const string Create = "Forum:Create";
        public const string Update = "Forum:Update"; 
        public const string Delete = "Forum:Delete";
    }
    
    public static class Topic
    {
        public const string View = "Topic:View";
        public const string Create = "Topic:Create";
        public const string Update = "Topic:Update";
        public const string Delete = "Topic:Delete";
    }
    
    public static class Prefix
    {
        public const string View = "Prefix:View";
        public const string Create = "Prefix:Create";
        public const string Update = "Prefix:Update";
        public const string Delete = "Prefix:Delete";
    }
    
    public static class Post
    {
        public const string View = "Post:View";
        public const string Create = "Post:Create";
        public const string Update = "Post:Update";
        public const string Delete = "Post:Delete";
        public const string Vote = "Post:Vote";
    }
    
    public static class Reply
    {
        public const string View = "Reply:View";
        public const string Create = "Reply:Create";
        public const string Update = "Reply:Update";
        public const string Delete = "Reply:Delete";
        public const string Vote = "Reply:Vote";
    }

    public static List<string> RequiredPolicies() => [
        User.Get,
        Forum.View,
        Topic.View,
        Prefix.View,
        Post.View,
        Reply.View,
    ];
}