using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

[Keyless]
public class LoginModel
{
  public string Eml {get; set;} = null!;
  public string Pwd {get; set;} = null!;
  public bool Rem {get; set;} 
}