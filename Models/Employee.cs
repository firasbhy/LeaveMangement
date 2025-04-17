using System;
using System.ComponentModel.DataAnnotations;
public class Employee
{
    public int Id {get; set;}
    [Required]
    public string FullName {get; set;}

    [Required]
    public string Departement {get; set;}
    [Required]
    public DateTime JoiningDate {get; set;}
}