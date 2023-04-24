using System;
using System.Collections.Generic;

namespace ReverseEngineer;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime PublishDate { get; set; }

    public decimal BasePrice { get; set; }

    public int AuthorId { get; set; }

    public virtual Author Author { get; set; } = null!;
}
