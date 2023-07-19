﻿namespace PubAPI.Dtos;

public class BookDto
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    public decimal BasePrice { get; set; }
    public int AuthorId { get; set; }
}