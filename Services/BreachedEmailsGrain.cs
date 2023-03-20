// using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Http.HttpResults;
// using Orleans.Runtime;
// // using OrleansEmailApp.Services.Interfaces;
//
// namespace OrleansEmailApp.Services;
//
//
// public class BreachedEmailsGrain : Grain<DomainBreachedEmails>, IBreachedEmailsGrain
// {
//     // private readonly IPersistentState<List<BreachedEmail>> _state;
//     //
//     // public BreachedEmailsGrain(
//     //     [PersistentState(
//     //         stateName: "email",
//     //         storageName: "emails")]
//     //     IPersistentState<List<BreachedEmail>> state)
//     // {
//     //     _state = state;
//     // }
//     
//     // bool stateChanged = false;
//     //
//     // public override async Task OnActivateAsync()
//     // {
//     //     RegisterTimer(WriteState, null, TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(300);
//     //     return base.OnActivateAsync();
//     // }
//     //
//     // async Task WriteState(object _)
//     // {
//     //     if (!stateChanged) return;
//     //     stateChanged = false; 
//     //     await State.WriteStateAsync();
//     // }
//     
//     public async Task<string?> AddBreachedEmail(string domain, string email)
//     {
//         string? breachedEmail = await GetBreachedEmail(email);
//
//         if (breachedEmail is null)
//         {
//             State.Domain = domain;
//             State.DomainEmails.Add(email);
//             await WriteStateAsync();
//         }
//
//         return breachedEmail == null ? email : null;
//     }
//
//     public async Task<string?> GetBreachedEmail(string domain, string email)
//     {
//         BreachedEmail? breachedEmail = State..FirstOrDefault(s => s.Equals( new BreachedEmail(email)));
//         
//         return breachedEmail != null ? await Task.FromResult(breachedEmail.Email) : null;
//     }
//     
// }
//
// // [GenerateSerializer]
// // public record BreachedEmail(string email)
// public class DomainBreachedEmails
// {
//     // [Required]
//     // [EmailAddress(ErrorMessage = "Invalid email address.")]
//     // public string Email { get;} = email;
//     public List<string> DomainEmails { get; set; }
//     
//     public string Domain { get; set; }
// }
