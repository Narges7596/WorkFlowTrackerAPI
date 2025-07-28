namespace DotnetAPI.DTOs
{
    public partial class UserForResetPasswordDto
    {
        public string Email { get; set; } = "";
        public string OldPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string NewPasswordConfirm { get; set; } = "";
    }
}
