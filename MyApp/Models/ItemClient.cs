using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

public partial class ItemClient
{
    [Key]
    public int Id { get; set; }

    public int ItemId { get; set; }

    public int ClientId { get; set; }

    public string? Remark { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("ItemClients")]
    public virtual Client Client { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("ItemClients")]
    public virtual Item Item { get; set; } = null!;
}
