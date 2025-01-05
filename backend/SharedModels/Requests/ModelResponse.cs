using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Requests;

public class ModelResponse
{
    public string SummarizedText { get; set; }
    public Guid UserId { get; set; }
}
