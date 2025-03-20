using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModels.Dtos;

namespace SharedModels.ValidationModels
{
    public class ConfirmEmail
    {
        public UserCreateDto UserData { get; set; } = null!;
        public int ConfirmationCode { get; set; }
    }
}
