﻿using Microsoft.AspNetCore.Components.Authorization;
using Pocky.WASM.Models;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace Pocky.WASM.Identity
{
    public class CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory) : AuthenticationStateProvider, IAccountManagement
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        private bool _authenticated;
        private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            _authenticated = false;

            var user = _unauthenticated;


            try
            {
                var userResponse = await _httpClient.GetAsync("manage/info");

                userResponse.EnsureSuccessStatusCode();

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, _jsonSerializerOptions);

                if (userInfo != null)
                {
                    _authenticated = true;

                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.Email),
                        new(ClaimTypes.Email, userInfo.Email),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                    user = new ClaimsPrincipal(claimsIdentity);
                }
            }
            catch
            {

            }
            return new AuthenticationState(user);
        }

        public async Task<AuthResult> LoginAsync(LoginModel credentials)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("login?useCookies=true", new
                {
                    credentials.Email,
                    credentials.Password,
                });
                if (result.IsSuccessStatusCode)
                {
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    return new AuthResult { Successed = true };
                }
            }
            catch
            {

            }
            return new AuthResult
            {
                Successed = false,
                ErrorList = ["Invalid email or password"]
            };
        }
    }
}
