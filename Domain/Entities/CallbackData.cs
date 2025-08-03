namespace Domain.Entities;

public class CallbackData<T>
{ 
    public string Name { get; set; } = string.Empty;
    public T Data { get; set; }
}