namespace MinimalChatApplication.Model
{
    public class GetUsers
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
    }

    public class Userss
    {
        public List<GetUsers>? Userrs { get; set; }
    }
    public class GetGroups
    {
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }

       
    }
    public class Groupss
    {
        public List<GetGroups>? Groupsss { get; set; }
    }
}
