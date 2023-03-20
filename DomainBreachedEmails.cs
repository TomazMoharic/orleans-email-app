namespace OrleansEmailApp;

[GenerateSerializer]
public class DomainBreachedEmails
{
    [Id(0)]
    public string Domain { get; set; }
    
    [Id(1)]
    public List<string> DomainEmails { get; set; } = new();

}
