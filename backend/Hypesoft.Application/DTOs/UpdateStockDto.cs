namespace Hypesoft.Application.DTOs;

public class UpdateStockDto
{
    public int Quantity { get; set; }
    public StockOperationType Operation { get; set; } = StockOperationType.Set;
}

public enum StockOperationType
{
    Set = 0,   
    Add = 1,    
    Remove = 2 
}