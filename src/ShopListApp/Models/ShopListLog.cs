﻿namespace ShopListApp.Models;

public class ShopListLog
{
    public int Id { get; set; }
    public int ShopListId { get; set; }
    public Operation Operation { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}
