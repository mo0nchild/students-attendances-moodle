namespace Attendances.Shared.Commons.Validations;

public interface IModelValidator<TModel> where TModel : class
{
    public void Check(TModel model);
    public Task CheckAsync(TModel model);
}