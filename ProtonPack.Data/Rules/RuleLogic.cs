using ProtonPack.Data.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data
{
    internal class RuleLogic : IDisposable
    {
        public CompanyUser CompanyUser { get; set; }

        public IRuleDefinition GetDefaultRule( Rule rule )
        {
            IRuleDefinition baseRule;
            switch (rule.RuleTypeID.ToString())
            {
                case "00000001-0000-0000-0000-000000000001":
                {
                    baseRule = new StaticAllocation( rule.BehaviorTypeID );
                    break;
                }
                case "00000001-0000-0000-0000-000000000002":
                {
                    baseRule = new RandomAllocation(rule.BehaviorTypeID);
                    break;
                }
                case "00000001-0000-0000-0000-000000000003":
                {
                    baseRule = new NaturalLogAllocation(rule.BehaviorTypeID);
                    break;
                }
                default:
                {
                    throw new WebThreeException("Invalid Rule Type");
                }
            }
            return baseRule;
        }

        public void Dispose()
        {
        }

        internal Customer? VerifyFunds(string userId, decimal value)
        {
            using var manager = DataManagerFactory.GetDataManager<Customer, CustomerDataManager>(CompanyUser);
            var user = manager.GetByUserID(userId);
            if (null != user)
            {
                if (!string.IsNullOrWhiteSpace(user.WalletAddress))
                {
                    if (user.WalletBalance >= value)
                        return user;
                    else
                        throw new WebThreeException($"Insufficient Funds for '{userId}'.");
                }
                else
                    throw new WebThreeException($"Invalid Wallet for '{userId}'");
            }
            else
                throw new WebThreeException($"Invalid UserId: '{userId}'");
        }

        internal void Allocate(Guid ruleID, string userId, decimal value, Guid transactionId, string assetNumber, Customer? user = null)
        {
            using var manager = DataManagerFactory.GetDataManager<Customer, CustomerDataManager>(CompanyUser);
            if (null == user)
                user = manager.GetByUserID(userId);
            if (null != user)
            {
                if (!string.IsNullOrWhiteSpace(assetNumber))
                {
                    using var assManager = DataManagerFactory.GetDataManager<Asset, AssetDataManager>(CompanyUser);
                    var asset = assManager.GetByAssetNumber(assetNumber);
                    user.AssetID = asset.ID;
                }
                user.WalletBalance += value;
                user.TransactionID = transactionId;
                user.RuleID = ruleID;
                manager.Update(user);
            }
            else
                throw new WebThreeException($"Invalid User - '{userId}'.");
        }

        internal void AllocateFromTreasury(Guid ruleID, string userId, decimal value, Guid transactionId, string assetNumber, Customer? user = null)
        {
            using var manager = DataManagerFactory.GetDataManager<Customer, CustomerDataManager>(CompanyUser);
            if (null == user)
                user = manager.GetByUserID(userId);
            if (null != user)
            {
                using var compManager = DataManagerFactory.GetDataManager<Company, CompanyDataManager>(CompanyUser);
                var company = compManager.Get(CompanyUser.CompanyId);
                if( null != company)
                {
                    if( !string.IsNullOrWhiteSpace( assetNumber ) )
                    {
                        using var assManager = DataManagerFactory.GetDataManager<Asset, AssetDataManager>(CompanyUser);
                        var asset = assManager.GetByAssetNumberForAllocation(assetNumber, CompanyUser.CompanyId);

                        user.AssetID = company.AssetID = asset.ID;
                    }
                    user.TransactionID = company.TransactionID = transactionId;
                    user.RuleID = company.RuleID = ruleID;
                    company.WalletBalance -= value;

                    user.WalletBalance += value;
                    compManager.Update(company);
                    manager.Update(user);
                }
                else
                    throw new WebThreeException($"Invalid Company - '{CompanyUser.CompanyId}'.");
            }
            else
                throw new WebThreeException($"Invalid User - '{userId}'.");
        }

        public decimal ProcessRule(UserAssetData data, Guid ruleId)
        {
            using var manager = DataManagerFactory.GetDataManager<Rule, RuleDataManager>(CompanyUser);
            var rule = manager.Get(ruleId);
            var value = GetDefaultRule(rule).ProcessRule(rule, CompanyUser);

            AllocateFromTreasury(rule.ID, data.UserID, value, Guid.NewGuid(), data.AssetID);

            return value;
        }

        internal bool ProcessRule(UserTipData data, Guid ruleId)
        {
            var rc = false;

            using var manager = DataManagerFactory.GetDataManager<Rule, RuleDataManager>(CompanyUser);
            var rule = manager.Get(ruleId);
            var value = GetDefaultRule(rule).ProcessRule(rule, CompanyUser);

            var fromUser = VerifyFunds(data.FromUserID, value);
            if (null != fromUser)
            {
                var transactionId = Guid.NewGuid();

                Allocate(rule.ID, data.FromUserID, value * -1M, transactionId, data.AssetID, fromUser);
                Allocate(rule.ID, data.ToUserID, value, transactionId, data.AssetID);
                rc = true;
            }
            return rc;
        }
    }
}
