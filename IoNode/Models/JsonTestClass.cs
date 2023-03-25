using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoNode.Models
{
    public record JsonTestClass(
        int? UserId = null,
        int? Id = null,
#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        string? Title = null,
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        bool? Completed = null
    );
}
