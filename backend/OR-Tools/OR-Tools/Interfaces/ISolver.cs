using Domains.Models.Input;
using Domains.Models.Output;
using Google.OrTools.ConstraintSolver;

namespace OR_Tools.Interfaces
{
    internal interface ISolver
    {
        FileOutput Solve(FileInput input);
        FileOutput Solve(FileInput input, RoutingSearchParameters searchParameters);
    }
}
