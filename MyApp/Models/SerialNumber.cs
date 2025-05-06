using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

public partial class SerialNumber
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? ItemId { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("SerialNumbers")]
    public virtual Item? Item { get; set; }
}
