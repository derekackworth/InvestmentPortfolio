/* 
    Author: Derek Ackworth
    Purpose: Store user information
*/

using InvestmentPortfolio.Facades;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentPortfolio.Models
{
    public static class User
    {
        public static uint UserId { get; private set; }
        public static string Username { get; private set; }
        public static List<Investment> Investments { get; private set; }

        public static void LogIn(uint userId, string username)
        {
            UserId = userId;
            Username = username.ToLower();
            Investments = DatabaseFacade.ReadInvestments(UserId);
        }

        public static void LogOut()
        {
            UserId = default;
            Username = default;
            Investments = default;
        }
    }
}
