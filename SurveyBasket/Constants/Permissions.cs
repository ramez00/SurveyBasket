namespace SurveyBasket.Constants;

public static class Permissions
{
    public static string[] GetAllPermissions()
        => new[]
        {
            Polls.GetPolls,
            Polls.AddPolls,
            Polls.EditPolls,
            Polls.DeletePolls,
            Questions.GetQuestions,
            Questions.AddQuestions,
            Questions.UpdateQuestions,
            Users.GetUsers,
            Users.AddUsers,
            Users.UpdateUsers,
            Roles.GetRoles,
            Roles.AddRoles,
            Roles.UpdateRoles,
            Results
        };

    public static class Polls
    {
        public const string GetPolls = "Polls.Read";
        public const string AddPolls = "Polls.Add";
        public const string EditPolls = "Polls.Edit";
        public const string DeletePolls = "Polls.Delete";
    }
    public static class Questions
    {
        public const string GetQuestions = "Questions.Read";
        public const string AddQuestions = "Questions.Add";
        public const string UpdateQuestions = "Questions.Edit";
    }
    public static class Users
    {
        public const string GetUsers = "Users.Read";
        public const string AddUsers = "Users.Add";
        public const string UpdateUsers = "Users.Edit";
    }
    public static class Roles
    {
        public const string GetRoles = "Roles.Read";
        public const string AddRoles = "Roles.Add";
        public const string UpdateRoles = "Roles.Edit";
    }
    public const string Results = "Results.Read";
}
