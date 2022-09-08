/* 
    Author: Derek Ackworth
    Purpose: More easily create, read, update, and delete database information
*/

using InvestmentPortfolio.Models;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace InvestmentPortfolio.Facades
{
    public static class DatabaseFacade
    {
        private static readonly MySqlConnection _connection = new MySqlConnection(ConfigurationManager.AppSettings["DatabaseConnection"]);
        private static readonly string _encryptionKey = ConfigurationManager.AppSettings["DatabaseEncryptionKey"];

        public static string CreateUser(string username, string password)
        {
            string message;

            try
            {
                _connection.Open();
                Random random = new Random();
                string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                string salt = new string(Enumerable.Repeat(characters, 20).Select(s => s[random.Next(s.Length)]).ToArray());
                MySqlCommand command = new MySqlCommand("INSERT INTO user (username, password, salt) " +
                    "values ('" + username.ToLower() + "'" +
                    ", AES_ENCRYPT(CONCAT('" + password + "', '" + salt + "'), '" + _encryptionKey + "')" +
                    ", '" + salt + "')", _connection);

                if (command.ExecuteNonQuery() == 1)
                {
                    message = "Signed up, you can now log in";
                }
                else
                {
                    message = "Exception: An unexpected error occured";
                }
            }
            catch (Exception ex)
            {
                message = "Exception: " + ex.Message;
            }

            _connection.Close();
            return message;
        }

        public static string ReadUser(string username, string password)
        {
            string message;

            try
            {
                _connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT user_id FROM user " +
                    "WHERE username = '" + username.ToLower() + "' " +
                    "AND password = AES_ENCRYPT(CONCAT('" + password + "', salt), '" + _encryptionKey + "') LIMIT 1", _connection);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    message = result.ToString();
                }
                else
                {
                    message = "Invalid username and/or password";
                }
            }
            catch (Exception ex)
            {
                message = "Exception: " + ex.Message;
            }

            _connection.Close();
            return message;
        }

        public static string CreateInvestment(uint userId, Investment investment)
        {
            string message;

            try
            {
                _connection.Open();
                string dateFormat = "yyyy-MM-dd HH:mm:ss";
                MySqlCommand command = new MySqlCommand("INSERT INTO investment (user_id, date, name, symbol, type, currency, amount, price) " +
                    "values ('" + userId + "', '" + investment.Date.ToString(dateFormat) + "', '" + investment.Name + "', '" + investment.Symbol + "', '" + investment.Type + "'" +
                    ", '" + investment.Currency + "', '" + investment.Amount + "', '" + investment.Price + "')", _connection);

                if (command.ExecuteNonQuery() == 1)
                {
                    message = command.LastInsertedId.ToString();
                }
                else
                {
                    message = "Exception: An unexpected error occured";
                }
            }
            catch (Exception ex)
            {
                message = "Exception: " + ex.Message;
            }

            _connection.Close();
            return message;
        }

        public static List<Investment> ReadInvestments(uint userId)
        {
            List<Investment> investments = new List<Investment>();

            try
            {
                _connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT investment_id, date, name, symbol, type, currency, amount, price FROM investment WHERE user_id = " + userId, _connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    investments.Add(new Investment(Convert.ToUInt32(reader[0]), Convert.ToDateTime(reader[1]), Convert.ToString(reader[2]), Convert.ToString(reader[3]),
                        Convert.ToString(reader[4]), Convert.ToString(reader[5]), Convert.ToDouble(reader[6]), Convert.ToDouble(reader[7])));
                }

                reader.Close();
            }
            catch { }

            _connection.Close();
            return investments;
        }

        public static string UpdateInvestment(Investment investment, DateTime newDate, string newAmount, string newPrice)
        {
            string message;

            try
            {
                _connection.Open();
                string dateFormat = "yyyy-MM-dd HH:mm:ss";
                MySqlCommand command = new MySqlCommand("UPDATE investment SET date = '" + newDate.ToString(dateFormat) + "', Amount = '" + newAmount + "', " +
                    "Price = '" + newPrice + "' " +
                    "WHERE investment_id = " + investment.InvestmentId, _connection);

                if (command.ExecuteNonQuery() == 1)
                {
                    message = "Investment updated";
                }
                else
                {
                    message = "Exception: An unexpected error occured";
                }
            }
            catch (Exception ex)
            {
                message = "Exception: " + ex.Message;
            }

            _connection.Close();
            return message;
        }

        public static string DeleteInvestment(Investment investment)
        {
            string message;

            try
            {
                _connection.Open();
                MySqlCommand command = new MySqlCommand("DELETE FROM investment WHERE investment_id = " + investment.InvestmentId, _connection);

                if (command.ExecuteNonQuery() == 1)
                {
                    message = "Investment deleted";
                }
                else
                {
                    message = "Exception: An unexpected error occured";
                }
            }
            catch (Exception ex)
            {
                message = "Exception: " + ex.Message;
            }

            _connection.Close();
            return message;
        }
    }
}
