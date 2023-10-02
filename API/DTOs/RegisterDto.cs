using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;

namespace API.DTOs;
public class RegisterDto
{
   [Required]
   public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}
