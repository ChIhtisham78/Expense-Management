using System.Security.Claims;

namespace ExpenseManagment.Custom
{
    public class HlperNS
    {
        public bool CheckIsInRole(ClaimsPrincipal _User , string _roleAttrVal)
        {
            string[] rolesList = _roleAttrVal.Split(',');
            bool IsAvailable = false;
            for (int i = 0; i < rolesList.Length; i++)
            {
                if (_User.IsInRole(rolesList[i].Trim()))
                {
                    IsAvailable = true;
                    break;
                }
            }
            return IsAvailable;
        }
    }
}
