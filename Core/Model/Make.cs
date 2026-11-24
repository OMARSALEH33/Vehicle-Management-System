using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Challenge_1_Build_APIS.Core.Models;

    
public class Make
{
    public int Id { get; set; }
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;




   
}