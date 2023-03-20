using Azure.Storage.Blobs;
using Orleans.Providers;
using OrleansEmailApp.Services;

namespace OrleansEmailApp;

public interface IDomainBreachedEmailsGrain : IStateHolderGrain<DomainBreachedEmails> { }

[StorageProvider(ProviderName = "emails")]
public class CustomerGrain : StateHolderGrain<DomainBreachedEmails>, IDomainBreachedEmailsGrain {}
