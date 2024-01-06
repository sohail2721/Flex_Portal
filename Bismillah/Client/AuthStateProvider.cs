using System;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace Bismillah.Client
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime jsr;
        public AuthStateProvider(IJSRuntime jsr)
        {
            this.jsr = jsr;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await jsr.InvokeAsync<string>("localStorage.getItem", "user");
            //string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZmFpcUBnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZWFjaGVyIiwic3ViIjoiZmFpcUBnbWFpbC5jb20iLCJlbWFpbCI6ImZhaXFAZ21haWwuY29tIiwianRpIjoiZmFpcUBnbWFpbC5jb20iLCJleHAiOjE2Nzg5OTQ0MTMsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcyNzgvIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzI3OC8ifQ.ORbahYCnqnq44Qdjml8BN7MULqepnvOac-QlCcNq76Y";
            var identity = new ClaimsIdentity();
            if (token != null)
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            }
            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;

        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);

        }
    }
}