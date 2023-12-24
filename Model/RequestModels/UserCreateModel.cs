using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace mebsoftwareApi.Model.RequestModels
{
    public class UserCreateModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserPhoneNumber { get; set; }
    }

}
