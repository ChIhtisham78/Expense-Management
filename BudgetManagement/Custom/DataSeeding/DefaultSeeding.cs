using ExpenseManagment.Data;
using ExpenseManagment.Data.DataBaseEntities;

namespace ExpenseManagment.Custom.DataSeeding
{
    public class DefaultSeeding
    {
        public static void seedDefaultAccounts(ApplicationDbContext db)
        {
            int GeneralExpenceAccountOfBusinessId = (int)Helper.AccountTypeId.GeneralExpenceAccountOfBusiness;
            var GexpenceAccInDb = db.AccountEntities.FirstOrDefault(x => x.AccountTypeId == GeneralExpenceAccountOfBusinessId);
            if (GexpenceAccInDb == null)
            {
                AccountEntity account = new AccountEntity();
                account.AccName = "Default Expence Account";
                account.AccountTypeId = GeneralExpenceAccountOfBusinessId;
                account.CreationDate = DateTime.Now;

                db.AccountEntities.Add(account);
                db.SaveChanges();
            }

            int BusinessCapitalAccount = (int)Helper.AccountTypeId.BusinessCapitalAccount;
            var CapitalAccInDb = db.AccountEntities.FirstOrDefault(x => x.AccountTypeId == BusinessCapitalAccount);
            if (CapitalAccInDb == null)
            {
                AccountEntity account = new AccountEntity();
                account.AccName = "Business Capital Account";
                account.AccountTypeId = BusinessCapitalAccount;
                account.CreationDate = DateTime.Now;

                db.AccountEntities.Add(account);
                db.SaveChanges();
            }

            int IncomeAccountOfBusiness = (int)Helper.AccountTypeId.IncomeAccountOfBusiness;
            var IncomeAccInDb = db.AccountEntities.FirstOrDefault(x => x.AccountTypeId == IncomeAccountOfBusiness);
            if (IncomeAccInDb == null)
            {
                AccountEntity account = new AccountEntity();
                account.AccName = "Business Income Account";
                account.AccountTypeId = IncomeAccountOfBusiness;
                account.CreationDate = DateTime.Now;

                db.AccountEntities.Add(account);
                db.SaveChanges();
            }

        }
    }
}
