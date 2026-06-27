namespace SchoolManagementSystem.Helpers.Email;

public class EmailOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string LocalUrl { get; set; } = string.Empty;
    public string? PublicUrl { get; set; }
}
