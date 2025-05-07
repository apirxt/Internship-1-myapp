using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

public partial class Item
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    public int? Price { get; set; }

    public int? CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Items")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Item")]
    public virtual ICollection<ItemClient> ItemClients { get; set; } = new List<ItemClient>();

    [InverseProperty("Item")]
    public virtual ICollection<SerialNumber> SerialNumbers { get; set; } = new List<SerialNumber>();
}
