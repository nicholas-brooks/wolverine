using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Frames;
using JasperFx.CodeGeneration.Model;
using JasperFx.Core.Reflection;
using Microsoft.AspNetCore.Http;

namespace Wolverine.Http.Policies;

internal class SetStatusCodeAndReturnIfEntityIsNullFrame : SyncFrame
{
    private readonly Type _entityType;
    private Variable _httpResponse;
    private Variable? _entity;

    public SetStatusCodeAndReturnIfEntityIsNullFrame(Type entityType)
    {
        _entityType = entityType;
    }

    public SetStatusCodeAndReturnIfEntityIsNullFrame(Variable entity)
    {
        _entity = entity;
        _entityType = entity.VariableType;
    }

    public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
    {
        writer.WriteComment("404 if this required object is null");
        writer.Write($"BLOCK:if ({_entity.Usage} == null)");
        writer.Write($"{_httpResponse.Usage}.{nameof(HttpResponse.StatusCode)} = 404;");
        if (method.AsyncMode == AsyncMode.ReturnCompletedTask)
        {
            writer.Write($"return {typeof(Task).FullNameInCode()}.{nameof(Task.CompletedTask)};");
        }
        else
        {
            writer.Write("return;");
        }

        writer.FinishBlock();

        Next?.GenerateCode(method, writer);
    }

    public override IEnumerable<Variable> FindVariables(IMethodVariables chain)
    {
        _entity ??= chain.FindVariable(_entityType);
        yield return _entity;

        _httpResponse = chain.FindVariable(typeof(HttpResponse));
        yield return _httpResponse;
    }
}