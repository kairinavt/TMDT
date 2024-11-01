using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApp.Models;


[Table("Member")]
public class Member
{
    public string MemberId { get; set; }= null!;
    public string Email { get; set; }= null!;
    public string Password { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string GivenName  { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public DateTime LoginDate  { get; set; } = DateTime.Now;
    public DateTime RegisterDate { get; set; } = DateTime.Now;
    public int RoleId {get; set;} 
}