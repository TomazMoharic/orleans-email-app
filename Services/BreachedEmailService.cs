namespace OrleansEmailApp.Services;

public class BreachedEmailService : IBreachedEmailService
{
    private readonly IGrainFactory _grainFactory;

    public BreachedEmailService(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }
    
    public async Task<string?> AddEmailToBreachedList(string email)
    {
        string emailDomain = Helpers.ExtractEmailDomain(email);
        
        IDomainBreachedEmailsGrain? emailDomainGrain = _grainFactory.GetGrain<IDomainBreachedEmailsGrain>(emailDomain);

        DomainBreachedEmails? domainObject = await emailDomainGrain.GetItem();

        string? existingEmail = domainObject?.DomainEmails.FirstOrDefault(de => de == email);
        
        if (domainObject is not null)
        {
            if (existingEmail is not null)
            {
                return null;
            }
            
            domainObject.DomainEmails.Add(email);
        }
        else
        {
            domainObject = new DomainBreachedEmails
            {
                Domain = emailDomain,
                DomainEmails = new List<string> { email },
            };
        }
        
        await emailDomainGrain.SetItem(domainObject);

        return email;
    }

    public async Task<string?> CheckIfEmailIsBreached(string email)
    {
        string emailDomain = Helpers.ExtractEmailDomain(email);
        
        IDomainBreachedEmailsGrain? emailDomainGrain = _grainFactory.GetGrain<IDomainBreachedEmailsGrain>(emailDomain);

        DomainBreachedEmails? domainObject = await emailDomainGrain.GetItem();
        
        var existingEmail = domainObject?.DomainEmails.FirstOrDefault(de => de == email);
        
        if (domainObject is null || existingEmail is null)
        {
            return null;
        }

        return email;
    }
}