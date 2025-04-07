namespace ExpenseManagment.Custom
{
	public static class Helper
	{
		//public static async Task<bool> DBSaveChangesAsync(ApplicationDbContext db)
		//{
		//    try
		//    {
		//        await db.SaveChangesAsync();
		//        return true;
		//    }
		//    catch (DbUpdateException ex)
		//    {
		//        Log.Error(ex, "Error occurred while saving changes to the database.");
		//        throw;
		//    }
		//}

		public static string ErrorInSaveChanges = "There is an error occure while saving record in database.";
		public static string ObjectNotFound = "Object Not Exixts.! There is an error while finding the object.";
		public static string ExceptionError = "Exception Error !.";
		public static string InvalidModelState = "Invalid Model State.!";
		public static string ImageNotFoundInModel = "Image not found in the model.!";
		public static string NoChangeInData = "There is no change in data.!";


		public const string SuperAdminUserName = "hafiz.bilal78@gmail.com";
		public const string AdminUserName = "admin@gmail.com";
		public const string DefaultExpenceAccount = "Default Expence Account";
		public enum AccountTypeId
		{
			client = 1,
			business = 2,
			partner = 3,
			contractor = 4,
			GeneralExpenceAccountOfBusiness = 5,
			BusinessCapitalAccount = 6,
			IncomeAccountOfBusiness = 7
		};

		public enum InvoiceTypeId
		{
			ExpenceFromBusinessAccount = 1,
			BusinessCapitalAccountTransaction = 2,
			BusinessInvoice = 3,
			SallaryInvoice = 4

		};

		public enum PageRoles
		{
			SuperAdmin,
			Admin,
			Client,
			Business,
			Partner,
			Contractor,
			Project,
			Expence,
			AddCapitalToBusiness,
			ProjectTransaction,
			Salary,
			GenerateSalary,
			AuditLog
		};

		public static class RolesAttrVal
		{
			public const string SuperAdmin = "SuperAdmin";
			public const string Admin = "Admin";
			public const string Client = "SuperAdmin,Client";
			public const string Business = "SuperAdmin,Business";
			public const string Partner = "SuperAdmin,Partner";
			public const string Contractor = "SuperAdmin,Contractor";
			public const string Project = "SuperAdmin,Project";
			public const string Expence = "SuperAdmin,Expence";
			public const string AddCapitalToBusiness = "SuperAdmin,AddCapitalToBusiness";
			public const string ProjectTransaction = "SuperAdmin,ProjectTransaction";
			public const string Salary = "SuperAdmin,Salary";
			public const string GenerateSalary = "SuperAdmin,GenerateSalary";
			public const string AuditLog = "SuperAdmin,AuditLog";
		}

	}
}
