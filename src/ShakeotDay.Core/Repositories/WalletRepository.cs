using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ShakeotDay.Core.Models;

namespace ShakeotDay.Core.Repositories
{
    public class WalletRepository
    {
        private string _connstr;
        protected internal SqlConnection _conn;

        public enum Jackpot
        {
            Wallet = -1
        }

        public WalletRepository(string connString)
        {
            _connstr = connString;
            _conn = new SqlConnection(_connstr);
        }

        public async Task<long> GetWalletValue(long userId)
        {
            var userSql = "select 1 from AspNetUsers u where u.FriendlyUserId = @UserId";
            var user = await _conn.QueryFirstOrDefaultAsync<int?>(userSql, new { UserId = userId });
            if (user != 1)
                throw new Exception($"User {userId} was not found.");

            var sql = "select WalletValue from Wallets where UserId = @UserId";
            var res = await _conn.QueryFirstOrDefaultAsync<long?>(sql, new { UserId = userId });

            if (res != null)
            {
                return res ?? -1;
            }else
            {
                var newWallet = await GetOrCreateWallet(userId);
                return newWallet.WalletValue;
            }
        }


        public async Task<Wallet> GetOrCreateWallet(long userId)
        {
            var idSql = "select Id, UserId, WalletValue, TimesBoughtIn from Wallets where UserId = @UserId";
            var res = await _conn.QueryFirstOrDefaultAsync<Wallet>(idSql, new { UserId = userId });

            if (res != null)
                return res; // null should never happen

            var insSql = "Insert into Wallets(UserId) values (@UserId)";

            var exec = await _conn.ExecuteAsync(insSql, new { UserId = userId });
            if (exec == 0)
                throw new Exception($"error creating wallet for user {userId}");

            res = await _conn.QueryFirstOrDefaultAsync<Wallet>(idSql, new { UserId = userId });

            return res;
        }

        private async Task<bool> SetWalletValue(long userId, long walletValue)
        {
            var sql = "Update wallets set WalletValue = @NewVal where UserId = @UserId";
            var res = await _conn.ExecuteAsync(sql, new { NewVal = walletValue, UserId = userId });

            if (res > 0)
                return true;

            return false;
        }

        public async Task<bool> SubtractABuck(long userId)
        {
            var currentVal = await GetOrCreateWallet(userId);
            var newVal = currentVal.WalletValue - 1;
            var success = await SetWalletValue(userId, newVal);

            //add to jackpot
            var currentJackpotwallet = await GetOrCreateWallet((int)Jackpot.Wallet);
            var newjackVal = currentJackpotwallet.WalletValue + 1;
            var jacksuccess = await SetWalletValue((int)Jackpot.Wallet, newjackVal);

            if (success && jacksuccess)
                return true;

            return false;
        }

        public void InitJackpot()
        {
           var jackpotwallet = GetOrCreateWallet((long)Jackpot.Wallet).Result;
           var success = SetWalletValue(jackpotwallet.UserId, 250).Result;

            if (!success)
                throw new Exception("error initializing jackpot");
        }

        public async Task<Wallet> GetJackpotWallet()
        {
            var sql = "Select Id, UserId, WalletValue, TimesBoughtIn from Wallets where UserId = -1";
            var res = await _conn.QueryFirstAsync<Wallet>(sql);

            return res;
        }


        public async Task<bool> TransferJackpot(long userId)
        {
            var jackpot = await GetJackpotWallet();
            var result = await TransferAmountFromJackpot(userId, jackpot.WalletValue);
            InitJackpot();

            return result;
        }

        public async Task<bool> TransferAmountFromJackpot(long userId, long amount)
        {
            var oldWallet = await GetOrCreateWallet(userId);
            var jackpot = await GetJackpotWallet();
            var newValue = oldWallet.WalletValue + amount;
            var newJackpotValue = jackpot.WalletValue - amount;

            var jackSuccess = await SetWalletValue((int)Jackpot.Wallet, newJackpotValue);
            var success = await SetWalletValue(userId, newValue);

            return success && jackSuccess;
        }
    }
}
