﻿using Microsoft.AspNetCore.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Extract the userId from the query string
        var userId = connection.GetHttpContext()?.Request.Query["userId"];
        return userId;
    }
}