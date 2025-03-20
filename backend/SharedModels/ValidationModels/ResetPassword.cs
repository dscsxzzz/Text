using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModels.Dtos;

namespace SharedModels.ValidationModels;

public class ResetPassword
{
    public UserUpdateDto UserData { get; set; } = null!;
    public string ConfirmationCode { get; set; }
}
